using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LadderGameManager : MonoBehaviour
{
    public StepSlot[] slots;            // ��� ����� �� ��������
    public TextMeshProUGUI statusText;  // �����-��������
    public GameObject finishPanel;      // ������ ������ (���� ��� � ���� ����)

    void Start()
    {
        // ��������� ����� �� ���������� ������
        statusText.text = "�������� ������ �� ��������� (�����) � ������������� (������)";
    }

    public void CheckLadder()
    {
        int correctCount = 0;
        int filledSlots = 0;

        foreach (var slot in slots)
        {
            // ���������, ���� �� ���-�� � �����
            if (slot != null && slot.transform.childCount > 0)
            {
                filledSlots++;

                // ���� ������ TitleStep �� ������ ������ �����
                TitleStep item = slot.GetComponentInChildren<TitleStep>();

                if (item != null)
                {
                    // ��������: ��� ������������ ����� ��� �����!
                    // ���� �������� �� ����� ������� ��� �� ��� ��������
                    Image itemImage = item.GetComponent<Image>();
                    if (itemImage == null) itemImage = item.GetComponentInParent<Image>();

                    // ������� ID ������ � ID �����
                    if (item.titleID == slot.slotID)
                    {
                        correctCount++;
                        if (itemImage != null) itemImage.color = Color.green; // ����� � �������
                    }
                    else
                    {
                        if (itemImage != null) itemImage.color = Color.red;   // ������ � �������
                    }
                }
            }
        }

        // ������ ������ ������
        if (filledSlots == 0)
        {
            statusText.text = "������� ���� �� ���� ������ �� ���������!";
        }
        else if (filledSlots < slots.Length)
        {
            statusText.text = $"�� ��������� �� ��! ���������: {correctCount} �� {filledSlots}.";
        }
        else
        {
            if (correctCount == slots.Length)
            {
                // ������������ �� ������ ������
                statusText.text = "�������!�� �������� ������� �������� ������ � ��������!";
                statusText.color = Color.forestGreen;
            ProgressService.Instance?.ReportGameCompleted(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, 1);
                if (finishPanel != null) finishPanel.SetActive(true);
            }
            else
            {
                statusText.text = $"�����! ���������: {correctCount} �� {slots.Length}. �������� �������� �������.";
                statusText.color = Color.red;
            }
        }
    }
    public void LoadLevelByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ResetGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}