using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string m_CharacterName = "Hero";
    public int m_Level = 1;
    public int m_ExperiencePoints = 0;

    //--State--
    public Attribute m_Health;
    public Attribute m_Energy;
    public Attribute m_Food;

    //--Stats--
    public Attribute m_Strength;
    public Attribute m_Dexterity;
    public Attribute m_Vitality;
    public Attribute m_Willpower;

    public float m_MoveSpeed = 5.0f;
    public float m_RotSpeed  = 5.0f;
    public int   m_CellSize  = 4; // Maybe fetch this from a map.

    public List<StatusEffect> m_StatusEffects;

    private void Start()
    {
        Initialize();
    }

    public virtual void Initialize()
    {
        m_Health    = new Attribute(100, 100);
        m_Energy    = new Attribute(100, 100);
        m_Food      = new Attribute(100, 100);
        m_Strength  = new Attribute(100, 100);
        m_Dexterity = new Attribute(100, 100);
        m_Vitality  = new Attribute(100, 100);
        m_Willpower = new Attribute(100, 100);

    }

    public virtual void Update()
    {
        for (int i = m_StatusEffects.Count - 1; i >= 0; --i)
        {
            StatusEffect effect = m_StatusEffects[i];
            if (effect.IsFinished())
            {
                m_StatusEffects.Remove(effect);
            }
            else
            {
                effect.Tick(this);
            }
        }
    }

    public void AddStatusEffect(StatusEffect effect)
    {
        if (effect != null)
        {
            m_StatusEffects.Add(effect);
        }
    }

    public bool IsDead()
    {
        return m_Health.CurrentValue <= 0;
    }
}
