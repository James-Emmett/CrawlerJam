using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Inventory
{
    const int               MaxSlots = 24;
    public Canvas           m_InventoryCanvas;
    public Transform        m_Panel;
    public ItemUI           m_DragItemUI;
    public List<ItemSlot>   m_Inventory; // The UI for the items
    public Player           m_Player;
    public int              m_Count;

    public void Initlaize()
    {
        m_Player = GameManager.Instance.Player;

        for (int i = 0; i < MaxSlots; i++)
        {
            m_Inventory[i].m_Item = null;
        }

        m_Count = 0;
    }

    public bool AddItem(Item item, bool isPickUp)
    {
        if (HasFreeSpace())
        {
            if (item.m_IsStackable)
            {
                bool contains = false;
                for (int i = 0; i < m_Inventory.Count; i++)
                {
                    if (m_Inventory[i].m_Item && m_Inventory[i].m_Item.m_ItemName == item.m_ItemName)
                    {
                        contains = true;
                        m_Inventory[i].m_Item.m_Amount++;
                        break;
                    }
                }

                if(contains == false)
                {
                    PlaceItemInUI(item);
                    m_Count++;
                }
            }
            else
            {
                PlaceItemInUI(item);
                m_Count++;
            }

            if (isPickUp)
            {
                item.OnPickUp(m_Player);
            }
            return true;
        }

        return false;
    }

    // Drops the item stack on the floor
    public void DropItem(Item item)
    {
        if (item != null)
        {
            Vector3 pos = Input.mousePosition;
            pos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, 5));     
            GameManager.Instance.ItemFactory.InstantiateWorldObjectExisting(item, m_Player.transform.position, (pos - m_Player.transform.position).normalized * 5);
            --m_Count;
        }
    }

    // Completly removes the item from the inventory
    public void RemoveItem(Item item)
    {
        if (item != null)
        {
            // Pretty much what Remove does anyways so its not really anymore expensive than
            // normal.
            for (int i = 0; i < m_Inventory.Count; ++i)
            {
                if(m_Inventory[i].m_Item == item)
                {
                    m_Inventory[i].SetItem(null);
                    --m_Count;
                }
            }
        }
    }

    // Retruns true if item used and valid, false if item removed
    public bool UseItem(Item item)
    {
        if (item)
        {
            if (item.m_Amount >= 1)
            {
                item.OnUseItem(GameManager.Instance.Player);
            }

            if (item.m_Amount <= 0)
            {
                RemoveItem(item);
                return false;
            }
        }

        return true;
    }

    public bool HasFreeSpace()
    {
        return m_Count < MaxSlots;
    }

    public List<ItemSlot> ItemList()
    {
        return m_Inventory;
    }

    public Item GetItem(int index)
    {
        if(index >= 0 && index < m_Inventory.Count)
        {
            return m_Inventory[index].m_Item;
        }

        return null;
    }

    private void PlaceItemInUI(Item item)
    {
        // Find unfilled slot and set the item on the slot
        for (int i = 0; i < m_Inventory.Count; ++i)
        {
            if(m_Inventory[i].IsFilled() == false)
            {
                m_Inventory[i].SetItem(item);
                break;
            }
        }

    }
}
