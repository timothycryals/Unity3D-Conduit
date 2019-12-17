using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkCameraController : NetworkBehaviour
{
    [SerializeField]
    private Transform camera;

    private NetworkPlayerController controller;

    private OnlinePlayer op;

    private Transform t;
    //public Transform objectToLookAt;

    public Transform[] Bones;

    public Animator anim;
    
    public Transform Head;

    private bool ControllerEnabled;

    [SerializeField]
    private float MouseSensitivity = 2.0f;
    [SerializeField]
    private float GamePadSensitivity = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private float maxX = 50f;
    private float minX = -50f;

    private float xAxis;
    private float yAxis;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<NetworkPlayerController>();
        t = transform;
        op = GetComponent<OnlinePlayer>();
        if (isLocalPlayer)
        {
            StartCoroutine(delayCheck());
            StartCoroutine(UpdateRotationOnClients());
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            SciFiWeaponBehaviour.AddRecoil += UpdateCameraForRecoil;
        }
    }

    void LateUpdate()
    {
        if (NetworkUIManager.isMenuOpen) return;

        if (isLocalPlayer && !op.IsDead)
        {
            if (!controller.isAimed)
            {
                if (ControllerEnabled)
                {
                    xAxis = Input.GetAxis("RightH") * (PlayerPreferences.GamepadSensitivity / 10f);
                    yAxis = Input.GetAxis("RightV") * (PlayerPreferences.GamepadSensitivity / 10f);
                }
                else
                {
                    xAxis = Input.GetAxis("Mouse X") * PlayerPreferences.MouseSensitivity;
                    yAxis = Input.GetAxis("Mouse Y") * PlayerPreferences.MouseSensitivity;
                }
            }
            else
            {
                if (ControllerEnabled)
                {
                    xAxis = Input.GetAxis("RightH") * (PlayerPreferences.GamepadSensitivity * PlayerPreferences.AimedSensitivityMultiplier);
                    yAxis = Input.GetAxis("RightV") * (PlayerPreferences.GamepadSensitivity * PlayerPreferences.AimedSensitivityMultiplier);
                }
                else
                {
                    xAxis = Input.GetAxis("Mouse X") * (PlayerPreferences.MouseSensitivity * PlayerPreferences.AimedSensitivityMultiplier);
                    yAxis = Input.GetAxis("Mouse Y") * (PlayerPreferences.MouseSensitivity * PlayerPreferences.AimedSensitivityMultiplier);
                }
            }

            yaw += xAxis;
            pitch -= yAxis;

            if (yaw > 360)
                yaw -= 360;
            else if (yaw < 0)
                yaw += 360;

            if (pitch > maxX)
                pitch = maxX;
            else if (pitch < minX)
                pitch = minX;

            t.eulerAngles = new Vector3(0, yaw, 0);
            camera.localEulerAngles = new Vector3(pitch, 0, 0);

            foreach (Transform b in Bones)
            {
                b.Rotate(t.right, pitch, Space.World);
            }
            
            //CmdUpdatePitch(pitch);
            //UpdateArms();
            camera.position = Head.position;
            //aIK.newPosition = camera.position + camera.forward;
            //positions.Clear();
            //rotations.Clear();
        }
        else
        {
            Bones[0].Rotate(t.right, pitch, Space.World);
        }
    }

    public void ResetOrientationOnSpawn()
    {
        pitch = 0;
        yaw = 0;

        t.eulerAngles = new Vector3(0, 0, 0);
        camera.localEulerAngles = new Vector3(0, 0, 0);

        foreach (Transform b in Bones)
        {
            b.Rotate(t.right, pitch, Space.World);
        }

        camera.position = Head.position;
    }

    IEnumerator UpdateRotationOnClients()
    {
        while (true)
        {
            CmdUpdatePitch(pitch);
            yield return new WaitForSeconds(1f / 9f);
        }
    }

    [Command]
    private void CmdUpdatePitch(float newPitch)
    {
        RpcUpdatePitch(newPitch);
    }

    [ClientRpc]
    private void RpcUpdatePitch(float newPitch)
    {
        if (!isLocalPlayer)
        {
            pitch = newPitch;
        }
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
                    MultiplayerGameManager.ControllerEnabled = true;
                    break;
                }
                else
                {
                    ControllerEnabled = false;
                    MultiplayerGameManager.ControllerEnabled = false;
                    break;
                }
            }
        }
    }

    private void UpdateCameraForRecoil(float x, float y)
    {
        yaw += x;
        pitch -= y;
        if (yaw > 360)
            yaw -= 360;
        else if (yaw < 0)
            yaw += 360;

        if (pitch > maxX)
            pitch = maxX;
        else if (pitch < minX)
            pitch = minX;

    }

    private void OnDisable()
    {
        SciFiWeaponBehaviour.AddRecoil -= UpdateCameraForRecoil;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
