using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OddOneOutManager : MonoBehaviour
{
    [System.Serializable]
    public class QuizGroup
    {
        public string groupName;       // �������� ������ (��� ����������)
        public Button[] buttons;       // 4 ������ ���� ������
        public int correctOddIndex;    // ������ ������� ����� (0, 1, 2 ��� 3)
        [HideInInspector] public int selectedIndex = -1; // ��� ������ �����
    }

    public List<QuizGroup> quizGroups; // ������ ���� ����� ��������
    public TextMeshProUGUI statusText;

    void Start()
    {
        // ����������� ������ ��� ������
        for (int g = 0; g < quizGroups.Count; g++)
        {
            int groupIdx = g;
            for (int b = 0; b < quizGroups[g].buttons.Length; b++)
            {
                int btnIdx = b;
                quizGroups[g].buttons[b].onClick.AddListener(() => SelectButton(groupIdx, btnIdx));
            }
        }
    }

    // ������ ������ ������
    public void SelectButton(int groupIdx, int btnIdx)
    {
        // ���������� ���� ���� ������ � ������
        foreach (Button btn in quizGroups[groupIdx].buttons)
        {
            btn.image.color = Color.white;
        }

        // ������������ ��������� ������ ����� (��� ����� ������ ������ ������)
        quizGroups[groupIdx].buttons[btnIdx].image.color = new Color(0.7f, 0.8f, 1f);
        quizGroups[groupIdx].selectedIndex = btnIdx;
    }

    public void CheckResults()
    {
        int correctCount = 0;
        bool allSelected = true;

        // ���������, �� ���� �� ������� ������ �����
        foreach (var group in quizGroups)
        {
            if (group.selectedIndex == -1) allSelected = false;
        }

        if (!allSelected)
        {
            statusText.text = "�������� ������ ����� � ������ ������!";
            statusText.color = Color.purple;
            return;
        }

        // ������� ����������
        foreach (var group in quizGroups)
        {
            if (group.selectedIndex == group.correctOddIndex)
            {
                correctCount++;
                group.buttons[group.selectedIndex].image.color = Color.green;
            }
            else
            {
                group.buttons[group.selectedIndex].image.color = Color.red;
            }
        }

        if (correctCount == quizGroups.Count)
        {
            statusText.text = "�����! �� ����� ���� ������!";
            statusText.color = Color.forestGreen;
            ProgressService.Instance?.ReportGameCompleted(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, 1);
        }
        else
        {
            statusText.text = "���� ������, ���������� ��� ���";
            statusText.color = Color.red;
        }
    }
    public void LoadLevelByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ResetGame()
    {
        foreach (var group in quizGroups)
        {
            group.selectedIndex = -1;
            foreach (Button btn in group.buttons)
            {
                btn.image.color = Color.white;
            }
        }
        statusText.text = "�������� �� ���������� � ������ �����";
        statusText.color = Color.black;
    }
}