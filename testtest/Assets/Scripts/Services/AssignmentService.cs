using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class AssignmentService
{
    public static async Task<AssignmentData> CreateAssignment(string name, string groupId, List<QuestionData> questions)
    {
        string id = Guid.NewGuid().ToString("N").Substring(0, 12);
        var assignment = new AssignmentData
        {
            assignmentId = id,
            name = name,
            groupId = groupId,
            teacherUid = SessionManager.Instance.UserId,
            teacherName = SessionManager.Instance.DisplayName,
            questions = questions,
            active = true,
            createdAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        bool ok = await FirebaseManager.Instance.Service.SetDocument("assignments", id, assignment.ToFirestore());
        if (!ok)
        {
            Debug.LogError("[AssignmentService] Failed to create assignment");
            return null;
        }
        Debug.Log($"[AssignmentService] Assignment created: {name} ({questions.Count} questions)");
        return assignment;
    }

    public static async Task<List<AssignmentData>> GetGroupAssignments(string groupId, bool activeOnly = false)
    {
        var results = await FirebaseManager.Instance.Service.QueryByField("assignments", "groupId", groupId);
        var list = new List<AssignmentData>();
        if (results == null) return list;

        foreach (var doc in results)
        {
            string docId = doc.ContainsKey("__docId__") ? doc["__docId__"].ToString() : "";
            var a = AssignmentData.FromFirestore(doc, docId);
            if (!activeOnly || a.active)
                list.Add(a);
        }
        return list;
    }

    public static async Task<AssignmentData> GetAssignment(string assignmentId)
    {
        var doc = await FirebaseManager.Instance.Service.GetDocument("assignments", assignmentId);
        if (doc == null) return null;
        return AssignmentData.FromFirestore(doc, assignmentId);
    }

    public static async Task<bool> ToggleAssignment(string assignmentId, bool active)
    {
        var data = new Dictionary<string, object> { { "active", active } };
        return await FirebaseManager.Instance.Service.UpdateDocument("assignments", assignmentId, data);
    }

    public static async Task<bool> DeleteAssignment(string assignmentId)
    {
        var data = new Dictionary<string, object> { { "active", false }, { "deleted", true } };
        return await FirebaseManager.Instance.Service.UpdateDocument("assignments", assignmentId, data);
    }

    public static async Task SaveAssignmentResult(string assignmentId, string assignmentName, int score, int total)
    {
        if (SessionManager.Instance == null || SessionManager.Instance.IsSelfStudy) return;

        string uid = SessionManager.Instance.UserId;
        var resultData = new Dictionary<string, object>
        {
            { "score", score },
            { "total", total },
            { "completedAt", DateTimeOffset.UtcNow.ToUnixTimeSeconds() }
        };

        var progressDoc = await FirebaseManager.Instance.Service.GetDocument("progress", uid);
        var assignmentsMap = new Dictionary<string, object>();
        if (progressDoc != null && progressDoc.ContainsKey("assignments") && progressDoc["assignments"] is Dictionary<string, object> existing)
            assignmentsMap = existing;

        assignmentsMap[assignmentId] = resultData;

        var update = new Dictionary<string, object> { { "assignments", assignmentsMap } };
        await FirebaseManager.Instance.Service.UpdateDocument("progress", uid, update);
        Debug.Log($"[AssignmentService] Result saved: {assignmentName} — {score}/{total}");
    }
}
