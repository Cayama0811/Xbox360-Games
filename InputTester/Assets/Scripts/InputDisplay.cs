using UnityEngine;
using UnityEngine.UI;

public class InputDisplay : MonoBehaviour
{
    // Buttons
    public Text AText;
    public Text BText;
    public Text XText;
    public Text YText;

    public Text StartText;
    public Text BackText;

    public Text LeftBumperText;
    public Text RightBumperText;

    // Sticks
    public Text LeftStickXText;
    public Text LeftStickYText;

    public Text RightStickXText;
    public Text RightStickYText;

    // D-Pad
    public Text DpadXText;
    public Text DpadYText;

    // Triggers
    public Text LeftTriggerText;
    public Text RightTriggerText;

    void Update()
    {
        UpdateButtons();
        UpdateSticks();
        UpdateDpad();
        UpdateTriggers();
    }

    void UpdateButtons()
    {
        AText.text = "A: " + Input.GetButton("AButton");
        BText.text = "B: " + Input.GetButton("BButton");
        XText.text = "X: " + Input.GetButton("XButton");
        YText.text = "Y: " + Input.GetButton("YButton");

        StartText.text = "Start: " + Input.GetButton("StartButton");
        BackText.text = "Back: " + Input.GetButton("BackButton");

        LeftBumperText.text = "LB: " + Input.GetButton("LeftBumper");
        RightBumperText.text = "RB: " + Input.GetButton("RightBumper");
    }

    void UpdateSticks()
    {
        float leftX = Input.GetAxis("LeftStickHorizontal");
        float leftY = Input.GetAxis("LeftStickVertical");

        float rightX = Input.GetAxis("RightStickHorizontal");
        float rightY = Input.GetAxis("RightStickVertical");

        LeftStickXText.text = "Left X: " + leftX.ToString("F2");
        LeftStickYText.text = "Left Y: " + leftY.ToString("F2");

        RightStickXText.text = "Right X: " + rightX.ToString("F2");
        RightStickYText.text = "Right Y: " + rightY.ToString("F2");
    }

    void UpdateDpad()
    {
        float dpadX = Input.GetAxis("DpadHorizontal");
        float dpadY = Input.GetAxis("DpadVertical");

        DpadXText.text = "DPad X: " + dpadX.ToString("F0");
        DpadYText.text = "DPad Y: " + dpadY.ToString("F0");
    }

    void UpdateTriggers()
    {
        float leftTrigger = Input.GetAxis("LeftTrigger");
        float rightTrigger = Input.GetAxis("RightTrigger");

        LeftTriggerText.text = "LT: " + leftTrigger.ToString("F2");
        RightTriggerText.text = "RT: " + rightTrigger.ToString("F2");
    }
}