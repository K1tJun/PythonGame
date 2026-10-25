using TMPro;
using UnityEngine;

public class LineNumbers : MonoBehaviour
{
    public TMP_InputField _input;
    public TextMeshProUGUI text;

    private TMP_Text codeText;
    private RectTransform numbersRect;

    private int previousLineCount = -1;

    void Start()
    {
        codeText = _input.textComponent;
        numbersRect = GetComponent<RectTransform>();

        UpdateNumbersIfNeeded();
    }

    void Update()
    {
        UpdateNumbersIfNeeded();
    }

    void UpdateNumbersIfNeeded()
    {
        int stringsCount = _input.text.Split('\n').Length;

        if (stringsCount == previousLineCount)
            return;

        previousLineCount = stringsCount;

        System.Text.StringBuilder builder =
            new System.Text.StringBuilder();

        for (int i = 1; i <= stringsCount; i++)
        {
            builder.Append(i).Append('\n');
        }

        text.text = builder.ToString();
    }

    void LateUpdate()
    {
        // Синхронизируем прокрутку номеров и кода
        Vector2 pos = numbersRect.anchoredPosition;
        pos.y = codeText.rectTransform.anchoredPosition.y;
        numbersRect.anchoredPosition = pos;
    }
}
