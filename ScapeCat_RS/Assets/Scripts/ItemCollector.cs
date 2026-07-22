using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ItemCollector : MonoBehaviour
{
    [Header("Configuración")]
    public ItemData itemData;
    public float rotationSpeed = 50f;
    public float floatSpeed = 1f;
    public float floatHeight = 0.3f;
    public string mensajeRecoleccion = "Presiona E para recoger";

    private Vector3 startPos;
    private bool canCollect = true;
    private bool playerNear = false;
    private InventarioController inventario;
    private GameObject mensajeUI;

    void Start()
    {
        startPos = transform.position;

        if (itemData == null)
        {
            itemData = ScriptableObject.CreateInstance<ItemData>();
            itemData.nombre = gameObject.name;
        }

        inventario = FindAnyObjectByType<InventarioController>();
        CrearMensajeUI();
    }

    void Update()
    {
        // Rotación y flotación
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        float offset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = startPos + Vector3.up * offset;

        // Mostrar mensaje si el jugador está cerca
        if (mensajeUI != null)
            mensajeUI.SetActive(playerNear && canCollect);

        // Recoger con E
        if (playerNear && canCollect && Keyboard.current.eKey.wasPressedThisFrame)
            Recoger();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Gato"))
            playerNear = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Gato"))
            playerNear = false;
    }

    void Recoger()
    {
        if (!canCollect || inventario == null) return;

        if (inventario.EstaLleno())
        {
            Debug.Log("⚠️ Inventario lleno!");
            return;
        }

        canCollect = false;
        inventario.AgregarItem(itemData);

        if (mensajeUI != null)
            mensajeUI.SetActive(false);

        Destroy(gameObject);
        Debug.Log($"🎯 {itemData.nombre} recogido!");
    }

    void CrearMensajeUI()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas_Mensajes");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        mensajeUI = new GameObject("Mensaje_Recoger");
        mensajeUI.transform.SetParent(canvas.transform);

        RectTransform rect = mensajeUI.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.8f);
        rect.anchorMax = new Vector2(0.5f, 0.8f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(300, 50);

        // Fondo
        Image bg = mensajeUI.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.7f);

        // Texto (hijo)
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(mensajeUI.transform);
        RectTransform rectText = textGO.AddComponent<RectTransform>();
        rectText.anchorMin = Vector2.zero;
        rectText.anchorMax = Vector2.one;
        rectText.sizeDelta = Vector2.zero;

        Text text = textGO.AddComponent<Text>();
        text.text = mensajeRecoleccion;
        text.font = Font.CreateDynamicFontFromOSFont("Arial", 14);
        text.fontSize = 20;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;

        mensajeUI.SetActive(false);
    }
}