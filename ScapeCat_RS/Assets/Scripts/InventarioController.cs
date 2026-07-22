using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class InventarioController : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject inventarioPanel;
    public List<GameObject> slots = new List<GameObject>();
    public GameObject emptyText;
    public GameObject usarBoton; // opcional

    private List<ItemData> items = new List<ItemData>();
    private GameManager gameManager;
    private bool abierto;

    public System.Action OnInventarioActualizado;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        inventarioPanel?.SetActive(false);

        for (int i = 0; i < slots.Count; i++)
            items.Add(null);

        for (int i = 0; i < slots.Count; i++)
        {
            int idx = i;
            slots[i]?.GetComponent<Button>()?.onClick.AddListener(() => OnSlotClick(idx));
        }
    }

    void Update()
    {
        if (Keyboard.current.iKey.wasPressedThisFrame)
            ToggleInventario();

        if (Keyboard.current.escapeKey.wasPressedThisFrame && abierto)
            CerrarInventario();
    }

    void OnSlotClick(int index)
    {
        if (index < items.Count && items[index] != null)
        {
            Debug.Log($"Usando {items[index].nombre}");
            // Aquí puedes llamar a una función para usar el item
            // Por ahora solo muestra el nombre
        }
    }

    public bool AgregarItem(ItemData item)
    {
        int vacio = items.FindIndex(i => i == null);
        if (vacio == -1) return false;

        items[vacio] = item;
        ActualizarUI();
        OnInventarioActualizado?.Invoke();
        Debug.Log($"📦 {item.nombre} guardado en slot {vacio + 1}");
        return true;
    }

    public void RemoverItem(int index)
    {
        if (index >= 0 && index < items.Count && items[index] != null)
        {
            items[index] = null;
            ActualizarUI();
            OnInventarioActualizado?.Invoke();
        }
    }

    public bool TieneItem(string nombre)
    {
        return items.Exists(i => i != null && i.nombre == nombre);
    }

    public ItemData ObtenerItem(string nombre)
    {
        return items.Find(i => i != null && i.nombre == nombre);
    }

    public bool EstaLleno() => items.TrueForAll(i => i != null);
    public bool EstaAbierto() => abierto;
    public List<ItemData> GetItems() => items;

    void ToggleInventario()
    {
        if (abierto) CerrarInventario();
        else AbrirInventario();
    }

    void AbrirInventario()
    {
        abierto = true;
        inventarioPanel?.SetActive(true);
        gameManager?.PausarJuego();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ActualizarUI();
    }

    void CerrarInventario()
    {
        abierto = false;
        inventarioPanel?.SetActive(false);
        gameManager?.ReanudarJuego();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void ActualizarUI()
    {
        bool tieneItems = false;

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] == null) continue;

            var icono = slots[i].transform.Find("Icono")?.gameObject;
            var cantidad = slots[i].transform.Find("Cantidad")?.gameObject;

            if (i < items.Count && items[i] != null)
            {
                var item = items[i];
                tieneItems = true;

                if (icono != null)
                {
                    icono.SetActive(true);
                    var img = icono.GetComponent<Image>();
                    if (img != null && item.icono != null)
                    {
                        img.sprite = item.icono;
                        img.color = Color.white;
                    }
                }

                if (cantidad != null)
                {
                    var txt = cantidad.GetComponent<Text>();
                    if (item.cantidad > 1 && txt != null)
                    {
                        cantidad.SetActive(true);
                        txt.text = item.cantidad.ToString();
                    }
                    else cantidad.SetActive(false);
                }
            }
            else
            {
                if (icono != null) icono.SetActive(false);
                if (cantidad != null) cantidad.SetActive(false);
            }
        }

        if (emptyText != null)
        {
            emptyText.SetActive(!tieneItems);
            var txt = emptyText.GetComponent<Text>();
            if (txt != null) txt.text = "Inventario vacío";
        }
    }
}