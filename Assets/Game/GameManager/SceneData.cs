using UnityEngine;

public class SceneData : MonoBehaviour
{
    public Vector3 robotPosition;
    public Vector3 robotTargetPosition;

    [Space] 
    public Vector2 limitX;
    public Vector2 limitZ;

    [Header("learn")] 
    public string[] learnText;


    [Header("targetComplete")]
    public Blinking bulbBlink;
}
