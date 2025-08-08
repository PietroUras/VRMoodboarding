using UnityEngine;

public class SphereBoundLimiter : MonoBehaviour
{
    public float sphereRadius = 2f;
    public float minLatitude = -15f;
    public float maxLatitude = 15f;

    public bool faceAway = false;
    public bool gravityAlign = true;

    private Vector3 sphereCenter;

    void Start()
    {
        // Capture the initial camera position as the fixed sphere center
        if (Camera.main != null)
        {
            sphereCenter = Camera.main.transform.position;
        }

        // Move the object to be on the sphere at the correct offset initially
        Vector3 direction = (transform.position - sphereCenter).normalized;
        transform.position = sphereCenter + direction * sphereRadius;
    }

    void Update()
    {
        SetSphere();
        FaceUser();

    }

    void SetSphere()
    {
        Vector3 direction = (transform.position - sphereCenter).normalized;
        float latitude = Mathf.Asin(direction.y) * Mathf.Rad2Deg;
        float longitude = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

        latitude = Mathf.Clamp(latitude, minLatitude, maxLatitude);

        float latRad = latitude * Mathf.Deg2Rad;
        float lonRad = longitude * Mathf.Deg2Rad;

        Vector3 newPosition = new Vector3(
            sphereRadius * Mathf.Cos(latRad) * Mathf.Cos(lonRad),
            sphereRadius * Mathf.Sin(latRad),
            sphereRadius * Mathf.Cos(latRad) * Mathf.Sin(lonRad)
        );

        transform.position = sphereCenter + newPosition;
    }

    void FaceUser()
    {
        if (Camera.main == null) return;

        Vector3 directionToCamera = Camera.main.transform.position - transform.position;

        if (gravityAlign)
        {
            directionToCamera = Vector3.ProjectOnPlane(directionToCamera, Vector3.up);
        }

        Quaternion targetRotation = Quaternion.LookRotation(faceAway ? directionToCamera : -directionToCamera);
        transform.rotation = targetRotation;
    }

    public void SetSphereRadius(float radius)
    {
        sphereRadius = radius;
        SetSphere();
    }
}
