using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    static UIManager instance;
    static GameObject mainMenu;
    static GameObject pauseMenu;
    static GameObject hud;

    Slider hpSlider;

    // public Gradient gradient;
    // public Image fill;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        if (this != instance)
            Destroy(gameObject);

        mainMenu = GameObject.FindGameObjectWithTag("MainMenu");
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
        hud = GameObject.FindGameObjectWithTag("HUD");

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(mainMenu);
        DontDestroyOnLoad(pauseMenu);
        DontDestroyOnLoad(hud);

        hpSlider = hud.transform.Find("HealthBar").GetComponent<Slider>();

        ActivateMenu("mainMenu");
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("CraigScene");

        ActivateMenu("HUD");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public static void Pause()
    {
        Time.timeScale = 0;
        ActivateMenu("pauseMenu");
    }

    public static void UnPause()
    {
        Time.timeScale = 1;
        ActivateMenu();
    }

    public void SetMaxHealth(int hp)
    {
        hpSlider.maxValue = hp;
        hpSlider.value = hp;
        // fill.color = gradient.Evaluate(1f);
    }

    public void SetHealth(int hp)
    {
        Debug.Log("hit");
        hpSlider.value = hp;
        // fill.color = gradient.Evaluate(hpSlider.normalizedValue);
    }

    public static void ActivateMenu(string canvasName = "")
    {
        mainMenu.SetActive(false);
        pauseMenu.SetActive(false);
        hud.SetActive(false);

        switch (canvasName)
        {
            case "mainMenu":
                mainMenu.SetActive(true);
                break;
            case "pauseMenu":
                pauseMenu.SetActive(true);
                break;
            case "hud":
                hud.SetActive(true);
                break;
            default:
                break;
        }
    }
}
