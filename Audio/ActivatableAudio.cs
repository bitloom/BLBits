using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BLBits
{
    public class ActivatableAudio : MonoBehaviour
    {
        protected bool active = false;

        public virtual void Activate()
        {
            active = true;
        }

        public virtual void Deactivate()
        {
            active = false;
        }
    }
}
