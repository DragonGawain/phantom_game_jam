using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using TMPro;

public class UIManager : MonoBehaviour
{
    static UIManager instance;
    static GameObject mainMenu;
    static GameObject pauseMenu;
    static GameObject hud;
    static GameObject shipInventoryMenu;
    static GameObject gameOverMenu;
    static GameObject winCanvas;

    static UIAudio uIAudio;

    public static bool isInShipInventory = false;
    public static bool isInPauseMenu = false;

    Slider hpSlider;

    static GameObject questRoot1;
    static GameObject questRoot2;

    static TextMeshProUGUI timerText;

    static int winCountdown = 300 * 50;

    static string winText1 = "SHIP TAKEOFF PROCEDURE COMPLETED - LAUNCH SUCCESSFUL.";
    static string winText2 = "NEW MISSION DIRECTIVES RECEIVED:";
    static string winText3 = "OPERATION: YOU JUST LOST THE GAME";

    // public Gradient gradient;
    // public Image fill;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        if (this != instance)
            Destroy(gameObject);


        uIAudio = GetComponent<UIAudio>();

        mainMenu = GameObject.FindGameObjectWithTag("MainMenu");
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
        hud = GameObject.FindGameObjectWithTag("HUD");
        shipInventoryMenu = GameObject.FindGameObjectWithTag("ShipInvMenu");
        gameOverMenu = GameObject.FindGameObjectWithTag("GameOverMenu");
        winCanvas = GameObject.FindGameObjectWithTag("winMenu");

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(mainMenu);
        DontDestroyOnLoad(pauseMenu);
        DontDestroyOnLoad(hud);
        DontDestroyOnLoad(shipInventoryMenu);
        DontDestroyOnLoad(gameOverMenu);
        DontDestroyOnLoad(winCanvas);

        hpSlider = hud.transform.Find("HealthBar").GetComponent<Slider>();

        questRoot1 = shipInventoryMenu.transform.Find("Quest1Root").gameObject;
        questRoot2 = shipInventoryMenu.transform.Find("Quest2Root").gameObject;

        timerText = hud.transform
            .Find("TimerRoot")
            .Find("TimerText")
            .GetComponent<TextMeshProUGUI>();

        ActivateMenu("mainMenu");
    }

    private void FixedUpdate()
    {
        if (PlayerController.isEndingSequence && winCountdown >= 0)
            UpdateWinCounter(winCountdown--);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("BuildScene");

        ActivateMenu("hud");
        AudioManager.PlayIntro();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public static void Pause()
    {
        isInPauseMenu = true;
        Time.timeScale = 0;
        ActivateMenu("pauseMenu");
    }

    public static void UnPause()
    {
        Time.timeScale = 1;
        isInPauseMenu = false;
        ActivateMenu("hud");
    }

    public static void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        isInPauseMenu = false;
        ActivateMenu("mainMenu");
        SceneManager.LoadScene("AltMenu");
        AudioManager.PlayMainMenu();
    }

    public void SetMaxHealth(int hp)
    {
        hpSlider.maxValue = hp;
        hpSlider.value = hp;
        // fill.color = gradient.Evaluate(1f);
    }

    public void SetHealth(int hp)
    {
        hpSlider.value = hp;
        // fill.color = gradient.Evaluate(hpSlider.normalizedValue);
    }

    public static void OpenShipInventory(
        Dictionary<ShipComponents, int> inventory,
        int maxHp,
        int hp
    )
    {
        uIAudio.OpenSound();
        isInShipInventory = true;

        Transform inventoryCountsParent = shipInventoryMenu.transform.Find("InventoryParent");
        TextMeshProUGUI text;
        foreach (ShipComponents sc in Enum.GetValues(typeof(ShipComponents)))
        {
            text = inventoryCountsParent
                .Find(sc.GetEnumDescription())
                .GetComponent<TextMeshProUGUI>();

            text.text = sc.GetEnumDescription().Replace("_", " ");
            text.text =
                text.text[..1].ToUpper()
                + text.text[1..]
                + ": "
                + inventory[sc]
                + "/"
                + Ship.GetRequiredInvetory()[sc];

            if (sc == ShipComponents.RCS)
                text.text = text.text.ToUpper();

            Debug.Log(
                "type: "
                    + sc.GetEnumDescription()
                    + " - val: "
                    + (inventory[sc] >= Ship.GetRequiredInvetory()[sc])
            );

            // can cheat if all else fails -> 2 GO's, one always animated, the other never
            // enable/disable the desired one
            if (inventory[sc] >= Ship.GetRequiredInvetory()[sc])
            {
                Debug.Log("hit");
                shipInventoryMenu.transform
                    .Find(sc.GetEnumDescription() + "_icon")
                    .GetComponent<Animator>()
                    .SetBool("isComplete", true);

                Debug.Log(
                    "test: "
                        + shipInventoryMenu.transform
                            .Find(sc.GetEnumDescription() + "_icon")
                            .GetComponent<Animator>()
                            .GetBool("isComplete")
                );
            }
        }
        questRoot1.SetActive(false);
        questRoot2.SetActive(false);

        ActivateMenu("shipInv");

        shipInventoryMenu.GetComponentInChildren<Slider>().maxValue = maxHp;
        shipInventoryMenu.GetComponentInChildren<Slider>().value = hp;

        Time.timeScale = 0;
    }

    public static void OpenShipInventory(
        Dictionary<ShipComponents, int> inventory,
        int maxHp,
        int hp,
        ShipComponents q1Item
    )
    {
        OpenShipInventory(inventory, maxHp, hp);
        questRoot1.SetActive(true);
        questRoot1.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = q1Item
            .GetEnumDescription()
            .Replace("_", " ");
    }

    public static void OpenShipInventory(
        Dictionary<ShipComponents, int> inventory,
        int hp,
        int maxHp,
        ShipComponents q1Item,
        ShipComponents q2Item
    )
    {
        OpenShipInventory(inventory, maxHp, hp);
        questRoot1.SetActive(true);
        questRoot2.SetActive(true);
        questRoot1.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = q1Item
            .GetEnumDescription()
            .Replace("_", " ");
        questRoot2.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = q2Item
            .GetEnumDescription()
            .Replace("_", " ");
    }

    public static void CloseShipInventory()
    {
        Time.timeScale = 1;
        uIAudio.CloseSound();
        isInShipInventory = false;
        ActivateMenu("hud");

        SetOperationText("OpCollectText");
    }

    public static void ActivateMenu(string canvasName = "")
    {
        mainMenu.SetActive(false);
        pauseMenu.SetActive(false);
        hud.SetActive(false);
        shipInventoryMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        winCanvas.SetActive(false);

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
            case "shipInv":
                shipInventoryMenu.SetActive(true);
                break;
            case "gameOver":
                gameOverMenu.SetActive(true);
                break;
            case "win":
                winCanvas.SetActive(true);
                break;
            default:
                break;
        }
    }

    public static void ActivateEndTimer()
    {
        ActivateMenu("hud");
        AudioManager.PlayBoss();
        hud.transform.Find("TimerRoot").gameObject.SetActive(true);
        hud.transform.Find("OpCollectText").gameObject.SetActive(false);
        hud.transform.Find("OpGoToShipText").gameObject.SetActive(false);
    }

    public static void ActivateHudItem(string item)
    {
        switch (item)
        {
            case "gun":
                hud.transform.Find("Gun").gameObject.SetActive(true);
                break;
            case "advGun":
                hud.transform.Find("AdvGun").gameObject.SetActive(true);
                break;
            case "boots":
                hud.transform.Find("Boots").gameObject.SetActive(true);
                break;
            case "flashlight":
                hud.transform.Find("Flashlight").gameObject.SetActive(true);
                break;
            case "coin":
                hud.transform.Find("Coin").gameObject.SetActive(true);
                break;
        }
    }

    public static void SetOperationText(string text)
    {

        hud.transform.Find("TimerRoot").gameObject.SetActive(false);
        hud.transform.Find("OpCollectText").gameObject.SetActive(false);
        hud.transform.Find("OpGoToShipText").gameObject.SetActive(false);

        hud.transform.Find(text).gameObject.SetActive(true);
    }

    public static void UpdateWinCounter(int time)
    {
        if (time == 0)
            WinConditionAchieved();

        time = Mathf.FloorToInt(time / 50f);
        int min = Mathf.FloorToInt(time / 60f);
        int sec = time - (min * 60);
        timerText.text = "TIME REMAINING: " + min + ":" + (sec < 10 ? ("0" + sec) : sec);
        
    }

    public static void LoseCondition(string text)
    {
        Time.timeScale = 0;
        AudioManager.PlayGameOver();
        ActivateMenu("gameOver");
        gameOverMenu.transform.Find("GOText").GetComponent<TextMeshProUGUI>().text = text;

    }

    public static void WinConditionAchieved()
    {
        ActivateMenu("win");
        Time.timeScale = 0;
        instance.StartCoroutine(YouIsWinner());
    }

    static IEnumerator YouIsWinner()
    {
        Debug.Log("test point");
        TextMeshProUGUI t1 = winCanvas.transform.Find("Text1").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI t2 = winCanvas.transform.Find("Text2").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI t3 = winCanvas.transform.Find("Text3").GetComponent<TextMeshProUGUI>();
        foreach (char letter in winText1)
        {
            t1.text += letter; 
            AudioManager.PlayTyping();
            yield return new WaitForSecondsRealtime(0.075f); 
        }

        foreach (char letter in winText2)
        {
            AudioManager.PlayTyping();
            t2.text += letter; 
            yield return new WaitForSecondsRealtime(0.09f); 
        }

        foreach (char letter in winText3)
        {
            AudioManager.PlayTyping();
            t3.text += letter; 
            yield return new WaitForSecondsRealtime(0.15f); 
        }
        yield return new WaitForSecondsRealtime(5);
        ReturnToMainMenu();
    }
}
