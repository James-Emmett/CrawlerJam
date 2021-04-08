using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Hack disctioanry Item Table, using strings as ID but wouldnt in a real game project.
public class ItemFactory : MonoBehaviour
{
    public ItemWorld m_WorldItemTemplate;
    public List<Consumeable> m_Consumeables;
    public List<Weapon> m_Weapons;
    private Dictionary<string, Item> m_ItemMap = new Dictionary<string, Item>();

    public void Start()
    {
        for (int i = 0; i < m_Consumeables.Count; ++i)
        {
            m_ItemMap.Add(m_Consumeables[i].m_ItemName, m_Consumeables[i]);
        }

        for (int i = 0; i < m_Weapons.Count; i++)
        {
            m_ItemMap.Add(m_Weapons[i].m_ItemName, m_Weapons[i]);
        }
    }

    // Instantiaites a brand new object, this should be used by the generator etc.
    public ItemWorld InstantiateWorldObject(string name, int stackAmount, Vector3 position, Vector3 impulse)
    {
        ItemWorld worldItem = null;
        Item item;
        if(m_ItemMap.TryGetValue(name, out item))
        {
            worldItem = Instantiate(m_WorldItemTemplate);
            worldItem.m_Item.m_Amount = stackAmount;
            worldItem.m_Item = Instantiate(item);
            worldItem.transform.position = position;
            if (impulse != Vector3.zero)
            {
                worldItem.m_Rigidbody.AddForce(impulse, ForceMode.Impulse);
            }
        }

        return worldItem;
    }

    //Instantiate the world object using your existing item, this means its a shared reference!!!
    public ItemWorld InstantiateWorldObjectExisting(Item item, Vector3 position, Vector3 impulse)
    {
        ItemWorld worldItem = Instantiate(m_WorldItemTemplate);
        worldItem.m_Item = item;
        worldItem.transform.position = position;

        if (impulse != Vector3.zero)
        {
            worldItem.m_Rigidbody.AddForce(impulse, ForceMode.Impulse);
        }
        return worldItem;
    }

    public Weapon GetRandomWeapon()
    {
        if (m_Weapons.Count >= 1)
        {
            return Instantiate(m_Weapons[UnityEngine.Random.Range(0, m_Weapons.Count)]);
        }

        return null;
    }

    public Consumeable GetRandomConsumeable()
    {
        if (m_Consumeables.Count >= 1)
        {
            return Instantiate(m_Consumeables[UnityEngine.Random.Range(0, m_Consumeables.Count)]);
        }

        return null;
    }
}
