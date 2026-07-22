using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Vidas")]
    public Image[] vidasIconos;
    public Sprite corazonSprite;
    public Color colorVida = new Color(1f, 0.2f, 0.2f);
    public Color colorVacio = new Color(0.2f, 0.2f, 0.2f, 0.5f);

    [Header("Alimentación")]
    public Image comidaIcono;
    public Slider barraAlimentacion;
    public Text textoAlimentacion;

    [Header("Batería")]
    public Image bateriaIcono;
    public Slider barraBateria;
    public Text textoBateria;

    [Header("Punto Central")]
    public Image puntoCentral;
    public Color colorNormal = new Color(1f, 1f, 1f, 0.7f);
    public Color colorInteractuable = new Color(0.3f, 0.9f, 0.3f);
    public Color colorRecogible = new Color(1f, 0.8f, 0.2f);

    [Header("Configuración")]
    public float raycastDistance = 5f;
    public LayerMask capasInteractuables = -1;

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
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
        if (Physics.Raycast(rayo, out RaycastHit hit, raycastDistance, capasInteractuables))
        {
            if (hit.collider.CompareTag("Collectible"))
                puntoCentral.color = colorRecogible;
            else if (hit.collider.CompareTag("Interactuable"))
                puntoCentral.color = colorInteractuable;
            else
                puntoCentral.color = colorNormal;
        }
        else
        {
            puntoCentral.color = colorNormal;
        }
    }

    public void ActualizarVidas(int cantidad)
    {
        cantidad = Mathf.Clamp(cantidad, 0, vidasIconos.Length);
        for (int i = 0; i < vidasIconos.Length; i++)
        {
            if (vidasIconos[i] != null)
            {
                vidasIconos[i].color = (i < cantidad) ? colorVida : colorVacio;
                vidasIconos[i].enabled = true;
            }
        }
    }

    public void ActualizarAlimentacion(float valor)
    {
        valor = Mathf.Clamp01(valor);
        if (barraAlimentacion != null) barraAlimentacion.value = valor;
        if (textoAlimentacion != null) textoAlimentacion.text = Mathf.RoundToInt(valor * 100) + "%";
    }

    public void ActualizarBateria(float valor)
    {
        valor = Mathf.Clamp01(valor);
        if (barraBateria != null) barraBateria.value = valor;
        if (textoBateria != null) textoBateria.text = Mathf.RoundToInt(valor * 100) + "%";
    }

    public void Mostrar(bool mostrar)
    {
        gameObject.SetActive(mostrar);
    }
}