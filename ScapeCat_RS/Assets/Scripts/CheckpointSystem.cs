using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    public static CheckpointSystem Instance { get; private set; }

    [Header("Spawn Points for Zones (0 = Zona1 Atico, 1 = Zona2 Pasillo, etc.)")]
    public Transform[] spawnPoints;
    
    private int currentZoneIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCheckpoint(int zoneIndex)
    {
        if (zoneIndex > currentZoneIndex)
        {
            currentZoneIndex = zoneIndex;
            Debug.Log($"[CheckpointSystem] Checkpoint updated to Zona {zoneIndex + 1}");
        }
    }

    public Vector3 GetSpawnPosition()
    {
        if (spawnPoints != null && currentZoneIndex < spawnPoints.Length && spawnPoints[currentZoneIndex] != null)
        {
            return spawnPoints[currentZoneIndex].position;
        }
        return Vector3.zero;
    }

    public int GetCurrentZone() => currentZoneIndex;
}
