using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LineNumbers : MonoBehaviour
{
    public TMP_InputField _input;
    public TextMeshProUGUI text;

    private TMP_Text codeText;
    private RectTransform numbersRect;

    void Start()
    {
        codeText = _input.textComponent;
        numbersRect = GetComponent<RectTransform>();

        _input.onValueChanged.AddListener(UpdateNumbers);
        UpdateNumbers(_input.text);
        
    }

    void UpdateNumbers(string input)
    {
        int stringsCount = input.Split("\n").Length;

        text.text = "";

        for(int i = 1; i < stringsCount + 1; i++)
        {
            text.text += i.ToString() + "\n";
        }
    }

    void LateUpdate()
    {
        // Синхронизируем прокрутку
        Vector2 pos = numbersRect.anchoredPosition;
        pos.y = codeText.rectTransform.anchoredPosition.y;
        numbersRect.anchoredPosition = pos;
    }
}
