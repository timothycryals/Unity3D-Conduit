using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInputs
{

    //Mappings for Xbox One controller

    public const KeyCode XBOX_A = KeyCode.Joystick1Button0;
    public const KeyCode XBOX_B = KeyCode.Joystick1Button1;
    public const KeyCode XBOX_X = KeyCode.Joystick1Button2;
    public const KeyCode XBOX_Y = KeyCode.Joystick1Button3;
    public const KeyCode XBOX_LB = KeyCode.Joystick1Button4;
    public const KeyCode XBOX_RB = KeyCode.Joystick1Button5;
    public const KeyCode XBOX_VIEW = KeyCode.Joystick1Button6;
    public const KeyCode XBOX_MENU = KeyCode.Joystick1Button7;
    public const string XBOX_RT = "XboxOneRightTrigger";
    public const string XBOX_LT = "XboxOneLeftTrigger";
    public const string XBOX_DPAD_VERTICAL = "XboxOneDpadVertical";
    public const string XBOX_DPAD_HORIZONTAL = "XboxOneDpadHorizontal";

    //Mappings for PlayStation 4 controller

    public const KeyCode PS4_SQUARE = KeyCode.Joystick1Button0;
    public const KeyCode PS4_X = KeyCode.Joystick1Button1;
    public const KeyCode PS4_CIRCLE = KeyCode.Joystick1Button2;
    public const KeyCode PS4_TRIANGLE = KeyCode.Joystick1Button3;
    public const KeyCode PS4_L1 = KeyCode.Joystick1Button4;
    public const KeyCode PS4_R1 = KeyCode.Joystick1Button5;
    public const KeyCode PS4_L2 = KeyCode.Joystick1Button6;
    public const KeyCode PS4_R2 = KeyCode.Joystick1Button7;
}
