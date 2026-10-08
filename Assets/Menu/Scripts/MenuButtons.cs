using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public string mainMenu;
    public string hub;

    public void ExitEscapeGame()
    {
        if (SceneManager.GetActiveScene().name == mainMenu)
            Application.Quit();

        else if (SceneManager.GetActiveScene().name == hub)
            SceneManager.LoadScene(mainMenu);

        else
            SceneManager.LoadScene(hub);
    }

    public void SetNewScene(string _scene)
    {
        SceneManager.LoadScene(_scene);
    }
}
