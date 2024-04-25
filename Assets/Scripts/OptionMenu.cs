using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionMenu : MonoBehaviour
{
    [SerializeField] private new AudioMixer audio;
    public void AudioController(float volume)
    {
        audio.SetFloat("MasterAudio", volume);
    }
    public void ChangeQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }
}
