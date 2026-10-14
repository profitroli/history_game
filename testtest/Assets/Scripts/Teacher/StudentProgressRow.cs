using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StudentProgressRow : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text lecturesText;
    [SerializeField] private TMP_Text gamesText;
    [SerializeField] private TMP_Text testsText;
    [SerializeField] private TMP_Text averageText;
    [SerializeField] private Image lecturesIndicator;
    [SerializeField] private Image gamesIndicator;
    [SerializeField] private Image testsIndicator;
    [SerializeField] private Button removeButton;

    private static readonly Color ColorGray = new Color(0.7f, 0.7f, 0.7f);
    private static readonly Color ColorYellow = new Color(0.95f, 0.85f, 0.2f);
    private static readonly Color ColorGreen = new Color(0.2f, 0.8f, 0.3f);
    private static readonly Color ColorRed = new Color(0.9f, 0.3f, 0.3f);

    private string studentUid;
    private string studentGroupId;

    public void SetData(UserData student, ProgressData progress)
    {
        studentUid = student.uid;
        studentGroupId = student.groupId;

        if (nameText != null) nameText.text = student.displayName;
        if (lecturesText != null) lecturesText.text = $"{progress.LecturesCompleted}/14";
        if (gamesText != null) gamesText.text = $"{progress.GamesCompleted}/34";
        if (testsText != null) testsText.text = $"{progress.TestsCompleted}/14";
        if (averageText != null)
        {
            float avg = progress.AverageTestScore;
            averageText.text = $"{avg:F0}%";
            if (progress.TestsCompleted > 0 && avg < 50f)
                averageText.color = ColorRed;
        }

        SetIndicator(lecturesIndicator, progress.LecturesCompleted, 14);
        SetIndicator(gamesIndicator, progress.GamesCompleted, 34);
        SetIndicator(testsIndicator, progress.TestsCompleted, 14);

        if (removeButton != null)
            removeButton.onClick.AddListener(OnRemoveStudent);
    }

    private void SetIndicator(Image indicator, int completed, int total)
    {
        if (indicator == null) return;

        if (completed == 0)
            indicator.color = ColorGray;
        else if (completed >= total)
            indicator.color = ColorGreen;
        else
            indicator.color = ColorYellow;
    }

    private async void OnRemoveStudent()
    {
        if (string.IsNullOrEmpty(studentUid)) return;

        var data = new System.Collections.Generic.Dictionary<string, object>
        {
            { "groupId", "" },
            { "role", "university_student" }
        };
        bool ok = await FirebaseManager.Instance.Service.UpdateDocument("users", studentUid, data);
        if (ok)
        {
            Debug.Log($"[Teacher] Ученик {studentUid} исключён из группы");
            Destroy(gameObject);
        }
    }
}
