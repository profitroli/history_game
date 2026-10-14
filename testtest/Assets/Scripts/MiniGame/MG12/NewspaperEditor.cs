using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewspaperEditor : MonoBehaviour
{
    [System.Serializable]
    public class NewspaperSlot
    {
        public string correctContent;
        public TextMeshProUGUI slotText;
        public Button slotButton;
        [HideInInspector] public string currentContent = "";
        [HideInInspector] public string initialText;
    }

    public List<NewspaperSlot> slots;
    public Button publishButton; // ���� ������ "� ������"
    public TextMeshProUGUI statusText; // ����� ��������� (������)
    public TextMeshProUGUI resultVerdictText; // ����� ���� ��� "��������/�����������"

    private string selectedMaterial = "";

    void Start()
    {
        
        foreach (var s in slots)
        {
            if (s.slotText != null)
                s.initialText = s.slotText.text;
        }
    }
    public void LoadLevelByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void SelectMaterial(string content)
    {
        selectedMaterial = content;
        statusText.text = "�������: " + content;
    }

    public void PlaceInSlot(int index)
    {
        if (string.IsNullOrEmpty(selectedMaterial)) return;

        slots[index].currentContent = selectedMaterial;
        slots[index].slotText.text = selectedMaterial;
        selectedMaterial = "";

        // ���������� ����� ������ ��� ��������������
        ColorBlock cb = slots[index].slotButton.colors;
        cb.normalColor = Color.white;
        cb.selectedColor = Color.white;
        slots[index].slotButton.colors = cb;

        CheckIfAllFilled();
    }

    void CheckIfAllFilled()
    {
        foreach (var s in slots) if (string.IsNullOrEmpty(s.currentContent)) return;
        if (publishButton != null) publishButton.interactable = true;
    }

    // ����� ��� ������ "� ������"
    public void FinalCheck()
    {
        int correctCount = 0;

        foreach (var s in slots)
        {
            ColorBlock cb = s.slotButton.colors;
            if (s.currentContent.Trim() == s.correctContent.Trim())
            {
                cb.normalColor = Color.green;
                cb.selectedColor = Color.green;
                correctCount++;
            }
            else
            {
                cb.normalColor = Color.red;
                cb.selectedColor = Color.red;
            }
            s.slotButton.colors = cb;
        }

        if (correctCount == slots.Count)
        {
            resultVerdictText.text = "�������� � ������!";
            resultVerdictText.color = Color.forestGreen;
            ProgressService.Instance?.ReportGameCompleted(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, 1);
        }
        else
        {
            resultVerdictText.text = "�����������! ���� ������";
            resultVerdictText.color = Color.red;
        }
    }

    public void ResetEditor()
    {
        selectedMaterial = "";
        if (resultVerdictText != null) resultVerdictText.text = "";

        foreach (var s in slots)
        {
            s.currentContent = "";
            s.slotText.text = s.initialText;

            ColorBlock cb = s.slotButton.colors;
            cb.normalColor = Color.white;
            cb.selectedColor = Color.white;
            s.slotButton.colors = cb;
        }
        if (publishButton != null) publishButton.interactable = false;
        statusText.text = "�������� �������� � �������� �� ���� � ������";
        statusText.color = Color.black;
    }
}