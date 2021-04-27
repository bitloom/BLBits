using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlaySoundOnCollision : MonoBehaviour 
{
    public bool staticOnly = true;
    public bool playFromSource = false;
    public AudioClip[] clips;
    public float volume = 1.0f;
    public float repeatCooldown = 0.5f;
    public float collisionSpeedThreshold = 5.0f;

    public AudioSource source;
    private float timer = 0.0f;
    private Rigidbody body;
    private float targetVolume = 0;
    private float baseVolume = 1.0f;
    private bool collided = false;

    void Start()
    {
        if (source == null)
        {
            source = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }

        if (collided)
        {
            PlaySound();
            collided = false;
        }
    }

    void PlaySound()
    {
        timer = repeatCooldown;
        if (playFromSource)
        {
            source.Play();
        }
        else if(clips.Length > 0)
        {
            source.PlayOneShot(clips[Random.Range(0, clips.Length)], volume);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if(staticOnly == false || (col.rigidbody == null || (col.rigidbody != null && col.rigidbody.isKinematic)))
        {
            if(col.relativeVelocity.magnitude >= collisionSpeedThreshold || collisionSpeedThreshold < 0)
            {
                if(timer <= 0)
                {
                    collided = true;
                }
            }
        }
    }
}
