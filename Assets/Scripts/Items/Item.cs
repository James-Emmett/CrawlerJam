using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemState { Floor, Inventory, Equipped};
public enum ItemType { Consumeable, Weapon, Misc};

public class Item : ScriptableObject
{
    public ItemState    m_ItemState         = ItemState.Floor;
    public string       m_ItemName          = "";
    public string       m_Description       = "";
    public float        m_PickupDistance    = 1.0f;
    public float        m_FloatAmplitude    = 0.08f;
    public int          m_Cost              = 0;
    public AudioClip    m_UseSound          = null;
    public AudioClip    m_Pickup            = null;
    public Sprite       m_Sprite            = null;
    public bool         m_IsStackable       = false;
    public int          m_Amount            = 1;

    public virtual void OnUseItem(Player player)
    {
        GameManager.Instance.PlaySound(m_UseSound);
    }
}
