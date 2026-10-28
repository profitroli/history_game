using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class GroupService
{
    private static List<GroupData> localGroups = new List<GroupData>();
    private static Dictionary<string, List<UserData>> localStudents = new Dictionary<string, List<UserData>>();
    private static Dictionary<string, ProgressData> localProgress = new Dictionary<string, ProgressData>();
    private static bool seeded;

    private static bool IsFirebaseAvailable =>
        FirebaseManager.Instance != null && FirebaseManager.Instance.Service != null;

    private static void SeedIfNeeded()
    {
        if (seeded || IsFirebaseAvailable) return;
        seeded = true;

        var g1 = new GroupData
        {
            groupId = "local_g1", name = "7А класс", code = "ABC123",
            program = "school", teacherUid = SessionManager.Instance?.UserId ?? "",
            teacherName = SessionManager.Instance?.DisplayName ?? "Учитель"
        };
        var g2 = new GroupData
        {
            groupId = "local_g2", name = "8Б класс", code = "XYZ789",
            program = "school", teacherUid = SessionManager.Instance?.UserId ?? "",
            teacherName = SessionManager.Instance?.DisplayName ?? "Учитель"
        };
        localGroups.AddRange(new[] { g1, g2 });

        localStudents["local_g1"] = new List<UserData>
        {
            new UserData { uid = "s1", displayName = "Мария Иванова", role = "school_student", groupId = "local_g1" },
            new UserData { uid = "s2", displayName = "Андрей Сидоров", role = "school_student", groupId = "local_g1" },
            new UserData { uid = "s3", displayName = "Ольга Петровна", role = "school_student", groupId = "local_g1" },
        };
        localStudents["local_g2"] = new List<UserData>
        {
            new UserData { uid = "s4", displayName = "Иван Козлов", role = "school_student", groupId = "local_g2" },
            new UserData { uid = "s5", displayName = "Елена Новикова", role = "school_student", groupId = "local_g2" },
        };

        localProgress["s1"] = new ProgressData { lectures = MakeLectures(10), games = MakeGames(25, 85), tests = MakeTests(8, 78) };
        localProgress["s2"] = new ProgressData { lectures = MakeLectures(7), games = MakeGames(18, 72), tests = MakeTests(5, 65) };
        localProgress["s3"] = new ProgressData { lectures = MakeLectures(12), games = MakeGames(30, 90), tests = MakeTests(10, 88) };
        localProgress["s4"] = new ProgressData { lectures = MakeLectures(5), games = MakeGames(12, 60), tests = MakeTests(3, 55) };
        localProgress["s5"] = new ProgressData { lectures = MakeLectures(9), games = MakeGames(22, 80), tests = MakeTests(7, 82) };

        Debug.Log("[GroupService] Local seed data created (2 groups, 5 students)");
    }

    private static Dictionary<string, long> MakeLectures(int count)
    {
        var d = new Dictionary<string, long>();
        for (int i = 1; i <= count; i++) d[$"Lecture {i}"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - (14 - i) * 86400;
        return d;
    }

    private static Dictionary<string, GameProgress> MakeGames(int count, int avgScore)
    {
        var d = new Dictionary<string, GameProgress>();
        var rng = new System.Random();
        for (int i = 1; i <= count; i++)
            d[$"MiniGame{i}"] = new GameProgress { score = rng.Next(avgScore - 15, avgScore + 15), completedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - (34 - i) * 86400 };
        return d;
    }

    private static Dictionary<string, TestProgress> MakeTests(int count, int avgScore)
    {
        var d = new Dictionary<string, TestProgress>();
        var rng = new System.Random();
        for (int i = 1; i <= count; i++)
            d[$"Test{i}"] = new TestProgress { score = rng.Next(avgScore - 10, avgScore + 10), total = 100, completedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - (14 - i) * 86400 };
        return d;
    }

    public static async Task<GroupData> CreateGroup(string name, string program, string institution = "")
    {
        string code = GenerateCode();
        string groupId = Guid.NewGuid().ToString("N").Substring(0, 12);
        string teacherUid = SessionManager.Instance.UserId;
        string teacherName = SessionManager.Instance.DisplayName;

        if (!IsFirebaseAvailable)
        {
            var group = new GroupData
            {
                groupId = groupId, name = name, code = code, program = program,
                teacherUid = teacherUid, teacherName = teacherName, institution = institution
            };
            localGroups.Add(group);
            localStudents[groupId] = new List<UserData>();
            Debug.Log($"[GroupService] Local group created: {name} code={code}");
            return group;
        }

        var data = new Dictionary<string, object>
        {
            { "name", name }, { "code", code }, { "program", program },
            { "teacherUid", teacherUid }, { "teacherName", teacherName },
            { "institution", institution }, { "archived", false },
            { "createdAt", DateTimeOffset.UtcNow.ToUnixTimeSeconds() }
        };

        bool ok = await FirebaseManager.Instance.Service.SetDocument("groups", groupId, data);
        if (!ok) { Debug.LogError("[GroupService] Failed to create group"); return null; }

        return new GroupData { groupId = groupId, name = name, code = code, program = program, teacherUid = teacherUid, teacherName = teacherName };
    }

    public static async Task<GroupData> JoinGroup(string code)
    {
        if (!IsFirebaseAvailable)
        {
            var local = localGroups.Find(g => g.code == code);
            if (local == null) return null;
            UserRole role = local.program == "school" ? UserRole.SchoolStudent : UserRole.UniversityStudent;
            SessionManager.Instance.UpdateGroup(local.groupId, role);
            return local;
        }

        var results = await FirebaseManager.Instance.Service.QueryByField("groups", "code", code);
        if (results == null || results.Count == 0) return null;

        var doc = results[0];
        string groupId = doc.ContainsKey("__docId__") ? doc["__docId__"].ToString() : "";
        string program = doc.ContainsKey("program") ? doc["program"].ToString() : "university";
        string teacherName = doc.ContainsKey("teacherName") ? doc["teacherName"].ToString() : "";
        string groupName = doc.ContainsKey("name") ? doc["name"].ToString() : "";

        UserRole joinRole = program == "school" ? UserRole.SchoolStudent : UserRole.UniversityStudent;
        SessionManager.Instance.UpdateGroup(groupId, joinRole);

        var update = new Dictionary<string, object> { { "groupId", groupId }, { "role", UserData.RoleToString(joinRole) } };
        await FirebaseManager.Instance.Service.UpdateDocument("users", SessionManager.Instance.UserId, update);

        return new GroupData { groupId = groupId, name = groupName, code = code, program = program, teacherName = teacherName };
    }

    public static async Task<List<GroupData>> GetTeacherGroups()
    {
        SeedIfNeeded();

        if (!IsFirebaseAvailable)
        {
            string uid = SessionManager.Instance?.UserId ?? "";
            return localGroups.FindAll(g => g.teacherUid == uid && !g.archived);
        }

        var results = await FirebaseManager.Instance.Service.QueryByField("groups", "teacherUid", SessionManager.Instance.UserId);
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
        if (!IsFirebaseAvailable)
        {
            return localStudents.ContainsKey(groupId) ? localStudents[groupId] : new List<UserData>();
        }

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
        if (!IsFirebaseAvailable)
        {
            return localProgress.ContainsKey(studentUid) ? localProgress[studentUid] : new ProgressData();
        }

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
        for (int i = 0; i < 6; i++) code[i] = chars[random.Next(chars.Length)];
        return new string(code);
    }
}