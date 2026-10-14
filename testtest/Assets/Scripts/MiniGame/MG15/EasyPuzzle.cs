using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EasyPuzzle : MonoBehaviour
{
    [System.Serializable]
    public class PuzzleSlot
    {
        public Image slotImage;           // ������ �������� � �����
        public List<Sprite> allVariants;  // ������ ���� 4-� ��������
        public int correctIndex;          // ����� ����������� ������� (0, 1, 2 ��� 3)
        [HideInInspector] public int currentIndex = 0;
    }

    public List<PuzzleSlot> slots;
    public TMP_InputField finalWordInput;
    public TextMeshProUGUI resultText;
    public string correctWord ;

    // ���������� ��� ����� �� ���� �������� � �����
    public void ChangeImage(int slotIndex)
    {
        PuzzleSlot s = slots[slotIndex];
        s.currentIndex = (s.currentIndex + 1) % s.allVariants.Count; // ������� �� �����
        s.slotImage.sprite = s.allVariants[s.currentIndex];
    }
    public void LoadLevelByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void CheckResult()
    {
        int correctCount = 0;
        foreach (var s in slots)
        {
            if (s.currentIndex == s.correctIndex) correctCount++;
        }

        // 1. ������� ������ �� ��������� �������� TMP (�������� \u200B �� �������)
        string cleanUserText = finalWordInput.text.Replace("\u200B", "").Trim().ToLower();
        string cleanCorrectWord = correctWord.Trim().ToLower();

        bool wordOk = cleanUserText == cleanCorrectWord;

        // 2. ��� � ������� (������� ������, � ��� ������ ��� �����)
        Debug.Log($"������� ������: {correctCount}/{slots.Count}. ����� �����: {wordOk}");

        if (correctCount == slots.Count && wordOk)
        {
            resultText.text = "�������! �� ���������";
            resultText.color = Color.forestGreen;
            ProgressService.Instance?.ReportGameCompleted(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, 1);
        }
        else
        {
            // �������� ��� ����, ��� ������ (����� ������ �����)
            if (correctCount != slots.Count && !wordOk)
                resultText.text = "������ � ����� � � �����";
            else if (correctCount != slots.Count)
                resultText.text = "�������� ������� �������";
            else
                resultText.text = "����� ������� �������";

            resultText.color = Color.red;
        }
    }

    public void ResetPuzzle()
    {
        foreach (var s in slots)
        {
            s.currentIndex = Random.Range(0, s.allVariants.Count); // ������������
            s.slotImage.sprite = s.allVariants[s.currentIndex];
        }
        finalWordInput.text = "";
        resultText.text = "�������� ���� � ���������� ���";
        resultText.color = Color.black;
    }
}