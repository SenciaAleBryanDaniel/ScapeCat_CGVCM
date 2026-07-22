using UnityEngine;
using UnityEngine.InputSystem;

public class InventarioDebug : MonoBehaviour
{
    [Header("items de prueba")]
    public ItemData llave;
    public ItemData pocion;
    public ItemData nota;

    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) Recoger(llave);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) Recoger(pocion);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) Recoger(nota);
        if (Keyboard.current.digit0Key.wasPressedThisFrame) Limpiar();
    }

    void Recoger(ItemData item)
    {
        if (item == null) return;
        var inv = FindObjectOfType<InventarioController>();
        inv?.RecogerItem(item);
    }

    void Limpiar()
    {
        var inv = FindObjectOfType<InventarioController>();
        if (inv == null) return;
        
        var items = inv.GetItems();
        for (int i = 0; i < items.Count; i++)
            inv.RemoverItem(i);
    }
}