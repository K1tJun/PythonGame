using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class HighLighting : MonoBehaviour
{
    public TextMeshProUGUI highlightText;
    public TMP_InputField InputText;


    public string[] keywords = { "for", "if", "def", "while", "else" };
    public string[] funs = { "forward", "backward", "right", "left" };

    public string keyword_color = "#569CD6";
    public string fun_color = "#569CD6";

    void Start()
    {
        InputText.onValueChanged.AddListener(UpdateHighLight);
    }

    private void UpdateHighLight(string code)
    {

        // Экранируем символы, чтобы текст игрока не воспринимался как теги
        string result = code
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;");

        foreach (string keyword in keywords)
        {
            result = Regex.Replace(
                result,
                $@"\b{keyword}\b",
                $"<color={keyword_color}>{keyword}</color>"
            );
        }
        foreach (string fun in funs)
        {
            result = Regex.Replace(
                result,
                $@"\b{fun}\b",
                $"<color={fun_color}>{fun}</color>"
            );
        }

        // Возвращаем исходные символы в отображаемом тексте
        result = result
            .Replace("&amp;", "&")
            .Replace("&lt;", "<")
            .Replace("&gt;", ">");

        highlightText.text = result;
    }
}

// $"<color=#569CD6>{keyword}</color>"

