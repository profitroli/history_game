using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniGameAssignController : MonoBehaviour
{
    private Transform listContent;
    private TMP_Text titleText;
    private string currentGroupId;
    private string currentGroupName;
    private HashSet<string> selectedGames = new HashSet<string>();

    public void Show(string groupId, string groupName, Transform parentContent)
    {
        currentGroupId = groupId;
        currentGroupName = groupName;
        gameObject.SetActive(true);
        BuildUI(parentContent);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void BuildUI(Transform parentContent)
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        var rt = GetComponent<RectTransform>();
        if (rt == null) rt = gameObject.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;

        var vlg = gameObject.AddComponent<VerticalLayoutGroup>();
        if (vlg == null) vlg = gameObject.GetComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(15, 15, 10, 10);
        vlg.spacing = 8;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;

        var bg = gameObject.GetComponent<Image>();
        if (bg == null) bg = gameObject.AddComponent<Image>();
        bg.color = new Color(0.98f, 0.96f, 0.92f, 1f);

        titleText = CreateLabel(transform, $"Мини-игры для: {currentGroupName}", 28, FontStyles.Bold, new Color(0.24f, 0.16f, 0.1f));
        CreateLabel(transform, "Выберите мини-игры для назначения группе:", 22, FontStyles.Normal, new Color(0.4f, 0.35f, 0.3f));

        var scrollGO = new GameObject("Scroll");
        scrollGO.transform.SetParent(transform, false);
        scrollGO.AddComponent<RectTransform>();
        var scrollLE = scrollGO.AddComponent<LayoutElement>();
        scrollLE.flexibleHeight = 1;
        scrollLE.minHeight = 200;
        var scrollImg = scrollGO.AddComponent<Image>();
        scrollImg.color = new Color(1, 1, 1, 0.3f);
        var scrollRect = scrollGO.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;

        var viewportGO = new GameObject("Viewport");
        viewportGO.transform.SetParent(scrollGO.transform, false);
        var vpRT = viewportGO.AddComponent<RectTransform>();
        vpRT.anchorMin = Vector2.zero;
        vpRT.anchorMax = Vector2.one;
        vpRT.offsetMin = vpRT.offsetMax = Vector2.zero;
        viewportGO.AddComponent<Image>();
        viewportGO.AddComponent<Mask>().showMaskGraphic = false;
        scrollRect.viewport = vpRT;

        var contentGO = new GameObject("Content");
        contentGO.transform.SetParent(viewportGO.transform, false);
        var contentRT = contentGO.AddComponent<RectTransform>();
        contentRT.anchorMin = new Vector2(0, 1);
        contentRT.anchorMax = Vector2.one;
        contentRT.pivot = new Vector2(0.5f, 1);
        contentRT.offsetMin = contentRT.offsetMax = Vector2.zero;
        var contentVLG = contentGO.AddComponent<VerticalLayoutGroup>();
        contentVLG.spacing = 6;
        contentVLG.padding = new RectOffset(8, 8, 8, 8);
        contentVLG.childForceExpandWidth = true;
        contentVLG.childForceExpandHeight = false;
        contentVLG.childControlWidth = true;
        contentVLG.childControlHeight = true;
        var csf = contentGO.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        scrollRect.content = contentRT;
        listContent = contentGO.transform;

        string lastCategory = "";
        foreach (var game in MiniGameCatalog.Games)
        {
            if (game.category != lastCategory)
            {
                lastCategory = game.category;
                CreateLabel(listContent, game.category.ToUpper(), 20, FontStyles.Bold, new Color(0.3f, 0.5f, 0.22f));
            }
            CreateGameRow(game);
        }

        var btnRow = new GameObject("BtnRow");
        btnRow.transform.SetParent(transform, false);
        btnRow.AddComponent<RectTransform>();
        var btnHLG = btnRow.AddComponent<HorizontalLayoutGroup>();
        btnHLG.spacing = 15;
        btnHLG.childForceExpandWidth = true;
        btnHLG.childControlWidth = true;
        btnHLG.childControlHeight = true;
        btnRow.AddComponent<LayoutElement>().preferredHeight = 45;

        CreateButton(btnRow.transform, $"Назначить ({selectedGames.Count})", new Color(0.3f, 0.5f, 0.22f), OnAssign);
        CreateButton(btnRow.transform, "Выбрать все", new Color(0.4f, 0.55f, 0.7f), OnSelectAll);
        CreateButton(btnRow.transform, "Закрыть", new Color(0.55f, 0.4f, 0.35f), Hide);
    }

    private void CreateGameRow(MiniGameCatalog.GameInfo game)
    {
        var rowGO = new GameObject($"Game_{game.sceneName}");
        rowGO.transform.SetParent(listContent, false);
        rowGO.AddComponent<RectTransform>();
        var hlg = rowGO.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 10;
        hlg.padding = new RectOffset(8, 8, 4, 4);
        hlg.childForceExpandWidth = false;
        hlg.childControlWidth = false;
        hlg.childControlHeight = true;
        rowGO.AddComponent<Image>().color = new Color(0.95f, 0.93f, 0.9f);
        rowGO.AddComponent<LayoutElement>().preferredHeight = 40;

        var toggleGO = new GameObject("Toggle");
        toggleGO.transform.SetParent(rowGO.transform, false);
        var toggleRT = toggleGO.AddComponent<RectTransform>();
        toggleRT.sizeDelta = new Vector2(30, 30);
        var toggleBG = toggleGO.AddComponent<Image>();
        toggleBG.color = Color.white;
        var toggle = toggleGO.AddComponent<Toggle>();
        toggleGO.AddComponent<LayoutElement>().preferredWidth = 30;

        var checkGO = new GameObject("Checkmark");
        checkGO.transform.SetParent(toggleGO.transform, false);
        var checkRT = checkGO.AddComponent<RectTransform>();
        checkRT.anchorMin = new Vector2(0.15f, 0.15f);
        checkRT.anchorMax = new Vector2(0.85f, 0.85f);
        checkRT.offsetMin = checkRT.offsetMax = Vector2.zero;
        var checkImg = checkGO.AddComponent<Image>();
        checkImg.color = new Color(0.3f, 0.5f, 0.22f);
        toggle.graphic = checkImg;

        var nameGO = new GameObject("Name");
        nameGO.transform.SetParent(rowGO.transform, false);
        nameGO.AddComponent<RectTransform>();
        var nameTMP = nameGO.AddComponent<TextMeshProUGUI>();
        nameTMP.text = game.displayName;
        nameTMP.fontSize = 22;
        nameTMP.color = new Color(0.2f, 0.15f, 0.1f);
        nameGO.AddComponent<LayoutElement>().preferredWidth = 400;

        var lecGO = new GameObject("Lecture");
        lecGO.transform.SetParent(rowGO.transform, false);
        lecGO.AddComponent<RectTransform>();
        var lecTMP = lecGO.AddComponent<TextMeshProUGUI>();
        lecTMP.text = $"Лекция {game.lectureNumber}";
        lecTMP.fontSize = 18;
        lecTMP.color = new Color(0.5f, 0.45f, 0.4f);
        lecGO.AddComponent<LayoutElement>().preferredWidth = 120;

        string sceneName = game.sceneName;
        toggle.onValueChanged.AddListener((isOn) =>
        {
            if (isOn) selectedGames.Add(sceneName);
            else selectedGames.Remove(sceneName);
        });
    }

    private void OnAssign()
    {
        if (selectedGames.Count == 0)
        {
            ToastManager.Show("Выберите хотя бы одну мини-игру");
            return;
        }
        var gamesList = new List<string>(selectedGames);
        _ = SaveAssignedGames(gamesList);
    }

    private async System.Threading.Tasks.Task SaveAssignedGames(List<string> gameScenes)
    {
        var assignment = new AssignmentData
        {
            assignmentId = System.Guid.NewGuid().ToString("N").Substring(0, 12),
            name = $"Мини-игры ({gameScenes.Count} шт.)",
            groupId = currentGroupId,
            teacherUid = SessionManager.Instance.UserId,
            teacherName = SessionManager.Instance.DisplayName,
            questions = new List<QuestionData>(),
            active = true,
            createdAt = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        foreach (var scene in gameScenes)
        {
            var game = MiniGameCatalog.Games.Find(g => g.sceneName == scene);
            assignment.questions.Add(new QuestionData
            {
                questionType = 3,
                text = game.displayName,
                optionA = scene,
                optionB = game.category,
                optionC = "",
                optionD = "",
                correctIndex = 0
            });
        }

        var result = await AssignmentService.CreateAssignment(assignment.name, currentGroupId, assignment.questions);
        if (result != null)
        {
            ToastManager.Show($"Назначено {gameScenes.Count} мини-игр для {currentGroupName}");
            selectedGames.Clear();
            Hide();
        }
    }

    private void OnSelectAll()
    {
        var toggles = listContent.GetComponentsInChildren<Toggle>(true);
        bool allOn = selectedGames.Count == MiniGameCatalog.Games.Count;
        foreach (var t in toggles)
            t.isOn = !allOn;
    }

    private TMP_Text CreateLabel(Transform parent, string text, int size, FontStyles style, Color color)
    {
        var go = new GameObject("Label");
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.fontStyle = style;
        tmp.color = color;
        go.AddComponent<LayoutElement>().preferredHeight = size + 10;
        return tmp;
    }

    private void CreateButton(Transform parent, string text, Color bgColor, UnityEngine.Events.UnityAction action)
    {
        var go = new GameObject("Btn");
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        go.AddComponent<Image>().color = bgColor;
        var btn = go.AddComponent<Button>();
        btn.onClick.AddListener(action);

        var txt = new GameObject("T").AddComponent<TextMeshProUGUI>();
        txt.transform.SetParent(go.transform, false);
        txt.text = text;
        txt.fontSize = 22;
        txt.color = Color.white;
        txt.alignment = TextAlignmentOptions.Center;
        var trt = txt.GetComponent<RectTransform>();
        trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one;
        trt.offsetMin = trt.offsetMax = Vector2.zero;
    }
}
