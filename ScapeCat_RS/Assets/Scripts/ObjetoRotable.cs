using System.Collections;
using UnityEngine;

public class ObjetoRotable : MonoBehaviour
{
    public enum EjeRotacion { X, Y, Z }

    [Header("Configuración de Rotación")]
    [SerializeField] private EjeRotacion eje = EjeRotacion.Y; // Elige el eje en el Inspector
    [SerializeField] private float angulo = 90f;             // Grados a girar por interacción
    [SerializeField] private float velocidadRotacion = 5f;  // Qué tan rápido gira

    private bool estaGirando = false;

    // Método público que llamará el gato cuando presione 'E'
    public void Interactuar()
    {
        if (estaGirando) return; // Evita interrumpir si ya está girando

        // Calculamos el eje de rotación según la opción elegida
        Vector3 ejeVector = Vector3.up; // Por defecto Y
        if (eje == EjeRotacion.X) ejeVector = Vector3.right;
        else if (eje == EjeRotacion.Z) ejeVector = Vector3.forward;

        // Calculamos la rotación objetivo agregando 90 grados a la rotación actual
        Quaternion rotacionObjetivo = transform.rotation * Quaternion.Euler(ejeVector * angulo);

        StartCoroutine(GirarObjeto(rotacionObjetivo));
    }

    private IEnumerator GirarObjeto(Quaternion destino)
    {
        estaGirando = true;
        Quaternion origen = transform.rotation;
        float t = 0f;

        // Transición suave entre la rotación actual y el objetivo
        while (t < 1f)
        {
            t += Time.deltaTime * velocidadRotacion;
            transform.rotation = Quaternion.Slerp(origen, destino, t);
            yield return null;
        }

        transform.rotation = destino; // Aseguramos precisión al finalizar
        estaGirando = false;
    }
}