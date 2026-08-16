using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public float speed = 10.0f;
    public float jumpForce = 10.0f;
    public float gravity = -20.0f;
    public float lerpSpeed = 20.0f;
    public Camera playerCamera;
    public Rigidbody rb;

    public AudioSource audioSource;
    public AudioClip[] footstepSounds;

    public float stepInterval = 0.4f;

    private float stepTimer = 0f;

    public float mouseSensitivity = 100.0f;
    public float upperLookLimit = -90f;
    public float lowerLookLimit = 90f;

    private float rotationX = 0;
    private float rotationY = 0;

    Vector3 targetVel;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Jump();
        MouseMovement();
        StepSound();
    }

    void FixedUpdate()
    {
        Move();

        if (!IsGrounded())
        {
            rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);
        }
    }

    void Move()
    {
        if (IsGrounded())
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 moveDirection = new Vector3(horizontal, 0f, vertical);

            if (moveDirection.magnitude > 1)
            {
                moveDirection.Normalize();
            }

            Vector3 globalMoveDirection = transform.TransformDirection(moveDirection);

            if (Input.GetButton("Sprint"))
                targetVel = globalMoveDirection * (speed * 1.5f / 10f);
            else
                targetVel = globalMoveDirection * (speed / 10f);

            Vector3 currentVel = rb.velocity;
            float newX = Mathf.Lerp(currentVel.x, targetVel.x, lerpSpeed * Time.fixedDeltaTime);
            float newZ = Mathf.Lerp(currentVel.z, targetVel.z, lerpSpeed * Time.fixedDeltaTime);

            rb.velocity = new Vector3(newX, currentVel.y, newZ);
        }
    }

    void Jump()
    {
        if (!IsGrounded()) return;
        if (Input.GetButtonDown("Jump"))
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
    }

    public bool IsGrounded()
    {
        Vector3 origin = transform.position + (Vector3.down * 0.9f);
        Vector3 halfExtents = new Vector3(0.3f, 0.05f, 0.3f);
        Vector3 direction = -transform.up;
        Quaternion orientation = transform.rotation;
        RaycastHit hitInfo;

        return Physics.BoxCast(origin, halfExtents, direction, out hitInfo, orientation, 0.15f, ~0);
    }

    public bool IsMoving()
    {
        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0f;

        return horizontalVelocity.magnitude > 0.01f;
    }

    void MouseMovement()
    {

        #if UNITY_XBOX360
            float mouseX = Input.GetAxis("HorizontalR") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("VerticalR") * mouseSensitivity * Time.deltaTime;
        #else
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        #endif

        rotationX -= mouseY;
        rotationY += mouseX;

        rotationX = Mathf.Clamp(rotationX, upperLookLimit, lowerLookLimit);

        transform.localRotation = Quaternion.Euler(0f, rotationY, 0f);

        if (playerCamera != null)
        {
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        }
    }

    void PlayFootStep()
    {
        if (footstepSounds.Length == 0)
            return;

        int randomIndex = Random.Range(0, footstepSounds.Length);

        audioSource.PlayOneShot(footstepSounds[randomIndex]);
    }

    void StepSound()
    {
        if (IsMoving() && IsGrounded())
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                PlayFootStep();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    public void OnTeleport(Quaternion newRotation)
    {
        transform.rotation = newRotation;

        rotationY = transform.eulerAngles.y;

        if (playerCamera != null)
        {
            rotationX = playerCamera.transform.localEulerAngles.x;
            if (rotationX > 180f) rotationX -= 360f;
        }
    }
}