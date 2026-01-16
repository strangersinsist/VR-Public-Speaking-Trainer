using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 完整UI生成器 - 一次性生成所有缺失的UI面板
/// 包括：评估面板、Relax提示、警告面板、进度条等
/// </summary>
public class CompleteUIGenerator : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== 开始生成所有缺失的UI ===");
        
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("未找到Canvas!");
            return;
        }
        
        // 生成所有UI
        CreateEvaluationPanel(canvas);
        CreateRelaxPanel(canvas);
        CreateWarningPanels(canvas);
        CreateAttentionBar(canvas);
        
        Debug.Log("=== 所有UI生成完成！===");
        Debug.Log("提示：停止Play后保存场景");
    }
    
    // 1. 评估面板
    void CreateEvaluationPanel(Canvas canvas)
    {
        GameObject panel = new GameObject("EvaluationPanel");
        panel.transform.SetParent(canvas.transform, false);
        
        Image panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0, 0, 0, 0.92f);
        
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        panel.SetActive(false);
        
        // 标题
        CreateText(panel.transform, "SummaryText", "Performance Summary", 42, new Vector2(0, 300), new Vector2(800, 60));
        
        // 5维度评分
        CreateText(panel.transform, "FluencyScore", "Fluency: 0", 24, new Vector2(-200, 180), new Vector2(180, 40));
        CreateText(panel.transform, "ContentScore", "Content: 0", 24, new Vector2(0, 180), new Vector2(180, 40));
        CreateText(panel.transform, "InteractionScore", "Interaction: 0", 24, new Vector2(200, 180), new Vector2(180, 40));
        CreateText(panel.transform, "TimeControlScore", "Time Control: 0", 24, new Vector2(-100, 110), new Vector2(180, 40));
        CreateText(panel.transform, "EmotionalStabilityScore", "Emotion: 0", 24, new Vector2(100, 110), new Vector2(180, 40));
        
        // 详细数据
        CreateText(panel.transform, "FillerWordsText", "Filler Words: 0", 22, new Vector2(0, 30), new Vector2(400, 35));
        CreateText(panel.transform, "ContentDeviationText", "Content Deviation: 0", 22, new Vector2(0, -10), new Vector2(400, 35));
        CreateText(panel.transform, "EyeContactText", "Eye Contact: 0%", 22, new Vector2(0, -50), new Vector2(400, 35));
        CreateText(panel.transform, "TimeOverrunText", "Time: Good", 22, new Vector2(0, -90), new Vector2(400, 35));
        CreateText(panel.transform, "AvgHeartRateText", "Avg Heart Rate: 0 BPM", 22, new Vector2(0, -130), new Vector2(400, 35));
        
        // 关闭按钮
        GameObject closeBtn = CreateButton(panel.transform, "CloseButton", "Close");
        RectTransform btnRect = closeBtn.GetComponent<RectTransform>();
        btnRect.anchoredPosition = new Vector2(0, -230);
        btnRect.sizeDelta = new Vector2(150, 50);
        
        Debug.Log("✓ 评估面板已创建");
    }
    
    // 2. Relax提示面板
    void CreateRelaxPanel(Canvas canvas)
    {
        GameObject panel = new GameObject("RelaxPanel");
        panel.transform.SetParent(canvas.transform, false);
        
        Image panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(1f, 0.3f, 0.3f, 0.8f); // 红色半透明
        
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchoredPosition = new Vector2(0, 200);
        panelRect.sizeDelta = new Vector2(300, 80);
        panel.SetActive(false);
        
        CreateText(panel.transform, "Text", "❤️ Relax! Take a deep breath", 28, Vector2.zero, new Vector2(280, 60));
        
        Debug.Log("✓ Relax面板已创建");
    }
    
    // 3. 警告面板
    void CreateWarningPanels(Canvas canvas)
    {
        // 卡壳警告
        GameObject stutterWarning = new GameObject("StutterWarning");
        stutterWarning.transform.SetParent(canvas.transform, false);
        Image img1 = stutterWarning.AddComponent<Image>();
        img1.color = new Color(1f, 0.5f, 0f, 0.85f); // 橙色
        RectTransform rect1 = stutterWarning.GetComponent<RectTransform>();
        rect1.anchoredPosition = new Vector2(-300, 150);
        rect1.sizeDelta = new Vector2(200, 60);
        stutterWarning.SetActive(false);
        CreateText(stutterWarning.transform, "Text", "⚠️ Stuttering", 22, Vector2.zero, new Vector2(180, 50));
        
        // 沉默警告
        GameObject silenceWarning = new GameObject("SilenceWarning");
        silenceWarning.transform.SetParent(canvas.transform, false);
        Image img2 = silenceWarning.AddComponent<Image>();
        img2.color = new Color(1f, 0.8f, 0f, 0.85f); // 黄色
        RectTransform rect2 = silenceWarning.GetComponent<RectTransform>();
        rect2.anchoredPosition = new Vector2(300, 150);
        rect2.sizeDelta = new Vector2(200, 60);
        silenceWarning.SetActive(false);
        CreateText(silenceWarning.transform, "Text", "⏸️ Keep Talking", 22, Vector2.zero, new Vector2(180, 50));
        
        Debug.Log("✓ 警告面板已创建");
    }
    
    // 4. 专注度进度条
    void CreateAttentionBar(Canvas canvas)
    {
        // 背景
        GameObject barBg = new GameObject("AttentionFillBarBackground");
        barBg.transform.SetParent(canvas.transform, false);
        Image bgImg = barBg.AddComponent<Image>();
        bgImg.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);
        RectTransform bgRect = barBg.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 1);
        bgRect.anchorMax = new Vector2(0, 1);
        bgRect.pivot = new Vector2(0, 1);
        bgRect.anchoredPosition = new Vector2(150, -170);
        bgRect.sizeDelta = new Vector2(300, 25);
        
        // 填充条
        GameObject bar = new GameObject("AttentionFillBar");
        bar.transform.SetParent(barBg.transform, false);
        Image barImg = bar.AddComponent<Image>();
        barImg.color = Color.green;
        barImg.type = Image.Type.Filled;
        barImg.fillMethod = Image.FillMethod.Horizontal;
        barImg.fillAmount = 0.8f;
        
        RectTransform barRect = bar.GetComponent<RectTransform>();
        barRect.anchorMin = Vector2.zero;
        barRect.anchorMax = Vector2.one;
        barRect.offsetMin = Vector2.zero;
        barRect.offsetMax = Vector2.zero;
        
        Debug.Log("✓ 专注度进度条已创建");
    }
    
    // 辅助函数
    GameObject CreateText(Transform parent, string name, string content, float fontSize, Vector2 pos, Vector2 size)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        
        TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();
        text.text = content;
        text.fontSize = fontSize;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchoredPosition = pos;
        rect.sizeDelta = size;
        
        return obj;
    }
    
    GameObject CreateButton(Transform parent, string name, string buttonText)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        
        Image img = obj.AddComponent<Image>();
        img.color = new Color(0.2f, 0.6f, 1f);
        Button btn = obj.AddComponent<Button>();
        
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
}
