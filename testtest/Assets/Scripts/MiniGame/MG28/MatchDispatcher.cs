using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MatchDispatcherPro : MonoBehaviour
{
    [System.Serializable]
    public class MatchPair
    {
        public Button problemButton;
        public Button departmentButton;
        [HideInInspector] public Button currentAssignedDepartment; // ������� ����� ������
    }

    public List<MatchPair> pairs;
    public TextMeshProUGUI statusText;

    private Button selectedProblem;
    private Color defaultColor = Color.white;
    private Color selectedColor = Color.yellow;

    void Start()
    {
        statusText.text = "��������� �������� � �����������, ����� ������� '���������'";

        // ����������� ������ �������
        foreach (var pair in pairs)
        {
            Button pBtn = pair.problemButton;
            pBtn.onClick.AddListener(() => SelectProblem(pBtn));
        }

        // ����������� ������ ��������
        // ��� ��������: ��� ������ �������� ������ ����� ���������� �����
        // � ���������� ����� ����� ���������, ��� ��� ������ �� ������ departmentButton 
        // �������� ���� � ��� �� ����� ����� ��� ����
        foreach (var pair in pairs)
        {
            Button dBtn = pair.departmentButton;
            dBtn.onClick.AddListener(() => SelectDepartment(dBtn));
        }
    }

    void SelectProblem(Button btn)
    {
        if (selectedProblem != null) selectedProblem.GetComponent<Image>().color = defaultColor;
        selectedProblem = btn;
        btn.GetComponent<Image>().color = selectedColor;
    }

    void SelectDepartment(Button btn)
    {
        if (selectedProblem == null)
        {
            statusText.text = "������� �������� �������� �����!";
            return;
        }

        // ���������� ����� ��� ���������� ��������
        foreach (var pair in pairs)
        {
            if (pair.problemButton == selectedProblem)
            {
                // ���� ��� ��������� ��� ���� ������� ���-�� ������, ������� � ����
                foreach (var p in pairs) if (p.currentAssignedDepartment == btn) p.currentAssignedDepartment = null;

                pair.currentAssignedDepartment = btn;
                statusText.text = "����� �����������. ����� ��������� ����.";
                break;
            }
        }

        selectedProblem.GetComponent<Image>().color = Color.cyan; // ��������, ��� ���� �������
        selectedProblem = null;
    }

    public void CheckResults()
    {
        int correct = 0;
        int total = pairs.Count;

        foreach (var pair in pairs)
        {
            if (pair.currentAssignedDepartment == null) continue;

            if (pair.currentAssignedDepartment == pair.departmentButton)
            {
                correct++;
                pair.problemButton.GetComponent<Image>().color = Color.green;
                pair.currentAssignedDepartment.GetComponent<Image>().color = Color.green;
            }
            else
            {
                pair.problemButton.GetComponent<Image>().color = Color.red;
                pair.currentAssignedDepartment.GetComponent<Image>().color = Color.red;
            }
        }

        statusText.text = $"����: ��������� {correct} �� {total}. " + (correct == total ? "�� �������� ����������!" : "���������� ��� ���.");
        if (correct == total)
            ProgressService.Instance?.ReportGameCompleted(SceneManager.GetActiveScene().name, correct);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}