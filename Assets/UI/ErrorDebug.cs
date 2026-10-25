using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ErrorDebug : MonoBehaviour
{
    [Header("References")]
    public TMP_InputField inputField;
    public RectTransform highlight;
    public Image highlightImage;

    [Space]


    [Header("Debug Panel Position")]
    public RectTransform debugPanelRect;
    public TextMeshProUGUI debugHead;
    public TextMeshProUGUI debugText;
    public float panelOffsetY = 8f;
    public float panelOffsetX = 0f;


    [Space]

    [Header("Line Settings")]
    public int lineNumber = 1;
    public string errorHead = "Head";
    public string errorText = "гг моим бабкам, блиииин";
    public float paddingX = 4f;
    public float paddingY = 5f;

    [Header("Blink Settings")]
    public bool blinkEnabled = true;
    public float minAlpha = 0.3f;
    public float maxAlpha = 1f;
    public float blinkSpeed = 2f;

    private void Update()
    {
        if (!blinkEnabled || highlightImage == null)
            return;

        float t = (Mathf.Sin(Time.time * blinkSpeed) + 1f) / 2f;

        Color color = highlightImage.color;
        color.a = Mathf.Lerp(minAlpha, maxAlpha, t);
        highlightImage.color = color;
    }

    public void HighlightLine(int number, ErrorMes message)
    {
        // Ноль — скрыть рамку
        if (number == 0)
        {
            highlight.gameObject.SetActive(false);
            debugPanelRect.gameObject.SetActive(false);
            return;
        }

        errorHead = message.errorName;
        errorText = message.description;
        lineNumber = number;
        Debug.Log("erros");

        UpdateHighlight();
    }

    public void UpdateHighlight()
    {
        if (inputField == null || highlight == null)
            return;

        TMP_Text text = inputField.textComponent;
        text.ForceMeshUpdate();

        TMP_TextInfo info = text.textInfo;

        if (lineNumber < 1 || lineNumber > info.lineCount)
        {
            HideError();
            return;
        }

        TMP_LineInfo line = info.lineInfo[lineNumber - 1];

        if (line.characterCount == 0)
        {
            highlight.gameObject.SetActive(false);
            debugPanelRect.gameObject.SetActive(false);
            return;
        }

        float minX = line.lineExtents.min.x - paddingX;
        float maxX = line.lineExtents.max.x + paddingX;
        float minY = line.descender - paddingY;
        float maxY = line.ascender + paddingY;

        RectTransform textRect = text.rectTransform;
        RectTransform parentRect = highlight.parent as RectTransform;

        Vector3 bottomLeft = parentRect.InverseTransformPoint(
            textRect.TransformPoint(new Vector3(minX, minY, 0f))
        );

        Vector3 topRight = parentRect.InverseTransformPoint(
            textRect.TransformPoint(new Vector3(maxX, maxY, 0f))
        );

        highlight.anchorMin = new Vector2(0f, 1f);
        highlight.anchorMax = new Vector2(0f, 1f);
        highlight.pivot = new Vector2(0f, 1f);

        highlight.localPosition = new Vector3(
            bottomLeft.x,
            topRight.y,
            0f
        );

        highlight.sizeDelta = new Vector2(
            topRight.x - bottomLeft.x,
            topRight.y - bottomLeft.y
        );

        highlight.gameObject.SetActive(true);
        debugPanelRect.gameObject.SetActive(true);
        UpdatePanelPosition();
    }


    private void UpdatePanelPosition()
    {
        if (highlight == null || debugPanelRect == null)
            return;

        RectTransform panelParent = debugPanelRect.parent as RectTransform;

        if (panelParent == null)
            return;

        // Получаем нижнюю левую точку рамки в мировых координатах
        Vector3 worldBottomLeft = highlight.TransformPoint(
            new Vector3(0f, -highlight.rect.height, 0f)
        );

        // Переводим координаты в пространство родителя панели
        Vector3 localBottomLeft = panelParent.InverseTransformPoint(worldBottomLeft);

        // Выравниваем панель по левой границе рамки
        debugPanelRect.anchorMin = new Vector2(0.5f, 0.5f);
        debugPanelRect.anchorMax = new Vector2(0.5f, 0.5f);
        debugPanelRect.pivot = new Vector2(0f, 1f);

        debugPanelRect.localPosition = new Vector3(
            localBottomLeft.x + panelOffsetX,
            localBottomLeft.y - panelOffsetY,
            0f
        );

        debugHead.text = errorHead;
        debugText.text = errorText;
    }


    
    void HideError()
    {
        highlight.gameObject.SetActive(false);
        debugPanelRect.gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        if (inputField != null)
            inputField.onValueChanged.AddListener(OnCodeChanged);
    }

    private void OnDisable()
    {
        if (inputField != null)
            inputField.onValueChanged.RemoveListener(OnCodeChanged);
    }

    private void OnCodeChanged(string newCode)
    {
        HideError();
    }
}