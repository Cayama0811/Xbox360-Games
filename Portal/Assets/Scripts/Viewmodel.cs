using UnityEngine;

public class Viewmodel : MonoBehaviour
{

    public GameObject viewmodelHolder;
    public GameObject viewbobbingHolder;
    public GameObject viewpunchHolder;

    public float swayAmount = 1.5f;
    public float maxSwayAmount = 4.0f;
    public float smoothSpeed = 8.0f;

    public float bobAmount = 10.0f;
    public float bobSpeed = 2.5f;
    float initialBobSpeed;

    private float timer = 0.0f;

    private Vector3 currentPunchPos;
    private Vector3 targetPunchPos;

    public float recoilAmount = -0.15f;
    public float snappiness = 20.0f;
    public float returnSpeed = 8.0f;


    private Quaternion initialRotation;
    private Vector3 defaultPosBob;
    private Vector3 defaultPosPunch;
    private float smoothMouseX;
    private float smoothMouseY;

    void Start()
    {
        if (viewmodelHolder != null)
        {
            initialRotation = viewmodelHolder.transform.localRotation;
        }

        if (viewbobbingHolder != null)
        {
            defaultPosBob = viewbobbingHolder.transform.localPosition;
        }

        if (viewpunchHolder != null)
        {
            defaultPosPunch = viewpunchHolder.transform.localPosition;
        }

        initialBobSpeed = bobSpeed;
    }

    void Update()
    {
        Sway();
        Bobbing();
        RecoilProcess();

        // if you are not compiling for the xbox 360, but for other og platforms, like idk the ps vita or whatever change the #if and check the input manager
    #if UNITY_XBOX360
        // bug bug bug, it will keep doing the animation if you just maintain the trigger pressed, i already made a solution for it but im too lazy to do it here
        if (Input.GetAxis("Fire1") > 0.5f || Input.GetAxis("Fire2") > 0.5f)
    #else
        if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2"))
    #endif
        {
            ShootAnim();
        }
    }

    void Sway()
    {
        if (viewmodelHolder == null) return;

        // if you are not compiling for the xbox 360, but for other og platforms, like idk the ps vita or whatever change the #if and check the input manager
        #if UNITY_XBOX360

            float rawMouseX = Input.GetAxis("HorizontalR") * swayAmount;
            float rawMouseY = Input.GetAxis("VerticalR") * swayAmount;

        #else

            float rawMouseX = Input.GetAxis("Mouse X") * swayAmount;
            float rawMouseY = Input.GetAxis("Mouse Y") * swayAmount;

        #endif

        rawMouseX = Mathf.Clamp(rawMouseX, -maxSwayAmount, maxSwayAmount);
        rawMouseY = Mathf.Clamp(rawMouseY, -maxSwayAmount, maxSwayAmount);

        smoothMouseX = Mathf.Lerp(smoothMouseX, rawMouseX, Time.deltaTime * smoothSpeed);
        smoothMouseY = Mathf.Lerp(smoothMouseY, rawMouseY, Time.deltaTime * smoothSpeed);

        Quaternion targetRotation = Quaternion.Euler(smoothMouseY, -smoothMouseX, 0) * initialRotation;

        viewmodelHolder.transform.localRotation = Quaternion.Slerp(
            viewmodelHolder.transform.localRotation,
            targetRotation,
            Time.deltaTime * smoothSpeed
        );
    }

    void Bobbing()
    {
        if (viewbobbingHolder == null) return;

        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        bool isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetButton("Sprint");

        float targetBobSpeed = isSprinting ? initialBobSpeed * 1.8f : initialBobSpeed;

        if (Mathf.Abs(inputX) > 0.1f || Mathf.Abs(inputY) > 0.1f)
        {
            timer += Time.deltaTime * targetBobSpeed;
        }
        else
        {
            timer = 0f;
        }

        float wavesliceX = Mathf.Sin(timer);
        float wavesliceY = Mathf.Sin(timer * 2f);

        Vector3 targetPosition = defaultPosBob + new Vector3(wavesliceX * bobAmount, wavesliceY * bobAmount, 0);

        viewbobbingHolder.transform.localPosition = Vector3.Lerp(
            viewbobbingHolder.transform.localPosition,
            targetPosition,
            Time.deltaTime * smoothSpeed
        );
    }

    void ShootAnim()
    {
        if (viewpunchHolder == null) return;

        targetPunchPos += new Vector3(0f, 0f, recoilAmount);
    }

    void RecoilProcess()
    {
        if (viewpunchHolder == null) return;

        targetPunchPos = Vector3.Lerp(targetPunchPos, Vector3.zero, Time.deltaTime * returnSpeed);
        currentPunchPos = Vector3.Lerp(currentPunchPos, targetPunchPos, Time.deltaTime * snappiness);
        viewpunchHolder.transform.localPosition = defaultPosPunch + currentPunchPos;
    }
}