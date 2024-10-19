using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public Button mode1;
    public Button mode2;
    public Button quit;

    void Start() {
        mode1.onClick.AddListener(Mode1);
        mode2.onClick.AddListener(Mode2);
        quit.onClick.AddListener(QuitGame);
    }

    public void Mode1() {
        PlayerPrefs.SetInt("maxPlayers", 1);
        SceneManager.LoadScene("1Player");
    }

    public void Mode2() {
        PlayerPrefs.SetInt("maxPlayers", 2);
        SceneManager.LoadScene("2Players");
    }

    public void QuitGame()  {
        Application.Quit();
        Debug.Log("Closed!");
    }
}
