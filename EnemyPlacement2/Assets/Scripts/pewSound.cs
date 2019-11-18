using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pewSound : MonoBehaviour
{
    // audio clip slots 
    public AudioClip pew;
    public AudioClip enemyDeath1;
    public AudioClip enemyDeath2;
    public AudioClip enemyDeath3;
    public AudioClip playerDeath;
    public AudioClip enemyHit;
    public AudioClip playerHurt;
    public AudioClip playerDeath1;
    public AudioClip playerDeath2;
    // integer that determines which sound to be used within a switch, the chosen sound will get called from separate scripts.
    public int chooseSound = 0;
    // randomly choose one of the 3 enemy death sounds
    private int randomSoundValue = 0;
    AudioSource audioSource;
    
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        switch (chooseSound)
        {
            case 0:
            {
                // chooseSound's default value is 0, so this message will appear telling that no audio was chosen
                Debug.Log("No audio chosen");
                break;
            }
            case 1:
            {
                audioSource.clip = pew;
                break;
            }
            case 2:
            {
                randomSoundValue = Random.Range(1,4);
                switch (randomSoundValue)
                {
                    case 0:
                    {
                        // randomSoundValue's default value is 0, so this message will appear telling that no audio was chosen
                        Debug.Log("No audio chosen");
                        break;
                    }
                    case 1:
                    {
                        audioSource.clip = enemyDeath1;                  
                        break;
                    }
                    case 2:
                    {
                        audioSource.clip = enemyDeath2;
                        
                        break;
                    }
                    case 3:
                    {
                        audioSource.clip = enemyDeath3;
                        
                        break;
                    }
                    default:
                    {
                        //error prevention in case chooseSound value is below 1 or more than 3
                        Debug.Log("randomSoundValue is above or below case range");
                        break;
                    }
                }
                break;
            }
            case 3:
            {
                audioSource.clip = playerDeath;
                break;
            }
            case 4:
                audioSource.clip = enemyHit;
                break;
            case 5:
                audioSource.clip = playerHurt;
                break;
            case 6:
                audioSource.clip = playerDeath1;
                break;
            case 7:
                audioSource.clip = playerDeath2;
                break;
            default:
            {
                //error prevention in case chooseSound value is below 0 or more than 7
                Debug.Log("randomSoundValue is above or below case range");
                break;
            }
        }
        // play audio after audiosource clip is chosen from switch
        audioSource.Play();
    }

    void Update()
    {
        // when the audiosource is done playing, destroy the gameObject
        if (!audioSource.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}