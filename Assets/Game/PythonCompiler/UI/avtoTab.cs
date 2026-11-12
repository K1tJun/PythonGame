using TMPro;
using UnityEngine;

public class CodeEditor : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;

    private const int IndentSize = 4;

    private bool pending;
    private string pendingText;
    private int pendingPosition;

    private void Awake()
    {
        inputField.onValidateInput += ValidateInput;
    }

    private char ValidateInput(string text, int charIndex, char addedChar)
    {
        if (addedChar != '\n' && addedChar != '\r')
            return addedChar;

        int lineStart = text.LastIndexOf(
            '\n', Mathf.Max(0, charIndex - 1));

        lineStart = lineStart == -1 ? 0 : lineStart + 1;

        string line = text.Substring(
            lineStart, charIndex - lineStart);

        int indent = 0;

        while (indent < line.Length && line[indent] == ' ')
            indent++;

        if (line.TrimEnd().EndsWith(":"))
            indent += IndentSize;

        string insertion = "\n" + new string(' ', indent);

        pendingText = text.Insert(charIndex, insertion);
        pendingPosition = charIndex + insertion.Length;
        pending = true;

        // Не даём TMP_InputField вставить Enter самостоятельно
        return '\0';
    }

    private void LateUpdate()
    {
        if (!pending || inputField == null)
            return;

        pending = false;

        inputField.SetTextWithoutNotify(pendingText);
        inputField.caretPosition = pendingPosition;
        inputField.ForceLabelUpdate();
    }

    private void OnDestroy()
    {
        if (inputField != null)
            inputField.onValidateInput -= ValidateInput;
    }
}