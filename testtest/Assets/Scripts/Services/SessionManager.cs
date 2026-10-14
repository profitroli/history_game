using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance { get; private set; }

    public string UserId { get; private set; }
    public string DisplayName { get; private set; }
    public UserRole Role { get; private set; }
    public string GroupId { get; private set; }
    public string Email { get; private set; }

    public bool IsLoggedIn => !string.IsNullOrEmpty(UserId);
    public bool IsSelfStudy => !string.IsNullOrEmpty(UserId) && string.IsNullOrEmpty(GroupId) && UserId.StartsWith("local_");
    public bool IsTeacher => Role == UserRole.Teacher;
    public bool IsSchoolStudent => Role == UserRole.SchoolStudent;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Initialize(string userId, string email, string displayName, UserRole role, string groupId)
    {
        UserId = userId;
        Email = email;
        DisplayName = displayName;
        Role = role;
        GroupId = groupId;
        Debug.Log($"[SessionManager] Пользователь: {displayName}, роль: {role}, группа: {groupId}");
    }

    public void InitializeSelfStudy(UserRole role, string displayName)
    {
        UserId = "local_" + System.Guid.NewGuid().ToString("N").Substring(0, 8);
        Email = "";
        DisplayName = displayName;
        Role = role;
        GroupId = "";
        Debug.Log($"[SessionManager] Самостоятельный режим: {displayName}, роль: {role}");
    }

    public void UpdateGroup(string groupId, UserRole role)
    {
        GroupId = groupId;
        Role = role;
    }

    public void Clear()
    {
        UserId = null;
        Email = null;
        DisplayName = null;
        Role = UserRole.UniversityStudent;
        GroupId = null;
        Debug.Log("[SessionManager] Сессия очищена");
    }
}
