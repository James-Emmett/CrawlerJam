using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemState { Floor, Inventory, Equipped};
public enum ItemType { None, Consumeable, Weapon, Misc};

public class Item : ScriptableObject
{
    public ItemState        m_ItemState             = ItemState.Floor;
    public ItemType         m_ItemType              = ItemType.None;
    public string           m_ItemName              = "";
    public string           m_Description           = "";
    public float            m_PickupDistance        = 1.0f;
    public int              m_Cost                  = 0;
    public List<AudioClip>  m_UseSound              = null;
    public List<AudioClip>  m_Pickup                = null;
    public Sprite           m_Sprite                = null;
    public bool             m_IsStackable           = false;
    public bool             m_IsUseableInInventory  = false;
    public int              m_Amount                = 1;

    public virtual void OnUseItem(Player owner)
    {
        if (m_UseSound.Count >= 1)
        {
            GameManager.Instance.PlaySound(m_UseSound[Random.Range(0, m_UseSound.Count)]);
        }
    }

    public virtual void OnPickUp(Player owner)
    {
        if (m_Pickup.Count >= 1)
        {
            GameManager.Instance.PlaySound(m_Pickup[Random.Range(0, m_Pickup.Count)]);
        }
    }
}
