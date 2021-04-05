using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Inventory
{
    public Canvas           m_InventoryCanvas;
    public Transform        m_Panel;
    public ItemUI           m_DefaultItem;
    public ItemWorld        m_DefaultObject;
    List<Item>              m_Items;
    public List<UISlot>     m_UISlots;
    int                     m_MaxItem = 24;
    public Player           m_Player;

    public void Initlaize()
    {
        m_Items = new List<Item>();
        m_Player = GameManager.Instance.Player;
    }

    public bool AddItem(Item item)
    {
        if (HasFreSpace())
        {
            if (item.m_IsStackable)
            {
                bool contains = false;
                for (int i = 0; i < m_Items.Count; i++)
                {
                    if (m_Items[i].m_ItemName == item.m_ItemName)
                    {
                        contains = true;
                        m_Items[i].m_Amount++;
                        break;
                    }
                }

                if(contains == false)
                {
                    m_Items.Add(item);
                    PlaceItemInUI(item);
                }
            }
            else
            {
                m_Items.Add(item);
                PlaceItemInUI(item);
            }

            return true;
        }

        return false;
    }

    public void RemoveItem(Item item)
    {
        if(item.m_Amount > 1)
        {
            // Do clone removal thing
        }
        else
        {
            ItemWorld itemWorld = GameObject.Instantiate(m_DefaultObject);
            itemWorld.m_Item = item;
            itemWorld.transform.position = new Vector3(m_Player.transform.position.x, 0.5f, m_Player.transform.position.z) + m_Player.transform.forward;
            m_Items.Remove(item);
        }
    }

    public bool HasFreSpace()
    {
        return m_Items.Count < m_MaxItem;
    }

    public List<Item> ItemList()
    {
        return m_Items;
    }

    public Item GetItem(int index)
    {
        if(index >= 0 && index < m_Items.Count)
        {
            return m_Items[index];
        }

        return null;
    }

    private void PlaceItemInUI(Item item)
    {
        ItemUI itemUI = GameObject.Instantiate(m_DefaultItem);

        for (int i = 0; i < m_UISlots.Count; ++i)
        {
            if(m_UISlots[i].m_IsFilled == false)
            {
                itemUI.gameObject.SetActive(true);
                itemUI.Initialized(m_InventoryCanvas, m_Panel, item, m_UISlots[i]);
                m_UISlots[i].m_IsFilled = true;
                break;
            }
        }

    }
}
