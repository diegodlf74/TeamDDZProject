using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LevelTransition : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;
    public string nextSceneName;

    private bool loading = false;

    private void OnTriggerEnter(Collider other)
    {
        if (loading) return;

        if (other.CompareTag("Player"))
        {
            loading = true;
            StartCoroutine(FadeAndLoad());
        }
    }

    IEnumerator FadeAndLoad()
    {
        float t = 0f;
        Color c = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            c.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            fadeImage.color = c;

            yield return null;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}