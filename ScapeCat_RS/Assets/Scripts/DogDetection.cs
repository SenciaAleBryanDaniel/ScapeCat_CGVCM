using UnityEngine;

public class DogDetection : MonoBehaviour
{
    [Header("Dog Audio & Feedback")]
    public AudioSource barkAudio;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CatController cat = other.GetComponent<CatController>();
            if (cat != null)
            {
                // If the cat is NOT hidden in a HideZone
                if (!cat.IsHidden)
                {
                    CatchCat(cat);
                }
            }
        }
    }

    private void CatchCat(CatController cat)
    {
        Debug.LogWarning("[DogDetection] Dog detected the cat! WOOF!");

        // Play bark sound if configured
        if (barkAudio != null)
        {
            barkAudio.Play();
        }

        // Teleport cat to the last checkpoint position
        if (CheckpointSystem.Instance != null)
        {
            Vector3 respawnPos = CheckpointSystem.Instance.GetSpawnPosition();
            if (respawnPos != Vector3.zero)
            {
                Debug.Log($"[DogDetection] Teleporting cat back to checkpoint at {respawnPos}");
                cat.transform.position = respawnPos;
                
                // Clear velocity to prevent sliding after teleport
                Rigidbody rb = cat.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
            else
            {
                Debug.LogError("[DogDetection] No valid checkpoint spawn position found!");
            }
        }
    }
}
