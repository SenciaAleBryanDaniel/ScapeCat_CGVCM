using UnityEngine;

public class CatCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 2.0f;
    public float height = 0.6f;
    public float positionSmooth = 10f;
    public float rotationSmooth = 5f;
    public LayerMask collisionLayers = ~0;

    private void Start()
    {
        if (target == null)
        {
            // Try to find the cat
            CatController cat = FindAnyObjectByType<CatController>();
            if (cat != null) target = cat.transform;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Calculate target camera position behind the player
        Vector3 targetDir = -target.forward;
        Vector3 desiredPosition = target.position + targetDir * distance + Vector3.up * height;

        // Perform raycast collision check from the target to the desired camera position
        Vector3 rayStart = target.position + Vector3.up * (height * 0.5f);
        Vector3 rayDir = desiredPosition - rayStart;
        float rayDist = rayDir.magnitude;

        if (Physics.Raycast(rayStart, rayDir.normalized, out RaycastHit hit, rayDist, collisionLayers))
        {
            // Move camera forward if hitting a wall to prevent clipping
            desiredPosition = hit.point + hit.normal * 0.15f;
        }

        // Smoothly interpolate position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * positionSmooth);

        // Smoothly look at the target's head area
        Vector3 lookTarget = target.position + Vector3.up * (height * 0.5f);
        Quaternion targetRotation = Quaternion.LookRotation(lookTarget - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmooth);
    }
}
