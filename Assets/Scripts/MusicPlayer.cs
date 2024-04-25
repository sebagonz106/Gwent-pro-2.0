using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip[] music;
    AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        StartCoroutine(PlayAudioClips());
    }
    IEnumerator PlayAudioClips()
    {
        while (true)
        {
            source.clip = music[Random.Range(0, music.Length)];
            source.Play();
            yield return new WaitForSeconds(source.clip.length);
        }
    }
}
