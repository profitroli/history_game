using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AvatarController : MonoBehaviour
{
    [Header("Сетка аватаров")]
    [SerializeField] private Transform avatarGrid;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Button closeButton;

    [Header("Выбранный аватар (отображение)")]
    [SerializeField] private Image selectedAvatarImage;
    [SerializeField] private TMP_Text selectedAvatarName;

    [Header("Шаблон карточки (опционально)")]
    [SerializeField] private GameObject avatarCardPrefab;

    private Color unlockedBg = new Color(0.22f, 0.55f, 0.85f);
    private Color lockedBg = new Color(0.35f, 0.37f, 0.42f);
    private Color selectedBorder = new Color(0.95f, 0.85f, 0.15f);

    void OnEnable()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(Close);
        }
        BuildGrid();
        UpdateSelectedDisplay();
    }

    private void BuildGrid()
    {
        if (avatarGrid == null) return;

        foreach (Transform child in avatarGrid)
            Destroy(child.gameObject);

        var unlocked = AvatarService.GetUnlockStatus();
        int selected = AvatarService.GetSelectedAvatar();

        for (int i = 0; i < AvatarConfig.TotalAvatars; i++)
        {
            GameObject card;
            if (avatarCardPrefab != null)
            {
                card = Instantiate(avatarCardPrefab, avatarGrid);
            }
            else
            {
                card = CreateDefaultCard(i, unlocked[i], i == selected);
                card.transform.SetParent(avatarGrid, false);
            }

            int capturedIndex = i;
            bool isUnlocked = unlocked[i];
            var btn = card.GetComponentInChildren<Button>();
            if (btn != null)
            {
                btn.interactable = isUnlocked;
                btn.onClick.AddListener(async () =>
                {
                    if (!isUnlocked) return;
                    await AvatarService.SelectAvatar(capturedIndex);
                    BuildGrid();
                    UpdateSelectedDisplay();
                    ToastManager.Show($"Аватар «{AvatarConfig.Names[capturedIndex]}» выбран");
                });
            }
        }
    }

    private GameObject CreateDefaultCard(int index, bool isUnlocked, bool isSelected)
    {
        var card = new GameObject($"Avatar_{index}");
        var rt = card.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(140, 180);

        var bg = card.AddComponent<Image>();
        bg.color = isSelected ? selectedBorder : (isUnlocked ? unlockedBg : lockedBg);

        var layout = card.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(8, 8, 8, 8);
        layout.spacing = 5;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        layout.childControlWidth = true;
        layout.childControlHeight = true;

        var imgGO = new GameObject("Image");
        imgGO.transform.SetParent(card.transform, false);
        var img = imgGO.AddComponent<Image>();
        var sprite = AvatarConfig.LoadSprite(index);
        if (sprite != null)
        {
            img.sprite = sprite;
            img.preserveAspect = true;
        }
        else
        {
            img.color = isUnlocked ? new Color(0.8f, 0.85f, 0.95f) : new Color(0.3f, 0.3f, 0.35f);
        }
        var imgLE = imgGO.AddComponent<LayoutElement>();
        imgLE.preferredHeight = 90;
        imgLE.preferredWidth = 90;

        if (!isUnlocked)
        {
            var lockGO = new GameObject("Lock");
            lockGO.transform.SetParent(imgGO.transform, false);
            var lockRT = lockGO.AddComponent<RectTransform>();
            lockRT.anchorMin = Vector2.zero;
            lockRT.anchorMax = Vector2.one;
            lockRT.offsetMin = lockRT.offsetMax = Vector2.zero;
            var lockTMP = lockGO.AddComponent<TextMeshProUGUI>();
            lockTMP.text = "\U0001f512";
            lockTMP.fontSize = 32;
            lockTMP.alignment = TextAlignmentOptions.Center;
            lockTMP.color = new Color(1, 1, 1, 0.7f);
        }

        var nameGO = new GameObject("Name");
        nameGO.transform.SetParent(card.transform, false);
        nameGO.AddComponent<RectTransform>();
        var nameTMP = nameGO.AddComponent<TextMeshProUGUI>();
        nameTMP.text = AvatarConfig.Names[index];
        nameTMP.fontSize = 25;
        nameTMP.alignment = TextAlignmentOptions.Center;
        nameTMP.color = Color.white;
        nameGO.AddComponent<LayoutElement>().preferredHeight = 20;

        var descGO = new GameObject("Desc");
        descGO.transform.SetParent(card.transform, false);
        descGO.AddComponent<RectTransform>();
        var descTMP = descGO.AddComponent<TextMeshProUGUI>();
        descTMP.text = isUnlocked ? (isSelected ? "Выбран" : "Доступен") : AvatarConfig.Descriptions[index];
        descTMP.fontSize = 20;
        descTMP.alignment = TextAlignmentOptions.Center;
        descTMP.color = isUnlocked ? new Color(0.8f, 1f, 0.8f) : new Color(0.7f, 0.7f, 0.75f);
        descGO.AddComponent<LayoutElement>().preferredHeight = 18;

        card.AddComponent<Button>();
        return card;
    }

    private void UpdateSelectedDisplay()
    {
        int selected = AvatarService.GetSelectedAvatar();
        if (selectedAvatarImage != null)
        {
            var sprite = AvatarConfig.LoadSprite(selected);
            if (sprite != null)
            {
                selectedAvatarImage.sprite = sprite;
                selectedAvatarImage.color = Color.white;
            }
        }
        if (selectedAvatarName != null)
            selectedAvatarName.text = AvatarConfig.Names[selected];
    }

    private void Close()
    {
        gameObject.SetActive(false);
        var uiCtrl = Object.FindFirstObjectByType<MainMenuUIController>();
        if (uiCtrl != null) uiCtrl.ReturnToProfile();
    }

    public static void ShowNewUnlockNotification()
    {
        int newCount = AvatarService.GetNewUnlocksCount();
        if (newCount > 0)
            ToastManager.Show($"\U0001f389 Новый аватар разблокирован! ({newCount})");
    }
}
