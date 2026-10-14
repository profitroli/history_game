#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Tools > Построить сцены — создаёт LoginScene и TeacherDashboardScene
/// со всеми UI-элементами и привязанными контроллерами.
/// После создания сцен запустите пункт 3 для добавления в Build Settings.
///
/// Панели StudentDashboard (в MainMenu) и Profile (в SettingsScene)
/// добавляются вручную — см. unity-editor-instructions.md секции 11-12.
/// </summary>
public static class SceneBuilder
{
    static readonly Color ColBg       = new Color(0.13f, 0.15f, 0.20f);
    static readonly Color ColPanel    = new Color(0.18f, 0.21f, 0.28f, 0.97f);
    static readonly Color ColInput    = new Color(0.12f, 0.14f, 0.19f);
    static readonly Color ColBlue     = new Color(0.25f, 0.48f, 0.85f);
    static readonly Color ColGray     = new Color(0.40f, 0.43f, 0.50f);
    static readonly Color ColGreen    = new Color(0.22f, 0.65f, 0.35f);
    static readonly Color ColRed      = new Color(0.80f, 0.25f, 0.25f);
    static readonly Color ColOrange   = new Color(0.85f, 0.55f, 0.15f);
    static readonly Color ColTextDim  = new Color(0.65f, 0.67f, 0.72f);
    static readonly Color ColError    = new Color(1.0f, 0.40f, 0.40f);

    // ======================== MENU ========================

    [MenuItem("Tools/Построить сцены/1 — LoginScene")]
    static void Menu_Login()
    {
        if (!EditorUtility.DisplayDialog("Scene Builder",
            "LoginScene.unity will be created in Assets/Scenes/.\nContinue?", "Yes", "Cancel")) return;
        BuildLoginScene();
    }

    [MenuItem("Tools/Построить сцены/2 — TeacherDashboardScene")]
    static void Menu_Teacher()
    {
        if (!EditorUtility.DisplayDialog("Scene Builder",
            "TeacherDashboardScene.unity will be created in Assets/Scenes/.\nContinue?", "Yes", "Cancel")) return;
        BuildTeacherDashboardScene();
    }

    [MenuItem("Tools/Построить сцены/3 — Add to Build Settings")]
    static void Menu_BuildSettings()
    {
        AddToBuildSettings();
    }

    // ==================== LOGIN SCENE ====================

    static void BuildLoginScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        AddCamera();
        AddEventSystem();
        var canvas = MakeCanvas();

        var bg = MakeImg(canvas.transform, "Background", ColBg);
        Stretch(bg);

        // App title at top
        var appTitle = MakeTMP(canvas.transform, "AppTitle",
            "История белорусской\nгосударственности", 30,
            TextAlignmentOptions.Center, Color.white);
        var atRT = RT(appTitle);
        atRT.anchorMin = new Vector2(0.1f, 0.85f);
        atRT.anchorMax = new Vector2(0.9f, 0.97f);
        atRT.offsetMin = atRT.offsetMax = Vector2.zero;

        // ---- Login Panel ----
        var loginPanel = MakePanel(canvas.transform, "LoginPanel", 480, 430);
        Center(loginPanel);
        var lVLG = AddVLG(loginPanel, 14, new RectOffset(30, 30, 25, 20));

        MakeTMP(loginPanel.transform, "Title", "Вход", 26,
            TextAlignmentOptions.Center, Color.white).gameObject.AddComponent<LayoutElement>().preferredHeight = 38;
        var loginEmailInput    = MakeInputField(loginPanel.transform, "LoginEmailInput", "Email", 45, TMP_InputField.ContentType.EmailAddress);
        var loginPasswordInput = MakeInputField(loginPanel.transform, "LoginPasswordInput", "Пароль", 45, TMP_InputField.ContentType.Password);
        var loginButton        = MakeBtn(loginPanel.transform, "LoginButton",        "Войти",                    ColBlue, 0, 46);
        var goToRegisterButton = MakeBtn(loginPanel.transform, "GoToRegisterButton", "Регистрация",            ColGray, 0, 40);
        var selfStudyButton    = MakeBtn(loginPanel.transform, "SelfStudyButton",    "Самостоятельное изучение", ColGray, 0, 40);
        var loginErrorText     = MakeTMP(loginPanel.transform, "LoginErrorText", "", 15,
            TextAlignmentOptions.Center, ColError);
        loginErrorText.gameObject.AddComponent<LayoutElement>().preferredHeight = 28;

        // ---- Register Panel ----
        var registerPanel = MakePanel(canvas.transform, "RegisterPanel", 480, 620);
        Center(registerPanel);
        AddVLG(registerPanel, 11, new RectOffset(30, 30, 25, 20));

        MakeTMP(registerPanel.transform, "Title", "Регистрация", 26,
            TextAlignmentOptions.Center, Color.white).gameObject.AddComponent<LayoutElement>().preferredHeight = 38;
        var regNameInput      = MakeInputField(registerPanel.transform, "RegNameInput",      "Ваше имя",                      45);
        var regEmailInput     = MakeInputField(registerPanel.transform, "RegEmailInput",     "Email",                       45, TMP_InputField.ContentType.EmailAddress);
        var regPasswordInput  = MakeInputField(registerPanel.transform, "RegPasswordInput",  "Пароль (мин. 6 символов)", 45, TMP_InputField.ContentType.Password);
        var regGroupCodeInput = MakeInputField(registerPanel.transform, "RegGroupCodeInput", "Код группы (необязательно)", 45);
        var regTeacherToggle  = MakeToggleUI(registerPanel.transform, "RegTeacherToggle", "Я преподаватель");
        var registerButton    = MakeBtn(registerPanel.transform, "RegisterButton",    "Зарегистрироваться", ColBlue,  0, 46);
        var backToLoginButton = MakeBtn(registerPanel.transform, "BackToLoginButton", "← Назад ко входу",       ColGray, 0, 38);
        var regErrorText      = MakeTMP(registerPanel.transform, "RegErrorText", "", 15,
            TextAlignmentOptions.Center, ColError);
        regErrorText.gameObject.AddComponent<LayoutElement>().preferredHeight = 28;
        registerPanel.SetActive(false);

        // ---- Role Selection Panel ----
        var rolePanel = MakePanel(canvas.transform, "RoleSelectionPanel", 480, 370);
        Center(rolePanel);
        AddVLG(rolePanel, 14, new RectOffset(30, 30, 25, 20));

        MakeTMP(rolePanel.transform, "Title", "Выберите тип обучения", 26,
            TextAlignmentOptions.Center, Color.white).gameObject.AddComponent<LayoutElement>().preferredHeight = 38;
        var selfStudyNameInput    = MakeInputField(rolePanel.transform, "SelfStudyNameInput", "Ваше имя (необязательно)", 45);
        var schoolStudentButton   = MakeBtn(rolePanel.transform, "SchoolStudentButton",   "Школьник",     ColBlue, 0, 50);
        var universityStudentButton = MakeBtn(rolePanel.transform, "UniversityStudentButton", "Студент ВУЗа", ColBlue, 0, 50);
        var backFromRoleButton    = MakeBtn(rolePanel.transform, "BackFromRoleButton",    "← Назад",   ColGray, 0, 38);
        rolePanel.SetActive(false);

        // ---- Loading Panel ----
        var loadingPanel = MakePanel(canvas.transform, "LoadingPanel", 300, 110);
        Center(loadingPanel);
        var loadTMP = MakeTMP(loadingPanel.transform, "LoadingText", "Загрузка...", 22,
            TextAlignmentOptions.Center, Color.white);
        Stretch(loadTMP.gameObject);
        loadingPanel.SetActive(false);

        // ---- Wire LoginController ----
        var ctrl = canvas.gameObject.AddComponent<LoginController>();
        var so = new SerializedObject(ctrl);
        W(so, "loginPanel",            loginPanel);
        W(so, "registerPanel",         registerPanel);
        W(so, "roleSelectionPanel",    rolePanel);
        W(so, "loadingPanel",          loadingPanel);
        W(so, "loginEmailInput",       loginEmailInput);
        W(so, "loginPasswordInput",    loginPasswordInput);
        W(so, "loginButton",           loginButton);
        W(so, "goToRegisterButton",    goToRegisterButton);
        W(so, "selfStudyButton",       selfStudyButton);
        W(so, "loginErrorText",        loginErrorText);
        W(so, "regNameInput",          regNameInput);
        W(so, "regEmailInput",         regEmailInput);
        W(so, "regPasswordInput",      regPasswordInput);
        W(so, "regGroupCodeInput",     regGroupCodeInput);
        W(so, "regTeacherToggle",      regTeacherToggle);
        W(so, "registerButton",        registerButton);
        W(so, "backToLoginButton",     backToLoginButton);
        W(so, "regErrorText",          regErrorText);
        W(so, "selfStudyNameInput",    selfStudyNameInput);
        W(so, "schoolStudentButton",   schoolStudentButton);
        W(so, "universityStudentButton", universityStudentButton);
        W(so, "backFromRoleButton",    backFromRoleButton);
        so.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/LoginScene.unity");
        Debug.Log("[SceneBuilder] LoginScene created at Assets/Scenes/LoginScene.unity");
        EditorUtility.DisplayDialog("Scene Builder", "LoginScene created!\nAssets/Scenes/LoginScene.unity", "OK");
    }

    // =============== TEACHER DASHBOARD SCENE ===============

    static void BuildTeacherDashboardScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        AddCamera();
        AddEventSystem();
        var canvas = MakeCanvas();

        var bg = MakeImg(canvas.transform, "Background", ColBg);
        Stretch(bg);

        // ---- Header bar ----
        var header = MakeImg(canvas.transform, "HeaderPanel", new Color(0.15f, 0.18f, 0.24f));
        var hRT = RT(header);
        hRT.anchorMin = new Vector2(0, 0.89f);
        hRT.anchorMax = Vector2.one;
        hRT.offsetMin = hRT.offsetMax = Vector2.zero;
        var hLayout = header.AddComponent<HorizontalLayoutGroup>();
        hLayout.padding = new RectOffset(20, 20, 10, 10);
        hLayout.spacing = 12;
        hLayout.childAlignment = TextAnchor.MiddleLeft;
        hLayout.childForceExpandWidth = false;
        hLayout.childForceExpandHeight = false;
        hLayout.childControlWidth = false;
        hLayout.childControlHeight = true;

        var headerText = MakeTMP(header.transform, "HeaderText",
            "Кабинет преподавателя", 22,
            TextAlignmentOptions.Left, Color.white);
        SetSize(headerText, 380, 40);

        var showArchivedToggle = MakeToggleUI(header.transform, "ShowArchivedToggle", "Архивные");
        SetSize(showArchivedToggle, 160, 35);

        var createGroupButton     = MakeBtn(header.transform, "CreateGroupButton",     "+ Группа",    ColGreen, 130, 38);
        var previewAsStudentButton = MakeBtn(header.transform, "PreviewAsStudentButton", "Как ученик", ColGray,  120, 38);
        var logoutButton          = MakeBtn(header.transform, "LogoutButton",          "Выход",         ColRed,   95, 38);

        // ---- Groups scroll area ----
        var (groupsScroll, groupsContent) = MakeScroll(canvas.transform, "GroupsScrollView");
        var gsRT = RT(groupsScroll.gameObject);
        gsRT.anchorMin = new Vector2(0.02f, 0.02f);
        gsRT.anchorMax = new Vector2(0.98f, 0.87f);
        gsRT.offsetMin = gsRT.offsetMax = Vector2.zero;

        // ---- Create Group Panel (overlay) ----
        var createGroupPanel = MakePanel(canvas.transform, "CreateGroupPanel", 500, 480);
        Center(createGroupPanel);
        AddVLG(createGroupPanel, 12, new RectOffset(30, 30, 22, 22));

        MakeTMP(createGroupPanel.transform, "Title", "Создание группы", 24,
            TextAlignmentOptions.Center, Color.white).gameObject.AddComponent<LayoutElement>().preferredHeight = 36;
        var groupNameInput     = MakeInputField(createGroupPanel.transform, "GroupNameInput",   "Название группы",    45);
        var institutionInput   = MakeInputField(createGroupPanel.transform, "InstitutionInput", "Учебное заведение", 45);
        var schoolProgramToggle = MakeToggleUI(createGroupPanel.transform, "SchoolProgramToggle", "Школьная программа");
        var confirmCreateButton = MakeBtn(createGroupPanel.transform, "ConfirmCreateButton", "Создать",  ColGreen, 0, 45);
        var cancelCreateButton  = MakeBtn(createGroupPanel.transform, "CancelCreateButton",  "Отмена", ColGray,  0, 38);

        var codeDisplayPanel = MakePanel(createGroupPanel.transform, "CodeDisplayPanel", 0, 0);
        var cdVLG = codeDisplayPanel.AddComponent<VerticalLayoutGroup>();
        cdVLG.padding = new RectOffset(10, 10, 8, 8);
        cdVLG.childForceExpandWidth = true;
        var cdLE = codeDisplayPanel.AddComponent<LayoutElement>();
        cdLE.preferredHeight = 70;
        var createdCodeText = MakeTMP(codeDisplayPanel.transform, "CreatedCodeText", "", 17,
            TextAlignmentOptions.Center, ColGreen);
        createdCodeText.gameObject.AddComponent<LayoutElement>().preferredHeight = 55;
        codeDisplayPanel.SetActive(false);

        createGroupPanel.SetActive(false);

        // ---- Group Detail Panel (overlay) ----
        var groupDetailPanel = MakePanel(canvas.transform, "GroupDetailPanel", 920, 680);
        Center(groupDetailPanel);
        AddVLG(groupDetailPanel, 10, new RectOffset(25, 25, 20, 18));

        var groupDetailName = MakeTMP(groupDetailPanel.transform, "GroupDetailName", "", 24,
            TextAlignmentOptions.Left, Color.white);
        groupDetailName.gameObject.AddComponent<LayoutElement>().preferredHeight = 34;

        // Info row
        var infoRow = MakeUIGO("InfoRow", groupDetailPanel.transform);
        infoRow.AddComponent<LayoutElement>().preferredHeight = 28;
        var irLayout = infoRow.AddComponent<HorizontalLayoutGroup>();
        irLayout.spacing = 20;
        irLayout.childForceExpandWidth = false;
        irLayout.childControlWidth = false;
        var groupDetailCode = MakeTMP(infoRow.transform, "GroupDetailCode", "", 16,
            TextAlignmentOptions.Left, ColTextDim);
        SetSize(groupDetailCode, 200, 28);
        var groupDetailProgram = MakeTMP(infoRow.transform, "GroupDetailProgram", "", 16,
            TextAlignmentOptions.Left, ColTextDim);
        SetSize(groupDetailProgram, 260, 28);

        // Button row
        var btnRow = MakeUIGO("ButtonRow", groupDetailPanel.transform);
        btnRow.AddComponent<LayoutElement>().preferredHeight = 40;
        var brLayout = btnRow.AddComponent<HorizontalLayoutGroup>();
        brLayout.spacing = 10;
        brLayout.childForceExpandWidth = false;
        brLayout.childControlWidth = false;
        brLayout.childControlHeight = true;

        var backFromDetailButton = MakeBtn(btnRow.transform, "BackFromDetailButton", "← Назад",      ColGray,   110, 38);
        var refreshButton        = MakeBtn(btnRow.transform, "RefreshButton",        "Обновить",     ColBlue,   115, 38);
        var exportCsvButton      = MakeBtn(btnRow.transform, "ExportCsvButton",      "CSV",          ColOrange,  75, 38);
        var archiveGroupButton   = MakeBtn(btnRow.transform, "ArchiveGroupButton",   "Архивировать", ColRed,    140, 38);

        // Students scroll
        var (studentsScroll, studentsContent) = MakeScroll(groupDetailPanel.transform, "StudentsScrollView");
        studentsScroll.gameObject.AddComponent<LayoutElement>().flexibleHeight = 1;

        groupDetailPanel.SetActive(false);

        // ---- Wire TeacherDashboardController ----
        var ctrl = canvas.gameObject.AddComponent<TeacherDashboardController>();
        var so = new SerializedObject(ctrl);
        W(so, "headerText",            headerText);
        W(so, "createGroupButton",     createGroupButton);
        W(so, "logoutButton",          logoutButton);
        W(so, "previewAsStudentButton", previewAsStudentButton);
        W(so, "groupsListContent",     groupsContent);
        // groupCardPrefab — null, controller creates cards dynamically
        W(so, "createGroupPanel",      createGroupPanel);
        W(so, "groupNameInput",        groupNameInput);
        W(so, "schoolProgramToggle",   schoolProgramToggle);
        W(so, "confirmCreateButton",   confirmCreateButton);
        W(so, "cancelCreateButton",    cancelCreateButton);
        W(so, "createdCodeText",       createdCodeText);
        W(so, "codeDisplayPanel",      codeDisplayPanel);
        W(so, "groupDetailPanel",      groupDetailPanel);
        W(so, "groupDetailName",       groupDetailName);
        W(so, "groupDetailCode",       groupDetailCode);
        W(so, "groupDetailProgram",    groupDetailProgram);
        W(so, "studentsListContent",   studentsContent);
        // studentRowPrefab — null, controller creates rows dynamically
        W(so, "backFromDetailButton",  backFromDetailButton);
        W(so, "refreshButton",         refreshButton);
        W(so, "exportCsvButton",       exportCsvButton);
        W(so, "archiveGroupButton",    archiveGroupButton);
        W(so, "showArchivedToggle",    showArchivedToggle);
        W(so, "institutionInput",      institutionInput);
        so.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/TeacherDashboardScene.unity");
        Debug.Log("[SceneBuilder] TeacherDashboardScene created at Assets/Scenes/TeacherDashboardScene.unity");
        EditorUtility.DisplayDialog("Scene Builder", "TeacherDashboardScene created!\nAssets/Scenes/TeacherDashboardScene.unity", "OK");
    }

    // ================== BUILD SETTINGS ==================

    static void AddToBuildSettings()
    {
        var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        string[] newPaths = {
            "Assets/Scenes/LoginScene.unity",
            "Assets/Scenes/TeacherDashboardScene.unity"
        };
        int added = 0;
        foreach (var path in newPaths)
        {
            if (!System.IO.File.Exists(
                System.IO.Path.Combine(Application.dataPath, "..", path)))
            {
                Debug.LogWarning($"[SceneBuilder] Scene not found: {path}. Create it first.");
                continue;
            }
            if (scenes.Exists(s => s.path == path))
            {
                Debug.Log($"[SceneBuilder] Already in Build Settings: {path}");
                continue;
            }
            scenes.Insert(Mathf.Min(1, scenes.Count),
                new EditorBuildSettingsScene(path, true));
            added++;
        }

        if (added > 0)
        {
            EditorBuildSettings.scenes = scenes.ToArray();
            Debug.Log($"[SceneBuilder] Added {added} scene(s) to Build Settings");
        }

        EditorUtility.DisplayDialog("Scene Builder",
            added > 0
                ? $"Added {added} scene(s) to Build Settings.\nOpen File > Build Settings to verify order."
                : "Scenes already in Build Settings (or not found).",
            "OK");
    }

    // ================ UI CREATION HELPERS ================

    static void AddCamera()
    {
        var go = new GameObject("Main Camera");
        go.tag = "MainCamera";
        var cam = go.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = ColBg;
        cam.orthographic = true;
    }

    static void AddEventSystem()
    {
        var go = new GameObject("EventSystem");
        go.AddComponent<EventSystem>();
        go.AddComponent<StandaloneInputModule>();
    }

    static Canvas MakeCanvas()
    {
        var go = new GameObject("Canvas");
        var c = go.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        go.AddComponent<GraphicRaycaster>();
        return c;
    }

    static GameObject MakeUIGO(string name, Transform parent)
    {
        var go = new GameObject(name);
        go.AddComponent<RectTransform>();
        go.transform.SetParent(parent, false);
        return go;
    }

    static GameObject MakePanel(Transform parent, string name, float w, float h)
    {
        var go = MakeUIGO(name, parent);
        RT(go).sizeDelta = new Vector2(w, h);
        go.AddComponent<Image>().color = ColPanel;
        return go;
    }

    static GameObject MakeImg(Transform parent, string name, Color color)
    {
        var go = MakeUIGO(name, parent);
        go.AddComponent<Image>().color = color;
        return go;
    }

    static TextMeshProUGUI MakeTMP(Transform parent, string name, string text,
        int fontSize, TextAlignmentOptions align, Color color)
    {
        var go = MakeUIGO(name, parent);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = align;
        tmp.color = color;
        tmp.enableAutoSizing = false;
        tmp.overflowMode = TextOverflowModes.Ellipsis;
        return tmp;
    }

    static Button MakeBtn(Transform parent, string name, string label,
        Color bgColor, float w, float h)
    {
        var go = MakeUIGO(name, parent);
        var rt = RT(go);
        rt.sizeDelta = new Vector2(w, h);

        var img = go.AddComponent<Image>();
        img.color = bgColor;
        var btn = go.AddComponent<Button>();
        btn.targetGraphic = img;
        var cb = btn.colors;
        cb.highlightedColor = bgColor * 1.15f;
        cb.pressedColor = bgColor * 0.85f;
        btn.colors = cb;

        var txtGO = MakeUIGO("Text", go.transform);
        var txtRT = RT(txtGO);
        txtRT.anchorMin = Vector2.zero;
        txtRT.anchorMax = Vector2.one;
        txtRT.offsetMin = new Vector2(6, 0);
        txtRT.offsetMax = new Vector2(-6, 0);
        var tmp = txtGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 17;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        var le = go.AddComponent<LayoutElement>();
        if (w > 0) le.preferredWidth = w;
        le.preferredHeight = h > 0 ? h : 38;
        le.minHeight = h > 0 ? h : 38;

        return btn;
    }

    static TMP_InputField MakeInputField(Transform parent, string name,
        string placeholder, float h,
        TMP_InputField.ContentType contentType = TMP_InputField.ContentType.Standard)
    {
        var go = MakeUIGO(name, parent);
        RT(go).sizeDelta = new Vector2(0, h);
        go.AddComponent<Image>().color = ColInput;

        // Text Area
        var ta = MakeUIGO("Text Area", go.transform);
        var taRT = RT(ta);
        taRT.anchorMin = Vector2.zero;
        taRT.anchorMax = Vector2.one;
        taRT.offsetMin = new Vector2(12, 4);
        taRT.offsetMax = new Vector2(-12, -4);
        ta.AddComponent<RectMask2D>();

        // Placeholder
        var ph = MakeUIGO("Placeholder", ta.transform);
        var phRT = RT(ph);
        phRT.anchorMin = Vector2.zero;
        phRT.anchorMax = Vector2.one;
        phRT.offsetMin = phRT.offsetMax = Vector2.zero;
        var phTMP = ph.AddComponent<TextMeshProUGUI>();
        phTMP.text = placeholder;
        phTMP.fontSize = 17;
        phTMP.fontStyle = FontStyles.Italic;
        phTMP.color = new Color(0.55f, 0.57f, 0.62f);
        phTMP.alignment = TextAlignmentOptions.Left;

        // Text
        var tx = MakeUIGO("Text", ta.transform);
        var txRT = RT(tx);
        txRT.anchorMin = Vector2.zero;
        txRT.anchorMax = Vector2.one;
        txRT.offsetMin = txRT.offsetMax = Vector2.zero;
        var txTMP = tx.AddComponent<TextMeshProUGUI>();
        txTMP.fontSize = 17;
        txTMP.color = Color.white;
        txTMP.alignment = TextAlignmentOptions.Left;

        var input = go.AddComponent<TMP_InputField>();
        input.textViewport = taRT;
        input.textComponent = txTMP;
        input.placeholder = phTMP;
        input.contentType = contentType;

        var le = go.AddComponent<LayoutElement>();
        le.preferredHeight = h;
        le.minHeight = h;

        return input;
    }

    static Toggle MakeToggleUI(Transform parent, string name, string label)
    {
        var go = MakeUIGO(name, parent);
        RT(go).sizeDelta = new Vector2(0, 32);
        var layout = go.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 8;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childForceExpandWidth = false;
        layout.childControlWidth = false;
        layout.childControlHeight = true;
        var goLE = go.AddComponent<LayoutElement>();
        goLE.preferredHeight = 32;
        goLE.minHeight = 32;

        // Background
        var bgGO = MakeUIGO("Background", go.transform);
        var bgImg = bgGO.AddComponent<Image>();
        bgImg.color = ColInput;
        var bgLE = bgGO.AddComponent<LayoutElement>();
        bgLE.preferredWidth = bgLE.preferredHeight = 26;
        bgLE.minWidth = bgLE.minHeight = 26;

        // Checkmark
        var cmGO = MakeUIGO("Checkmark", bgGO.transform);
        var cmRT = RT(cmGO);
        cmRT.anchorMin = new Vector2(0.15f, 0.15f);
        cmRT.anchorMax = new Vector2(0.85f, 0.85f);
        cmRT.offsetMin = cmRT.offsetMax = Vector2.zero;
        var cmImg = cmGO.AddComponent<Image>();
        cmImg.color = ColBlue;

        // Label
        var lblGO = MakeUIGO("Label", go.transform);
        var lblTMP = lblGO.AddComponent<TextMeshProUGUI>();
        lblTMP.text = label;
        lblTMP.fontSize = 15;
        lblTMP.color = new Color(0.78f, 0.80f, 0.84f);
        lblTMP.alignment = TextAlignmentOptions.Left;
        var lblLE = lblGO.AddComponent<LayoutElement>();
        lblLE.flexibleWidth = 1;
        lblLE.preferredHeight = 26;

        var toggle = go.AddComponent<Toggle>();
        toggle.targetGraphic = bgImg;
        toggle.graphic = cmImg;
        toggle.isOn = false;
        return toggle;
    }

    static (ScrollRect, Transform) MakeScroll(Transform parent, string name)
    {
        var go = MakeUIGO(name, parent);
        go.AddComponent<Image>().color = new Color(0, 0, 0, 0.08f);

        var vp = MakeUIGO("Viewport", go.transform);
        var vpRT = RT(vp);
        vpRT.anchorMin = Vector2.zero;
        vpRT.anchorMax = Vector2.one;
        vpRT.offsetMin = vpRT.offsetMax = Vector2.zero;
        vp.AddComponent<Image>().color = Color.clear;
        vp.AddComponent<Mask>().showMaskGraphic = false;

        var content = MakeUIGO("Content", vp.transform);
        var cRT = RT(content);
        cRT.anchorMin = new Vector2(0, 1);
        cRT.anchorMax = new Vector2(1, 1);
        cRT.pivot = new Vector2(0.5f, 1);
        cRT.offsetMin = cRT.offsetMax = Vector2.zero;
        var vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(10, 10, 10, 10);
        vlg.spacing = 8;
        vlg.childForceExpandHeight = false;
        vlg.childForceExpandWidth = true;
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;
        var csf = content.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var sr = go.AddComponent<ScrollRect>();
        sr.viewport = vpRT;
        sr.content = cRT;
        sr.horizontal = false;
        sr.vertical = true;
        sr.movementType = ScrollRect.MovementType.Elastic;

        return (sr, content.transform);
    }

    // ================ LAYOUT HELPERS ================

    static VerticalLayoutGroup AddVLG(GameObject go, float spacing, RectOffset padding)
    {
        var vlg = go.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = spacing;
        vlg.padding = padding;
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        return vlg;
    }

    static void Stretch(GameObject go)
    {
        var rt = RT(go);
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }

    static void Center(GameObject go)
    {
        var rt = RT(go);
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
    }

    static void SetSize(Component c, float w, float h)
    {
        RT(c).sizeDelta = new Vector2(w, h);
        var le = c.gameObject.GetComponent<LayoutElement>();
        if (le == null) le = c.gameObject.AddComponent<LayoutElement>();
        le.preferredWidth = w;
        le.preferredHeight = h;
    }

    static RectTransform RT(GameObject go) => go.GetComponent<RectTransform>();
    static RectTransform RT(Component c) => c.GetComponent<RectTransform>();

    // ================ WIRING HELPER ================

    static void W(SerializedObject so, string field, UnityEngine.Object value)
    {
        var prop = so.FindProperty(field);
        if (prop != null)
            prop.objectReferenceValue = value;
        else
            Debug.LogWarning($"[SceneBuilder] Field '{field}' not found on {so.targetObject.GetType().Name}");
    }
}
#endif
