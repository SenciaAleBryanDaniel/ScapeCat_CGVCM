using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class CatController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 4f;
    public float crawlSpeed = 2f;
    public float jumpHeight = 1.2f;
    public float rotationSpeed = 720f;
    public LayerMask groundLayer = ~0;

    private Rigidbody rb;
    private BoxCollider catCollider;
    private bool isGrounded;
    private int crawlSpaceCount = 0;
    private int hideZoneCount = 0;

    public bool IsHidden => hideZoneCount > 0;

    
    // Original dimensions of the box collider
    private Vector3 normalSize = new Vector3(0.3f, 0.4f, 0.9f);
    private Vector3 normalCenter = new Vector3(0f, 0.2f, 0f);
    
    // Crawl dimensions
    private Vector3 crawlSize = new Vector3(0.3f, 0.25f, 0.9f);
    private Vector3 crawlCenter = new Vector3(0f, 0.125f, 0f);

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        catCollider = GetComponent<BoxCollider>();
        
        // Setup rigidbody for stable physics movement
        rb.useGravity = true;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Initialize collider size
        catCollider.size = normalSize;
        catCollider.center = normalCenter;
    }

    private void Start()
    {
        // Teleport to initial spawn point if available
        if (CheckpointSystem.Instance != null)
        {
            Vector3 spawnPos = CheckpointSystem.Instance.GetSpawnPosition();
            if (spawnPos != Vector3.zero)
            {
                transform.position = spawnPos;
            }
        }
    }

    private void Update()
    {
        // Ground check
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.15f, groundLayer);

        // Movement input using New Input System (polling API for direct keyboard checks)
        Vector2 input = Vector2.zero;
        bool jumpPressed = false;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) input.y += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) input.y -= 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) input.x -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) input.x += 1f;

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                jumpPressed = true;
            }
        }

        // Apply crawl state based on crawl space triggers
        bool shouldCrawl = crawlSpaceCount > 0;
        if (shouldCrawl)
        {
            catCollider.size = crawlSize;
            catCollider.center = crawlCenter;
        }
        else
        {
            catCollider.size = normalSize;
            catCollider.center = normalCenter;
        }

        // Calculate movement
        Vector3 moveDir = new Vector3(input.x, 0f, input.y).normalized;
        float currentSpeed = shouldCrawl ? crawlSpeed : moveSpeed;

        if (moveDir.magnitude > 0.1f)
        {
            // Rotate towards direction of movement
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);

            // Move the Rigidbody
            Vector3 targetVelocity = moveDir * currentSpeed;
            rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
            
            // Try climbing automatic ledges while moving
            TryClimb();
        }
        else
        {
            // Dampen horizontal movement when no keys are pressed
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }

        // Jump physics
        if (jumpPressed && isGrounded && !shouldCrawl)
        {
            float gravity = Physics.gravity.y;
            float jumpVelocity = Mathf.Sqrt(2f * -gravity * jumpHeight);
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpVelocity, rb.linearVelocity.z);
        }
    }

    private void TryClimb()
    {
        // Horizontal raycast at feet level
        Vector3 rayStart = transform.position + Vector3.up * 0.05f;
        Vector3 dir = transform.forward;
        float checkDist = 0.5f;

        if (Physics.Raycast(rayStart, dir, out RaycastHit hit, checkDist))
        {
            // Check if there is free space above the ledge (0.6u high)
            Vector3 highRayStart = transform.position + Vector3.up * 0.65f;
            if (!Physics.Raycast(highRayStart, dir, checkDist))
            {
                // Raycast downwards from above the ledge to find its top surface
                Vector3 checkDownStart = hit.point + dir * 0.15f + Vector3.up * 0.7f;
                if (Physics.Raycast(checkDownStart, Vector3.down, out RaycastHit downHit, 0.8f))
                {
                    float heightDiff = downHit.point.y - transform.position.y;
                    if (heightDiff > 0.05f && heightDiff <= 0.65f)
                    {
                        // Safe climb up
                        transform.position = downHit.point + Vector3.up * 0.02f;
                        rb.linearVelocity = Vector3.zero;
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CrawlSpace"))
        {
            crawlSpaceCount++;
        }
        else if (other.CompareTag("HideZone"))
        {
            hideZoneCount++;
            Debug.Log("[CatController] Entered HideZone. Current count: " + hideZoneCount);
        }
        
        // Handle Zone transitions via specific trigger names or tag
        if (other.name == "ExitToHallway")
        {
            CheckpointSystem.Instance?.SetCheckpoint(1); // Zona 2
        }
        else if (other.name == "ExitToTragaluz")
        {
            CheckpointSystem.Instance?.SetCheckpoint(2); // Zona 3
        }
        else if (other.name == "ExitToBasement")
        {
            CheckpointSystem.Instance?.SetCheckpoint(3); // Zona 4
        }
        else if (other.name == "ExitToGarden" || other.name == "ExitDuctToGarden")
        {
            CheckpointSystem.Instance?.SetCheckpoint(4); // Zona 5
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CrawlSpace"))
        {
            crawlSpaceCount = Mathf.Max(0, crawlSpaceCount - 1);
        }
        else if (other.CompareTag("HideZone"))
        {
            hideZoneCount = Mathf.Max(0, hideZoneCount - 1);
            Debug.Log("[CatController] Exited HideZone. Current count: " + hideZoneCount);
        }
    }
}
