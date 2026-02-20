using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PauseMenuBuilder : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Team Rocket/Generate Pause Menu UI")]
    public static void GeneratePauseMenu()
    {
        // --- Find or create the HUD Canvas ---
        Canvas existingCanvas = FindObjectOfType<Canvas>();
        Transform canvasTransform;

        if (existingCanvas != null)
        {
            canvasTransform = existingCanvas.transform;
            Debug.Log("[PauseMenuBuilder] Using existing Canvas: " + existingCanvas.name);
        }
        else
        {
            GameObject canvasGO = new GameObject("HUD_Canvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGO.AddComponent<GraphicRaycaster>();
            canvasTransform = canvasGO.transform;
        }

        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }

        // =============================================
        // ROOT PANEL — full-screen, semi-transparent
        // =============================================
        GameObject pauseRoot = new GameObject("PauseMenu_Panel");
        pauseRoot.transform.SetParent(canvasTransform, false);
        RectTransform rootRT = pauseRoot.AddComponent<RectTransform>();
        rootRT.anchorMin = Vector2.zero;
        rootRT.anchorMax = Vector2.one;
        rootRT.offsetMin = Vector2.zero;
        rootRT.offsetMax = Vector2.zero;

        Image rootImg = pauseRoot.AddComponent<Image>();
        rootImg.color = new Color(0.04f, 0.06f, 0.1f, 0.97f); // near-black navy

        // =============================================
        // CENTRE CARD — fixed-size content box
        // =============================================
        GameObject card = new GameObject("ContentCard");
        card.transform.SetParent(pauseRoot.transform, false);
        RectTransform cardRT = card.AddComponent<RectTransform>();
        cardRT.anchorMin = new Vector2(0.5f, 0.5f);
        cardRT.anchorMax = new Vector2(0.5f, 0.5f);
        cardRT.pivot = new Vector2(0.5f, 0.5f);
        cardRT.sizeDelta = new Vector2(640, 720);
        cardRT.anchoredPosition = Vector2.zero;

        Image cardImg = card.AddComponent<Image>();
        cardImg.color = new Color(0.08f, 0.12f, 0.18f, 1f);

        // Vertical layout
        VerticalLayoutGroup vlg = card.AddComponent<VerticalLayoutGroup>();
        vlg.childAlignment = TextAnchor.MiddleCenter;
        vlg.spacing = 36;
        vlg.padding = new RectOffset(48, 48, 56, 56);
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        // ---- TITLE ----
        CreateLabel(card.transform, "PAUSED", 58, new Color(0.55f, 0.85f, 1f), 60);
        CreateDivider(card.transform);

        // ---- MUSIC SLIDER ----
        CreateLabel(card.transform, "MUSIC VOLUME", 28, Color.white, 30);
        Slider musicSlider = CreateSlider(card.transform, "Slider_Music", 48);

        // ---- SFX SLIDER ----
        CreateLabel(card.transform, "SOUND EFFECTS", 28, Color.white, 30);
        Slider sfxSlider = CreateSlider(card.transform, "Slider_SFX", 48);

        CreateDivider(card.transform);

        // ---- DYSLEXIC MODE TOGGLE ----
        Toggle dyslexicToggle = CreateToggleRow(card.transform, "DYSLEXIC MODE", 48);

        CreateDivider(card.transform);

        Button resumeBtn = CreateButton(card.transform, "RESUME", new Color(0.2f, 0.72f, 0.45f), 70);

        // =============================================
        // WIRE UP PauseMenuController
        // =============================================
        PauseMenuController controller = FindObjectOfType<PauseMenuController>();
        if (controller == null)
        {
            GameObject pmGO = new GameObject("PauseMenuController");
            controller = pmGO.AddComponent<PauseMenuController>();
            Debug.Log("[PauseMenuBuilder] Created PauseMenuController GameObject.");
        }

        // Assign panel + sliders via SerializedObject
        SerializedObject so = new SerializedObject(controller);
        so.FindProperty("pausePanel").objectReferenceValue  = pauseRoot;
        so.FindProperty("musicSlider").objectReferenceValue = musicSlider;
        so.FindProperty("sfxSlider").objectReferenceValue   = sfxSlider;
        so.FindProperty("dyslexicToggle").objectReferenceValue = dyslexicToggle;
        so.ApplyModifiedProperties();

        // Wire Resume button
        UnityEditor.Events.UnityEventTools.AddPersistentListener(
            resumeBtn.onClick, controller.OnResumeClicked);

        // Wire Dyslexic toggle
        UnityEditor.Events.UnityEventTools.AddBoolPersistentListener(
            dyslexicToggle.onValueChanged, controller.OnDyslexicToggled, false);

        // Disable panel by default (hidden in-game)
        pauseRoot.SetActive(false);

        Undo.RegisterCreatedObjectUndo(pauseRoot, "Generate Pause Menu");
        Selection.activeGameObject = pauseRoot;
        Debug.Log("[PauseMenuBuilder] Pause Menu generated and wired. Assign the HUD pause button's OnClick to UIManager.OnPauseClicked().");
    }

    // ---- Helpers ----

    static void CreateLabel(Transform parent, string text, float size, Color color, float height)
    {
        GameObject go = new GameObject("Label_" + text.Replace(" ", "_"));
        go.transform.SetParent(parent, false);
        LayoutElement le = go.AddComponent<LayoutElement>();
        le.preferredHeight = height;

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        TMP_FontAsset font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (font != null) tmp.font = font;
    }

    static void CreateDivider(Transform parent)
    {
        GameObject go = new GameObject("Divider");
        go.transform.SetParent(parent, false);
        LayoutElement le = go.AddComponent<LayoutElement>();
        le.preferredHeight = 2;
        Image img = go.AddComponent<Image>();
        img.color = new Color(0.3f, 0.45f, 0.6f, 0.5f);
    }

    static Slider CreateSlider(Transform parent, string name, float height)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        LayoutElement le = go.AddComponent<LayoutElement>();
        le.preferredHeight = height;

        Slider slider = go.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;

        // Background
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(go.transform, false);
        RectTransform bgRT = bg.AddComponent<RectTransform>();
        bgRT.anchorMin = new Vector2(0, 0.25f);
        bgRT.anchorMax = new Vector2(1, 0.75f);
        bgRT.offsetMin = bgRT.offsetMax = Vector2.zero;
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.15f, 0.2f, 0.28f);

        // Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(go.transform, false);
        RectTransform faRT = fillArea.AddComponent<RectTransform>();
        faRT.anchorMin = new Vector2(0, 0.25f);
        faRT.anchorMax = new Vector2(1, 0.75f);
        faRT.offsetMin = new Vector2(5, 0);
        faRT.offsetMax = new Vector2(-15, 0);

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        RectTransform fillRT = fill.AddComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = new Vector2(1, 1);
        fillRT.offsetMin = fillRT.offsetMax = Vector2.zero;
        Image fillImg = fill.AddComponent<Image>();
        fillImg.color = new Color(0.33f, 0.78f, 1f);

        // Handle Area
        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(go.transform, false);
        RectTransform haRT = handleArea.AddComponent<RectTransform>();
        haRT.anchorMin = Vector2.zero;
        haRT.anchorMax = Vector2.one;
        haRT.offsetMin = new Vector2(10, 0);
        haRT.offsetMax = new Vector2(-10, 0);

        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        RectTransform hRT = handle.AddComponent<RectTransform>();
        hRT.sizeDelta = new Vector2(24, 24);
        Image hImg = handle.AddComponent<Image>();
        hImg.color = Color.white;

        slider.fillRect = fillRT;
        slider.handleRect = hRT;
        slider.targetGraphic = hImg;

        return slider;
    }

    static Toggle CreateToggleRow(Transform parent, string label, float height)
    {
        // Outer row container
        GameObject row = new GameObject("ToggleRow_" + label.Replace(" ", "_"));
        row.transform.SetParent(parent, false);
        LayoutElement rowLE = row.AddComponent<LayoutElement>();
        rowLE.preferredHeight = height;

        HorizontalLayoutGroup hlg = row.AddComponent<HorizontalLayoutGroup>();
        hlg.childAlignment = TextAnchor.MiddleLeft;
        hlg.spacing = 20;
        hlg.childControlHeight = true;
        hlg.childControlWidth = false;
        hlg.childForceExpandHeight = true;
        hlg.childForceExpandWidth = false;

        // --- Toggle widget ---
        GameObject toggleGO = new GameObject("Toggle");
        toggleGO.transform.SetParent(row.transform, false);
        LayoutElement toggleLE = toggleGO.AddComponent<LayoutElement>();
        toggleLE.preferredWidth = 48;

        Toggle toggle = toggleGO.AddComponent<Toggle>();

        // Background (the checkbox box)
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(toggleGO.transform, false);
        RectTransform bgRT = bg.AddComponent<RectTransform>();
        bgRT.anchorMin = new Vector2(0f, 0.5f);
        bgRT.anchorMax = new Vector2(0f, 0.5f);
        bgRT.pivot     = new Vector2(0f, 0.5f);
        bgRT.sizeDelta = new Vector2(36, 36);
        bgRT.anchoredPosition = Vector2.zero;
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.15f, 0.2f, 0.28f);

        // Checkmark
        GameObject checkGO = new GameObject("Checkmark");
        checkGO.transform.SetParent(bg.transform, false);
        RectTransform checkRT = checkGO.AddComponent<RectTransform>();
        checkRT.anchorMin = new Vector2(0.1f, 0.1f);
        checkRT.anchorMax = new Vector2(0.9f, 0.9f);
        checkRT.offsetMin = checkRT.offsetMax = Vector2.zero;
        Image checkImg = checkGO.AddComponent<Image>();
        checkImg.color = new Color(0.33f, 0.78f, 1f);

        toggle.targetGraphic = bgImg;
        toggle.graphic       = checkImg;
        toggle.isOn          = false;

        // --- Label ---
        GameObject labelGO = new GameObject("Label_" + label.Replace(" ", "_"));
        labelGO.transform.SetParent(row.transform, false);
        LayoutElement labelLE = labelGO.AddComponent<LayoutElement>();
        labelLE.flexibleWidth = 1;

        TextMeshProUGUI tmp = labelGO.AddComponent<TextMeshProUGUI>();
        tmp.text      = label;
        tmp.fontSize  = 26;
        tmp.color     = Color.white;
        tmp.alignment = TextAlignmentOptions.MidlineLeft;
        TMP_FontAsset font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (font != null) tmp.font = font;

        return toggle;
    }

    static Button CreateButton(Transform parent, string label, Color bgColor, float height)
    {
        GameObject go = new GameObject("Button_" + label.Replace(" ", "_"));
        go.transform.SetParent(parent, false);
        LayoutElement le = go.AddComponent<LayoutElement>();
        le.preferredHeight = height;

        Image img = go.AddComponent<Image>();
        img.color = bgColor;
        Button btn = go.AddComponent<Button>();

        ColorBlock cb = btn.colors;
        cb.highlightedColor = new Color(bgColor.r * 1.2f, bgColor.g * 1.2f, bgColor.b * 1.2f);
        cb.pressedColor = new Color(bgColor.r * 0.8f, bgColor.g * 0.8f, bgColor.b * 0.8f);
        btn.colors = cb;

        // Label
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(go.transform, false);
        RectTransform tRT = textGO.AddComponent<RectTransform>();
        tRT.anchorMin = Vector2.zero;
        tRT.anchorMax = Vector2.one;
        tRT.offsetMin = tRT.offsetMax = Vector2.zero;

        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 34;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        TMP_FontAsset font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (font != null) tmp.font = font;

        return btn;
    }
#endif
}
