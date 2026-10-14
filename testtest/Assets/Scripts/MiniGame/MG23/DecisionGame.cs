using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class DecisionGame : MonoBehaviour
{
    [System.Serializable]
    public class DecisionNode
    {
        public string situation;   // �������� ��������
        public string optionA;     // ����� �� ������ �
        public string optionB;     // ����� �� ������ �
        public bool isCorrectA;    // ����� ������� ����������� ������
        public string resultInfo;  // ������������ ������� ����� ������
    }

    public List<DecisionNode> nodes;
    public TextMeshProUGUI situationText;
    public TextMeshProUGUI btnAText;
    public TextMeshProUGUI btnBText;
    public TextMeshProUGUI infoText;
    public GameObject finishPanel;       // ���� ������ (������)
    public TextMeshProUGUI resultText;   // �������� ����� �� ������

    public float speed = 250f;
    private int currentStep = 0;

    void Start() => ShowNode();
    void FinishGame()
    {
        ProgressService.Instance?.ReportGameCompleted(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, 1);

        if (finishPanel != null)
        {
            finishPanel.SetActive(true); // �������� ���� ������
            resultText.text = $"�����! ������� ���������, ��������� �������!";
        }
        
    }
    public void MakeDecision(bool choiceA)
    {
        if (currentStep >= nodes.Count) return;

        bool wasCorrect = (choiceA == nodes[currentStep].isCorrectA);

        if (wasCorrect)
        {
            infoText.text = "���������: " + nodes[currentStep].resultInfo;
            infoText.color = Color.forestGreen;
            currentStep++;
            Invoke("ShowNode", 9f); // ���� ����� ��������� �������
        }
        else
        {
            infoText.text = "������: ���� ���� �� ������������� ������� ��.";
            infoText.color = Color.red;
        }
    }
    public void LoadLevelByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    void ShowNode()
    {
        if (currentStep < nodes.Count)
        {
            situationText.text = nodes[currentStep].situation;
            btnAText.text = nodes[currentStep].optionA;
            btnBText.text = nodes[currentStep].optionB;
            infoText.text = "�������� �����...";
            infoText.color = Color.black;
        }
        else
        {
            FinishGame();
        }
    }
}