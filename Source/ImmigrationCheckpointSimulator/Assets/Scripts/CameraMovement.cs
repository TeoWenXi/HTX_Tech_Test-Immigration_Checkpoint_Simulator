using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class CameraMovement : MonoBehaviour
{
    Camera camData;
    
    public float camHorizontalSpd = 10f;
    public float camVerticalSpd = 5f;
    public float zoomSensitivity = 1f;
    public float defaultZoom = 70f;
    public float minZoom = 30f;
    public float maxZoom = 80f;
    public Vector2 minPos = new Vector2(0, -80);
    public Vector2 maxPos = new Vector2(260, 80);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camData = GetComponent<Camera>();
        camData.orthographicSize = defaultZoom;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 scrollInput = Mouse.current.scroll.ReadValue();

        if (scrollInput.y != 0f)
        {
            // Zoom out (scroll down) or in (scroll up)
            camData.orthographicSize -= scrollInput.y * zoomSensitivity;

            // Clamp the orthographic size so it doesn't zoom too far in or out
            camData.orthographicSize = Mathf.Clamp(camData.orthographicSize, minZoom, maxZoom);
        }

        //Horizontal Movement
        if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
            transform.position = new Vector3(transform.position.x + Time.deltaTime * camHorizontalSpd, transform.position.y, transform.position.z);
        else if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
            transform.position = new Vector3(transform.position.x - Time.deltaTime * camHorizontalSpd, transform.position.y, transform.position.z);

        //Vertical Movement
        if (Keyboard.current.upArrowKey.isPressed || Keyboard.current.wKey.isPressed)
            transform.position = new Vector3(transform.position.x, transform.position.y + Time.deltaTime * camVerticalSpd, transform.position.z);
        else if (Keyboard.current.downArrowKey.isPressed || Keyboard.current.sKey.isPressed)
            transform.position = new Vector3(transform.position.x, transform.position.y - Time.deltaTime * camVerticalSpd, transform.position.z);

        //Clamp
        float camHeight = 2f * camData.orthographicSize;
        float camWidth = camHeight * camData.aspect;
        float halfWidth = camWidth / 2f;

        if (transform.position.x < minPos.x + halfWidth)
            transform.position = new Vector3(minPos.x + halfWidth, transform.position.y, transform.position.z);
        else if (transform.position.x > maxPos.x - halfWidth)
            transform.position = new Vector3(maxPos.x - halfWidth, transform.position.y, transform.position.z);

        if (transform.position.y < minPos.y + camData.orthographicSize)
            transform.position = new Vector3(transform.position.x, minPos.y + camData.orthographicSize, transform.position.z);
        else if (transform.position.y > maxPos.y - camData.orthographicSize)
            transform.position = new Vector3(transform.position.x, maxPos.y - camData.orthographicSize, transform.position.z);
    }
}
