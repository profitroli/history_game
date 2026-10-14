using System.Collections.Generic;

public static class ContentFilter
{
    // TODO: заполнить реальный список школьных лекций
    private static readonly HashSet<string> SchoolLectures = new HashSet<string>
    {
        "Lecture 1", "Lecture 2", "Lecture 3", "Lecture 4", "Lecture 5",
        "Lecture 6", "Lecture 7", "Lecture 8", "Lecture 9", "Lecture 10",
        "Lecture 11-12", "Lecture 13", "Lecture 14"
    };

    public static bool IsLectureVisible(string lectureName, UserRole role)
    {
        if (role == UserRole.UniversityStudent || role == UserRole.Teacher)
            return true;
        return SchoolLectures.Contains(lectureName);
    }

    public static string GetPreparationLabel(UserRole role)
    {
        return role == UserRole.SchoolStudent
            ? "Подготовка к Контрольной"
            : "Подготовка к ОКР";
    }

    public static string GetPreparationSceneName(UserRole role)
    {
        return "PreparationScene";
    }

    public static bool IsGameVisible(string gameName, UserRole role)
    {
        return true;
    }

    public static int GetTotalLectures(UserRole role)
    {
        if (role == UserRole.SchoolStudent)
            return SchoolLectures.Count;
        return 14;
    }

    public static int GetTotalGames()
    {
        return 34;
    }

    public static int GetTotalTests()
    {
        return 14;
    }
}
