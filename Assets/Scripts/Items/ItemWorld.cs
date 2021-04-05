using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SpriteRenderer))]
public class ItemWorld : MonoBehaviour
{
    public SpriteRenderer   m_SpriteRenderer = null;
    public Item             m_Item;
    private float           m_YOffset = 0;
    private float           m_Speed = 0;

    public void Initialize(Item item)
    {
        m_Item = item;
        m_SpriteRenderer.sprite = m_Item.m_Sprite;
        m_YOffset = transform.position.y;
        m_Speed = Random.Range(1, 1.5f);
    }

    // Start is called before the first frame update
    public void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_YOffset = transform.position.y;
        m_Speed = Random.Range(1, 1.5f);

        if (m_Item != null)
        {
            m_SpriteRenderer.sprite = m_Item.m_Sprite;
        }
    }

    // Update is called once per frame
    public void Update()
    {
        if (m_Item.m_ItemState == ItemState.Floor)
        {
            Vector3 forward = (GameManager.Instance.Player.transform.position - transform.position).normalized;
            forward.y = 0;
            transform.rotation = Quaternion.LookRotation(forward);

            float y = (Mathf.Sin(2 * Time.realtimeSinceStartup * m_Speed) * m_Item.m_FloatAmplitude) + m_YOffset;
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }
    }

    public virtual void OnPickUp()
    {
        GameManager.Instance.PlaySound(m_Item.m_Pickup);
        GameManager.Instance.Player.m_Inventory.AddItem(m_Item);
        Destroy(this.gameObject); // Yuk
    }

}
