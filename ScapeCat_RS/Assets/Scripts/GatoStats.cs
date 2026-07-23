using UnityEngine;
using UnityEngine.InputSystem;

public class GatoStats : MonoBehaviour
{
    [Header("Vida")]
    public float vidaMax = 7f;
    public float vidaActual = 7f;

    [Header("Hambre")]
    public float hambreMax = 100f;
    public float hambreActual = 100f;
    public float hambrePorSegundo = 0.5f;

    [Header("Batería")]
    public float bateriaMax = 100f;
    public float bateriaActual = 100f;
    public float consumoBateriaPorSegundo = 2f;

    [Header("Linterna")]
    public Light linternaLight;
    public bool linternaEncendida = false;

    [Header("Referencias")]
    public HUDController hud;

    private GameManager gameManager;
    private Flashlight flashlightScript;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        hud = FindAnyObjectByType<HUDController>();

        // Buscar linterna en el hijo si no está asignada
        if (linternaLight == null)
            linternaLight = GetComponentInChildren<Light>();

        if (linternaLight != null)
        {
            linternaLight.enabled = false;
            flashlightScript = linternaLight.GetComponent<Flashlight>();
            if (flashlightScript != null)
                flashlightScript.enabled = false; // Desactivar el script viejo
        }

        vidaActual = vidaMax;
        hambreActual = hambreMax;
        bateriaActual = bateriaMax;

        ActualizarHUD();
    }

    void Update()
    {
        if (gameManager != null && gameManager.EstaPausado()) return;

        // Hambre baja con el tiempo
        hambreActual = Mathf.Max(hambreActual - hambrePorSegundo * Time.deltaTime, 0f);

        // Si hambre llega a 0, pierde vida lentamente
        if (hambreActual <= 0f)
        {
            hambreActual = 0f;
            vidaActual = Mathf.Max(vidaActual - 0.5f * Time.deltaTime, 0f);
        }

        // Consumo de batería si linterna encendida
        if (linternaEncendida)
        {
            bateriaActual = Mathf.Max(bateriaActual - consumoBateriaPorSegundo * Time.deltaTime, 0f);

            if (bateriaActual <= 0f)
            {
                bateriaActual = 0f;
                ToggleLinterna(false);
            }
        }

        // Tecla F para linterna (usando Input System)
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            ToggleLinterna(!linternaEncendida);
        }

        ActualizarHUD();

        // Si vida llega a 0, reiniciar posición o game over
        if (vidaActual <= 0f)
        {
            vidaActual = 0f;
            Debug.Log("💀 Gato murió");
            // Puedes llamar a ResetPosition o VolverAlMenu
            // GetComponent<CatMovement>()?.ResetPosition(Vector3.zero);
        }
    }

    void ToggleLinterna(bool encender)
    {
        if (bateriaActual <= 0f && encender) return;

        linternaEncendida = encender;
        if (linternaLight != null)
            linternaLight.enabled = encender;
    }

    void ActualizarHUD()
    {
        if (hud == null) return;

        hud.ActualizarVidas(Mathf.RoundToInt(vidaActual));
        hud.ActualizarAlimentacion(hambreActual / hambreMax);
        hud.ActualizarBateria(bateriaActual / bateriaMax);
    }

    // ===== MÉTODOS PÚBLICOS =====

    public void Comer(float cantidad)
    {
        hambreActual = Mathf.Min(hambreActual + cantidad, hambreMax);
        ActualizarHUD();
    }

    public void RecibirDaño(float cantidad)
    {
        vidaActual = Mathf.Max(vidaActual - cantidad, 0f);
        ActualizarHUD();
    }

    public void Curar(float cantidad)
    {
        vidaActual = Mathf.Min(vidaActual + cantidad, vidaMax);
        ActualizarHUD();
    }

    public void RecargarBateria(float cantidad)
    {
        bateriaActual = Mathf.Min(bateriaActual + cantidad, bateriaMax);
        ActualizarHUD();
    }
}