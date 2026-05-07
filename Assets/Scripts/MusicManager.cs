using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioSource audioSource;

    [Header("Music")]
    public AudioClip mainMenuMusic;
    public AudioClip levelMusic;


    public float normalPitch = 1f;
    public float pausedPitch = 0.6f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        audioSource.pitch = normalPitch;

        if (scene.name == "MainMenu")
        {
            PlayMusic(mainMenuMusic);
        }
        else
        {
            PlayMusic(levelMusic);
        }
    }

    private void PlayMusic(AudioClip clip)
    {
        if (audioSource.clip == clip) return;

        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void SetPausedMusic(bool paused)
    {
        audioSource.pitch = paused ? pausedPitch : normalPitch;
    }
}