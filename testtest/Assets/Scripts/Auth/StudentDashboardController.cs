using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StudentDashboardController : MonoBehaviour
{
    [Header("Общая информация")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text groupInfoText;

    [Header("Прогресс")]
    [SerializeField] private TMP_Text lecturesProgressText;
    [SerializeField] private TMP_Text gamesProgressText;
    [SerializeField] private TMP_Text testsProgressText;
    [SerializeField] private TMP_Text averageScoreText;
    [SerializeField] private TMP_Text readinessText;

    [Header("Прогресс-бары (опционально)")]
    [SerializeField] private Image lecturesBar;
    [SerializeField] private Image gamesBar;
    [SerializeField] private Image testsBar;

    [Header("Кнопки")]
    [SerializeField] private Button closeButton;

    void OnEnable()
    {
        UpdateDisplay();
        if (closeButton != null) closeButton.onClick.AddListener(() => gameObject.SetActive(false));
    }

    public void UpdateDisplay()
    {
        if (SessionManager.Instance == null || !SessionManager.Instance.IsLoggedIn) return;
        if (ProgressService.Instance == null) return;

        var progress = ProgressService.Instance.GetProgress();
        var role = SessionManager.Instance.Role;
        int totalLectures = ContentFilter.GetTotalLectures(role);
        int totalGames = ContentFilter.GetTotalGames();
        int totalTests = ContentFilter.GetTotalTests();

        if (nameText != null)
            nameText.text = SessionManager.Instance.DisplayName;

        if (groupInfoText != null)
        {
            if (SessionManager.Instance.IsSelfStudy)
                groupInfoText.text = "Самостоятельное изучение";
            else
                groupInfoText.text = $"Группа: {SessionManager.Instance.GroupId}";
        }

        float lecturesPct = totalLectures > 0 ? (float)progress.LecturesCompleted / totalLectures : 0;
        float gamesPct = totalGames > 0 ? (float)progress.GamesCompleted / totalGames : 0;
        float testsPct = totalTests > 0 ? (float)progress.TestsCompleted / totalTests : 0;

        if (lecturesProgressText != null)
            lecturesProgressText.text = $"Лекции: {progress.LecturesCompleted}/{totalLectures} ({lecturesPct * 100:F0}%)";
        if (gamesProgressText != null)
            gamesProgressText.text = $"Мини-игры: {progress.GamesCompleted}/{totalGames} ({gamesPct * 100:F0}%)";
        if (testsProgressText != null)
            testsProgressText.text = $"Тесты: {progress.TestsCompleted}/{totalTests} ({testsPct * 100:F0}%)";
        if (averageScoreText != null)
            averageScoreText.text = $"Средний балл за тесты: {progress.AverageTestScore:F0}%";

        float overallReadiness = (lecturesPct + gamesPct + testsPct) / 3f;
        if (readinessText != null)
        {
            string label = role == UserRole.SchoolStudent ? "Готовность к Контрольной" : "Готовность к ОКР";
            string level;
            Color color;
            if (overallReadiness >= 0.8f) { level = "Высокая"; color = new Color(0.2f, 0.8f, 0.3f); }
            else if (overallReadiness >= 0.5f) { level = "Средняя"; color = new Color(0.95f, 0.85f, 0.2f); }
            else { level = "Низкая"; color = new Color(0.9f, 0.3f, 0.3f); }

            readinessText.text = $"{label}: {level} ({overallReadiness * 100:F0}%)";
            readinessText.color = color;
        }

        if (lecturesBar != null) lecturesBar.fillAmount = lecturesPct;
        if (gamesBar != null) gamesBar.fillAmount = gamesPct;
        if (testsBar != null) testsBar.fillAmount = testsPct;
    }
}
