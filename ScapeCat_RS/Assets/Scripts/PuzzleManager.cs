using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }

    [Header("Zone 1: Attic")]
    public Rigidbody atticToolbox;
    public GameObject atticTrapdoor;
    public float toolboxUnlockY = 9.2f;

    [Header("Zone 2: Bedroom")]
    public Rigidbody bedroomSharpObject; // Coatrack
    public GameObject bedroomClothWall;
    public float sharpObjectUnlockY = 5.0f;

    [Header("Zone 3: Kitchen")]
    public Rigidbody kitchenBroom; // DoorLatch
    public GameObject basementTrapdoor;
    public float broomUnlockY = 0.5f;

    private bool atticUnlocked = false;
    private bool bedroomUnlocked = false;
    private bool basementUnlocked = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        // 1. Attic Puzzle: Toolbox falls off shelf
        if (!atticUnlocked && atticToolbox != null && atticTrapdoor != null)
        {
            if (atticToolbox.transform.position.y < toolboxUnlockY)
            {
                UnlockAttic();
            }
        }

        // 2. Bedroom Puzzle: Coatrack/SharpObject falls down to tear fabric wall
        if (!bedroomUnlocked && bedroomSharpObject != null && bedroomClothWall != null)
        {
            if (bedroomSharpObject.transform.position.y < sharpObjectUnlockY)
            {
                UnlockBedroom();
            }
        }

        // 3. Kitchen Puzzle: Broom/DoorLatch falls to release trapdoor
        if (!basementUnlocked && kitchenBroom != null && basementTrapdoor != null)
        {
            // Broom starts diagonal. If it falls flat (Y drops), or gets knocked away
            if (kitchenBroom.transform.position.y < broomUnlockY || Mathf.Abs(kitchenBroom.transform.position.y - 0.9f) > 0.5f)
            {
                UnlockBasement();
            }
        }
    }

    private void UnlockAttic()
    {
        atticUnlocked = true;
        Debug.Log("[PuzzleManager] Attic trapdoor unlocked!");
        // Disable trapdoor collider to let cat drop through
        var col = atticTrapdoor.GetComponent<Collider>();
        if (col != null) col.enabled = false;
        
        // Add some physical feedback: let the trapdoor fall or swing down
        Rigidbody rb = atticTrapdoor.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }
        else
        {
            atticTrapdoor.SetActive(false); // Fallback: hide it
        }
    }

    private void UnlockBedroom()
    {
        bedroomUnlocked = true;
        Debug.Log("[PuzzleManager] Bedroom cloth wall torn open!");
        if (bedroomClothWall != null)
        {
            bedroomClothWall.SetActive(false); // Destroy/hide the cloth wall
        }
    }

    private void UnlockBasement()
    {
        basementUnlocked = true;
        Debug.Log("[PuzzleManager] Basement trapdoor unlocked!");
        var col = basementTrapdoor.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Rigidbody rb = basementTrapdoor.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }
        else
        {
            basementTrapdoor.SetActive(false);
        }
    }
}
