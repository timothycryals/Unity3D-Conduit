using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform camera;

    public Animator anim;

    private bool ControllerEnabled;

    [SerializeField]
    private float MouseSensitivity = 2.0f;
    [SerializeField]
    private float GamePadSensitivity = 2.0f;

    private bool changingSensitivity = false;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private float maxX = 50f;
    private float minX = -70f;

    private float xAxis;
    private float yAxis;
    

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        StartCoroutine(delayCheck());
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        ChangeSensitivity();
        if (ControllerEnabled)
        {
            xAxis = Input.GetAxis("RightH") * GamePadSensitivity;
            yAxis = Input.GetAxis("RightV") * GamePadSensitivity;
        }
        else
        {
            xAxis = Input.GetAxis("Mouse X");
            yAxis = Input.GetAxis("Mouse Y");
        }

        yaw += MouseSensitivity * xAxis;
        pitch -= MouseSensitivity * yAxis;

        if (yaw > 360)
            yaw -= 360;
        else if (yaw < 0)
            yaw += 360;

        if (pitch > maxX)
            pitch = maxX;
        else if (pitch < minX)
            pitch = minX;

        transform.eulerAngles = new Vector3(0, yaw, 0);
        camera.localEulerAngles = new Vector3(pitch, 0, 0);
    }

    private IEnumerator delayCheck()
    {
        string[] inputs;
        while (true)
        {
            yield return new WaitForSecondsRealtime(2f);
            inputs = Input.GetJoystickNames();
            for (int i = 0; i < inputs.Length; i++)
            {
                if (!string.IsNullOrEmpty(inputs[i]))
                {
                    ControllerEnabled = true;
                    break;
                }
                else
                {
                    ControllerEnabled = false;
                    break;
                }
            }
        }
    }

    private void ChangeSensitivity()
    {
        if (!changingSensitivity)
        {
            if (ControllerEnabled)
            {
                if (Input.GetAxis(ControllerInputs.XBOX_DPAD_HORIZONTAL) < -0.1f)
                {
                    StartCoroutine(UpdateSensitivity());
                    GamePadSensitivity -= 0.1f;
                }
                else if (Input.GetAxis(ControllerInputs.XBOX_DPAD_HORIZONTAL) > 0.1f)
                {
                    StartCoroutine(UpdateSensitivity());
                    GamePadSensitivity += 0.1f;
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    StartCoroutine(UpdateSensitivity());
                    MouseSensitivity -= 0.1f;
                }
                else if (Input.GetKey(KeyCode.RightArrow))
                {
                    StartCoroutine(UpdateSensitivity());
                    MouseSensitivity += 0.1f;
                }
            }
        }
    }

    private IEnumerator UpdateSensitivity()
    {
        changingSensitivity = true;
        yield return new WaitForSeconds(0.2f);
        changingSensitivity = false;
    }
}
