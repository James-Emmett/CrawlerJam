using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType { Melee, Jab, Ranged };

[CreateAssetMenu(fileName = "Data", menuName = "Items/Weapon", order = 2)]
public class Weapon : Item
{
    public int m_Damage;
    public float m_Delay;
    public float m_EnergyCost;
    public StatusEffectType m_StatusType;
    public WeaponType m_WeaponType;

    public override void OnUseItem(Player owner)
    {
        GameManager.Instance.PlaySound(m_UseSound[Random.Range(0, m_UseSound.Count)]);
        owner.m_Energy.DecreaseCurrentValue(m_EnergyCost);

        switch (m_WeaponType)
        {
            case WeaponType.Melee:
            case WeaponType.Jab:
                Collider[] colliders = Physics.OverlapSphere(owner.transform.position + owner.transform.forward, 1);
                foreach (Collider entity in colliders)
                {
                    if (entity.CompareTag("Enemy"))
                    {
                        Enemy enemy = entity.gameObject.GetComponent<Enemy>();
                        enemy.TakeDamage(m_Damage);
                    }
                }
                break;
            case WeaponType.Ranged:
                break;
            default:
                break;
        }
    }
}
