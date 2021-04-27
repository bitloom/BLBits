using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IPausableSound
{
    void SetPaused(bool paused);
}

[RequireComponent(typeof(AudioSource))]
public class AudioPauser : DogButtonReceiver
{
    public bool pauseOnGamePause = true;
    public AudioSource targetSource;

    private bool paused = false;
    private IPausableSound pausable;

    protected override void Start()
    {
	    base.Start();
        if(targetSource == null)
        {
            targetSource = GetComponent<AudioSource>();
        }

        pausable = targetSource.GetComponent(typeof(IPausableSound)) as IPausableSound;
    }

    void Update()
    {
        if (pauseOnGamePause)
        {
            if (GameManager.CheckGameState(GameState.PAUSED))
            {
                //checks if the source is playing because it might have been paused by the activate functions
                if (paused == false)
                {
                    if (pausable != null)
                    {
                        paused = true;
                        pausable.SetPaused(true);
                    }
                    else
                    {
                        paused = true;
                        targetSource.Pause();
                    }
                }
            }
            else if (GameManager.CheckGameState(GameState.PLAYING) && paused)
            {
                if (pausable != null)
                {
                    paused = false;
                    pausable.SetPaused(false);
                }
                else
                {
                    paused = false;
                    targetSource.UnPause();
                }
            }
        }
    }

    public override void Activate(bool asSyncReceipt)
    {
        base.Activate(asSyncReceipt);

        if(pausable != null)
        {
            pausable.SetPaused(true);
        }
        else if (targetSource.isPlaying)
        {
            targetSource.Pause();
        }
    }

    public override void Deactivate(bool asSyncReceipt)
    {
        base.Deactivate(asSyncReceipt);
        if (pausable != null)
        {
            pausable.SetPaused(false);
        }
        else
        {
            targetSource.UnPause();
        }
    }
}
