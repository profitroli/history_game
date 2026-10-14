using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MemorialFinalManager : MonoBehaviour
{
    [System.Serializable]
    public class MemorialBlock
    {
        public Image iconImage;           // �������� �� ������
        public List<Sprite> allSprites;   // ������ ���� ���������� (�����, ������ � �.�.)
        public int correctSpriteIndex;    // ������ ����������� ���� � ������
        public TMP_InputField nameInput;  // ���� �����
        public string correctName;        // ������ ����� (��������, "������")

        [HideInInspector] public int currentIndex = 0;
    }

    public List<MemorialBlock> blocks;
    public TextMeshProUGUI statusText;

    private string originalMessage;

    void Start()
    {
        // ���������� �������� ����� �������, ��� �� �������
        if (statusText != null) originalMessage = statusText.text;
        ResetGame();
    }

    // ����� ��� ������-�������� (������ ������, �� �� ���������)
    public void ChangeMemorial(int blockIndex)
    {
        MemorialBlock b = blocks[blockIndex];
        b.currentIndex = (b.currentIndex + 1) % b.allSprites.Count;
        b.iconImage.sprite = b.allSprites[b.currentIndex];
    }

    // ������� �������� (�� ������ "���������")
    public void CheckAll()
    {
        int score = 0;

        foreach (var b in blocks)
        {
            // ������ ����� �� ��������� �������� TMP
            string userText = b.nameInput.text.Replace("\u200B", "").Trim().ToLower();
            string targetText = b.correctName.Trim().ToLower();

            bool isImageRight = (b.currentIndex == b.correctSpriteIndex);
            bool isWordRight = (userText == targetText);

            if (isImageRight && isWordRight)
            {
                score++;
                b.nameInput.image.color = Color.green;
            }
            else
            {
                b.nameInput.image.color = Color.red;
            }
        }

        if (score == blocks.Count)
        {
            statusText.text = "�������! �� ���������";
            statusText.color = Color.forestGreen;
            ProgressService.Instance?.ReportGameCompleted(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, 1);
        }
        else
        {
            statusText.text = "�����������! ���� ������";
            statusText.color = Color.red;
        }
    }
    public void LoadLevelByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ResetGame()
    {
        foreach (var b in blocks)
        {
            b.currentIndex = Random.Range(0, b.allSprites.Count);
            b.iconImage.sprite = b.allSprites[b.currentIndex];
            b.nameInput.text = "";
            b.nameInput.image.color = Color.white;
        }

        if (statusText != null)
        {
            statusText.text = originalMessage;
            statusText.color = Color.black;
        }
    }
}