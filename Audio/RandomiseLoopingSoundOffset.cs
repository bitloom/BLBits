using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomiseLoopingSoundOffset : MonoBehaviour
{
    private AudioSource source;
    public float maxRandomOffset = 1.0f;

    private bool sourcePlaying = false;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        if(source.playOnAwake)
        {
            //source.playOnAwake = false;
            source.PlayScheduled(AudioSettings.dspTime + Random.Range(0, maxRandomOffset));
        }
    }

    private void OnDisable()
    {
        sourcePlaying = false;
    }

    private void Update()
    {
        if(source.isPlaying && !sourcePlaying)
        {
            //source.time = Random.Range(0.0f, source.clip.length);
            source.PlayScheduled(AudioSettings.dspTime + Random.Range(0, maxRandomOffset));
        }
        sourcePlaying = source.isPlaying;
    }
}
