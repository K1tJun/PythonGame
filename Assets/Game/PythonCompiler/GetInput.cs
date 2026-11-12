using UnityEngine;
using TMPro;

public class GetInput : MonoBehaviour
{
    public TMP_InputField _inputField;
    public PythonParser parser;

    private string UserCode;

    public void StartCode()
    {
        UserCode = _inputField.text;
        LinesCutter(UserCode);
    }

    private void LinesCutter(string _text)
    {
        string[] lines = _text.Split("\n");
        parser.chekTokens(lines);
    }
}
