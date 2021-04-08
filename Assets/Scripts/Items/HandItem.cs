using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(SpriteRenderer))]
public class HandItem : MonoBehaviour
{
    public Player m_Player;
    public Animator m_Animator;
    public SpriteRenderer m_SpriteRenderer = null;
    public Item m_Item;
    public int m_Index;

    public void SetItem(Item item, int index)
    {
        m_Item = item;
        m_Index = index;

        if (item != null)
        {
            m_SpriteRenderer.sprite = m_Item.m_Sprite;
        }
        else
        {
            m_SpriteRenderer.sprite = null;
        }
    }

    // Start is called before the first frame update
    public void Start()
    {
        m_SpriteRenderer    = GetComponent<SpriteRenderer>();
        
        if (m_Item != null)
        {
            m_SpriteRenderer.sprite = m_Item.m_Sprite;
        }
    }

    // Update is called once per frame
    public void Update()
    {
        if(GameManager.Instance.GameState == GameState.Playing && Input.GetMouseButtonDown(0))
        {
            UseItem();
        }
    }

    // Why cant i just one shot animations in unity? seems super bad
    public void UseItem()
    {
        if (m_Item && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Default"))
        {
            if (m_Item.m_Amount >= 1)
            {
                // Play the correct animation based on the type?
                switch (m_Item.m_ItemType)
                {
                    case ItemType.Consumeable:
                        m_Animator.Play("Consumeable", -1, 0);
                        m_Item.OnUseItem(m_Player);
                        break;
                    case ItemType.Weapon:
                        Weapon weapon = (Weapon)m_Item;
                        if (m_Player.m_Energy.CurrentValue > weapon.m_EnergyCost)
                        {
                            if (weapon.m_WeaponType == WeaponType.Melee)
                            {
                                m_Animator.Play("Swing", -1, 0);
                            }
                            else if (weapon.m_WeaponType == WeaponType.Jab)
                            {
                                m_Animator.Play("Jab", -1, 0);
                            }
                            else
                            {
                                m_Animator.Play("Bow", -1, 0);
                            }

                            m_Item.OnUseItem(m_Player);
                        }
                        break;
                    case ItemType.Misc:
                        m_Item.OnUseItem(m_Player);
                        break;
                }
            }

            // If item is out of uses, we have too remove it
            if (m_Item.m_Amount <= 0)
            {
                m_Player.RemoveItemFromHotBar(m_Index);
                m_Item = null;
            }
        }
    }

    bool AnimatorIsPlaying()
    {
        return m_Animator.GetCurrentAnimatorStateInfo(0).length >
               m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }


    bool AnimatorIsPlaying(string stateName)
    {
        return AnimatorIsPlaying() && m_Animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }
}
