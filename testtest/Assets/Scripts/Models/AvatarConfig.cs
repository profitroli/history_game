using UnityEngine;

public static class AvatarConfig
{
    public const int TotalAvatars = 6;
    public static readonly string[] Names = {
        "Новичок",
        "Ученик",
        "Игрок",
        "Знаток",
        "Исследователь",
        "Мастер"
    };

    public static readonly string[] Descriptions = {
        "Доступен сразу",
        "Просмотри 3 лекции",
        "Пройди 5 мини-игр",
        "Сдай 3 теста",
        "Просмотри 8 лекций и пройди 10 игр",
        "Заверши 80% всего контента"
    };

    public static bool IsUnlocked(int index, int lectures, int games, int tests, float overallPercent)
    {
        switch (index)
        {
            case 0: return true;
            case 1: return lectures >= 3;
            case 2: return games >= 5;
            case 3: return tests >= 3;
            case 4: return lectures >= 8 && games >= 10;
            case 5: return overallPercent >= 0.8f;
            default: return false;
        }
    }

    public static Sprite LoadSprite(int index)
    {
        return Resources.Load<Sprite>($"Avatars/avatar_{index + 1}");
    }

    public static int CountUnlocked(int lectures, int games, int tests, float overallPercent)
    {
        int count = 0;
        for (int i = 0; i < TotalAvatars; i++)
            if (IsUnlocked(i, lectures, games, tests, overallPercent)) count++;
        return count;
    }
}
