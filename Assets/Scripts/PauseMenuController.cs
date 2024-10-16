using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public void ResumeGame()
    {
        SceneManager.LoadScene("CraigScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
} // class
