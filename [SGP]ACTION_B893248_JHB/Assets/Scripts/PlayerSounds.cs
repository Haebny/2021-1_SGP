using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    PlayerControl.ANIMAL_TYPE animal_type;
    PlayerControl player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        animal_type =player.AnimalType;
        SetAudioClips();
    }

    void SetAudioClips()
    {
        switch (animal_type)
        {
            case PlayerControl.ANIMAL_TYPE.DOG:
                player.audioClips[0] = Resources.Load("Sounds/Dog/jump") as AudioClip ;
                player.audioClips[1] = Resources.Load("Sounds/Dog/run") as AudioClip;
                player.audioClips[2] = Resources.Load("Sounds/Dog/cry") as AudioClip;
                player.audioClips[3] = Resources.Load("Sounds/Dog/falldown") as AudioClip;
                break;
            case PlayerControl.ANIMAL_TYPE.CAT:
                player.audioClips[0] = Resources.Load("Sounds/Cat/cat_jump") as AudioClip;
                player.audioClips[1] = Resources.Load("Sounds/Cat/cat_run") as AudioClip;
                player.audioClips[2] = Resources.Load("Sounds/Cat/cat_cry") as AudioClip;
                player.audioClips[3] = Resources.Load("Sounds/Cat/cat_falldown") as AudioClip;
                break;
            case PlayerControl.ANIMAL_TYPE.CHICKEN:
                player.audioClips[0] = Resources.Load("Sounds/Chicken/chicken-jump") as AudioClip;
                player.audioClips[0] = Resources.Load("Sounds/Chicken/chicken-run") as AudioClip;
                player.audioClips[0] = Resources.Load("Sounds/Chicken/chicken-cry") as AudioClip;
                player.audioClips[0] = Resources.Load("Sounds/Chicken/chicken-falldown") as AudioClip;
                break;
            default:
                break;
        }
        player.audioClips[4] = Resources.Load("Sounds/Items/key") as AudioClip;
        player.audioClips[5] = Resources.Load("Sounds/Items/box") as AudioClip;
    }
}
