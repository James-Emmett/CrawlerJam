using System;

[Serializable]
public class PoisionEffect : StatusEffect
{
    public int m_ModifierValue = 1;

    public PoisionEffect()
    {
        m_Name = "Poision Effect";
    }

    public void Initialize(string name, float time, int value)
    {
        m_Name = name;
        m_Timer = time;
        m_ModifierValue = value;
    }

    public override void ApplyEffect(Unit owner)
    {
        owner.m_Health.CurrentValue -= m_ModifierValue;
    }
}