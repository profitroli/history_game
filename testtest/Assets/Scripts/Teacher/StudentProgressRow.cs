using UnityEngine;
using TMPro;

public class StudentProgressRow : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text lecturesText;
    [SerializeField] private TMP_Text gamesText;
    [SerializeField] private TMP_Text testsText;
    [SerializeField] private TMP_Text averageText;

    public void SetData(UserData student, ProgressData progress)
    {
        if (nameText != null) nameText.text = student.displayName;
        if (lecturesText != null) lecturesText.text = $"{progress.LecturesCompleted}/14";
        if (gamesText != null) gamesText.text = $"{progress.GamesCompleted}/34";
        if (testsText != null) testsText.text = $"{progress.TestsCompleted}/14";
        if (averageText != null) averageText.text = $"{progress.AverageTestScore:F0}%";
    }
}
