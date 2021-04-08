using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[RequireComponent(typeof(Image))]
public class ItemUI : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private Canvas          m_Canvas;
    private RectTransform   m_Transform;
    private Image           m_Image;
    public  ItemSlot        m_PrevItemSlot = null;
    public  Item            m_Item;
    public  int             m_Index;
    public  bool            m_ItemDropped = true;

    private void Awake()
    {
        m_Transform = GetComponent<RectTransform>();
        m_Image = GetComponent<Image>();
    }

    // what item are we representing, and whats are starting slot;
    public void Initialized(Item item, ItemSlot prevSlot)
    {
        m_Item = item;
        m_PrevItemSlot = prevSlot;
        m_Image.sprite = m_Item.m_Sprite;
        m_Image.SetNativeSize();
        m_ItemDropped = true;
        SetPosition(Input.mousePosition);

        if(prevSlot.m_IsHotBar)
        {
            m_Index = prevSlot.m_BarIndex;
        }
    }

    public void Update()
    {
    }

    // Will this actuall trigger?
    public void OnDrag(PointerEventData eventData)
    {
        m_Transform.anchoredPosition += eventData.delta / m_Canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(m_ItemDropped)
        {
            GameManager.Instance.Inventory.DropItem(m_Item);

            if (m_PrevItemSlot.m_IsHotBar)
            {
                GameManager.Instance.Player.RemoveItemFromHotBar(m_Index);
            }
        }

        m_Item = null;
        m_PrevItemSlot = null;
        gameObject.SetActive(false);
    }

    public void SetPosition(Vector3 position)
    {
        m_Transform.position = position;
    }
}
