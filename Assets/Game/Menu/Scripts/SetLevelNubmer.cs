using UnityEngine;

public class SetLevelNubmer : MonoBehaviour
{
    public GameManager gManager;
    
    public void setNUm(int i)
    {
        dataBetweenScenes.currentLevel = i;
    }

    public void SetNextLvlNum()
    {
        if((gManager.scenePref.Length - 1) > dataBetweenScenes.currentLevel)
            dataBetweenScenes.currentLevel++;
    }

}
