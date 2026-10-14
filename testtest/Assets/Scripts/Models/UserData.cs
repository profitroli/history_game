using System;

[Serializable]
public class UserData
{
    public string uid;
    public string email;
    public string displayName;
    public string role;
    public string groupId;
    public long createdAt;

    public UserRole GetRole()
    {
        switch (role)
        {
            case "teacher": return UserRole.Teacher;
            case "school_student": return UserRole.SchoolStudent;
            default: return UserRole.UniversityStudent;
        }
    }

    public static string RoleToString(UserRole r)
    {
        switch (r)
        {
            case UserRole.Teacher: return "teacher";
            case UserRole.SchoolStudent: return "school_student";
            default: return "university_student";
        }
    }
}
