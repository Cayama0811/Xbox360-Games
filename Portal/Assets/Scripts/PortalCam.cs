using UnityEngine;
using System.Collections;

//there is a bug with unity 5.4.1f that when you change a render texture quality the cameras linked to it will unlink, so if the error appears just link them back

public class PortalCam : MonoBehaviour
{
    public Transform playerCamera;
    public Transform portalCam;
    public Camera plCam;
    public Camera poCam;

    public Transform portalEntrada;
    public Transform portalSalida;

    void LateUpdate()
    {
        if (playerCamera == null || portalEntrada == null || portalSalida == null) return;

        poCam.fieldOfView = plCam.fieldOfView;
        poCam.aspect = plCam.aspect;

        CamRotation();
        CamMovement();
        SetObliqueMatrix();
    }

    void CamRotation()
    {
        if (playerCamera == null || portalEntrada == null || portalSalida == null) return;

        Quaternion relativeRot = Quaternion.Inverse(portalEntrada.rotation) * playerCamera.rotation;

        portalCam.rotation = portalSalida.rotation * Quaternion.Euler(0, 180, 0) * relativeRot;
    }

    void CamMovement()
    {
        if (playerCamera == null || portalEntrada == null || portalSalida == null) return;

        Vector3 relativePos = portalEntrada.InverseTransformPoint(playerCamera.position);

        Vector3 relativeRotPos = Quaternion.Euler(0, 180, 0) * relativePos;

        portalCam.position = portalSalida.TransformPoint(relativeRotPos);
    }


    //not sure if it works
    void SetObliqueMatrix()
    {
        Vector3 clipPlaneNormal = portalSalida.forward;
        Vector3 clipPlanePos = portalSalida.position;

        Vector3 cameraSpacePos = poCam.worldToCameraMatrix.MultiplyPoint(clipPlanePos);
        Vector3 cameraSpaceNormal = poCam.worldToCameraMatrix.MultiplyVector(clipPlaneNormal).normalized;

        Vector4 clipPlane = new Vector4(cameraSpaceNormal.x, cameraSpaceNormal.y, cameraSpaceNormal.z, -Vector3.Dot(cameraSpacePos, cameraSpaceNormal));
        poCam.projectionMatrix = plCam.CalculateObliqueMatrix(clipPlane);
    }
}