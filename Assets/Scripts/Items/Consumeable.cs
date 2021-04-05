using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Consumeable", order = 1)]
public class Consumeable : Item
{
    public override void OnUseItem(Player player)
    {
        GameManager.Instance.PlaySound(m_UseSound);
    }
}
