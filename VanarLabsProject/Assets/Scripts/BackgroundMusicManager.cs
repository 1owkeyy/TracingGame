using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager instance = null;
    public AudioSource musicSource;

    void Awake()
    {
        // Singleton pattern to keep only one instance across scenes
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // Ensure music source loops
        if (musicSource != null)
        {
            musicSource.loop = true;
            if (!musicSource.isPlaying)
                musicSource.Play();
        }
    }
}
