using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FamilyTreeManager : MonoBehaviour
{
    [System.Serializable]
    public class Member
    {
        public DraggableCard2 card; // ���� �����
        public int correctSlotID;  // ���� ��� ����� ���������
    }

    public Member[] familyMembers;
    public TextMeshProUGUI statusText;

    public void CheckTree()
    {
        int correct = 0;
        foreach (var m in familyMembers)
        {
            if (m.card.currentSlotID == m.correctSlotID) correct++;
        }

        if (correct == familyMembers.Length)
        {
            statusText.text = "����� ������������! �����.";
            statusText.color = Color.forestGreen;
            ProgressService.Instance?.ReportGameCompleted(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, 1);
        }
        else
        {
            statusText.text = "���-�� �� �� ���� �����...";
            statusText.color = Color.red;
        }
    }
    public void LoadLevelByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ResetGame()
    {
        foreach (var m in familyMembers) m.card.ResetPosition();
        statusText.text = "���������� ��������� ����������� ����� ";
        statusText.color = Color.black;
    }
}