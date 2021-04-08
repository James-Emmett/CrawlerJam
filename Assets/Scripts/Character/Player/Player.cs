using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : Unit
{
    public Inventory        m_Inventory;
    public StatusBar        m_HealthBar;
    public StatusBar        m_EnergyBar;
    public List<ItemSlot>   m_HotBar;
    public HandItem         m_EquippedItem;

    // Movment Targets
    private Vector3     m_TargetPosition;
    private MouseLook   m_Camera;
    private int         m_HotBarIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        m_Camera = Camera.main.GetComponent<MouseLook>();
    }

    // Pretty much resets the player
    public override void Initialize()
    {
        base.Initialize();

        // Set Players UI Up
        m_StatusEffects.Clear();
        m_HealthBar.Initialize(m_Health);
        m_EnergyBar.Initialize(m_Energy);
        m_Inventory.Initlaize();
        m_HotBarIndex = 0;

        m_HotBar[m_HotBarIndex].SetItem(GameManager.Instance.m_ItemFactory.GetRandomWeapon());
        SetEquipedItem(m_HotBar[m_HotBarIndex].m_Item);

        EnergyEffect effect = (EnergyEffect)StatusEffect.GetStatusEffect(StatusEffectType.Energy);
        effect.m_Endless = true;
        effect.m_ModifierValue = 2;
        AddStatusEffect(effect);
    }

    // Update is called once per frame
    public override void Update()
    {
        if (GameManager.Instance.GameState == GameState.Playing)
        {
            base.Update(); // Updates status effects
            // UpdaeBuffs(); ??
            UpdateRotation();
            UpdateMovment();
            // UpdateCombat();

            // Check for inventory key
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                GameManager.Instance.GameState = GameState.Inventory;
            }

            if(IsDead())
            {
                Die();
            }
        }
        else if (GameManager.Instance.GameState == GameState.Inventory)
        {
            // Resume playing it i or escape pressed
            if(Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.Instance.GameState = GameState.Playing;
            }
        }

        float scrolled = Input.mouseScrollDelta.y;
        if (scrolled > 0)
        {
            IncrementHotBar();
        }
        else if (scrolled < 0)
        {
            DecrementHotBar();
        }
    }

    public float AttackPower()
    {
        return m_Strength.CurrentValue;
    }

    public int NextLevelXP()
    {
        // figure out how we determine next level
        return 0;
    }

    public void UpdateRotation()
    {
        transform.rotation = Quaternion.LookRotation(m_Camera.GetCardinalForward(), Vector3.up);
    }

    public void UpdateMovment()
    {
        if (IsMoving() == false)
        {
            // Cardinal Movement based on forward direction
            Vector3 dir = new Vector3(0, 0, 0);
            if (Input.GetKeyDown(KeyCode.A))
            {
                dir.x -= m_CellSize;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                dir.x += m_CellSize;
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                dir.z += m_CellSize;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                dir.z -= m_CellSize;
            }

            if (dir.magnitude > 0.1)
            {
                Vector3 goalPosition = m_TargetPosition + transform.TransformDirection(dir);
                if (GameManager.Instance.m_Dungeon.m_Grid.GetTileFromWorld(goalPosition.x, goalPosition.z).IsBlocked() == false)
                {
                    GameManager.Instance.m_Dungeon.m_Grid.SetTileBlockedFromWorld(m_TargetPosition.x, m_TargetPosition.z, false);
                    m_TargetPosition = goalPosition;
                    GameManager.Instance.m_Dungeon.m_Grid.SetTileBlockedFromWorld(m_TargetPosition.x, m_TargetPosition.z, true);
                    GameManager.Instance.m_Dungeon.TilePathFinder.FindPath(GameManager.Instance.m_Dungeon.m_Grid.WorldToTilePosition(goalPosition.x, goalPosition.z));
                }
            }
        }

        transform.position = Vector3.Lerp(transform.position, m_TargetPosition, m_MoveSpeed * Time.deltaTime);
    }

    public bool IsMoving()
    {
        float dist = (m_TargetPosition - transform.position).sqrMagnitude;
        if(dist > 0.5f)
        {
            return true;
        }
        return false;
    }

    public void IncrementHotBar()
    {
        m_HotBarIndex = (m_HotBarIndex + 1);
        if (m_HotBarIndex > m_HotBar.Count -1)
        {
            m_HotBarIndex = 0;
        }

        if (m_HotBar[m_HotBarIndex] != null)
        {
            SetEquipedItem(m_HotBar[m_HotBarIndex].m_Item);
        }
    }

    public void DecrementHotBar()
    {
        --m_HotBarIndex;
        if(m_HotBarIndex < 0)
        {
            m_HotBarIndex = m_HotBar.Count - 1;
        }

        if (m_HotBar[m_HotBarIndex] != null)
        {
            SetEquipedItem(m_HotBar[m_HotBarIndex].m_Item);
        }
    }

    public void SetEquipedItem(Item item)
    {
        m_EquippedItem.SetItem(item, m_HotBarIndex);

        for (int i = 0; i < m_HotBar.Count; i++)
        {
            if (i == m_HotBarIndex)
            {
                m_HotBar[i].m_Slot.color = Color.white;
            }
            else
            {
                m_HotBar[i].m_Slot.color = Color.white * 0.5f;
            }
        }
    }

    public void AddItemToHotBar(int index, Item item)
    {
        if (index < m_HotBar.Count)
        {
            m_HotBar[index].SetItem(item);

            if (index == m_HotBarIndex)
            {
                SetEquipedItem(item);
            }
        }
    }

    public Item RemoveItemFromHotBar(int index)
    {
        Item item = null;
        if (index < m_HotBar.Count)
        {
            item = m_HotBar[index].m_Item;
            m_HotBar[index].SetItem(null);

            if (index == m_HotBarIndex)
            {
                SetEquipedItem(null);
            }
        }

        return item;
    }

    public void RefreshEquipped()
    {
        m_EquippedItem.SetItem(m_HotBar[m_HotBarIndex].m_Item, m_HotBarIndex);
    }

    public void TakeDamage(int amount)
    {
        m_Health.CurrentValue -= amount;
    }

    private void Die()
    {
        GameManager.Instance.GameState = GameState.Dead;
    }

    public void SetPosition(Vector3 position)
    {
        m_TargetPosition = position;
        transform.position = position;
    }
}
