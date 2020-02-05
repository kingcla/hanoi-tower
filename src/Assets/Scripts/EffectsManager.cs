using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour {

    // Handles the sound effects of the game
    public AudioSource winningSound1;
    public AudioSource delayedWinningSound2;
    public float delayWinnigSound2 = 1f;
    public AudioSource dropSound;
    public AudioSource endSound;
    
    public void PlayWinningSounds()
    {
        winningSound1.Play();
        delayedWinningSound2.PlayDelayed(delayWinnigSound2);
    }

    public void PlayDropSound()
    {
        dropSound.Play();
    }

    public void PlayEndSound()
    {   
        endSound.Play();
        MusicManager.instance.StopMusic();
    }
}
