#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

public static class ScenePolisher
{
    static readonly Color BgTeal       = new Color(0.19f, 0.62f, 0.82f);
    static readonly Color BgMuted      = new Color(0.50f, 0.69f, 0.77f);
    static readonly Color PanelBlue    = new Color(0.10f, 0.46f, 0.69f, 0.97f);
    static readonly Color PanelDark    = new Color(0.08f, 0.35f, 0.55f, 0.97f);
    static readonly Color BtnBlue      = new Color(0.12f, 0.50f, 0.75f);
    static readonly Color BtnGreen     = new Color(0.18f, 0.62f, 0.40f);
    static readonly Color BtnRed       = new Color(0.75f, 0.22f, 0.22f);
    static readonly Color BtnGray      = new Color(0.35f, 0.55f, 0.65f);
    static readonly Color BtnOrange    = new Color(0.80f, 0.55f, 0.18f);
    static readonly Color InputBg      = new Color(1f, 1f, 1f, 0.15f);
    static readonly Color HeaderColor  = new Color(0.08f, 0.38f, 0.58f);
    static readonly Color OverlayBg    = new Color(0.06f, 0.30f, 0.48f, 0.97f);

    static Sprite uiSprite;

    static Sprite GetUISprite()
    {
        if (uiSprite == null)
            uiSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        return uiSprite;
    }

    [MenuItem("Tools/Полировка/Все сцены")]
    static void PolishAll()
    {
        if (!EditorUtility.DisplayDialog("Scene Polisher",
            "Обновить визуал всех сцен?", "Yes", "Cancel")) return;
        PolishLoginScene();
        PolishTeacherScene();
        PolishMainMenuPanels();
        PolishSettingsPanels();
        EditorUtility.DisplayDialog("Scene Polisher", "Все сцены обновлены!", "OK");
    }

    [MenuItem("Tools/Полировка/LoginScene")]
    static void Menu_Login() { PolishLoginScene(); }

    [MenuItem("Tools/Полировка/TeacherDashboard")]
    static void Menu_Teacher() { PolishTeacherScene(); }

    [MenuItem("Tools/Полировка/MainMenu панели")]
    static void Menu_MainMenu() { PolishMainMenuPanels(); }

    [MenuItem("Tools/Полировка/Settings панели")]
    static void Menu_Settings() { PolishSettingsPanels(); }

    // ===================== LOGIN =====================

    static void PolishLoginScene()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/LoginScene.unity");
        var canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null) return;

        FixBackground(canvas.transform, "Background", BgTeal);
        FixCameraColor(BgTeal);

        EnlargePanel(canvas.transform, "LoginPanel", 600, 500, PanelBlue);
        EnlargePanel(canvas.transform, "RegisterPanel", 600, 650, PanelBlue);
        EnlargePanel(canvas.transform, "RoleSelectionPanel", 600, 420, PanelBlue);
        EnlargePanel(canvas.transform, "LoadingPanel", 400, 150, PanelBlue);

        var appTitle = canvas.transform.Find("AppTitle");
        if (appTitle != null)
        {
            var tmp = appTitle.GetComponent<TextMeshProUGUI>();
            if (tmp != null) { tmp.fontSize = 38; tmp.fontStyle = FontStyles.Bold; }
        }

        StyleAllButtons(canvas.transform);
        StyleAllInputs(canvas.transform);
        EnlargeFonts(canvas.transform);

        EditorSceneManager.SaveScene(scene);
        Debug.Log("[ScenePolisher] LoginScene polished");
    }

    // =================== TEACHER ====================

    static void PolishTeacherScene()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/TeacherDashboardScene.unity");
        var canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null) return;

        FixBackground(canvas.transform, "Background", BgMuted);
        FixCameraColor(BgMuted);

        var header = canvas.transform.Find("HeaderPanel");
        if (header != null)
        {
            var img = header.GetComponent<Image>();
            if (img != null)
            {
                img.color = HeaderColor;
                img.sprite = GetUISprite();
                img.type = Image.Type.Sliced;
            }
            var rt = header.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = new Vector2(0, 0.87f);
                rt.anchorMax = Vector2.one;
            }
        }

        EnlargePanel(canvas.transform, "CreateGroupPanel", 620, 520, PanelBlue);
        EnlargePanel(canvas.transform, "GroupDetailPanel", 1000, 720, PanelDark);

        var abp = canvas.transform.Find("AssignmentBuilderPanel");
        if (abp != null)
        {
            var img = abp.GetComponent<Image>();
            if (img != null) img.color = OverlayBg;
        }

        StyleAllButtons(canvas.transform);
        StyleAllInputs(canvas.transform);
        EnlargeFonts(canvas.transform);

        EditorSceneManager.SaveScene(scene);
        Debug.Log("[ScenePolisher] TeacherDashboardScene polished");
    }

    // =================== MAINMENU ===================

    static void PolishMainMenuPanels()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        var canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null) return;

        var navPanel = canvas.transform.Find("AuthNavPanel");
        if (navPanel != null)
        {
            var img = navPanel.GetComponent<Image>();
            if (img == null) img = navPanel.gameObject.AddComponent<Image>();
            img.color = new Color(0.08f, 0.35f, 0.55f, 0.85f);
            img.sprite = GetUISprite();
            img.type = Image.Type.Sliced;

            var rt = navPanel.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = new Vector2(0.05f, 0.02f);
                rt.anchorMax = new Vector2(0.95f, 0.09f);
            }
        }

        string[] overlayPanels = { "StudentDashboardPanel", "AssignmentPlayerPanel", "AvatarPanel" };
        foreach (var pName in overlayPanels)
        {
            var p = canvas.transform.Find(pName);
            if (p == null) continue;
            var img = p.GetComponent<Image>();
            if (img != null) img.color = OverlayBg;
        }

        StyleAllButtons(canvas.transform);
        StyleAllInputs(canvas.transform);
        EnlargeFonts(canvas.transform);

        EditorSceneManager.SaveScene(scene);
        Debug.Log("[ScenePolisher] MainMenu panels polished");
    }

    // ================== SETTINGS ====================

    static void PolishSettingsPanels()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/SettingsScene.unity");
        var canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null) return;

        EnlargePanel(canvas.transform, "ProfilePanel", 600, 600, PanelBlue);

        StyleAllButtons(canvas.transform);
        StyleAllInputs(canvas.transform);
        EnlargeFonts(canvas.transform);

        EditorSceneManager.SaveScene(scene);
        Debug.Log("[ScenePolisher] SettingsScene panels polished");
    }

    // ================ STYLE HELPERS =================

    static void FixBackground(Transform root, string name, Color color)
    {
        var bg = root.Find(name);
        if (bg != null)
        {
            var img = bg.GetComponent<Image>();
            if (img != null) img.color = color;
        }
    }

    static void FixCameraColor(Color color)
    {
        var cam = Camera.main;
        if (cam != null) cam.backgroundColor = color;
    }

    static void EnlargePanel(Transform root, string name, float w, float h, Color color)
    {
        var panel = root.Find(name);
        if (panel == null) return;

        var rt = panel.GetComponent<RectTransform>();
        if (rt != null) rt.sizeDelta = new Vector2(w, h);

        var img = panel.GetComponent<Image>();
        if (img != null)
        {
            img.color = color;
            img.sprite = GetUISprite();
            img.type = Image.Type.Sliced;
            img.pixelsPerUnitMultiplier = 1;
        }
    }

    static void StyleAllButtons(Transform root)
    {
        var buttons = root.GetComponentsInChildren<Button>(true);
        var sprite = GetUISprite();

        foreach (var btn in buttons)
        {
            var img = btn.GetComponent<Image>();
            if (img == null) continue;

            img.sprite = sprite;
            img.type = Image.Type.Sliced;

            Color baseColor = img.color;
            if (IsCloseToColor(baseColor, new Color(0.25f, 0.48f, 0.85f)))
                img.color = BtnBlue;
            else if (IsCloseToColor(baseColor, new Color(0.40f, 0.43f, 0.50f)))
                img.color = BtnGray;
            else if (IsCloseToColor(baseColor, new Color(0.22f, 0.65f, 0.35f)))
                img.color = BtnGreen;
            else if (IsCloseToColor(baseColor, new Color(0.80f, 0.25f, 0.25f)))
                img.color = BtnRed;
            else if (IsCloseToColor(baseColor, new Color(0.85f, 0.55f, 0.15f)))
                img.color = BtnOrange;

            var le = btn.GetComponent<LayoutElement>();
            if (le != null)
            {
                if (le.preferredHeight > 0 && le.preferredHeight < 48)
                    le.preferredHeight = 50;
                if (le.minHeight > 0 && le.minHeight < 48)
                    le.minHeight = 50;
            }

            var rt = btn.GetComponent<RectTransform>();
            if (rt != null && rt.sizeDelta.y > 0 && rt.sizeDelta.y < 48)
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, 50);

            var txt = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null)
            {
                if (txt.fontSize < 18) txt.fontSize = 19;
                txt.fontStyle = FontStyles.Bold;
            }
        }
    }

    static void StyleAllInputs(Transform root)
    {
        var inputs = root.GetComponentsInChildren<TMP_InputField>(true);
        var sprite = GetUISprite();

        foreach (var input in inputs)
        {
            var img = input.GetComponent<Image>();
            if (img != null)
            {
                img.color = InputBg;
                img.sprite = sprite;
                img.type = Image.Type.Sliced;
            }

            var le = input.GetComponent<LayoutElement>();
            if (le != null)
            {
                if (le.preferredHeight > 0 && le.preferredHeight < 48)
                    le.preferredHeight = 50;
                if (le.minHeight > 0 && le.minHeight < 48)
                    le.minHeight = 50;
            }

            var rt = input.GetComponent<RectTransform>();
            if (rt != null && rt.sizeDelta.y > 0 && rt.sizeDelta.y < 48)
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, 50);

            var txtComp = input.textComponent;
            if (txtComp != null && txtComp.fontSize < 19) txtComp.fontSize = 19;
            var ph = input.placeholder as TextMeshProUGUI;
            if (ph != null && ph.fontSize < 19) ph.fontSize = 19;
        }
    }

    static void EnlargeFonts(Transform root)
    {
        var texts = root.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (var t in texts)
        {
            if (t.GetComponentInParent<Button>() != null) continue;
            if (t.GetComponentInParent<TMP_InputField>() != null) continue;

            if (t.fontSize <= 15) t.fontSize = 18;
            else if (t.fontSize <= 18) t.fontSize = 20;
            else if (t.fontSize >= 22 && t.fontSize <= 24) t.fontSize = 26;
        }
    }

    static bool IsCloseToColor(Color a, Color b, float threshold = 0.15f)
    {
        return Mathf.Abs(a.r - b.r) < threshold &&
               Mathf.Abs(a.g - b.g) < threshold &&
               Mathf.Abs(a.b - b.b) < threshold;
    }
}
#endif
