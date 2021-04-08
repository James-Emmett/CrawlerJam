using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "Data", menuName = "Items/Consumeable", order = 1)]
public class Consumeable : Item
{
    public int m_Duration = 5;
    public int m_HealingAmount;
    public int m_EnergyAmount;
    public bool m_IsPoision;

    public override void OnUseItem(Player unit)
    {
        base.OnUseItem(unit);
        m_Amount--;

        if (m_IsPoision)
        {
            PoisionEffect poision = (PoisionEffect)StatusEffect.GetStatusEffect(StatusEffectType.Poision);
            poision.Initialize(m_ItemName + " Poisioned for: " + m_HealingAmount.ToString() + " For: " + m_Duration.ToString(), m_Duration, m_HealingAmount);
            unit.AddStatusEffect(poision);

            if (m_EnergyAmount > 0)
            {
                EnergyEffect energy = (EnergyEffect)StatusEffect.GetStatusEffect(StatusEffectType.Energy);
                energy.Initialize(m_ItemName + " Effected Energy For: " + m_EnergyAmount.ToString() + " For: " + m_Duration.ToString(), m_Duration, -m_EnergyAmount);
                unit.AddStatusEffect(energy);
            }
        }
        else
        {
            // Just do normal health and energy effects
            if (m_HealingAmount > 0)
            {
                HealthEffect health = (HealthEffect)StatusEffect.GetStatusEffect(StatusEffectType.Health);
                health.Initialize(m_ItemName + " Effected Health For: " + m_HealingAmount.ToString() + " For: " + m_Duration.ToString(), m_Duration, m_HealingAmount);
                unit.AddStatusEffect(health);
            }

            if(m_EnergyAmount > 0)
            {
                EnergyEffect energy = (EnergyEffect)StatusEffect.GetStatusEffect(StatusEffectType.Energy);
                energy.Initialize(m_ItemName + " Effected Energy For: " + m_EnergyAmount.ToString() + " For: " + m_Duration.ToString(), m_Duration, m_EnergyAmount);
                unit.AddStatusEffect(energy);
            }
        }
    }
}
