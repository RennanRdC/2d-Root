/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class CinemachineShake : MonoBehaviour {

    public static CinemachineShake Instance { get; private set; }

    private CinemachineVirtualCamera cinemachineVirtualCamera;
    public VolumeProfile volume;
    public ChromaticAberration chromatic;
    public Coroutine chromaticCorou;
    private float shakeTimer;
    private float shakeTimerTotal;
    private float startingIntensity;

    private void Awake()
    {
        Instance = this;
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        volume.TryGet(out chromatic);
        chromatic.intensity.Override(0f);


        // You can leave this variable out of your function, so you can reuse it throughout your class.

    }
    public void bShakeCamera(float intensity, float time) {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = 
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;

        startingIntensity = intensity;
        shakeTimerTotal = time;
        shakeTimer = time;
    }

    private void Update() {
        if (shakeTimer > 0) {
            shakeTimer -= Time.deltaTime;
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
                cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 
                Mathf.Lerp(startingIntensity, 0f, 1 - shakeTimer / shakeTimerTotal);
        }






    }


    public void ChromaticCamera(float intensity,float velocity)
	{
        if(chromaticCorou != null)
		{
            StopCoroutine(chromaticCorou);
		}


        chromaticCorou = StartCoroutine(ChromaticCameraCorou(intensity,velocity));
	}    
    
    IEnumerator ChromaticCameraCorou(float intensity, float velocity)
	{
        float lerpIntensity = chromatic.intensity.value;

        while (Mathf.Abs(intensity - chromatic.intensity.value) > 0.1f)
		{
            lerpIntensity = Mathf.Lerp(lerpIntensity, intensity, Time.deltaTime*velocity);
            chromatic.intensity.Override(lerpIntensity);
            yield return new WaitForEndOfFrame();
		}
        chromatic.intensity.Override(intensity);

        while (Mathf.Abs(0 - chromatic.intensity.value) > 0.1f)
        {
            lerpIntensity = Mathf.Lerp(lerpIntensity, 0, Time.deltaTime * velocity);
            chromatic.intensity.Override(lerpIntensity);
            yield return new WaitForEndOfFrame();
        }
        chromatic.intensity.Override(0);

        //ChromaticCamera(0, velocity);
        chromaticCorou = null;
    }



    public void ShakeCamera(float intensity, float time)
    {
        StartCoroutine(ShakeCorou(intensity, time));
    }

    IEnumerator ShakeCorou(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
    cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        yield return new WaitForSeconds(time);
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
    }

    }
