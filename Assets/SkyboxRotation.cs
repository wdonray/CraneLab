using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxRotation : MonoBehaviour
{
    public Material skyboxMaterialDay;
    public Material skyboxMaterialNight;

    // Update is called once per frame
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", (Time.time % 360f));
    }
}
