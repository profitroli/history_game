using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LineManager : MonoBehaviour
{
    [Header("��������� UI")]
    public GameObject linePrefab;      // ������ Image (Pivot 0, 0.5)
    public RectTransform canvasRect;  // ������ �� ��� Canvas
    public TextMeshProUGUI statusText; // ����� ��� ������ ���������

    [Header("��������� ����")]
    public int totalConnectionsNeeded = 7; // ������� ��� ����� ��������� (� ��� 7)
    public string mainMenuSceneName = "MainMenu"; // �������� ������� �����

    private GameObject currentLine;
    private ConnectionDot startDot;
    public string originalStatusText;

    void Start()
    {
        // ���������� �����, ������� �� �������� � Unity ����� ��������
        if (statusText != null)
        {
            originalStatusText = statusText.text;
        }
    }
    // ������ ��� �������� ������ � ��������� �����������
    private List<GameObject> activeLines = new List<GameObject>();
    private List<KeyValuePair<ConnectionDot, ConnectionDot>> connections = new List<KeyValuePair<ConnectionDot, ConnectionDot>>();

    /// <summary>
    /// �������� ��������� ����� �� �����
    /// </summary>
    public void StartDrawing(ConnectionDot dot)
    {
        startDot = dot;

        // ������� ����� ��� ������� Canvas
        currentLine = Instantiate(linePrefab, canvasRect);

        // ���������� ����� � ����� ���� �������� ������ Canvas, 
        // ����� ��� ���� ��������� ��� �������
        currentLine.transform.SetAsLastSibling();

        // �����: ��������� Raycast � �����, ����� ��� �� ����������� ������ �����
        if (currentLine.TryGetComponent<Image>(out Image img))
        {
            img.raycastTarget = false;
        }
    }

    void Update()
    {
        // ���� �� ������ ����� �����, ��������� � ��������� �� ������
        if (currentLine != null)
        {
            UpdateLine(Input.mousePosition);
        }
    }

    /// <summary>
    /// ��������� ��������� �����
    /// </summary>
    public void EndDrawing(ConnectionDot endDot)
    {
        // ���������: ��������� �� ��� ������, �� �� �� �� ��� ����� � � ������ �� ��� �������
        if (endDot != null && endDot != startDot && endDot.isLeft != startDot.isLeft)
        {
            // ��������� ����� � �������� �����
            UpdateLine(endDot.transform.position);

            activeLines.Add(currentLine);
            connections.Add(new KeyValuePair<ConnectionDot, ConnectionDot>(startDot, endDot));

            currentLine = null;
        }
        else
        {
            // ���� ������� �� ��������� (��������� � �������) � ������� �����
            if (currentLine != null) Destroy(currentLine);
            currentLine = null;
        }
    }

    /// <summary>
    /// �������������� ������ ���������, ����� � �������� �����
    /// </summary>
    void UpdateLine(Vector3 targetWorldPos)
    {
        if (currentLine == null) return;

        RectTransform rt = currentLine.GetComponent<RectTransform>();

        // ������������ ������� ���������� � ��������� ���������� Canvas
        Vector2 localStartPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, startDot.transform.position, null, out localStartPos);

        Vector2 localTargetPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, targetWorldPos, null, out localTargetPos);

        // ������ ������ ����� � ��������� �����
        rt.anchoredPosition = localStartPos;

        // ��������� ������ �����������
        Vector2 direction = localTargetPos - localStartPos;

        // ������������� ����� (������) �����
        rt.sizeDelta = new Vector2(direction.magnitude, 5f); // 5f � ������� �����

        // ������������� ���� ��������
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rt.rotation = Quaternion.Euler(0, 0, angle);
    }

    /// <summary>
    /// �������� ����������� (��� ������ "���������")
    /// </summary>
    public void CheckResults()
    {
 

        if (connections.Count == 0)
        {
            statusText.text = "������� ��������� �����!";
            statusText.color = Color.black;
            return;
        }

        if (connections.Count < totalConnectionsNeeded)
        {
            statusText.text = "��������� ��� �����!";
            statusText.color = Color.black;
            return;
        }

        int correctCount = 0;
        foreach (var pair in connections)
        {
            // ���������� ID ����� � ������ �����
            if (pair.Key.id == pair.Value.id)
            {
                correctCount++;
            }
        }

        if (correctCount == totalConnectionsNeeded)
        {
            statusText.text = "�������!!�� ���������!";
            statusText.color = Color.forestGreen;
            ProgressService.Instance?.ReportGameCompleted(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, 1);
        }
        else
        {
            statusText.text = "�����������, �������� ��� ���";
            statusText.color = Color.red;
        }
    }

    /// <summary>
    /// ������ ����� ���� (��� ������ "�����")
    /// </summary>
    public void ResetGame()
    {
        // ������� ��� ������� �����
        foreach (GameObject line in activeLines)
        {
            if (line != null) Destroy(line);
        }

        activeLines.Clear();
        connections.Clear();

        if (statusText != null) { statusText.text = originalStatusText; statusText.color = Color.black; }
    }

    /// <summary>
    /// ������� � ���� (��� ������ "�����")
    /// </summary>
    public void LoadLevelByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}