using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public Material emission_material;
    public Color _emColor;
    public float minblink = 1f;
    public float maxblink = 3f;

    public float speedblink = 0.5f;

    private float blickNow = 1f;

    private void Start()
    {
        blickNow = minblink;
    }


    private void Update()
    {
        blickNow = Mathf.Lerp(
            minblink,
            maxblink,
            Mathf.PingPong(speedblink * Time.time, 1f)
            );

        emission_material.SetColor("_EmissionColor", _emColor * blickNow);
    }
}
