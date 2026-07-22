using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public string escenaJuego = "Juego";
    public string escenaMenu = "MenuPrincipal";
    public GameObject hudPrefab;  // <-- Arrastra el prefab aquí

    private bool pausado;
    private bool enJuego;
    private HUDController hud;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        PausarJuego();
    }

    public void IniciarJuego()
    {
        enJuego = true;
        ReanudarJuego();
        SceneManager.LoadScene(escenaJuego);
        StartCoroutine(InstanciarYMostrarHUD());
    }

    public void CargarPartida()
    {
        enJuego = true;
        ReanudarJuego();
        SceneManager.LoadScene(escenaJuego);
        StartCoroutine(InstanciarYMostrarHUD());
    }

    IEnumerator InstanciarYMostrarHUD()
    {
        yield return null; // esperar que cargue la escena

        // Si ya existe un HUD, usarlo
        hud = FindAnyObjectByType<HUDController>();
        if (hud != null)
        {
            hud.Mostrar(true);
            yield break;
        }

        // Si no existe y hay prefab, instanciar
        if (hudPrefab != null)
        {
            GameObject hudGO = Instantiate(hudPrefab);
            DontDestroyOnLoad(hudGO);
            hud = hudGO.GetComponent<HUDController>();
            hud.Mostrar(true);
            Debug.Log("✅ HUD instanciado desde prefab");
        }
        else
        {
            Debug.LogError("❌ No hay hudPrefab asignado en GameManager");
        }
    }

    public void VolverAlMenu()
    {
        enJuego = false;
        ReanudarJuego();
        SceneManager.LoadScene(escenaMenu);
        if (hud != null)
        {
            hud.Mostrar(false);
            // Opcional: destruir el HUD al volver al menú
            // Destroy(hud.gameObject);
        }
    }

    public void PausarJuego()
    {
        pausado = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ReanudarJuego()
    {
        pausado = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void TogglePausa()
    {
        if (!enJuego) return;
        if (pausado) ReanudarJuego();
        else PausarJuego();
    }

    public bool EstaPausado() => pausado;

    public void SalirJuego()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}