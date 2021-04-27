using UnityEngine;

namespace BLBits
{
    public class GenericMovementAudio : MonoBehaviour, IOccludableSound
    {
        public Transform targetTransform;
        public AudioSource audioSource;
        public float movementThreshold = 0.1f;

        AREnvelope envelope;

        public bool useRotation = false;
        public bool useRigidbody = false;
        public bool ignoreFalling = false;

        public bool randomiseLoopStartPosition = false;

        private Rigidbody body;

        private float curSpeed = 0;
        private Vector3 lastPosition;
        private Quaternion lastRotation;
        private float occlusionAmount = 0;

        void Start()
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }

            if (audioSource == null)
            {
                enabled = false;
                return;
            }

            if (targetTransform == null)
            {
                targetTransform = transform;
            }

            audioSource.volume = 0;

            if (useRigidbody)
            {
                body = targetTransform.GetComponent<Rigidbody>();
            }

            if (body == null)
            {
                useRigidbody = false;
            }

            lastPosition = targetTransform.position;
            lastRotation = targetTransform.rotation;
        }

        void FixedUpdate()
        {
            if (useRigidbody)
            {
                if (!useRotation)
                {
                    if (ignoreFalling)
                    {
                        curSpeed = Vector3.ProjectOnPlane(body.velocity, Vector3.up).magnitude;
                    }
                    else
                    {
                        curSpeed = body.velocity.magnitude;
                    }
                    curSpeed = body.velocity.magnitude;
                }
                else
                {
                    curSpeed = body.angularVelocity.magnitude;
                }
            }
        }

        void Update()
        {
            if (!useRigidbody)
            {
                if (useRotation)
                {
                    curSpeed = Quaternion.Angle(targetTransform.rotation, lastRotation);
                    lastRotation = targetTransform.rotation;
                }
                else
                {
                    Vector3 movement = targetTransform.position - lastPosition;
                    if (ignoreFalling)
                    {
                        movement.y = 0;
                    }
                    curSpeed = movement.magnitude;
                    lastPosition = targetTransform.position;
                }
            }

            UpdateAudio();
        }

        private void UpdateAudio()
        {
            bool audioOn = false;
            if (curSpeed >= movementThreshold)
            {
                audioOn = true;
            }

            if (targetTransform == null)
            {
                audioOn = false;
            }

            float targetVolume = envelope.Evaluate(audioOn) * (1.0f - occlusionAmount);
            audioSource.volume = targetVolume;

            if (targetVolume > 0)
            {
                if (!audioSource.isPlaying)
                {
                    if (randomiseLoopStartPosition)
                    {
                        audioSource.Stop();
                        audioSource.timeSamples = Random.Range(0, audioSource.clip.samples);
                    }

                    audioSource.Play();
                }
            }
            else if (audioSource.volume == 0)
            {
                audioSource.Pause();
            }
        }

        public void SetOcclusionAmount(float amount)
        {
            occlusionAmount = amount;
        }

        public bool Occludable()
        {
            return true;
        }
    }
}