using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class AvatarService
{
    private static int cachedSelection = 0;

    public static int GetSelectedAvatar()
    {
        return cachedSelection;
    }

    public static Sprite GetSelectedSprite()
    {
        return AvatarConfig.LoadSprite(cachedSelection);
    }

    public static bool[] GetUnlockStatus()
    {
        var progress = ProgressService.Instance?.GetProgress() ?? new ProgressData();
        var role = SessionManager.Instance?.Role ?? UserRole.UniversityStudent;
        int totalLec = ContentFilter.GetTotalLectures(role);
        int totalGames = ContentFilter.GetTotalGames();
        int totalTests = ContentFilter.GetTotalTests();
        float overall = 0f;
        if (totalLec + totalGames + totalTests > 0)
            overall = (float)(progress.LecturesCompleted + progress.GamesCompleted + progress.TestsCompleted)
                      / (totalLec + totalGames + totalTests);

        var unlocked = new bool[AvatarConfig.TotalAvatars];
        for (int i = 0; i < AvatarConfig.TotalAvatars; i++)
            unlocked[i] = AvatarConfig.IsUnlocked(i, progress.LecturesCompleted, progress.GamesCompleted, progress.TestsCompleted, overall);
        return unlocked;
    }

    public static async Task SelectAvatar(int index)
    {
        var unlocked = GetUnlockStatus();
        if (index < 0 || index >= AvatarConfig.TotalAvatars || !unlocked[index]) return;

        cachedSelection = index;
        PlayerPrefs.SetInt("SelectedAvatar", index);
        PlayerPrefs.Save();

        if (SessionManager.Instance != null && !SessionManager.Instance.IsSelfStudy)
        {
            var data = new Dictionary<string, object> { { "avatarIndex", index } };
            await FirebaseManager.Instance.Service.UpdateDocument("users", SessionManager.Instance.UserId, data);
        }
    }

    public static async Task LoadSelection()
    {
        cachedSelection = PlayerPrefs.GetInt("SelectedAvatar", 0);

        if (SessionManager.Instance != null && !SessionManager.Instance.IsSelfStudy)
        {
            var doc = await FirebaseManager.Instance.Service.GetDocument("users", SessionManager.Instance.UserId);
            if (doc != null && doc.ContainsKey("avatarIndex"))
            {
                var val = doc["avatarIndex"];
                if (val is long l) cachedSelection = (int)l;
                else if (val is string s && int.TryParse(s, out int p)) cachedSelection = p;
            }
        }
    }

    public static int GetNewUnlocksCount()
    {
        int shown = PlayerPrefs.GetInt("AvatarsShown", 1);
        var unlocked = GetUnlockStatus();
        int current = 0;
        for (int i = 0; i < unlocked.Length; i++)
            if (unlocked[i]) current++;
        int newOnes = current - shown;
        if (newOnes > 0)
        {
            PlayerPrefs.SetInt("AvatarsShown", current);
            PlayerPrefs.Save();
        }
        return Mathf.Max(0, newOnes);
    }
}
