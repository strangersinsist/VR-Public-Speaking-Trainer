using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 评估面板自动生成器
/// 挂载后Play一次即可生成完整的评估UI面板
/// </summary>
public class EvaluationPanelGenerator : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== 开始生成评估面板 ===");
        
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("未找到Canvas!");
            return;
        }
        
        CreateEvaluationPanel(canvas);
        
        Debug.Log("=== 评估面板生成完成！===");
        Debug.Log("提示：停止Play后面板会保留");
    }
    
    void CreateEvaluationPanel(Canvas canvas)
    {
        // 创建主面板
        GameObject panel = new GameObject("EvaluationPanel");
        panel.transform.SetParent(canvas.transform, false);
        
        Image panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0, 0, 0, 0.9f); // 半透明黑色背景
        
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        panel.SetActive(false); // 默认隐藏
        
        // 创建标题
        CreateText(panel.transform, "SummaryText", "评估结果", 42, new Vector2(0, 300), new Vector2(800, 60));
        
        // 创建5个维度评分
        CreateText(panel.transform, "FluencyScore", "流畅度: 0", 24, new Vector2(-200, 180), new Vector2(180, 40));
        CreateText(panel.transform, "ContentScore"," 内容逻辑: 0", 24, new Vector2(0, 180), new Vector2(180, 40));
        CreateText(panel.transform, "InteractionScore", "互动表现: 0", 24, new Vector2(200, 180), new Vector2(180, 40));
        CreateText(panel.transform, "TimeControlScore", "时间控制: 0", 24, new Vector2(-100, 110), new Vector2(180, 40));
        CreateText(panel.transform, "EmotionalStabilityScore", "情绪稳定: 0", 24, new Vector2(100, 110), new Vector2(180, 40));
        
        // 创建详细数据
        CreateText(panel.transform, "FillerWordsText", "填充词使用: 0个", 22, new Vector2(0, 30), new Vector2(400, 35));
        CreateText(panel.transform, "ContentDeviationText", "内容偏离: 0次", 22, new Vector2(0, -10), new Vector2(400, 35));
        CreateText(panel.transform, "EyeContactText", "眼神交流: 0%", 22, new Vector2(0, -50), new Vector2(400, 35));
        CreateText(panel.transform, "TimeOverrunText", "时间控制: 优秀", 22, new Vector2(0, -90), new Vector2(400, 35));
        CreateText(panel.transform, "AvgHeartRateText", "平均心率: 0 BPM", 22, new Vector2(0, -130), new Vector2(400, 35));
        
        // 创建关闭按钮
        GameObject closeBtn = CreateButton(panel.transform, "CloseButton", "关闭");
        RectTransform btnRect = closeBtn.GetComponent<RectTransform>();
        btnRect.anchoredPosition = new Vector2(0, -230);
        btnRect.sizeDelta = new Vector2(150, 50);
        
        Debug.Log("✓ 评估面板UI已创建");
    }
    
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
}
