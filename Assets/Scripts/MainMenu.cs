using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuUI;
    public GameObject controlsUI;

    void Start()
    {
        controlsUI.SetActive(false);
    }

    public void Play()
    {
        print("Playing Game");
        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelDemo");
    }

    public void Controls()
    {
        print("this is where controls go");
        mainMenuUI.SetActive(false);
        controlsUI.SetActive(true);
    }

    public void QuitGame()
    {
        print("Quitting game");
        Application.Quit();
    }

    public void BackButton()
    {
        mainMenuUI.SetActive(true);
        controlsUI.SetActive(false);
    }
}
