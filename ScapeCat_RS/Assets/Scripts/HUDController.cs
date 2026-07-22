using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public Image[] vidasIconos;
    public Slider barraAlimentacion;
    public Slider barraBateria;
    public Image puntoCentral;
    public Color colorNormal = Color.white;
    public Color colorInteractuable = Color.green;
    public Color colorRecogible = Color.yellow;

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
        if (Physics.Raycast(rayo, out RaycastHit hit, 5f))
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

    public void Mostrar(bool mostrar)
    {
        gameObject.SetActive(mostrar);
    }

    public void ActualizarVidas(int cantidad)
    {
        cantidad = Mathf.Clamp(cantidad, 0, vidasIconos.Length);
        for (int i = 0; i < vidasIconos.Length; i++)
        {
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