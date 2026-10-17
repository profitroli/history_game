using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class GroupService
{
    public static async Task<GroupData> CreateGroup(string name, string program, string institution = "")
    {
        string code = GenerateCode();
        string groupId = Guid.NewGuid().ToString("N").Substring(0, 12);
        string teacherUid = SessionManager.Instance.UserId;
        string teacherName = SessionManager.Instance.DisplayName;

        var data = new Dictionary<string, object>
        {
            { "name", name },
            { "code", code },
            { "program", program },
            { "teacherUid", teacherUid },
            { "teacherName", teacherName },
            { "institution", institution },
            { "archived", false },
            { "createdAt", DateTimeOffset.UtcNow.ToUnixTimeSeconds() }
        };

        bool ok = await FirebaseManager.Instance.Service.SetDocument("groups", groupId, data);
        if (!ok)
        {
            Debug.LogError("[GroupService] Не удалось создать группу");
            return null;
        }

        return new GroupData
        {
            groupId = groupId,
            name = name,
            code = code,
            program = program,
            teacherUid = teacherUid,
            teacherName = teacherName
        };
    }

    public static async Task<GroupData> JoinGroup(string code)
    {
        var results = await FirebaseManager.Instance.Service.QueryByField("groups", "code", code);
        if (results == null || results.Count == 0)
            return null;

        var doc = results[0];
        string groupId = doc.ContainsKey("__docId__") ? doc["__docId__"].ToString() : "";
        string program = doc.ContainsKey("program") ? doc["program"].ToString() : "university";
        string teacherName = doc.ContainsKey("teacherName") ? doc["teacherName"].ToString() : "";
        string groupName = doc.ContainsKey("name") ? doc["name"].ToString() : "";

        UserRole role = program == "school" ? UserRole.SchoolStudent : UserRole.UniversityStudent;
        SessionManager.Instance.UpdateGroup(groupId, role);

        var update = new Dictionary<string, object>
        {
            { "groupId", groupId },
            { "role", UserData.RoleToString(role) }
        };
        await FirebaseManager.Instance.Service.UpdateDocument("users", SessionManager.Instance.UserId, update);

        return new GroupData
        {
            groupId = groupId,
            name = groupName,
            code = code,
            program = program,
            teacherName = teacherName
        };
    }

    public static async Task<List<GroupData>> GetTeacherGroups()
    {
        var results = await FirebaseManager.Instance.Service.QueryByField(
            "groups", "teacherUid", SessionManager.Instance.UserId);

        var groups = new List<GroupData>();
        if (results == null) return groups;

        foreach (var doc in results)
        {
            groups.Add(new GroupData
            {
                groupId = doc.ContainsKey("__docId__") ? doc["__docId__"].ToString() : "",
                name = doc.ContainsKey("name") ? doc["name"].ToString() : "",
                code = doc.ContainsKey("code") ? doc["code"].ToString() : "",
                program = doc.ContainsKey("program") ? doc["program"].ToString() : "",
                teacherUid = doc.ContainsKey("teacherUid") ? doc["teacherUid"].ToString() : "",
                teacherName = doc.ContainsKey("teacherName") ? doc["teacherName"].ToString() : "",
                institution = doc.ContainsKey("institution") ? doc["institution"].ToString() : "",
                archived = doc.ContainsKey("archived") && doc["archived"] is bool b && b
            });
        }
        return groups;
    }

    public static async Task<List<UserData>> GetGroupStudents(string groupId)
    {
        var results = await FirebaseManager.Instance.Service.QueryByField("users", "groupId", groupId);

        var students = new List<UserData>();
        if (results == null) return students;

        foreach (var doc in results)
        {
            students.Add(new UserData
            {
                uid = doc.ContainsKey("__docId__") ? doc["__docId__"].ToString() : "",
                displayName = doc.ContainsKey("displayName") ? doc["displayName"].ToString() : "",
                email = doc.ContainsKey("email") ? doc["email"].ToString() : "",
                role = doc.ContainsKey("role") ? doc["role"].ToString() : "",
                groupId = doc.ContainsKey("groupId") ? doc["groupId"].ToString() : ""
            });
        }
        return students;
    }

    public static async Task<ProgressData> GetStudentProgress(string studentUid)
    {
        var doc = await FirebaseManager.Instance.Service.GetDocument("progress", studentUid);
        return ProgressData.FromFirestoreDoc(doc);
    }

    public static string GetInviteLink(string code)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        string baseUrl = Application.absoluteURL;
        int queryIdx = baseUrl.IndexOf('?');
        if (queryIdx > 0) baseUrl = baseUrl.Substring(0, queryIdx);
        int hashIdx = baseUrl.IndexOf('#');
        if (hashIdx > 0) baseUrl = baseUrl.Substring(0, hashIdx);
        return $"{baseUrl}?code={code}";
#else
        return $"https://your-game-url.com/?code={code}";
#endif
    }

    private static string GenerateCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var random = new System.Random();
        var code = new char[6];
        for (int i = 0; i < 6; i++)
            code[i] = chars[random.Next(chars.Length)];
        return new string(code);
    }
}
