using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class CameraManager : MonoBehaviour
{
    private Camera cam;
    private PixelPerfectCamera pixelPerfect;

    private float dirX = 0.0f;
    private float dirY = 0.0f;
    // Camera Zoom var.
    private int curZoom;
    private int defualtZoom = 6;
    private int zoomMax = 8;
    private int zoomMin = 5;

    // private float borderScreen = 10f;
    private Vector3 defaultCamPos = new Vector3(0.0f, 0.0f, -10f);
    private float speedCam = 75f;


    private void Awake()
    {
        cam = GetComponent<Camera>();
        pixelPerfect = cam.GetComponent<PixelPerfectCamera>();
    }

    private void Start()
    {
        curZoom = defualtZoom;
        defaultCamPos = cam.transform.position;
    }

    private void Update()
    {
        CamZoom();
        CamMovement();
        DefaultCam();
    }

    private void CamZoom()
    {
        if (curZoom < zoomMax && Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            curZoom++;
        }
        if (curZoom > zoomMin && Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            curZoom--;
        }

        pixelPerfect.assetsPPU = (int)Mathf.Pow(2f, curZoom);
    }


    private void CamMovement()
    {
        if (Input.GetMouseButton(1) || Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            dirX += -Input.GetAxis("Mouse X") + Input.GetAxisRaw("Horizontal");
            dirY += -Input.GetAxis("Mouse Y") + Input.GetAxisRaw("Vertical");

            Vector2 direction = new Vector2(dirX, dirY).normalized;

            cam.transform.Translate(direction * (speedCam / curZoom) * Time.deltaTime);
        }

        else
        {
            dirX = 0;
            dirY = 0;
        }
    }

    private void DefaultCam()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            curZoom = defualtZoom;
            transform.position = defaultCamPos;
        }
    }
}

