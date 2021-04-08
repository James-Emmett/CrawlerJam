using System;
using UnityEngine;

[Serializable]
public class HealthEffect : StatusEffect
{
    public int m_ModifierValue = 1;

    public HealthEffect()
    {
        m_Name = "Health Effect";
    }

    public void Initialize(string name, float time, int value)
    {
        m_Name = name;
        m_Timer = time;
        m_ModifierValue = value;
    }

    public override void ApplyEffect(Unit owner)
    {
        owner.m_Health.CurrentValue += m_ModifierValue;
    }
}