using UnityEngine;

public class PortalLogic : MonoBehaviour
{
    // Its very easy for the player to fall from the world to their dead, im not sure how to make it not horrible, lets just say glados gave you a horrid portal gun

    public PortalLogic linkedPortal;
    public Collider wallCollider;

    public void SetWallCollider(Collider newWallCollider)
    {
        if (wallCollider != null &&
            wallCollider != newWallCollider)
        {
            wallCollider.enabled = true;
        }

        wallCollider = newWallCollider;
    }

    public void SetWallCollision(
        Collider playerCollider,
        bool enableWall)
    {
        if (wallCollider != null &&
            playerCollider != null)
        {
            wallCollider.enabled = enableWall;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PortalTraveler traveller =
            other.GetComponentInParent<PortalTraveler>();

        if (traveller == null)
            return;

        traveller.EnterPortal(this);
    }

    private void OnTriggerExit(Collider other)
    {
        PortalTraveler traveller =
            other.GetComponentInParent<PortalTraveler>();

        if (traveller == null)
            return;

        traveller.ExitPortal(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawLine(
            transform.position,
            transform.position +
            transform.forward * 2f
        );

        Gizmos.color = Color.green;

        Gizmos.DrawLine(
            transform.position,
            transform.position +
            transform.up * 1.5f
        );
    }
}