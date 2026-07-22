using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class HUDBuilder : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite corazonSprite;
    public Sprite comidaSprite;
    public Sprite bateriaSprite;

    [Header("Colores")]
    public Color colorVida = new Color(1f, 0.2f, 0.2f);
    public Color colorVacio = new Color(0.2f, 0.2f, 0.2f, 0.5f);
    public Color colorComida = new Color(0.2f, 0.9f, 0.3f);
    public Color colorBateria = new Color(1f, 0.8f, 0.1f);
    public Color colorPuntoNormal = new Color(1f, 1f, 1f, 0.7f);

    [Header("Tamaños")]
    public float tamañoCorazon = 32f;
    public float tamañoIcono = 36f;
    public float tamañoBarra = 180f;
    public float tamañoPunto = 14f;

    [Header("Nombres")]
    public string nombreHUD = "HUD";

    private Font defaultFont;

    void Awake()
    {
        defaultFont = Font.CreateDynamicFontFromOSFont("Arial", 14);
    }

#if UNITY_EDITOR
    [ContextMenu("Generar HUD Elegante")]
    public void GenerarYGuardar()
    {
        if (defaultFont == null)
            defaultFont = Font.CreateDynamicFontFromOSFont("Arial", 14);

        // Limpiar HUD anterior
        GameObject oldHUD = GameObject.Find(nombreHUD);
        if (oldHUD != null) DestroyImmediate(oldHUD);

        GameObject hudGO = new GameObject(nombreHUD);
        hudGO.transform.position = Vector3.zero;

        GameObject canvasGO = CrearCanvas(hudGO);

        // ===== 1. VIDAS (esquina superior izquierda) =====
        GameObject vidasContainer = CrearVidas(canvasGO);

        // ===== 2. ALIMENTACIÓN (esquina superior derecha) =====
        GameObject comidaContainer = CrearBarraConIcono(canvasGO, "Comida",
            new Vector2(1, 1), new Vector2(-30, -50),
            comidaSprite, colorComida, "ENERGÍA", tamañoIcono, tamañoBarra);

        // ===== 3. BATERÍA (debajo de comida) =====
        GameObject bateriaContainer = CrearBarraConIcono(canvasGO, "Bateria",
            new Vector2(1, 1), new Vector2(-30, -105),
            bateriaSprite, colorBateria, "BATERÍA", tamañoIcono, tamañoBarra);

        // ===== 4. PUNTO CENTRAL =====
        GameObject puntoGO = CrearPuntoCentral(canvasGO);

        // ===== 5. AGREGAR SCRIPT HUDController =====
        HUDController hudScript = hudGO.AddComponent<HUDController>();
        
        // Asignar referencias
        hudScript.vidasIconos = vidasContainer.GetComponentsInChildren<Image>();
        hudScript.corazonSprite = corazonSprite;
        hudScript.corazonVacioSprite = corazonSprite; // mismo sprite, se coloreará en código
        hudScript.colorVida = colorVida;
        hudScript.colorVacio = colorVacio;
        
        // Alimentación
        hudScript.comidaIcono = comidaContainer.transform.Find("Icono").GetComponent<Image>();
        hudScript.barraAlimentacion = comidaContainer.GetComponentInChildren<Slider>();
        hudScript.textoAlimentacion = comidaContainer.transform.Find("Texto").GetComponent<Text>();
        
        // Batería
        hudScript.bateriaIcono = bateriaContainer.transform.Find("Icono").GetComponent<Image>();
        hudScript.barraBateria = bateriaContainer.GetComponentInChildren<Slider>();
        hudScript.textoBateria = bateriaContainer.transform.Find("Texto").GetComponent<Text>();
        
        // Punto
        hudScript.puntoCentral = puntoGO.GetComponent<Image>();
        hudScript.colorNormal = colorPuntoNormal;

        // Ocultar al inicio
        hudGO.SetActive(false);

        // Guardar prefab
        #if UNITY_EDITOR
        if (!System.IO.Directory.Exists("Assets/Prefabs"))
            System.IO.Directory.CreateDirectory("Assets/Prefabs");
        
        string prefabPath = "Assets/Prefabs/HUD.prefab";
        PrefabUtility.SaveAsPrefabAsset(hudGO, prefabPath);
        Debug.Log($"✅ HUD generado en {prefabPath}");
        #endif

        Selection.activeGameObject = hudGO;
    }
#endif

    // ==================== MÉTODOS DE CONSTRUCCIÓN ====================

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

    GameObject CrearVidas(GameObject canvas)
    {
        GameObject container = new GameObject("VidasContainer");
        container.transform.SetParent(canvas.transform);

        RectTransform rect = container.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(40, -50);
        rect.sizeDelta = new Vector2(tamañoCorazon * 7 + 8 * 6, tamañoCorazon);

        HorizontalLayoutGroup layout = container.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 8;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.padding = new RectOffset(0, 0, 0, 0);

        for (int i = 0; i < 7; i++)
        {
            GameObject corazon = new GameObject($"Vida_{i}");
            corazon.transform.SetParent(container.transform);
            
            RectTransform rectCor = corazon.AddComponent<RectTransform>();
            rectCor.sizeDelta = new Vector2(tamañoCorazon, tamañoCorazon);
            
            Image img = corazon.AddComponent<Image>();
            img.sprite = corazonSprite;
            img.color = colorVida;
            img.raycastTarget = false;

            // Sombra
            GameObject shadow = new GameObject("Shadow");
            shadow.transform.SetParent(corazon.transform);
            RectTransform rectShadow = shadow.AddComponent<RectTransform>();
            rectShadow.anchorMin = Vector2.zero;
            rectShadow.anchorMax = Vector2.one;
            rectShadow.offsetMin = new Vector2(2, -2);
            rectShadow.offsetMax = new Vector2(2, -2);
            Image imgShadow = shadow.AddComponent<Image>();
            imgShadow.sprite = corazonSprite;
            imgShadow.color = new Color(0, 0, 0, 0.3f);
            imgShadow.raycastTarget = false;
        }

        return container;
    }

    GameObject CrearBarraConIcono(GameObject canvas, string nombre, Vector2 anchor, Vector2 pos, 
        Sprite icono, Color color, string textoLabel, float iconSize, float barWidth)
    {
        GameObject contenedor = new GameObject(nombre);
        contenedor.transform.SetParent(canvas.transform);

        RectTransform rect = contenedor.AddComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.anchoredPosition = pos;
        rect.sizeDelta = new Vector2(barWidth + iconSize + 20, iconSize + 10);

        // Icono
        GameObject iconoGO = new GameObject("Icono");
        iconoGO.transform.SetParent(contenedor.transform);
        RectTransform rectIcono = iconoGO.AddComponent<RectTransform>();
        rectIcono.anchorMin = new Vector2(0, 0.5f);
        rectIcono.anchorMax = new Vector2(0, 0.5f);
        rectIcono.anchoredPosition = new Vector2(0, 0);
        rectIcono.sizeDelta = new Vector2(iconSize, iconSize);
        Image imgIcono = iconoGO.AddComponent<Image>();
        imgIcono.sprite = icono;
        imgIcono.color = Color.white;
        imgIcono.raycastTarget = false;

        // Contenedor barra
        GameObject barraContainer = new GameObject("BarraContainer");
        barraContainer.transform.SetParent(contenedor.transform);
        RectTransform rectBarra = barraContainer.AddComponent<RectTransform>();
        rectBarra.anchorMin = new Vector2(0.2f, 0.5f);
        rectBarra.anchorMax = new Vector2(0.9f, 0.5f);
        rectBarra.anchoredPosition = new Vector2(0, 0);
        rectBarra.sizeDelta = new Vector2(0, 18);

        // Fondo barra
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(barraContainer.transform);
        RectTransform rectBg = bg.AddComponent<RectTransform>();
        rectBg.anchorMin = Vector2.zero;
        rectBg.anchorMax = Vector2.one;
        rectBg.sizeDelta = Vector2.zero;
        Image imgBg = bg.AddComponent<Image>();
        imgBg.color = new Color(0.05f, 0.05f, 0.05f, 0.8f);
        imgBg.raycastTarget = false;

        // Relleno
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(barraContainer.transform);
        RectTransform rectFill = fill.AddComponent<RectTransform>();
        rectFill.anchorMin = new Vector2(0, 0.1f);
        rectFill.anchorMax = new Vector2(0, 0.9f);
        rectFill.anchoredPosition = new Vector2(3, 0);
        rectFill.sizeDelta = new Vector2(0, 0);
        Image imgFill = fill.AddComponent<Image>();
        imgFill.color = color;
        imgFill.raycastTarget = false;

        // Slider
        Slider slider = barraContainer.AddComponent<Slider>();
        slider.minValue = 0;
        slider.maxValue = 1;
        slider.value = 1;
        slider.fillRect = rectFill;
        slider.targetGraphic = imgFill;
        slider.interactable = false;

        // Texto porcentaje
        GameObject textoGO = new GameObject("Texto");
        textoGO.transform.SetParent(contenedor.transform);
        RectTransform rectTexto = textoGO.AddComponent<RectTransform>();
        rectTexto.anchorMin = new Vector2(1, 0.5f);
        rectTexto.anchorMax = new Vector2(1, 0.5f);
        rectTexto.anchoredPosition = new Vector2(-5, 0);
        rectTexto.sizeDelta = new Vector2(55, 20);
        Text texto = textoGO.AddComponent<Text>();
        texto.text = "100%";
        texto.font = defaultFont;
        texto.fontSize = 14;
        texto.fontStyle = FontStyle.Bold;
        texto.alignment = TextAnchor.MiddleLeft;
        texto.color = Color.white;
        texto.raycastTarget = false;

        return contenedor;
    }

    GameObject CrearPuntoCentral(GameObject canvas)
    {
        GameObject punto = new GameObject("PuntoCentral");
        punto.transform.SetParent(canvas.transform);

        RectTransform rect = punto.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(tamañoPunto, tamañoPunto);

        Image img = punto.AddComponent<Image>();
        img.color = new Color(1f, 1f, 1f, 0.6f);
        img.raycastTarget = false;

        // Sombra suave alrededor
        GameObject glow = new GameObject("Glow");
        glow.transform.SetParent(punto.transform);
        RectTransform rectGlow = glow.AddComponent<RectTransform>();
        rectGlow.anchorMin = Vector2.zero;
        rectGlow.anchorMax = Vector2.one;
        rectGlow.offsetMin = new Vector2(-12, -12);
        rectGlow.offsetMax = new Vector2(12, 12);
        Image imgGlow = glow.AddComponent<Image>();
        imgGlow.color = new Color(1f, 1f, 1f, 0.08f);
        imgGlow.raycastTarget = false;

        // Sombra exterior (círculo decorativo)
        GameObject outer = new GameObject("OuterRing");
        outer.transform.SetParent(punto.transform);
        RectTransform rectOuter = outer.AddComponent<RectTransform>();
        rectOuter.anchorMin = Vector2.zero;
        rectOuter.anchorMax = Vector2.one;
        rectOuter.offsetMin = new Vector2(-6, -6);
        rectOuter.offsetMax = new Vector2(6, 6);
        Image imgOuter = outer.AddComponent<Image>();
        imgOuter.color = new Color(1f, 1f, 1f, 0.12f);
        imgOuter.raycastTarget = false;

        return punto;
    }
}