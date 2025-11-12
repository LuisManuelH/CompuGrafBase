using UnityEngine;

public class scripManagerSky : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float skySpeed;

    // Update is called once per frame
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time*skySpeed); 
    }
}
