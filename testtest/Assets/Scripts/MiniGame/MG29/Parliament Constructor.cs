using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ParliamentConstructor : MonoBehaviour
{
    [System.Serializable]
    public class ConstructionSlot
    {
        public TextMeshProUGUI slotText; // ������ �� ����� � ����� ��������
        public string[] options;         // ������ ��������� (��������: "110 ���������", "64 �����")
        public int correctAnswerIndex;   // ����� ����������� ������ � ������
        [HideInInspector] public int currentIndex = -1; // ��� ������ �����
    }

    public ConstructionSlot[] slots;
    public TextMeshProUGUI statusText;

    // ����� ��� ������-���������
    public void ClickSlot(int slotIndex)
    {
        ConstructionSlot slot = slots[slotIndex];
        slot.currentIndex = (slot.currentIndex + 1) % slot.options.Length;
        slot.slotText.text = slot.options[slot.currentIndex];
        slot.slotText.color = Color.black; // ���������� ���� ��� �����
    }

    public void CheckResults()
    {// 1. ������� ���������, ��� �� ����� ���������
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].currentIndex == -1)
            {
                statusText.text = "<color=black>������� �������� �������� �� ���� ������ ���������!</color>";
                return; // ��������� ���������� ������, �������� �� ��������
            }
        }
        int correctCount = 0;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].currentIndex == slots[i].correctAnswerIndex)
            {
                correctCount++;
                slots[i].slotText.color = Color.green; // ������������ ������
            }
            else
            {
                slots[i].slotText.color = Color.red;   // ������������ ������
            }
        }

        if (correctCount == slots.Length)
        {
            statusText.text = "���������! ��������� ����������� �����.";
            statusText.color = Color.forestGreen;
            ProgressService.Instance?.ReportGameCompleted(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, 1);
        }
        else
        {
            statusText.text = $"������ � �����! �����: {correctCount} �� {slots.Length}";
            statusText.color = Color.red;
        }
    }

    public void LoadLevelByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void Restart() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}