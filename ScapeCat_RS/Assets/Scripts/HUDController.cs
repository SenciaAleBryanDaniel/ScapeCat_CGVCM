using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class HUDController : MonoBehaviour
{
    [Header("Referencias (asignadas automáticamente)")]
    public GameObject vidasContainer;
    public GameObject barraAlimentacion;
    public GameObject barraBateria;
    public GameObject puntoCentral;

    [Header("Colores del punto")]
    public Color colorNormal = Color.white;
    public Color colorInteractuable = Color.green;
    public Color colorRecogible = Color.yellow;
    public Color colorEnemigo = Color.red;

    [Header("Configuración")]
    public float raycastDistance = 5f;
    public LayerMask capasInteractuables = -1;

    private List<Image> vidas = new List<Image>();
    private Slider sliderAlimentacion;
    private Slider sliderBateria;
    private Image puntoImg;
    private GameManager gameManager;
    private bool visible = false;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();

        // Obtener referencias de los hijos
        if (vidasContainer != null)
        {
            foreach (Transform child in vidasContainer.transform)
            {
                Image img = child.GetComponent<Image>();
                if (img != null) vidas.Add(img);
            }
        }

        if (barraAlimentacion != null)
            sliderAlimentacion = barraAlimentacion.GetComponent<Slider>();

        if (barraBateria != null)
            sliderBateria = barraBateria.GetComponent<Slider>();

        if (puntoCentral != null)
            puntoImg = puntoCentral.GetComponent<Image>();

        // Inicializar
        ActualizarVidas(7);
        ActualizarAlimentacion(1f);
        ActualizarBateria(1f);
        Mostrar(false);
    }

    void Update()
    {
        if (gameManager != null && gameManager.EstaPausado()) return;
        if (!visible) return;

        RaycastParaPunto();
    }

    void RaycastParaPunto()
    {
        if (Camera.main == null || puntoImg == null) return;

        Ray rayo = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(rayo, out RaycastHit hit, raycastDistance, capasInteractuables))
        {
            if (hit.collider.CompareTag("Collectible"))
                puntoImg.color = colorRecogible;
            else if (hit.collider.CompareTag("Interactuable"))
                puntoImg.color = colorInteractuable;
            else if (hit.collider.CompareTag("Enemigo"))
                puntoImg.color = colorEnemigo;
            else
                puntoImg.color = colorNormal;
        }
        else
        {
            puntoImg.color = colorNormal;
        }
    }

    // ===== MÉTODOS PÚBLICOS =====

    public void Mostrar(bool mostrar)
    {
        visible = mostrar;
        gameObject.SetActive(mostrar);
    }

    public void ActualizarVidas(int cantidad)
    {
        cantidad = Mathf.Clamp(cantidad, 0, vidas.Count);
        for (int i = 0; i < vidas.Count; i++)
        {
            if (vidas[i] != null)
                vidas[i].enabled = (i < cantidad);
        }
    }

    public void ActualizarAlimentacion(float valor)
    {
        if (sliderAlimentacion != null)
            sliderAlimentacion.value = Mathf.Clamp01(valor);
    }

    public void ActualizarBateria(float valor)
    {
        if (sliderBateria != null)
            sliderBateria.value = Mathf.Clamp01(valor);
    }

    public void AsignarReferencias(GameObject vidas, GameObject alimentacion, GameObject bateria, GameObject punto,
        Color colorInt, Color colorRec, Color colorEne)
    {
        vidasContainer = vidas;
        barraAlimentacion = alimentacion;
        barraBateria = bateria;
        puntoCentral = punto;
        colorInteractuable = colorInt;
        colorRecogible = colorRec;
        colorEnemigo = colorEne;
    }
}