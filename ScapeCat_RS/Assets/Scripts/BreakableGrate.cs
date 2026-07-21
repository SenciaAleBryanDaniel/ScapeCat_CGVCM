using UnityEngine;

public class BreakableGrate : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Break if hit by player or a pushable object
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Pushable"))
        {
            BreakGrate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Also support triggers if configured as a trigger
        if (other.CompareTag("Player") || other.CompareTag("Pushable"))
        {
            BreakGrate();
        }
    }

    private void BreakGrate()
    {
        Debug.Log("[BreakableGrate] Grate broken!");
        // Disable or destroy the grate pieces
        gameObject.SetActive(false);

        // Optional: Play standard wood breaking sound / instantiate simple particles
    }
}
