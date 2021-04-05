using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ItemUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    // The item Slot that where currently inside?
    [SerializeField]
    private Canvas m_Canvas;
    private Image m_Image;
    private RectTransform m_Transform;
    UISlot m_PrevItemSlot = null;
    UISlot m_ItemSlot = null;
    Item m_Item;

    private void Awake()
    {
        m_Transform = GetComponent<RectTransform>();
        m_Image = GetComponent<Image>();
    }

    // what item are we representing, and whats are starting slot;
    public void Initialized(Canvas canvas, Transform panel, Item item, UISlot slot)
    {
        m_Canvas = canvas;
        m_Item = item;
        m_ItemSlot = slot;
        m_Image.sprite = m_Item.m_Sprite;
        m_Image.SetNativeSize();
        transform.SetParent(panel.transform);
        SetPosition(slot.GetComponent<RectTransform>().localPosition);
    }

    public void Update()
    {
        if (m_Item != null && m_Item.m_Amount > 1 && m_ItemSlot != null)
        {
            m_ItemSlot.m_Text.text = "x" + m_Item.m_Amount.ToString();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_ItemSlot.m_IsFilled = false;
        m_ItemSlot.m_Text.text = "";
        m_PrevItemSlot = m_ItemSlot;
        m_ItemSlot = null;
        m_Image.raycastTarget = false;
        m_Transform.position += new Vector3(0, 0, 0.1f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_Transform.anchoredPosition += eventData.delta / m_Canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (m_ItemSlot == null)
        {
            m_ItemSlot = m_PrevItemSlot;
        }

        m_PrevItemSlot = null;
        m_ItemSlot.m_IsFilled = true;
        m_Image.raycastTarget = true;
        m_Transform.position += new Vector3(0, 0, -0.1f);

        SetPosition(m_ItemSlot.GetComponent<RectTransform>().localPosition);
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void SetSlot(UISlot itemSlot)
    {
        m_ItemSlot = itemSlot;
        SetPosition(m_ItemSlot.GetComponent<RectTransform>().localPosition);
    }

    public void SetPosition(Vector3 position)
    {
        m_Transform.anchoredPosition = position;
    }

    public void Remove()
    {
        GameManager.Instance.Player.m_Inventory.RemoveItem(m_Item);
        Destroy(this.gameObject); // Yuk
    }
}
