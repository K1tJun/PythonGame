using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public void ExitEscapeGame()
    {
        Application.Quit();
        Debug.Log("A");
    }

    public void SetNewScene(string _scene)
    {
        SceneManager.LoadScene(_scene);
    }
}
