using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 简化版评估面板生成器 - 只生成基础评估面板
/// </summary>
public class SimpleEvalPanelGen : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== 开始生成简化评估面板 ===");
        
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("未找到Canvas!");
            return;
        }
        
        // 检查是否已存在
        if (GameObject.Find("EvaluationPanel") != null)
        {
            Debug.Log("EvaluationPanel已存在，跳过生成");
            return;
        }
        
        CreateSimpleEvaluationPanel(canvas);
        
        Debug.Log("=== 评估面板生成完成！停止Play后会保留 ===");
    }
    
    void CreateSimpleEvaluationPanel(Canvas canvas)
    {
        // 主面板 - 全屏黑色半透明
        GameObject panel = new GameObject("EvaluationPanel");
        panel.transform.SetParent(canvas.transform, false);
        
        Image panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0, 0, 0, 0.95f); // 几乎不透明
        
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        panel.SetActive(true); // 先设为true，方便查看
        
        // 大标题
        GameObject title = CreateText(panel.transform, "SummaryText", 
            "Performance Summary\nOverall Score: 0/100", 
            36, new Vector2(0, 250), new Vector2(700, 100), Color.white);
        
        // 5个维度（横排）
        CreateText(panel.transform, "FluencyScore", "Fluency: 0", 
            22, new Vector2(-300, 150), new Vector2(140, 35), Color.cyan);
        CreateText(panel.transform, "ContentScore", "Content: 0", 
            22, new Vector2(-150, 150), new Vector2(140, 35), Color.cyan);
        CreateText(panel.transform, "InteractionScore", "Interaction: 0", 
            22, new Vector2(0, 150), new Vector2(140, 35), Color.cyan);
        CreateText(panel.transform, "TimeControlScore", "Time: 0", 
            22, new Vector2(150, 150), new Vector2(140, 35), Color.cyan);
        CreateText(panel.transform, "EmotionalStabilityScore", "Emotion: 0", 
            22, new Vector2(300, 150), new Vector2(140, 35), Color.cyan);
        
        // 详细数据（竖排）
        CreateText(panel.transform, "FillerWordsText", "Filler Words: 0", 
            20, new Vector2(0, 60), new Vector2(400, 30), Color.yellow);
        CreateText(panel.transform, "ContentDeviationText", "Content Deviation: 0", 
            20, new Vector2(0, 20), new Vector2(400, 30), Color.yellow);
        CreateText(panel.transform, "EyeContactText", "Eye Contact: 0%", 
            20, new Vector2(0, -20), new Vector2(400, 30), Color.yellow);
        CreateText(panel.transform, "TimeOverrunText", "Time Control: Good", 
            20, new Vector2(0, -60), new Vector2(400, 30), Color.yellow);
        CreateText(panel.transform, "AvgHeartRateText", "Avg Heart Rate: 0 BPM", 
            20, new Vector2(0, -100), new Vector2(400, 30), Color.yellow);
        
        // 关闭按钮
        CreateButton(panel.transform, "CloseButton", "Close", 
            new Vector2(0, -200), new Vector2(200, 50));
        
        Debug.Log("✓ 评估面板已创建（全屏黑色背景，白色/黄色文字）");
    }
    
    GameObject CreateText(Transform parent, string name, string content, 
        float fontSize, Vector2 pos, Vector2 size, Color color)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        
        TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();
        text.text = content;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = TextAlignmentOptions.Center;
        
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchoredPosition = pos;
        rect.sizeDelta = size;
        
        return obj;
    }
    
    void CreateButton(Transform parent, string name, string btnText, 
        Vector2 pos, Vector2 size)
    {
        GameObject btn = new GameObject(name);
        btn.transform.SetParent(parent, false);
        
        Image img = btn.AddComponent<Image>();
        img.color = new Color(0.2f, 0.7f, 0.2f); // 绿色按钮
        
        Button button = btn.AddComponent<Button>();
        
        // 添加点击事件 - 关闭面板
        button.onClick.AddListener(() => {
            parent.gameObject.SetActive(false);
            Debug.Log("评估面板已关闭");
        });
        
        RectTransform btnRect = btn.GetComponent<RectTransform>();
        btnRect.anchoredPosition = pos;
        btnRect.sizeDelta = size;
        
        // 按钮文字
        GameObject txtObj = new GameObject("Text");
        txtObj.transform.SetParent(btn.transform, false);
        
        TextMeshProUGUI txt = txtObj.AddComponent<TextMeshProUGUI>();
        txt.text = btnText;
        txt.fontSize = 24;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = Color.white;
        
        RectTransform txtRect = txtObj.GetComponent<RectTransform>();
        txtRect.anchorMin = Vector2.zero;
        txtRect.anchorMax = Vector2.one;
        txtRect.offsetMin = Vector2.zero;
        txtRect.offsetMax = Vector2.zero;
    }
}
