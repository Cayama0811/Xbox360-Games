using UnityEngine;

public class PortalGun : MonoBehaviour
{
    public Transform PortalB;
    public Transform PortalO;

    public Transform playerCamera;

    public AudioSource audioSource;
    public AudioClip shootSound;

    private bool fire1WasPressed = false;
    private bool fire2WasPressed = false;

    public float portalOffset = 0.01f;


    void Update()
    {
        Shoot();
    }

    void Shoot()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

#if UNITY_XBOX360

    float fire1 = Input.GetAxis("Fire1");
    float fire2 = Input.GetAxis("Fire2");

    bool fire1Pressed = fire1 > 0.5f;
    bool fire2Pressed = fire2 > 0.5f;

    bool fire1Down = fire1Pressed && !fire1WasPressed;
    bool fire2Down = fire2Pressed && !fire2WasPressed;

    if (fire1Down)
    {
        if (PlacePortal(PortalB))
            audioSource.PlayOneShot(shootSound);
    }
    else if (fire2Down)
    {
        if (PlacePortal(PortalO))
            audioSource.PlayOneShot(shootSound);
    }

    fire1WasPressed = fire1Pressed;
    fire2WasPressed = fire2Pressed;

#else

        if (Input.GetButtonDown("Fire1"))
        {
            if (PlacePortal(PortalB))
                audioSource.PlayOneShot(shootSound);
        }
        else if (Input.GetButtonDown("Fire2"))
        {
            if (PlacePortal(PortalO))
                audioSource.PlayOneShot(shootSound);
        }

#endif

        Debug.DrawRay(
            origin,
            direction * 999f,
            Color.red
        );
    }

    bool PlacePortal(Transform portal)
    {
        if (portal == null)
            return false;

        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        RaycastHit hitInfo;

        if (!Physics.Raycast(
            origin,
            direction,
            out hitInfo,
            999f))
        {
            return false;
        }

        if (hitInfo.collider.tag == "Portal")
        {
            return false;
        }

        PortalLogic logic =
            portal.GetComponent<PortalLogic>();

        if (logic == null)
        {
            Debug.LogError(
                "the portal has no portal logic"
            );

            return false;
        }

        portal.position =
            hitInfo.point +
            hitInfo.normal * portalOffset;

        portal.rotation =
            CalculatePortalRotation(
                hitInfo.normal
            );

        logic.SetWallCollider(
            hitInfo.collider
        );

        return true;
    }

    Quaternion CalculatePortalRotation(
        Vector3 surfaceNormal)
    {

        Vector3 portalForward =
            surfaceNormal;

        Vector3 portalUp =
            Vector3.ProjectOnPlane(
                Vector3.up,
                surfaceNormal
            );

        if (portalUp.sqrMagnitude > 0.001f)
        {
            portalUp.Normalize();

            return Quaternion.LookRotation(
                portalForward,
                portalUp
            );
        }

        Vector3 playerForward =
            playerCamera != null
            ? playerCamera.forward
            : transform.forward;


        Vector3 floorForward =
            Vector3.ProjectOnPlane(
                playerForward,
                surfaceNormal
            );


        if (floorForward.sqrMagnitude < 0.001f)
        {
            floorForward =
                Vector3.ProjectOnPlane(
                    Vector3.forward,
                    surfaceNormal
                );
        }


        floorForward.Normalize();

        return Quaternion.LookRotation(
            portalForward,
            floorForward
        );
    }
}