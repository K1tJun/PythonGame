using UnityEngine;

public class Tests : MonoBehaviour
{
    public int lineNum = 1;
    public ErrorDebug erDebug;

    [ContextMenu("Test Debug")]
    public void TestDebug()
    {
        ErrorMes newMes = new();
        newMes.errorName = "If you are developer";
        newMes.description = "Test Debug";

        erDebug.HighlightLine(lineNum, newMes);
    }


}
