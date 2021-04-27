using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioOccludeZone : DogButtonReceiver
{
    public AudioSource[] targetSources;

    private List<IOccludableSound> occludableSounds = new List<IOccludableSound>();

    protected override void Start()
    {
	    base.Start();
        for(int i = 0; i < targetSources.Length; i++)
        {
            IOccludableSound occludable = targetSources[i].GetComponent(typeof(IOccludableSound)) as IOccludableSound;
            if(occludable != null)
            {
                occludableSounds.Add(occludable);
            }
        }
    }

    private void LateUpdate()
    {
        if (active)
        {
            for (int i = 0; i < occludableSounds.Count; i++)
            {
                if (occludableSounds[i].Occludable())
                {
                    occludableSounds[i].SetOcclusionAmount(0);
                }
            }
        }
        else
        {
            for (int i = 0; i < occludableSounds.Count; i++)
            {
                if (occludableSounds[i].Occludable())
                {
                    occludableSounds[i].SetOcclusionAmount(1);
                }
            }
        }
        
    }
}
