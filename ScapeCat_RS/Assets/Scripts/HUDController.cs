using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // <-- Necesario para la tecla E

public class HUDController : MonoBehaviour
{
    public Image[] vidasIconos;
    public Slider barraAlimentacion;
    public Slider barraBateria;
    public Image puntoCentral;
    public Color colorNormal = Color.white;
    public Color colorInteractuable = Color.green;
    public Color colorRecogible = Color.yellow;

    [Header("Configuración de Interacción")]
    public float distanciaRaycast = 7f; // Un poco más amplio para facilitar alcanzar objetos

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        ActualizarVidas(7);
        ActualizarAlimentacion(1f);
        ActualizarBateria(1f);
    }

    void Update()
    {
        if (gameManager != null && gameManager.EstaPausado()) return;
        RaycastParaPunto();
    }

    void RaycastParaPunto()
    {
        if (Camera.main == null || puntoCentral == null) return;

        Ray rayo = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(rayo, out RaycastHit hit, distanciaRaycast))
        {
            // 1. Buscamos si el objeto o alguno de sus padres tiene el script ObjetoRotable
            ObjetoRotable rotable = hit.collider.GetComponentInParent<ObjetoRotable>();

            // 2. Si lo encuentra, o si el collider directo tiene la etiqueta Interactuable
            if (rotable != null || hit.collider.CompareTag("Interactuable"))
            {
                puntoCentral.color = colorInteractuable; // Se pone verde

                // Al presionar 'E'
                if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                {
                    if (rotable != null)
                    {
                        rotable.Interactuar();
                    }
                }
            }
            else if (hit.collider.CompareTag("Collectible"))
            {
                puntoCentral.color = colorRecogible;
            }
            else
            {
                puntoCentral.color = colorNormal;
            }
        }
        else
        {
            puntoCentral.color = colorNormal;
        }
    }

    public void Mostrar(bool mostrar)
    {
        gameObject.SetActive(mostrar);
    }

    public void ActualizarVidas(int cantidad)
    {
        cantidad = Mathf.Clamp(cantidad, 0, vidasIconos.Length);
        for (int i = 0; i < vidasIconos.Length; i++)
        {
            if (vidasIconos[i] != null)
                vidasIconos[i].enabled = (i < cantidad);
        }
    }

    public void ActualizarAlimentacion(float valor)
    {
        if (barraAlimentacion != null)
            barraAlimentacion.value = Mathf.Clamp01(valor);
    }

    public void ActualizarBateria(float valor)
    {
        if (barraBateria != null)
            barraBateria.value = Mathf.Clamp01(valor);
    }
}