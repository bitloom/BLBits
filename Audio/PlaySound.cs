using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlaySoundMode{RANDOM, CYCLING, SINGLE, LOOPING, SCATTER}

[RequireComponent(typeof(AudioSource))]
public class PlaySound : DogButtonReceiver, IOccludableSound, IPausableSound
{
    public AudioClip[] clips;

    public PlaySoundMode playMode;
    public bool autoPlay = false;
    public float startCooldownOffset = 0;

    public bool randomStartIndex = false;
    public int curIndex = 0;

    public float cooldown = 0;

    public AudioSource source;
    private float cooldownTimer = 0;

    //scatter
    public float minCooldownTime = 0;
    public float maxCooldownTime = 1;

    //looping
    private float targetVolume = 0.0f;
    [Range(0,1)]
    public float maxVolume = 1.0f;
    public float attackTime = 0.5f;
    public float releaseTime = 0.5f;
    public bool randomiseLoopStart = false;
    [Range(0.0f, 1.0f)]
    public float clipStartOffset = 0;

    //pitch
    public Vector2 pitchRange = Vector2.one;
    private float targetPitch = 1.0f;

    private bool initialised = false;

    private float occlusionAmount = 0;
    private bool paused = false;

    void OnEnable()
    {
        if(autoPlay && initialised)
        {
            Activate();
        }
    }

    void Awake()
    {
        if (source == null)
        {
            source = GetComponent<AudioSource>();
        }

        if (playMode == PlaySoundMode.LOOPING)
        {
            targetVolume = 0;
            source.volume = 0;
            source.loop = true;
            if (clips.Length >= 1)
            {
                source.clip = clips[0];
            }
            source.Stop();
        }
    }

    protected override void Start()
    {
	    base.Start();
        curIndex = Mathf.Clamp(curIndex, 0, clips.Length);

        if (randomStartIndex)
        {
            curIndex = Random.Range(0, clips.Length);
        }

        if (playMode == PlaySoundMode.LOOPING)
        {
            targetPitch = pitchRange.x;
            source.pitch = targetPitch;

            if (clips.Length > 0)
            {    
                source.clip = clips[Random.Range(0, clips.Length)];   
            }
        }

        if (autoPlay)
        {
            if (startCooldownOffset > 0)
            {   
                cooldownTimer = startCooldownOffset;
            }
            else
            {
                Activate();
            }
        }

        initialised = true;
	}

    void Update()
    {
        if (playMode == PlaySoundMode.LOOPING)
        {
            if(paused)
            {
                return;
            }

            float curVolume = source.volume;

            targetVolume = active ? maxVolume * (1 - occlusionAmount) : 0;

            if ((targetVolume > 0 && attackTime == 0) || (targetVolume == 0 && releaseTime == 0))
            {
                source.volume = targetVolume;
                source.pitch = targetPitch;
            }
            else
            {
                
                float speed = maxVolume / (targetVolume > 0 ? attackTime : releaseTime);
                curVolume = Mathf.MoveTowards(curVolume, targetVolume, speed * Time.unscaledDeltaTime);

                source.volume = curVolume;

                float curPitch = source.pitch;
                speed = (pitchRange.y - pitchRange.x) / (targetPitch > pitchRange.x ? attackTime : releaseTime);
                curPitch = Mathf.MoveTowards(curPitch, targetPitch, speed * Time.unscaledDeltaTime);

                source.pitch = curPitch;
            }

            if (targetVolume > 0)
            {
                if (!source.isPlaying)
                {
                    source.Play();
                }
            }
            else if(curVolume == 0)
            {
                source.Pause();
            }
        }

        if(playMode == PlaySoundMode.SCATTER)
        {
            if (cooldownTimer > 0 && active)
            {
                if (!source.isPlaying && !paused)
                {
                    cooldownTimer -= Time.deltaTime;
                    if (cooldownTimer <= 0)
                    {
                        curIndex = Random.Range(0, clips.Length);
                        source.clip = clips[curIndex];

                        source.pitch = Random.Range(pitchRange.x, pitchRange.y);

                        source.Play();
                        cooldownTimer = Random.Range(minCooldownTime, maxCooldownTime);
                    }
                }
            }
        }
        else if (cooldownTimer > 0 && !paused)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0 && autoPlay)
            {
                Activate();
            }
        }
    }

    public override void Activate(bool asSyncReceipt)
    {
        //don't do anything if this source has been paused
        if(paused)
        {
            return;
        }

        if(source == null)
        {
            source = GetComponent<AudioSource>();
            if(source == null)
            { 
                Debug.LogErrorFormat(gameObject, "PlaySound.Activate was called on '{0}', but no AudioSource was found!", gameObject.name);
                return;
            }
        }

        if (playMode == PlaySoundMode.LOOPING)
        {
            if(source.isPlaying == false)
            {
                source.Stop();

                if(clips.Length > 1)
                {
                    source.clip = clips[Random.Range(0, clips.Length)];
                }

                if (randomiseLoopStart)
                {
                    source.timeSamples = Random.Range(0, source.clip.samples);
                }
                else
                {
                    source.time = source.clip.length * clipStartOffset;
                }
                
            }

            targetVolume = maxVolume;
            targetPitch = pitchRange.y;

            base.Activate(asSyncReceipt);

            return;
        }

        if (playMode == PlaySoundMode.SCATTER)
        {
            base.Activate(asSyncReceipt);
        }

        if (cooldownTimer > 0)
        {
            return;
        }

        source.pitch = Random.Range(pitchRange.x, pitchRange.y);
        source.PlayOneShot(clips[curIndex]);

        if (playMode == PlaySoundMode.RANDOM || playMode == PlaySoundMode.SCATTER)
        {
            curIndex = Random.Range(0, clips.Length);
        }
        else if (playMode == PlaySoundMode.CYCLING)
        {
            curIndex++;
            if (curIndex >= clips.Length)
            {
                curIndex = 0;
            }
        }

        if (playMode != PlaySoundMode.SCATTER)
        {
            cooldownTimer = cooldown;
        }
        else
        {
            cooldownTimer = Random.Range(minCooldownTime, maxCooldownTime);
        }
    }

    public override void Deactivate(bool asSyncReceipt)
    {
        if (playMode == PlaySoundMode.LOOPING)
        {
            targetVolume = 0;
            targetPitch = pitchRange.x;
        }

        base.Deactivate(asSyncReceipt);
    }

    public void SetOcclusionAmount(float amount)
    {
        occlusionAmount = amount;
    }

    public bool Occludable()
    {
        return playMode == PlaySoundMode.LOOPING;
    }

    public void SetPaused(bool paused)
    {
        //Debug.Log("Setting Pause State: " + paused);
        this.paused = paused;
        if(paused && source.isPlaying)
        {
            source.Pause();
        }
        
        if(!paused)
        {
            source.UnPause();
        }
    }
}
