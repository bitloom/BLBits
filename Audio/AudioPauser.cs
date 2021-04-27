using UnityEngine;

namespace BLBits
{
    interface IPausableSound
    {
        void SetPaused(bool paused);
    }

    [RequireComponent(typeof(AudioSource))]
    public class AudioPauser : ActivatableAudio
    {
        public static bool globalPause = false;

        public bool pauseOnGamePause = true;
        public AudioSource targetSource;

        private bool paused = false;
        private IPausableSound pausable;

        protected void Start()
        {
            if (targetSource == null)
            {
                targetSource = GetComponent<AudioSource>();
            }

            pausable = targetSource.GetComponent(typeof(IPausableSound)) as IPausableSound;
        }

        void Update()
        {
            if (pauseOnGamePause)
            {
                if (globalPause)
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
                else if (globalPause == false && paused)
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

        public override void Activate()
        {
            base.Activate();

            if (pausable != null)
            {
                pausable.SetPaused(true);
            }
            else if (targetSource.isPlaying)
            {
                targetSource.Pause();
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();

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
}