using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    AudioSource audioSource;

    [SerializeField] AudioClip hitJumpableSurface, hitWall, jump;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        GetComponent<PlayerMovement>().PlaySoundEvent += OnPlaySoundEvent;
    }


    void OnPlaySoundEvent(object sender, PlayerMovement.PlaySoundEventArgs args)
    {
        switch (args.sfx)
        {
            case "hitJumpableSurface":
                audioSource.PlayOneShot(hitJumpableSurface);
                break;
            case "hitWall":
                audioSource.PlayOneShot(hitWall);
                break;
            case "jump":
                audioSource.PlayOneShot(jump);
                break;
        }

    }
}
