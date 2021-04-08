using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
class ObjectPool
{
    public List<GameObject> m_PooledObjects;
    public GameObject       m_Template;
    public int              m_Amount;

    public ObjectPool()
    {
        // Does Nothing
    }

    public ObjectPool(GameObject prefab, int baseAmount)
    {
        InitializePool(prefab, baseAmount);
    }

    public void InitializePool(GameObject objToPool, int amount)
    {
        m_Amount = amount;
        m_PooledObjects = new List<GameObject>();
        m_Template = objToPool;

        GameObject temp;
        for (int i = 0; i < m_Amount; ++i)
        {
            temp = GameObject.Instantiate(m_Template);
            temp.SetActive(false);
            m_PooledObjects.Add(temp);
        }
    }

    public GameObject GetPooledObject()
    {
        GameObject poolObject = null;
        for (int i = 0; i < m_Amount; ++i)
        {
            if(m_PooledObjects[i].activeInHierarchy == false)
            {
                poolObject = m_PooledObjects[i];
                poolObject.SetActive(true);
                break;
            }
        }

        if(poolObject == null)
        {
            poolObject = GameObject.Instantiate(m_Template);
            poolObject.SetActive(true);
            m_PooledObjects.Add(poolObject);
            m_Amount++;
        }

        return poolObject;
    }

    // This sets all the elements for reuses
    public void ResetPool()
    {
        for (int i = 0; i < m_Amount; ++i)
        {
            m_PooledObjects[i].SetActive(false);
        }
    }
}