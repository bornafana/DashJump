using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSettings : MonoBehaviour
{
    private Camera mainCam;
    public float portraitOrientation = 5f;
    public float landscapeOrientation = 12f;

    private void Start()
    {
        mainCam = GetComponent<Camera>();
    }

    private void Update()
    {
        if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft || Input.deviceOrientation == DeviceOrientation.LandscapeRight)
        {
            mainCam.orthographicSize = landscapeOrientation;
            Debug.Log("LAND");
        }
        else if (Input.deviceOrientation == DeviceOrientation.Portrait || Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)
            mainCam.orthographicSize = portraitOrientation;
    }
}
