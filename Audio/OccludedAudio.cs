using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IOccludableSound
{
    void SetOcclusionAmount(float amount);
    bool Occludable();
}

[RequireComponent(typeof(AudioSource), typeof(AudioLowPassFilter))]
public class OccludedAudio : MonoBehaviour
{
    [Header("Filter")]
    [Range(10.0f, 22000.0f)]
    public float minCutoffValue = 10.0f;
    [Range(10.0f, 22000.0f)]
    public float maxCutoffValue = 22000.0f;
    public AnimationCurve cutoffRemap = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Volume")]
    public bool ignoreVolume = false;
    [Range(0.0f, 1.0f)]
    public float minVolume = 0;
    public AnimationCurve volumeRemap = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private AudioLowPassFilter filter;
    private AudioSource source;
    private float curT = 0;
    private float baseVolume = 1;
    private IOccludableSound occludableSound;

    private Camera mainCamera;

    private void OnEnable()
    {
        if(source != null)
        {
            baseVolume = source.volume;
        }

        mainCamera = Camera.main;
    }

    private void OnDisable()
    {
        if(source != null)
        {
            source.volume = baseVolume;
        }
    }

    void Awake()
    {
        filter = GetComponent<AudioLowPassFilter>();
        source = GetComponent<AudioSource>();

        occludableSound = GetComponent<IOccludableSound>();

        baseVolume = source.volume;
    }

    void Update()
    {
        float angleToCamera = Vector3.Angle(Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up), Vector3.ProjectOnPlane(transform.position - mainCamera.transform.position, Vector3.up));
        curT = Mathf.Clamp01(angleToCamera / 180.0f);

        filter.cutoffFrequency = Mathf.Lerp(minCutoffValue, maxCutoffValue, cutoffRemap.Evaluate(1 - curT));

        if (ignoreVolume == false)
        {
            if (occludableSound != null && occludableSound.Occludable())
            {
                occludableSound.SetOcclusionAmount(volumeRemap.Evaluate(curT));
            }
            else
            {
                source.volume = Mathf.Lerp(minVolume, baseVolume, volumeRemap.Evaluate(1 - curT));
            }
        }
    }

    public float GetOcclusionVolumeMultiplier()
    {
        return volumeRemap.Evaluate(1 - curT);
    }
}