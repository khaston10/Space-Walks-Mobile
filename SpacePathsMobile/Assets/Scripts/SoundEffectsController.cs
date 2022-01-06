using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundEffectsController : MonoBehaviour
{

    private AudioSource sfxAudioSource;

    public AudioClip[] clips;

    // Start is called before the first frame update
    void Start()
    {
        sfxAudioSource = GetComponent<AudioSource>();
    }

    public void PlayClip(int clipNum)
    {
        sfxAudioSource.PlayOneShot(clips[clipNum]);
    }
}
