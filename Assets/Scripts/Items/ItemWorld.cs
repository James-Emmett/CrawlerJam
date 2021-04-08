using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SpriteRenderer))]
public class ItemWorld : MonoBehaviour
{
    public SpriteRenderer   m_SpriteRenderer = null;
    public Rigidbody        m_Rigidbody = null;
    public Item             m_Item;

    public void Initialize(Item item)
    {
        m_Item = item;
        m_SpriteRenderer.sprite = m_Item.m_Sprite;
    }

    // Start is called before the first frame update
    public void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Rigidbody = GetComponent<Rigidbody>();

        if (m_Item != null)
        {
            m_SpriteRenderer.sprite = m_Item.m_Sprite;
        }
    }

    public virtual void OnPickUp()
    {
        if (GameManager.Instance.Player.m_Inventory.AddItem(m_Item, true))
        {
            Destroy(gameObject); // Yuk
        }
    }

}
