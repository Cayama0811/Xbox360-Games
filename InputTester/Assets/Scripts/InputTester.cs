using UnityEngine;
using System.Collections;

public class InputTester : MonoBehaviour
{
    //Camera
    public GameObject center;

    //Input
    public GameObject LeftStickCenter;
    public GameObject RightStickCenter;
    public GameObject DpadCenter;
    public GameObject AButtonCenter;
    public GameObject BButtonCenter;
    public GameObject XButtonCenter;
    public GameObject YButtonCenter;
    public GameObject StartButtonCenter;
    public GameObject BackButtonCenter;
    public GameObject RightBumperCenter;
    public GameObject LeftBumperCenter;
    public GameObject RightTriggerCenter;
    public GameObject LeftTriggerCenter;

    //Skyboxes
    public GameObject[] skyboxes;

    //Cam Movement
    public float mouseSensitivity = 300.0f;
    private float rotationX = 25;
    private float rotationY = 0;

    //Limiters
    float stickMaxAngle = 20.0f;
    float dpadMaxAngle = 5.0f;
    float triggerMaxAngle = 25.0f;
    float pressDepth = 0.025f;

    //FreeCam
    bool freeCamera = false;
    bool combinationLocked = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        ChooseSkybox();
        MouseMovement();
    }

    void Update()
    {
        CheckCameraToggle();

        if (freeCamera)
        {
            MouseMovement();
        }
        else
        {
            InputTest();
        }
    }

    void MouseMovement()
    {

    #if UNITY_XBOX360
        float mouseX = Input.GetAxis("LeftStickHorizontal") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("LeftStickVertical") * mouseSensitivity * Time.deltaTime;
    #else
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
    #endif

        rotationX -= mouseY;
        rotationY += mouseX;

        if (center != null)
        {
            center.transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
        }
    }

    void ButtonAnimation(GameObject buttonCenter, string Button, bool isBumper)
    {
        if (Input.GetButtonDown(Button))
        {
            Vector3 position = buttonCenter.transform.localPosition;
            if (!isBumper)
                position.x += pressDepth;
            else
                position.y -= pressDepth;

            buttonCenter.transform.localPosition = position;
        }

        if (Input.GetButtonUp(Button))
        {
            Vector3 position = buttonCenter.transform.localPosition;
            if (!isBumper)
                position.x -= pressDepth;
            else
                position.y += pressDepth;

            buttonCenter.transform.localPosition = position;
        }
    }

    void StickAnimation(GameObject stickCenter, string HorizontalAxis, string VerticalAxis, float stickMaxAngle)
    {
        float horizontal = Input.GetAxis(HorizontalAxis);
        float vertical = Input.GetAxis(VerticalAxis);

        horizontal = Mathf.Clamp(horizontal, -1f, 1f);
        vertical = Mathf.Clamp(vertical, -1f, 1f);

        float xRotation = -vertical * stickMaxAngle;
        float yRotation = -horizontal * stickMaxAngle;

        if (stickCenter != null)
        {
            stickCenter.transform.localRotation = Quaternion.Euler(0.0f, -90.0f, 0.0f) * Quaternion.Euler(xRotation, yRotation, 0f);
        }
    }

    void TriggerAnimation(GameObject triggerCenter, string axis, float triggerMaxAngle)
    {
        float value = Mathf.Clamp01(-Input.GetAxis(axis));

        float rotation = value * triggerMaxAngle;

        if (triggerCenter != null)
        {
            triggerCenter.transform.localRotation = Quaternion.Euler(0.0f, -90.0f, 0.0f) * Quaternion.Euler(-rotation, 0f, 0f);
        }
    }

    void ChooseSkybox()
    {
        if (skyboxes.Length == 0)
            return;

        int randomIndex = Random.Range(0, skyboxes.Length);

        skyboxes[randomIndex].SetActive(true);
    }

    void CheckCameraToggle()
    {
        bool aPressed = Input.GetButtonUp("AButton");
        bool xPressed = Input.GetButtonUp("XButton");

        if (aPressed && xPressed)
        {
            if (!combinationLocked)
            {
                freeCamera = !freeCamera;
                combinationLocked = true;
            }
        }
        else
        {
            combinationLocked = false;
        }
    }

    void InputTest() {
        StickAnimation(LeftStickCenter, "LeftStickHorizontal", "LeftStickVertical", stickMaxAngle);
        StickAnimation(RightStickCenter, "RightStickHorizontal", "RightStickVertical", stickMaxAngle);
        StickAnimation(DpadCenter, "DpadHorizontal", "DpadVertical", dpadMaxAngle);
        ButtonAnimation(AButtonCenter, "AButton", false);
        ButtonAnimation(BButtonCenter, "BButton", false);
        ButtonAnimation(XButtonCenter, "XButton", false);
        ButtonAnimation(YButtonCenter, "YButton", false);
        ButtonAnimation(StartButtonCenter, "StartButton", false);
        ButtonAnimation(BackButtonCenter, "BackButton", false);
        ButtonAnimation(RightBumperCenter, "RightBumper", true);
        ButtonAnimation(LeftBumperCenter, "LeftBumper", true);
        TriggerAnimation(RightTriggerCenter, "RightTrigger", triggerMaxAngle);
        TriggerAnimation(LeftTriggerCenter, "LeftTrigger", triggerMaxAngle);
    }
}
