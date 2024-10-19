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

    static UIAudio uIAudio;

    public static bool isInShipInventory = false;

    Slider hpSlider;
    
    static GameObject questRoot1;
    static GameObject questRoot2;

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

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(mainMenu);
        DontDestroyOnLoad(pauseMenu);
        DontDestroyOnLoad(hud);
        DontDestroyOnLoad(shipInventoryMenu);

        hpSlider = hud.transform.Find("HealthBar").GetComponent<Slider>();
        
        questRoot1 = shipInventoryMenu.transform.Find("Quest1Root").gameObject;
        questRoot2 = shipInventoryMenu.transform.Find("Quest2Root").gameObject;

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

    public static void OpenShipInventory(Dictionary<ShipComponents, int> inventory)
    {
        uIAudio.OpenSound();
        isInShipInventory = true;
        Time.timeScale = 0;
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
                + " "
                + inventory[sc]
                + " / "
                + Ship.GetRequiredInvetory()[sc];

            if (sc == ShipComponents.RCS)
                text.text = text.text.ToUpper();
        }
        questRoot1.SetActive(false);
        questRoot2.SetActive(false);
        ActivateMenu("shipInv");
    }

    public static void OpenShipInventory(
        Dictionary<ShipComponents, int> inventory,
        ShipComponents q1Item
    )
    {
        OpenShipInventory(inventory);
        questRoot1.SetActive(true);
        questRoot1.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = q1Item.GetEnumDescription().Replace("_", " ");
    }

    public static void OpenShipInventory(
        Dictionary<ShipComponents, int> inventory,
        ShipComponents q1Item,
        ShipComponents q2Item
    )
    {
        OpenShipInventory(inventory);
        questRoot1.SetActive(true);
        questRoot2.SetActive(true);
        questRoot1.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = q1Item.GetEnumDescription().Replace("_", " ");
        questRoot2.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = q1Item.GetEnumDescription().Replace("_", " ");
    }

    public static void CloseShipInventory()
    {
        Time.timeScale = 1;
        uIAudio.CloseSound();
        isInShipInventory = false;
        ActivateMenu("hud");
    }

    public static void ActivateMenu(string canvasName = "")
    {
        mainMenu.SetActive(false);
        pauseMenu.SetActive(false);
        hud.SetActive(false);
        shipInventoryMenu.SetActive(false);

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
            default:
                break;
        }
    }
}
