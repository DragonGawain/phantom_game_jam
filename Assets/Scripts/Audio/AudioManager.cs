using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    static AudioManager instance;
    static AudioSource mainAudioSource;
    static AudioSource componentAudioSource;

    static AudioClip mainMenu;
    static AudioClip intro;
    static AudioClip boss;
    static AudioClip gameOver;

    static AudioClip defaultFull;
    static AudioClip swampFull;
    static AudioClip asphaltFull;
    static AudioClip forestFull;

    static TerrainTypes currentTerrain = TerrainTypes.NORMAL;

    static bool isBoss = false;
    static bool introPlayed = false;

    static AudioClip shipComponentPickupSound;
    static AudioClip playerComponentPickupSound;
    static AudioClip healthSound;
    static AudioClip sonicSound;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
        if (this != instance)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        AudioSource[] temp = GetComponents<AudioSource>();
        mainAudioSource = temp[0];
        componentAudioSource = temp[1];

        mainMenu = Resources.Load<AudioClip>("Audio/OST/main_menu");
        intro = Resources.Load<AudioClip>("Audio/OST/OST_intro");
        boss = Resources.Load<AudioClip>("Audio/OST/OST_boxx");
        gameOver = Resources.Load<AudioClip>("Audio/OST/game_over");

        defaultFull = Resources.Load<AudioClip>("Audio/OST/Normal/OST_default_full");
        swampFull = Resources.Load<AudioClip>("Audio/OST/Swamp/OST_swamp_full");
        asphaltFull = Resources.Load<AudioClip>("Audio/OST/Asphalt/OST_asphalt_full");
        forestFull = Resources.Load<AudioClip>("Audio/OST/Forest/OST_forest_full");

        shipComponentPickupSound = Resources.Load<AudioClip>("Audio/component/ship_component_get");
        playerComponentPickupSound = Resources.Load<AudioClip>(
            "Audio/component/player_component_get"
        );
        healthSound = Resources.Load<AudioClip>("Audio/component/hp_pickup");
        sonicSound = Resources.Load<AudioClip>("Audio/component/sonic_ring");

        PlayMainMenu();
    }

    public static void PlayMainMenu()
    {
        instance.StopAllCoroutines();
        mainAudioSource.Stop();
        mainAudioSource.PlayOneShot(mainMenu, 2);
        mainAudioSource.loop = true;
        isBoss = false;
        introPlayed = false;
    }

    public static void PlayIntro()
    {
        mainAudioSource.Stop();
        mainAudioSource.PlayOneShot(intro, 3);
        mainAudioSource.loop = false;
        instance.StartCoroutine(WaitForIntroToFinish());
    }

    static IEnumerator WaitForIntroToFinish()
    {
        yield return new WaitForSeconds(2);
        while (mainAudioSource.isPlaying)
            yield return null;
        introPlayed = true;
        PlayBiomeTrack();
    }

    public static void PlayBiomeTrack()
    {
        if (isBoss || !introPlayed)
            return;
        mainAudioSource.Stop();
        switch (currentTerrain)
        {
            case TerrainTypes.NORMAL:
                mainAudioSource.PlayOneShot(defaultFull, 2);
                break;
            case TerrainTypes.SWAMP:
                mainAudioSource.PlayOneShot(swampFull, 2);
                break;
            case TerrainTypes.ASPHALT:
                mainAudioSource.PlayOneShot(asphaltFull, 2);
                break;
            case TerrainTypes.FOREST:
                mainAudioSource.PlayOneShot(forestFull, 2);
                break;
        }

        mainAudioSource.loop = true;
    }

    public static void SetTerrainType(TerrainTypes type)
    {
        currentTerrain = type;
        PlayBiomeTrack();
    }

    public static void PlayBoss()
    {
        mainAudioSource.Stop();
        mainAudioSource.PlayOneShot(boss, 2);
        mainAudioSource.loop = true;
        isBoss = true;
    }

    public static void PlayGameOver()
    {
        mainAudioSource.Stop();
        mainAudioSource.PlayOneShot(gameOver, 2);
        instance.StartCoroutine(WaitForGameOverToFinish());
        isBoss = false;
    }

    static IEnumerator WaitForGameOverToFinish()
    {
        while (mainAudioSource.isPlaying)
            yield return null;
        UIManager.ReturnToMainMenu();
    }

    public static void PlayShipCompGet() =>
        componentAudioSource.PlayOneShot(shipComponentPickupSound);

    public static void PlayPlayerCompGet() =>
        componentAudioSource.PlayOneShot(playerComponentPickupSound);

    public static void HealthSound() => componentAudioSource.PlayOneShot(healthSound, 2.5f);

    public static void SonicSound() => componentAudioSource.PlayOneShot(sonicSound);
}
