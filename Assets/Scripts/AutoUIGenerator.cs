using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI自动生成器 - 一键创建所有UI元素（复用现有Canvas版本）
/// </summary>
public class AutoUIGenerator : MonoBehaviour
{
    [Header("配置")]
    public bool skipButtons = true;  // 跳过按钮生成（使用原有按钮）
    
    void Start()
    {
        Debug.Log("=== 开始自动生成UI（复用现有Canvas）===");
        
        // 查找现有Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            Debug.Log("✓ Canvas已创建");
        }
        else
        {
            Debug.Log("✓ 使用现有Canvas: " + canvas.gameObject.name);
        }
        
        // 确保有EventSystem
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            Debug.Log("✓ EventSystem已创建");
        }
        
        // 创建UI元素
        CreateHeartRateUI(canvas);
        CreateAttentionUI(canvas);
        CreateSpeechUI(canvas);
        
        if (!skipButtons)
        {
            CreateControlButtons(canvas);
        }
        else
        {
            Debug.Log("⏭ 跳过按钮生成（将使用原有按钮）");
        }
        
        Debug.Log("=== UI生成完成！===");
        Debug.Log("提示1：停止Play后UI会保留在Canvas下");
        Debug.Log("提示2：如需调整位置，在Scene视图中手动拖动");
    }
    
    void CreateHeartRateUI(Canvas canvas)
    {
        // 检查是否已存在
        if (GameObject.Find("HeartRateText") != null)
        {
            Debug.Log("⏭ HeartRateText已存在，跳过");
            return;
        }
        
        // 心率标签
        GameObject label = CreateText(canvas.transform, "HeartRateLabel", "❤️ 心率", 20);
        SetPosition(label, TextAnchor.UpperLeft, 150, -30, 200, 30);
        label.GetComponent<TextMeshProUGUI>().color = Color.red;
        
        // 心率数值
        GameObject text = CreateText(canvas.transform, "HeartRateText", "0 BPM", 36);
        SetPosition(text, TextAnchor.UpperLeft, 150, -60, 200, 50);
        
        Debug.Log("✓ 心率UI已创建");
    }
    
    void CreateAttentionUI(Canvas canvas)
    {
        if (GameObject.Find("AttentionPercentText") != null)
        {
            Debug.Log("⏭ AttentionPercentText已存在，跳过");
            return;
        }
        
        // 专注度文字
        GameObject text = CreateText(canvas.transform, "AttentionPercentText", "观众专注度: 0%", 28);
        SetPosition(text, TextAnchor.UpperLeft, 150, -130, 300, 40);
        text.GetComponent<TextMeshProUGUI>().color = Color.green;
        
        Debug.Log("✓ 观众专注度UI已创建");
    }
    
    void CreateSpeechUI(Canvas canvas)
    {
        // 字幕
        if (GameObject.Find("SubtitleText") == null)
        {
            GameObject subtitle = CreateText(canvas.transform, "SubtitleText", "（字幕将在这里显示）", 32);
            SetPosition(subtitle, TextAnchor.LowerCenter, 0, 100, 800, 100);
            subtitle.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        }
        
        // 填充词计数
        if (GameObject.Find("FillerWordCountText") == null)
        {
            GameObject counter = CreateText(canvas.transform, "FillerWordCountText", "填充词: 0", 24);
            SetPosition(counter, TextAnchor.UpperRight, -150, -50, 200, 40);
            counter.GetComponent<TextMeshProUGUI>().color = Color.yellow;
        }
        
        Debug.Log("✓ 语音反馈UI已创建");
    }
    
    void CreateControlButtons(Canvas canvas)
    {
        // 开始按钮
        GameObject startBtn = CreateButton(canvas.transform, "StartButton", "开始演讲");
        SetPosition(startBtn, TextAnchor.LowerLeft, 120, 50, 150, 60);
        
        // 停止按钮
        GameObject stopBtn = CreateButton(canvas.transform, "StopButton", "停止演讲");
        SetPosition(stopBtn, TextAnchor.LowerLeft, 290, 50, 150, 60);
        
        Debug.Log("✓ 控制按钮已创建");
    }
    
    // ===== 辅助函数 =====
    
    GameObject CreateText(Transform parent, string name, string content, float fontSize)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        
        TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();
        text.text = content;
        text.fontSize = fontSize;
        text.color = Color.white;
        
        return obj;
    }
    
    GameObject CreateButton(Transform parent, string name, string buttonText)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        
        Image img = obj.AddComponent<Image>();
        img.color = new Color(0.2f, 0.6f, 1f);
        
        Button btn = obj.AddComponent<Button>();
        
        // 创建按钮文字
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(obj.transform, false);
        
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = buttonText;
        text.fontSize = 24;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        return obj;
    }
    
    void SetPosition(GameObject obj, TextAnchor anchor, float x, float y, float width, float height)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        if (rect == null) rect = obj.AddComponent<RectTransform>();
        
        // 设置锚点
        switch (anchor)
        {
            case TextAnchor.UpperLeft:
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(0, 1);
                rect.pivot = new Vector2(0, 1);
                break;
            case TextAnchor.UpperRight:
                rect.anchorMin = new Vector2(1, 1);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(1, 1);
                break;
            case TextAnchor.LowerLeft:
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(0, 0);
                rect.pivot = new Vector2(0, 0);
                break;
            case TextAnchor.LowerCenter:
                rect.anchorMin = new Vector2(0.5f, 0);
                rect.anchorMax = new Vector2(0.5f, 0);
                rect.pivot = new Vector2(0.5f, 0);
                break;
        }
        
        rect.anchoredPosition = new Vector2(x, y);
        rect.sizeDelta = new Vector2(width, height);
    }
}
