using UnityEngine;

public class Blinking : MonoBehaviour
{
    public MeshRenderer blinkObj;

    public Material emission_materialRed;
    public Material emission_materialGreen;

    public Color _emColorRed;
    public Color _emColorGreen;

    public float minblink = 1f;
    public float maxblink = 3f;
    public float speedblink = 0.5f;

    public bool levelComplete = false;

    private Material currentMaterial;
    private Color currentColor;

    private void Start()
    {
        blickNow = minblink;
        SetMaterial();
    }

    private float blickNow = 1f;

    private void SetMaterial()
    {
        currentMaterial = levelComplete
            ? emission_materialGreen
            : emission_materialRed;

        currentColor = levelComplete
            ? _emColorGreen
            : _emColorRed;

        blinkObj.material = currentMaterial;
    }

    private void Update()
    {
        blickNow = Mathf.Lerp(
            minblink,
            maxblink,
            Mathf.PingPong(speedblink * Time.time, 1f)
        );

        if (currentMaterial != (levelComplete
                ? emission_materialGreen
                : emission_materialRed))
        {
            SetMaterial();
        }

        currentMaterial.SetColor(
            "_EmissionColor",
            currentColor * blickNow
        );
    }
}