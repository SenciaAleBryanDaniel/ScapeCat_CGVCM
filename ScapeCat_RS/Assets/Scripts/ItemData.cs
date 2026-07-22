using UnityEngine;

[CreateAssetMenu(fileName = "NuevoItem", menuName = "Inventario/Item")]
public class ItemData : ScriptableObject
{
    public string nombre = "Item";
    public Sprite icono;
    public int cantidad = 1;
    public string descripcion = "";
    public bool esConsumible = true;
    public bool esClave = false;

    [Header("Tipo de Item (para consumibles)")]
    public TipoItem tipoItem = TipoItem.Normal;
    public float valorEfecto = 10f; // Cantidad de vida, hambre o batería que restaura
}

public enum TipoItem
{
    Normal,     // Va al inventario sin efecto inmediato
    Comida,     // Restaura hambre
    Curacion,   // Restaura vida
    Bateria     // Recarga batería
}