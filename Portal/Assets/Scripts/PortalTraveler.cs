using UnityEngine;
using System.Collections;

public class PortalTraveler : MonoBehaviour
{
    // no physics props allowed yet, they will fall from the world because of the wall collision disabling, so check it out

    public Transform player;
    public Rigidbody rb;

    public GameObject visualRoot;

    public PortalLogic currentPortal;
    public PortalLogic destinationPortal;

    [HideInInspector]
    public GameObject cloneObject;

    [HideInInspector]
    public Transform cloneTransform;

    private Vector3 previousLocalPosition;

    private bool insidePortal = false;
    private bool justTeleported = false;

    private readonly Quaternion halfTurn =
        Quaternion.Euler(0f, 180f, 0f);

    void Start()
    {
        if (player == null)
            player = transform;

        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }

    void LateUpdate()
    {
        if (currentPortal == null)
            return;

        CheckPortalCrossing();

        if (insidePortal)
        {
            UpdateClone();
        }
    }

    public void EnterPortal(PortalLogic portal)
    {
        if (justTeleported) return;

        currentPortal = portal;
        destinationPortal = portal.linkedPortal;

        previousLocalPosition = currentPortal.transform.InverseTransformPoint(player.position);
        insidePortal = true;

        CreateClone();

        Collider playerCol = GetComponent<Collider>();
        if (playerCol == null) playerCol = GetComponentInChildren<Collider>();

        currentPortal.SetWallCollision(playerCol, false);
    }

    public void ExitPortal(PortalLogic portal)
    {
        if (currentPortal != portal) return;

        if (!justTeleported)
        {
            Collider playerCol = GetComponent<Collider>();
            if (playerCol == null) playerCol = GetComponentInChildren<Collider>();

            currentPortal.SetWallCollision(playerCol, true);

            currentPortal = null;
            destinationPortal = null;
            insidePortal = false;

            DestroyClone();
        }
    }

    private void CheckPortalCrossing()
    {
        if (currentPortal == null)
            return;

        Vector3 currentLocalPosition =
            currentPortal.transform.InverseTransformPoint(
                player.position
            );

        if (previousLocalPosition.z > 0f &&
            currentLocalPosition.z <= 0f)
        {
            Teleport();
            return;
        }

        previousLocalPosition = currentLocalPosition;
    }

    private void Teleport()
    {
        if (currentPortal == null || destinationPortal == null) return;

        justTeleported = true;

        Transform entry = currentPortal.transform;
        Transform exit = destinationPortal.transform;

        Vector3 localPosition = entry.InverseTransformPoint(player.position);
        Vector3 rotatedPosition = halfTurn * localPosition;
        Vector3 newWorldPosition = exit.TransformPoint(rotatedPosition);

        Quaternion relativeRotation = Quaternion.Inverse(entry.rotation) * player.rotation;
        Quaternion newRotation = exit.rotation * halfTurn * relativeRotation;

        Vector3 localVelocity = entry.InverseTransformDirection(rb.velocity);
        Vector3 rotatedVelocity = halfTurn * localVelocity;
        Vector3 newVelocity = exit.TransformDirection(rotatedVelocity);

        rb.position = newWorldPosition;
        rb.velocity = newVelocity;

        PlayerController controller = player.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.OnTeleport(newRotation);
        }
        else
        {
            player.rotation = newRotation;
            rb.rotation = newRotation;
        }

        Collider playerCol = GetComponent<Collider>();
        if (playerCol == null) playerCol = GetComponentInChildren<Collider>();

        currentPortal.SetWallCollision(playerCol, true);

        DestroyClone();

        currentPortal = null;
        destinationPortal = null;
        insidePortal = false;
        previousLocalPosition = Vector3.zero;

        StartCoroutine(TeleportCooldown());
    }

    private IEnumerator TeleportCooldown()
    {
        yield return new WaitForSeconds(0.05f);

        justTeleported = false;
    }

    private void CreateClone()
    {
        if (visualRoot == null)
        {
            Debug.LogError(
                "PortalTraveler: no visual root"
            );

            return;
        }

        if (cloneObject != null)
        {
            Destroy(cloneObject);
            cloneObject = null;
        }

        cloneObject =
            Instantiate(visualRoot);

        cloneObject.name =
            "Portal Player Clone";

        Collider[] colliders =
            cloneObject.GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }

        cloneTransform =
            cloneObject.transform;
    }

    private void DestroyClone()
    {
        if (cloneObject != null)
        {
            Destroy(cloneObject);

            cloneObject = null;
            cloneTransform = null;
        }
    }

    private void UpdateClone()
    {
        if (cloneTransform == null)
            return;

        if (destinationPortal == null)
            return;

        Transform entry =
            currentPortal.transform;

        Transform exit =
            destinationPortal.transform;

        Vector3 localPosition =
            entry.InverseTransformPoint(
                player.position
            );

        Vector3 rotatedPosition =
            halfTurn *
            localPosition;

        cloneTransform.position =
            exit.TransformPoint(
                rotatedPosition
            );

        Quaternion relativeRotation =
            Quaternion.Inverse(entry.rotation) *
            player.rotation;

        Quaternion rotatedRelativeRotation =
            halfTurn *
            relativeRotation;

        cloneTransform.rotation =
            exit.rotation *
            rotatedRelativeRotation;

        cloneTransform.localScale =
            visualRoot.transform.lossyScale;
    }
}