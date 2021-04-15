using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IDropHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Text  m_Text;
    public Image m_Slot;
    public Image m_Icon;
    public Item  m_Item;
    public int   m_BarIndex;
    public bool  m_IsHotBar = false;

    public void Awake()
    {
        m_Icon.gameObject.SetActive(false);
    }

    public void SetItem(Item item)
    {
        m_Item = item;

        if(m_Item)
        {
            m_Icon.sprite = m_Item.m_Sprite;
            m_Icon.SetNativeSize();
        }
    }

    public bool IsFilled()
    {
        return m_Item != null;
    }

    // Started drag from slot
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (m_Item)
        {
            // Enable drag icon and initialize it with our data, then empty ourselves
            GameManager.Instance.Inventory.m_DragItemUI.gameObject.SetActive(true);
            GameManager.Instance.Inventory.m_DragItemUI.Initialized(m_Item, this);
            m_Item = null;
        }
        else
        {
            // Just ignore the drag
            ExecuteEvents.Execute<IEndDragHandler>(gameObject, eventData, ExecuteEvents.endDragHandler);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Transfer Drag too our Drag Object
        ExecuteEvents.Execute<IEndDragHandler>(gameObject, eventData, ExecuteEvents.endDragHandler);
        eventData.pointerDrag = GameManager.Instance.Inventory.m_DragItemUI.gameObject;
        ExecuteEvents.Execute<IDragHandler>(GameManager.Instance.Inventory.m_DragItemUI.gameObject, eventData, ExecuteEvents.dragHandler);
    }

    // WOW WORST CODE EVER WRITTEN BECAUSE OF DISCONNECTED UI URGGGGGGG
    public void OnDrop(PointerEventData eventData)
    {
        ItemUI itemUI = eventData.pointerDrag.GetComponent<ItemUI>();

        if(itemUI.m_PrevItemSlot == this) { SetItem(itemUI.m_Item); itemUI.m_ItemDropped = false;  return; }
        // Empty slot drop on here
        if (IsFilled() == false)
        {
            itemUI.m_ItemDropped = false;
            SetItem(itemUI.m_Item);

            if (itemUI.m_PrevItemSlot != null && itemUI.m_PrevItemSlot.m_IsHotBar)
            {
                GameManager.Instance.Player.RemoveItemFromHotBar(itemUI.m_PrevItemSlot.m_BarIndex);
            }
        }
        else
        {
            if (itemUI.m_Item.m_ItemName == m_Item.m_ItemName && m_Item.m_IsStackable)
            {
                m_Item.m_Amount += m_Item.m_Amount;

                // If its dropped onto a hotbar remove from inventory as well
                if (m_IsHotBar)
                {
                    GameManager.Instance.Inventory.RemoveItem(itemUI.m_Item);
                }
                itemUI.m_ItemDropped = false;
                itemUI.m_PrevItemSlot.SetItem(null);
            }
            else
            {
                if (m_IsHotBar)
                {
                    if (itemUI.m_PrevItemSlot.m_IsHotBar)
                    {
                        itemUI.m_PrevItemSlot.SetItem(m_Item);
                        itemUI.m_ItemDropped = false;
                        SetItem(itemUI.m_Item);
                    }
                    else
                    {
                        GameManager.Instance.Inventory.RemoveItem(itemUI.m_Item);
                        GameManager.Instance.Inventory.AddItem(m_Item, false);
                        SetItem(itemUI.m_Item);
                        itemUI.m_ItemDropped = false;
                    }
                }
                else
                {
                    if (itemUI.m_PrevItemSlot.m_IsHotBar)
                    {
                        GameManager.Instance.Player.RemoveItemFromHotBar(m_BarIndex);
                    }
                    // Just swap them.
                    itemUI.m_PrevItemSlot.SetItem(m_Item);
                    SetItem(itemUI.m_Item);
                    itemUI.m_ItemDropped = false;
                }
            }
        }

        // Refresh euqipped incase
        GameManager.Instance.Player.RefreshEquipped();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Does nothing
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsFilled() && m_Item.m_IsUseableInInventory)
        {
            if (m_IsHotBar)
            {
                if (m_Item.m_Amount >= 1)
                {
                    m_Item.OnUseItem(GameManager.Instance.Player);
                }

                if (m_Item.m_Amount <= 0)
                {
                    GameManager.Instance.Player.RemoveItemFromHotBar(m_BarIndex);
                }
            }

            // Remove the item from our slot as its now used
            if (GameManager.Instance.Inventory.UseItem(m_Item) == false)
            {
                m_Item = null;
            }
        }
    }

    public void Update()
    {
        if(m_Item)
        {
            m_Icon.gameObject.SetActive(true);
            m_Text.text = "x" + m_Item.m_Amount;
        }
        else
        {
            m_Icon.gameObject.SetActive(false);
            m_Text.text = "";
        }
    }
}
