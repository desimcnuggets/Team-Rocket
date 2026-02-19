using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MainMenuBuilder : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Team Rocket/Generate Main Menu UI")]
    public static void GenerateMainMenu()
    {
        // 1. Create Canvas
        GameObject canvasGO = new GameObject("MainMenu_Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        
        canvasGO.AddComponent<GraphicRaycaster>();
        
        // Ensure EventSystem
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject esFromBuilder = new GameObject("EventSystem");
            esFromBuilder.AddComponent<EventSystem>();
            esFromBuilder.AddComponent<StandaloneInputModule>();
        }

        // 2. Background Panel
        GameObject bgPanel = CreatePanel(canvasGO.transform, "BackgroundPanel", new Color(0.05f, 0.1f, 0.15f, 1f)); // Dark Navy
        
        // 3. Vertical Layout Group for spacing
        GameObject layoutGO = new GameObject("LayoutGroup");
        layoutGO.transform.SetParent(canvasGO.transform, false);
        RectTransform layoutRT = layoutGO.AddComponent<RectTransform>();
        layoutRT.anchorMin = Vector2.zero;
        layoutRT.anchorMax = Vector2.one;
        layoutRT.offsetMin = new Vector2(100, 100); // Increased padding
        layoutRT.offsetMax = new Vector2(-100, -100);
        
        VerticalLayoutGroup vlg = layoutGO.AddComponent<VerticalLayoutGroup>();
        vlg.childAlignment = TextAnchor.MiddleCenter;
        vlg.spacing = 50; // Increased spacing
        vlg.childControlHeight = true;
        vlg.childControlWidth = true;

        // 4. Header
        CreateText(layoutGO.transform, "METROPOLITAN POLICE SERVICE", 42, Color.cyan);

        // 5. Title (Placeholder for Image)
        GameObject titleObj = new GameObject("Title_Image_Placeholder");
        titleObj.transform.SetParent(layoutGO.transform, false);
        Image titleImg = titleObj.AddComponent<Image>();
        titleImg.color = new Color(0.6f, 0f, 0f); // Dark Red
        LayoutElement titleLE = titleObj.AddComponent<LayoutElement>();
        titleLE.preferredHeight = 250;
        titleLE.preferredWidth = 900;
        
        // Title Text Overlay (in case image missing)
        CreateText(titleObj.transform, "THE NECESSARY EVIL", 110, Color.white);

        // 6. Subtitle
        CreateText(layoutGO.transform, "SCOTLAND YARD SIMULATOR", 36, Color.gray);

        // 7. Tagline
        CreateText(layoutGO.transform, "\"Keep London in Manageable Chaos\"\nNo Crime - Safety Stats - 100% Crime - The Purge\nYou need both.", 32, Color.gray, true);

        // 8. Info Bar
        CreateText(layoutGO.transform, "GAME JAM 2026 | THEME: EQUILIBRIUM | 10-MINUTE SESSION", 28, Color.gray);

        // 9. Play Button
        GameObject buttonObj = new GameObject("Button_Start");
        buttonObj.transform.SetParent(layoutGO.transform, false);
        Image btnImg = buttonObj.AddComponent<Image>();
        btnImg.color = new Color(0f, 1f, 1f, 0.1f); // Cyan tint
        Button btn = buttonObj.AddComponent<Button>();
        LayoutElement btnLE = buttonObj.AddComponent<LayoutElement>();
        btnLE.preferredHeight = 100;
        btnLE.preferredWidth = 400;
        
        CreateText(buttonObj.transform, "ASSUME COMMAND", 42, Color.cyan);

        // Add Controller
        MainMenuController controller = canvasGO.AddComponent<MainMenuController>();
        
        // Link Button
        // use UnityEditor.Events to link automatically
        UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, controller.StartGame);

        // 10. Footer
        CreateText(layoutGO.transform, "CONTAINS: MORAL COMPROMISE - SATIRE - LONDON", 24, new Color(0.5f, 0.5f, 0.5f));

        Debug.Log("Main Menu Generated! Button has been auto-linked to StartGame().");
    }

    static GameObject CreatePanel(Transform parent, string name, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        
        Image img = panel.AddComponent<Image>();
        img.color = color;
        return panel;
    }

    static void CreateText(Transform parent, string content, float fontSize, Color color, bool italic = false)
    {
        GameObject textObj = new GameObject("Text_" + (content.Length > 10 ? content.Substring(0,10) : content));
        textObj.transform.SetParent(parent, false);
        
        TextMeshProUGUI txt = textObj.AddComponent<TextMeshProUGUI>();
        txt.text = content;
        
        // Try to load default font
        TMP_FontAsset font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (font != null) txt.font = font;

        txt.fontSize = fontSize;
        txt.color = color;
        txt.alignment = TextAlignmentOptions.Center;
        txt.fontStyle = italic ? FontStyles.Italic : FontStyles.Normal;
        txt.enableWordWrapping = true;
        txt.overflowMode = TextOverflowModes.Overflow;
    }
#endif
}
