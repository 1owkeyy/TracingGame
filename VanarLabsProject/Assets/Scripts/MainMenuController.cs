using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    public Image playButtonImage;
    public Image quitButtonImage;
    public float popAnimationDuration = 0.2f;
    public float popScaleAmount = 1.2f;
    public float actionDelay = 0.35f; // Slightly longer than most short click sounds

    public AudioSource buttonAudioSource;
    public AudioClip buttonClickClip;

    private Vector3 originalScalePlay;
    private Vector3 originalScaleQuit;

    void Start()
    {
        if (playButtonImage != null)
            originalScalePlay = playButtonImage.transform.localScale;
        if (quitButtonImage != null)
            originalScaleQuit = quitButtonImage.transform.localScale;
    }

    public void OnPlayButtonClicked()
    {
        PlayButtonClickSound();
        if (playButtonImage != null)
        {
            StopAllCoroutines();
            StartCoroutine(PlayButtonPopupAnimation(playButtonImage.transform, () =>
            {
                StartCoroutine(DelayedSceneLoad("Game"));
            }));
        }
        else
        {
            StartCoroutine(DelayedSceneLoad("Game"));
        }
    }

    public void OnQuitButtonClicked()
    {
        PlayButtonClickSound();
        if (quitButtonImage != null)
        {
            StopAllCoroutines();
            StartCoroutine(PlayButtonPopupAnimation(quitButtonImage.transform, () =>
            {
                StartCoroutine(DelayedQuit());
            }));
        }
        else
        {
            StartCoroutine(DelayedQuit());
        }
    }

    private void PlayButtonClickSound()
    {
        if (buttonAudioSource != null && buttonClickClip != null)
        {
            buttonAudioSource.PlayOneShot(buttonClickClip);
        }
    }

    private IEnumerator PlayButtonPopupAnimation(Transform buttonTransform, System.Action onComplete)
    {
        Vector3 originalScale = buttonTransform.localScale;
        Vector3 targetScale = originalScale * popScaleAmount;
        float elapsed = 0f;
        while (elapsed < popAnimationDuration)
        {
            buttonTransform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / popAnimationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        buttonTransform.localScale = targetScale;
        elapsed = 0f;
        while (elapsed < popAnimationDuration)
        {
            buttonTransform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / popAnimationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        buttonTransform.localScale = originalScale;
        onComplete?.Invoke();
    }

    private IEnumerator DelayedSceneLoad(string sceneName)
    {
        yield return new WaitForSeconds(actionDelay);
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator DelayedQuit()
    {
        yield return new WaitForSeconds(actionDelay);
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
