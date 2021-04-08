using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class Torch_Flicker : MonoBehaviour
{
    Light m_Light = null;
    [SerializeField, Min(0)] public float m_MinIntensity = 0.5f;
    [SerializeField, Min(0)] public float m_MaxIntensity = 0.8f;
    [SerializeField, Min(0)] float m_MinFlickerFrequency = 0.1f;
    [SerializeField, Min(0)] float m_MaxFlickerFrequency = 1f;
    [SerializeField, Min(0)] public float m_Strength = 6;

    private float m_TargetIntensity;
    private float m_Timer = 0;

    private void Start()
    {
        m_Light = GetComponent<Light>();
    }

    public void Update()
    {
        m_Timer -= Time.deltaTime;
        if(m_Timer <= 0.0f)
        {
            m_Timer = Random.Range(m_MinFlickerFrequency, m_MaxFlickerFrequency);
            m_TargetIntensity = Random.Range(m_MinIntensity, m_MaxIntensity);
        }
        Flicker();
    }

    private void Flicker()
    {
        m_Light.intensity = Mathf.Lerp(m_Light.intensity, m_TargetIntensity, m_Strength * Time.deltaTime);
    }
}
