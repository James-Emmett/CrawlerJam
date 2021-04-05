using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISlot : MonoBehaviour, IDropHandler
{
    public bool m_IsFilled = false;
    public Text m_Text;

    public void Awake()
    {
        m_Text = GetComponentInChildren<Text>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Only fill if not filled.!!!
        if (m_IsFilled == false)
        {
            m_IsFilled = true;
            eventData.pointerDrag.GetComponent<ItemUI>().SetSlot(this);
        }
    }
}
