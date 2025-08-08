using System;
using UnityEngine;

public class AudioHelper : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip voiceInputSound;
    [SerializeField] private AudioClip frameGestureSound;
    [SerializeField] private AudioClip thumbsUpSound;
    [SerializeField] private AudioClip swipeSound;

    public void PlayListeningSound()
    {
        audioSource.PlayOneShot(voiceInputSound);
    }

    public void PlayFrameGestureSound()
    {
        audioSource.PlayOneShot(frameGestureSound);
    }

    public void PlayThumbsUpSound()
    {
        audioSource.PlayOneShot(thumbsUpSound);
    }

    public void PlaySwipeSound()
    {
        audioSource.PlayOneShot(swipeSound);
    }
}
