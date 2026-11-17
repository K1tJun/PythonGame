using UnityEngine;

public class SceneData : MonoBehaviour
{
    public Vector3 robotPosition;
    public Vector3 robotTargetPosition;
    

    [Header("learn")] 
    public string[] learnText;


    [Header("targetComplete")]
    public Blinking bulbBlink;
}
