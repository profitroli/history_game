using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToastManager : MonoBehaviour
{
    public static ToastManager Instance { get; private set; }

    private Canvas toastCanvas;
    private GameObject currentToast;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        CreateCanvas();
    }

    private void CreateCanvas()
    {
        var go = new GameObject("ToastCanvas");
        go.transform.SetParent(transform);
        toastCanvas = go.AddComponent<Canvas>();
        toastCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        toastCanvas.sortingOrder = 999;
        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        go.AddComponent<GraphicRaycaster>();
    }

    public static void Show(string message, float duration = 2.5f)
    {
        if (Instance != null) Instance.ShowToast(message, duration, false);
    }

    public static void ShowError(string message, float duration = 3.5f)
    {
        if (Instance != null) Instance.ShowToast(message, duration, true);
    }

    private void ShowToast(string message, float duration, bool isError)
    {
        if (currentToast != null) Destroy(currentToast);

        var go = new GameObject("Toast");
        go.transform.SetParent(toastCanvas.transform, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.2f, 0.02f);
        rt.anchorMax = new Vector2(0.8f, 0.08f);
        rt.offsetMin = rt.offsetMax = Vector2.zero;

        var bg = go.AddComponent<Image>();
        bg.color = isError ? new Color(0.8f, 0.2f, 0.2f, 0.92f) : new Color(0.18f, 0.2f, 0.26f, 0.92f);

        var textGO = new GameObject("Text");
        textGO.transform.SetParent(go.transform, false);
        var textRT = textGO.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = new Vector2(20, 5);
        textRT.offsetMax = new Vector2(-20, -5);
        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = message;
        tmp.fontSize = 18;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;

        currentToast = go;
        StartCoroutine(FadeAndDestroy(go, bg, tmp, duration));
    }

    private IEnumerator FadeAndDestroy(GameObject go, Image bg, TMP_Text txt, float duration)
    {
        yield return new WaitForSeconds(duration);
        float fade = 0.5f;
        float t = 0;
        Color bgC = bg.color;
        Color txtC = txt.color;
        while (t < fade)
        {
            float a = 1f - t / fade;
            bg.color = new Color(bgC.r, bgC.g, bgC.b, bgC.a * a);
            txt.color = new Color(txtC.r, txtC.g, txtC.b, a);
            t += Time.deltaTime;
            yield return null;
        }
        if (go != null) Destroy(go);
        if (currentToast == go) currentToast = null;
    }
}
