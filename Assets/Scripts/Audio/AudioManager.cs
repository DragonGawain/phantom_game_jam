using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    static AudioManager instance;
    static AudioSource audioSource;

    static AudioClip mainMenu;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
        if (this != instance)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        mainMenu = Resources.Load<AudioClip>("Audio/OST/main_menu");
    }
}
