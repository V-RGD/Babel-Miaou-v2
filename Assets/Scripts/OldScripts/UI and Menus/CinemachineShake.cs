using Cinemachine;
using UnityEngine;

public class CinemachineShake : MonoBehaviour
{
    private CinemachineVirtualCamera _vCam;
    //private CinemachineBasicMultiChannelPerlin channel;
    private float _startIntensity;
    private float _shakeTimerTotal;

    private float _shakeTimer;

    private void Awake()
    {
        _vCam = GetComponent<CinemachineVirtualCamera>();
    }

    public void ShakeCamera(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = _vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        _shakeTimer = time;
        _startIntensity = intensity;
        _shakeTimerTotal = time;
    }

    private void Update()
    {
        if (_shakeTimer > 0)
        {
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = _vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            _shakeTimer -= Time.deltaTime;
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(_startIntensity, 0, _shakeTimer/_shakeTimerTotal);
        }
    }
}
