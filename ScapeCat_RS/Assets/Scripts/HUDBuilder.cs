using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class HUDBuilder : MonoBehaviour
{
    [Header("Colores y Estilos")]
    public Color hudFondoColor = new Color(0, 0, 0, 0.4f);
    public Color barraColor = new Color(0.8f, 0.2f, 0.1f);
    public Color barraAlimentacionColor = new Color(0.2f, 0.7f, 0.2f);
    public Color barraBateriaColor = new Color(0.9f, 0.7f, 0.1f);
    public Color puntoColor = Color.white;
    public Color puntoInteractuable = Color.green;
    public Color puntoRecogible = Color.yellow;
    public Color puntoEnemigo = Color.red;

    [Header("Fuentes")]
    public Font fuenteHUD;
    public int textoSize = 16;

    [Header("Nombre del HUD")]
    public string nombreHUD = "HUD";

    private List<GameObject> generatedObjects = new List<GameObject>();
    private Font defaultFont;

    void Awake()
    {
        defaultFont = Font.CreateDynamicFontFromOSFont("Arial", 14);
        if (fuenteHUD == null) fuenteHUD = defaultFont;
    }

#if UNITY_EDITOR
    [ContextMenu("Generar HUD (Guardar)")]
    public void GenerarYGuardar()
    {
        if (defaultFont == null)
            defaultFont = Font.CreateDynamicFontFromOSFont("Arial", 14);

        LimpiarHUD();

        GameObject hudGO = new GameObject(nombreHUD);
        hudGO.transform.position = Vector3.zero;

        // DontDestroyOnLoad solo en Play Mode
        #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            Debug.LogWarning("⚠️ DontDestroyOnLoad solo funciona en Play Mode. Se aplicará al entrar al juego.");
        }
        else
        #endif
        {
            DontDestroyOnLoad(hudGO);
        }

        CrearHUDCompleto(hudGO);

        HUDController hudScript = hudGO.AddComponent<HUDController>();
        hudScript.AsignarReferencias(
            FindChild(hudGO, "VidasContainer"),
            FindChild(hudGO, "BarraAlimentacion"),
            FindChild(hudGO, "BarraBateria"),
            FindChild(hudGO, "PuntoCentral"),
            puntoInteractuable,
            puntoRecogible,
            puntoEnemigo
        );

        hudGO.SetActive(false);

        #if UNITY_EDITOR
        if (EditorSceneManager.GetActiveScene().isDirty)
        {
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            Debug.Log("💾 HUD guardado permanentemente");
        }
        #endif

        Debug.Log($"✅ HUD generado en '{nombreHUD}'");

        #if UNITY_EDITOR
        Selection.activeGameObject = hudGO;
        #endif
    }

    [ContextMenu("Limpiar HUD")]
    public void LimpiarHUD()
    {
        GameObject hudGO = GameObject.Find(nombreHUD);
        if (hudGO != null)
        {
#if UNITY_EDITOR
            DestroyImmediate(hudGO);
#else
            Destroy(hudGO);
#endif
        }
        generatedObjects.Clear();
    }
#endif

    void CrearHUDCompleto(GameObject parent)
    {
        if (defaultFont == null)
            defaultFont = Font.CreateDynamicFontFromOSFont("Arial", 14);

        GameObject canvas = CrearCanvas(parent);

        // ==================== PUNTO CENTRAL ====================
        GameObject punto = new GameObject("PuntoCentral");
        punto.transform.SetParent(canvas.transform);
        RectTransform rectPunto = punto.AddComponent<RectTransform>();
        rectPunto.anchorMin = new Vector2(0.5f, 0.5f);
        rectPunto.anchorMax = new Vector2(0.5f, 0.5f);
        rectPunto.anchoredPosition = Vector2.zero;
        rectPunto.sizeDelta = new Vector2(12, 12);
        Image imgPunto = punto.AddComponent<Image>();
        imgPunto.color = puntoColor;
        imgPunto.raycastTarget = false;

        // ==================== VIDAS (7 corazones) ====================
        GameObject vidasContainer = new GameObject("VidasContainer");
        vidasContainer.transform.SetParent(canvas.transform);
        RectTransform rectVidas = vidasContainer.AddComponent<RectTransform>();
        rectVidas.anchorMin = new Vector2(0, 1);
        rectVidas.anchorMax = new Vector2(0, 1);
        rectVidas.anchoredPosition = new Vector2(80, -40);
        rectVidas.sizeDelta = new Vector2(400, 40);

        for (int i = 0; i < 7; i++)
        {
            GameObject corazon = new GameObject($"Vida_{i}");
            corazon.transform.SetParent(vidasContainer.transform);
            RectTransform rectCor = corazon.AddComponent<RectTransform>();
            rectCor.anchorMin = new Vector2(0, 0.5f);
            rectCor.anchorMax = new Vector2(0, 0.5f);
            rectCor.anchoredPosition = new Vector2(i * 32, 0);
            rectCor.sizeDelta = new Vector2(28, 28);
            Image imgCor = corazon.AddComponent<Image>();
            imgCor.color = Color.red;
            imgCor.raycastTarget = false;
            // Puedes asignar sprite de corazón si tienes
            generatedObjects.Add(corazon);
        }

        // ==================== BARRA DE ALIMENTACIÓN ====================
        GameObject barraAlimentacion = CrearBarra(canvas, "BarraAlimentacion", 
            new Vector2(0, -40), barraAlimentacionColor, "🍖", 300f);

        // ==================== BARRA DE BATERÍA ====================
        GameObject barraBateria = CrearBarra(canvas, "BarraBateria", 
            new Vector2(0, -90), barraBateriaColor, "🔋", 300f);

        // ==================== TEXTO "ENERGÍA" y "BATERÍA" ====================
        GameObject textoEnergia = CrearTexto(canvas, "TextoEnergia", "ENERGÍA", new Vector2(-180, -40), 14);
        GameObject textoBateria = CrearTexto(canvas, "TextoBateria", "BATERÍA", new Vector2(-180, -90), 14);

        // Guardar referencias
        generatedObjects.Add(punto);
        generatedObjects.Add(vidasContainer);
        generatedObjects.Add(barraAlimentacion);
        generatedObjects.Add(barraBateria);
        generatedObjects.Add(textoEnergia);
        generatedObjects.Add(textoBateria);
    }

    GameObject CrearCanvas(GameObject parent)
    {
        GameObject canvas = new GameObject("Canvas_HUD");
        canvas.transform.SetParent(parent.transform);
        canvas.transform.position = Vector3.zero;

        Canvas canvasComp = canvas.AddComponent<Canvas>();
        canvasComp.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasComp.sortingOrder = 200;

        CanvasScaler scaler = canvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        canvas.AddComponent<GraphicRaycaster>();

        return canvas;
    }

    GameObject CrearBarra(GameObject canvas, string nombre, Vector2 pos, Color color, string icono, float ancho)
    {
        GameObject contenedor = new GameObject(nombre);
        contenedor.transform.SetParent(canvas.transform);

        RectTransform rect = contenedor.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0);
        rect.anchorMax = new Vector2(0.5f, 0);
        rect.anchoredPosition = pos;
        rect.sizeDelta = new Vector2(ancho, 28);

        // Fondo
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(contenedor.transform);
        RectTransform rectBg = bg.AddComponent<RectTransform>();
        rectBg.anchorMin = Vector2.zero;
        rectBg.anchorMax = Vector2.one;
        rectBg.sizeDelta = Vector2.zero;
        Image imgBg = bg.AddComponent<Image>();
        imgBg.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        imgBg.raycastTarget = false;

        // Relleno
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(contenedor.transform);
        RectTransform rectFill = fill.AddComponent<RectTransform>();
        rectFill.anchorMin = new Vector2(0, 0.1f);
        rectFill.anchorMax = new Vector2(0, 0.9f);
        rectFill.anchoredPosition = new Vector2(4, 0);
        rectFill.sizeDelta = new Vector2(0, 0);
        Image imgFill = fill.AddComponent<Image>();
        imgFill.color = color;
        imgFill.raycastTarget = false;

        // Slider
        Slider slider = contenedor.AddComponent<Slider>();
        slider.minValue = 0;
        slider.maxValue = 1;
        slider.value = 1;
        slider.fillRect = rectFill;
        slider.targetGraphic = imgFill;
        slider.interactable = false;

        return contenedor;
    }

    GameObject CrearTexto(GameObject canvas, string nombre, string texto, Vector2 pos, int size)
    {
        GameObject txtGO = new GameObject(nombre);
        txtGO.transform.SetParent(canvas.transform);

        RectTransform rect = txtGO.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0);
        rect.anchorMax = new Vector2(0.5f, 0);
        rect.anchoredPosition = pos;
        rect.sizeDelta = new Vector2(100, 30);

        Text text = txtGO.AddComponent<Text>();
        text.text = texto;
        text.font = fuenteHUD != null ? fuenteHUD : defaultFont;
        text.fontSize = size;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleRight;
        text.color = Color.white;
        text.raycastTarget = false;

        return txtGO;
    }

    GameObject FindChild(GameObject parent, string nombre)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.name == nombre)
                return child.gameObject;
            foreach (Transform sub in child)
            {
                if (sub.name == nombre)
                    return sub.gameObject;
            }
        }
        return null;
    }
}