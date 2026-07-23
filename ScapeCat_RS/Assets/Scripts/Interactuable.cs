using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Interactuable : MonoBehaviour
{
    [Header("Configuración")]
    public string itemRequerido;
    public string mensajeSinItem = "Necesitas algo para abrir esto";
    public string mensajeConItem = "Presiona E para abrir";
    public GameObject objetoParaActivar;

    private bool estaAbierto = false;
    private bool jugadorCerca = false;
    private InventarioController inventario;
    private GameObject mensajeUI;
    private ObjetoRotable objetoRotable;

    void Start()
    {
        inventario = FindAnyObjectByType<InventarioController>();
        objetoRotable = GetComponent<ObjetoRotable>();
        CrearMensajeUI();
        Debug.Log("✅ Interactuable iniciado en: " + gameObject.name);
    }

    void Update()
    {
        // Mostrar mensaje
        if (mensajeUI != null)
        {
            if (jugadorCerca && !estaAbierto)
            {
                mensajeUI.SetActive(true);
                Text texto = mensajeUI.GetComponentInChildren<Text>();
                texto.text = TieneItemRequerido() ? mensajeConItem : mensajeSinItem;
            }
            else
            {
                mensajeUI.SetActive(false);
            }
        }

        // Abrir con E
        if (jugadorCerca && !estaAbierto && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("🔍 Presionaste E");
            Debug.Log($"📦 Item requerido: '{itemRequerido}'");
            Debug.Log($"🎒 Tiene item? {TieneItemRequerido()}");

            if (TieneItemRequerido())
                Abrir();
            else
                Debug.Log("🔒 " + mensajeSinItem);
        }
    }

    bool TieneItemRequerido()
    {
        if (inventario == null) return false;
        return inventario.TieneItem(itemRequerido);
    }

    void Abrir()
    {
        estaAbierto = true;
        Debug.Log($"🔓 ABRIENDO: {gameObject.name}");

        // Activar objeto extra
        if (objetoParaActivar != null)
        {
            objetoParaActivar.SetActive(true);
            Debug.Log("✅ Objeto activado");
        }

        // 🔥 USAR ObjetoRotable (girar)
        if (objetoRotable != null)
        {
            objetoRotable.Interactuar();
            Debug.Log("🔄 Llamando a ObjetoRotable.Interactuar()");
        }
        else
        {
            Debug.LogWarning("⚠️ No hay ObjetoRotable en este objeto");
        }

        // Consumir item del inventario
        if (inventario != null && !string.IsNullOrEmpty(itemRequerido))
        {
            var items = inventario.GetItems();
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null && items[i].nombre == itemRequerido)
                {
                    inventario.RemoverItem(i);
                    Debug.Log($"🗑️ Item '{itemRequerido}' consumido");
                    break;
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Gato"))
        {
            jugadorCerca = true;
            Debug.Log($"✅ Jugador cerca de {gameObject.name}");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Gato"))
            jugadorCerca = false;
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

        mensajeUI = new GameObject("Mensaje_Interactuable");
        mensajeUI.transform.SetParent(canvas.transform);

        RectTransform rect = mensajeUI.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.7f);
        rect.anchorMax = new Vector2(0.5f, 0.7f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(400, 50);

        Image bg = mensajeUI.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.7f);

        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(mensajeUI.transform);
        RectTransform rectText = textGO.AddComponent<RectTransform>();
        rectText.anchorMin = Vector2.zero;
        rectText.anchorMax = Vector2.one;
        rectText.sizeDelta = Vector2.zero;

        Text text = textGO.AddComponent<Text>();
        text.text = mensajeSinItem;
        text.font = Font.CreateDynamicFontFromOSFont("Arial", 14);
        text.fontSize = 18;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;

        mensajeUI.SetActive(false);
    }
}