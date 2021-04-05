using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Void : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        ItemUI item = eventData.pointerDrag.GetComponent<ItemUI>();
        if (item != null)
        {
            item.Remove();
        }
    }
}
