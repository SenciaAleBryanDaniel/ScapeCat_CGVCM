using UnityEngine;

public class VictoryManager : MonoBehaviour
{
    public static VictoryManager Instance { get; private set; }

    [Header("UI & Objects")]
    public GameObject victoryCanvas;

    private bool hasWon = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (victoryCanvas != null)
        {
            victoryCanvas.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasWon && other.CompareTag("Player"))
        {
            WinGame(other.gameObject);
        }
    }

    private void WinGame(GameObject player)
    {
        hasWon = true;
        Debug.Log("=====================================");
        Debug.Log("[VictoryManager] ¡VICTORIA FELINA! The cat escaped!");
        Debug.Log("=====================================");

        // Disable player controller to stop movement
        CatController controller = player.GetComponent<CatController>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        // Stop rigidbody movement
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // Show victory canvas/screen
        if (victoryCanvas != null)
        {
            victoryCanvas.SetActive(true);
        }
    }

    private void OnGUI()
    {
        if (hasWon)
        {
            // Dark overlay
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, new Color(0, 0, 0, 0.75f));
            texture.Apply();
            GUI.skin.box.normal.background = texture;
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), GUIContent.none);

            // Large title
            GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.fontSize = 48;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.normal.textColor = Color.yellow;

            GUI.Label(new Rect(0, Screen.height * 0.3f, Screen.width, 100), "¡VICTORIA FELINA!", titleStyle);

            // Subtitle
            GUIStyle subStyle = new GUIStyle(GUI.skin.label);
            subStyle.fontSize = 24;
            subStyle.alignment = TextAnchor.MiddleCenter;
            subStyle.normal.textColor = Color.white;

            GUI.Label(new Rect(0, Screen.height * 0.5f, Screen.width, 50), "¡Has ayudado al gato a escapar de la casa!", subStyle);

            // Restart tip
            GUIStyle hintStyle = new GUIStyle(GUI.skin.label);
            hintStyle.fontSize = 16;
            hintStyle.alignment = TextAnchor.MiddleCenter;
            hintStyle.normal.textColor = Color.gray;

            GUI.Label(new Rect(0, Screen.height * 0.7f, Screen.width, 50), "Presiona 'R' para volver a jugar.", hintStyle);

            // Check for restart key
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.R)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            }
        }
    }
}
