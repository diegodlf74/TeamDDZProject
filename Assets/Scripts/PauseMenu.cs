using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;


    [Header("Sound Effects")]
    public AudioSource sfxSource;
    public AudioClip attackSound;
    public AudioClip hitConnectSound;

    public GameObject pauseMenuUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            } else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        MusicManager.Instance.SetPausedMusic(false);
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        MusicManager.Instance.SetPausedMusic(true);
    }

    public void LoadMenu()
    {
        print("Returning to menu");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        MusicManager.Instance.SetPausedMusic(false);
    }

    public void QuitGame()
    {
        print("Quitting game");
        Application.Quit();
    }
}
