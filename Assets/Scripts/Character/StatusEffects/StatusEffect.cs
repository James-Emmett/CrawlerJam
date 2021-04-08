using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

// Add more effects here, just hard coded its easier for now.
public enum StatusEffectType { None, Health, Energy, Poision}


[Serializable]
public class StatusEffect
{
    // How many seconds the Effect Lasts For
    public string m_Name        = "DefaultEffect";
    public float  m_Timer       = 10;
    public bool   m_Endless     = false;
    private float m_Elapsed     = 0.0f;
    private bool  m_Finished    = false;

    public virtual void Initialize(float time)
    {
        m_Timer = time;
    }

    public bool IsFinished()
    {
        return m_Finished;
    }

    public void Tick(Unit owner)
    {
        if (m_Endless == false)
        {
            if (m_Timer > 0)
            {
                m_Timer -= Time.deltaTime;
            }
            else
            {
                m_Finished = true;
            }
        }

        if(m_Finished == false)
        {
            // Applies the effect every 1 second
            m_Elapsed += Time.deltaTime;
            if (m_Elapsed >= 1.0f)
            {
                // Set elapsed too the remainder of the time so we dont slip.
                m_Elapsed = (m_Elapsed - 1.0f);
                ApplyEffect(owner);
            }
        }
    }

    public virtual void ApplyEffect(Unit owner)
    {
        // Does nothign you implement this.
    }

    // This is basically a factory that returns an object of a type for your use.
    public static StatusEffect GetStatusEffect(StatusEffectType type)
    {
        switch (type)
        {
            case StatusEffectType.Health:
                return new HealthEffect();
            case StatusEffectType.Energy:
                return new EnergyEffect();
            case StatusEffectType.Poision:
                return new PoisionEffect();
        }

        return null;
    }
}