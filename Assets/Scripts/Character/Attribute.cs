using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AttributeType
{
    Health,
    Energy,
    Food,
    CaryWeight,
    Strength,
    Dexterity,
    Vitality,
    Willpower,
}

public enum ModiferType
{
    Addition,
    FlatAddition,
    Multiplication
}

public class Attribute
{
    private float m_CurrentValue;   // Current value
    private float m_BaseMaxValue;   // The base max value
    private float m_MaxValue;       // The actual max with all additions and multiplcations
    private float m_FlatAddition;   // Applies additon to Base
    private float m_Multiplier;     // Applies Multiplication to base.
    private bool  m_Dirty = true;

    public float MaxValue
    {
        get
        {
            if (m_Dirty)
            {
                m_MaxValue = (m_BaseMaxValue + m_FlatAddition) * m_Multiplier;
                return m_MaxValue;
            }
            else
            {
                return m_MaxValue;
            }
        }
    }

    public Attribute(float initialValue, float maxValue)
    {
        m_CurrentValue = Mathf.Clamp(initialValue, 0, maxValue);
        m_BaseMaxValue = maxValue;
        m_Multiplier = 1;
        m_FlatAddition = 0;
        m_MaxValue = m_BaseMaxValue;
    }

    public void IncreaseCurrentValue(float amount)
    {
        if (m_Dirty)
        {
           UpdateCurrent();
        }

        m_CurrentValue = Mathf.Clamp(m_CurrentValue + amount, 0, m_MaxValue);
    }

    public void DecreaseCurrentValue(float amount)
    {
        if(m_Dirty)
        {
            UpdateCurrent();
        }

        m_CurrentValue = Mathf.Clamp(m_CurrentValue - amount, 0, m_MaxValue);
    }

    public float CurrentValue
    {
        get { if (m_Dirty) { UpdateCurrent(); } return m_CurrentValue; }
        set 
        {
            m_CurrentValue = Mathf.Clamp(value, 0, MaxValue);
        }
    }

    // Increase the base, use flat addition for buffs!!! only 
    // leveling up or PERMENANT status should affect BaseValue!!!
    public float BaeValue
    {
        get { return m_BaseMaxValue; }
        set
        {
            m_BaseMaxValue = value;
            m_Dirty = false;
        }
    }

    
    public float FlatAddition
    {
        get { return m_FlatAddition; }
        set
        {
            m_FlatAddition = value;
            m_Dirty = false;
        }
    }

    public float Multiplier
    {
        get { return m_Multiplier; }
        set
        {
            m_Multiplier = value;
            m_Dirty = false;
        }
    }


    public void UpdateCurrent()
    {
        float max = MaxValue;
        if(m_CurrentValue > max)
        {
            m_CurrentValue = max;
        }
    }
}
