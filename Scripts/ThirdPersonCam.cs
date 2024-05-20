using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;

    public float rotationSpeed;

    public CameraStyle cameraStyle;

    public GameObject thirdPersonCam;
    public GameObject topDownCam;

    public enum CameraStyle
    {
        Basic,
        TopDown
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cameraStyle = CameraStyle.Basic;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SwitchCameraStyle(CameraStyle.Basic);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            SwitchCameraStyle(CameraStyle.TopDown);

        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        if (cameraStyle == CameraStyle.Basic || cameraStyle == CameraStyle.TopDown)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

            if (inputDir != Vector3.zero)
            {
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
            }
        }
    }

    void SwitchCameraStyle(CameraStyle newStyle)
    {
        Debug.Log("Started Switch");

        if (newStyle == CameraStyle.Basic)
        {
            thirdPersonCam.SetActive(true);
            topDownCam.SetActive(false);
            Debug.Log("Switched to basic");
        }
        else if (newStyle == CameraStyle.TopDown)
        {
            topDownCam.SetActive(true);
            thirdPersonCam.SetActive(false);
            Debug.Log("Switched to topdown");
        }

        cameraStyle = newStyle;
    }
}
