using UnityEngine;

public class MusicManager : MonoBehaviour {

    // Make the music background a Singleton so that it won't restart
    // everytime the scene is loaded
    public static MusicManager instance;

    void Awake()
    {
        // Singleton instantiation
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // At start plays the sound if it's not playing already
        AudioSource src = instance.GetComponent<AudioSource>();
        if (!src.isPlaying)
        {
            src.Play();
        }
    }
    
    // Destroy the instance when quitting the game
    void OnApplicationQuit()
    {
        instance = null;
    }

    /// <summary>
    /// Stop playing the background music by destryoing the instance
    /// </summary>
    public void StopMusic()
    {
        AudioSource src = instance.GetComponent<AudioSource>();
        if (src.isPlaying)
        {
            src.Stop();
        }
        Destroy(gameObject);
        instance = null;
    }
}
