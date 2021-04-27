using UnityEngine;

namespace BLBits
{
    [System.Serializable]
    public struct AREnvelope
    {
        public float attack;
        public float release;
        [Range(0.0f,1.0f)]
        public float volume;

        private float evalVolume;

        public AREnvelope(float attack, float release, float volume)
        {
            this.attack = attack;
            this.release = release;
            this.volume = volume;
            this.evalVolume = 0;
        }

        public float Evaluate(bool on)
        {
            if(on)
            {
                if(attack <= 0)
                {
                    return volume;
                }

                float speed = volume / attack;
                evalVolume = Mathf.Min(evalVolume + speed * Time.unscaledDeltaTime, volume);
            }
            else
            {
                if(release <= 0)
                {
                    return 0;
                }

                float speed = volume / release;
                evalVolume = Mathf.Max(evalVolume - speed * Time.unscaledDeltaTime, 0);
            }
            return evalVolume;
        }
    }
}
