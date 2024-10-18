using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    static GameObject mainMenu;
    static GameObject pauseMenu;
    static GameObject hud;

    private void Awake()
    {
        mainMenu = GameObject.FindGameObjectWithTag("MainMenu");
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
        hud = GameObject.FindGameObjectWithTag("HUD");

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(mainMenu);
        DontDestroyOnLoad(pauseMenu);
        DontDestroyOnLoad(hud);

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

    public Slider slider;
    public Gradient gradient;
    public Image fill;

    public void SetMaxHealth(int hp)
    {
        slider.maxValue = hp;
        slider.value = hp;
        fill.color = gradient.Evaluate(1f);
    }
    public void SetHealth(int hp)
    {
        slider.value = hp;
        fill.color = gradient.Evaluate(slider.normalizedValue);
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
