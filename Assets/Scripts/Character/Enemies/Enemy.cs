using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIState { Idle, Chasing, Attacking }

[RequireComponent(typeof(SpriteRenderer))]
public class Enemy : Unit
{
    public AIState          m_State = AIState.Idle;
    public StatusEffectType m_StatusOnAttack;
    public int              m_Damage;
    public float            m_AttackRate = 1;
    public float            m_MoveRate = 1;

    private float m_AttackTimer = 0;
    private float m_MoveTimer = 0;
    private Vector3 m_TargetPosition;
    private Dungeon m_Dungeon;

    private void Start()
    {
        Initialize();
        m_Dungeon = GameManager.Instance.m_Dungeon;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update(); // Updates status effects

        if (GameManager.Instance.GameState == GameState.Playing)
        {
            if (IsDead())
            {
                Die();
            }

            Vector3 forward = (GameManager.Instance.Player.transform.position - transform.position).normalized;
            forward.y = 0;
            if (forward == Vector3.zero) { forward = Vector3.forward; }
            transform.rotation = Quaternion.LookRotation(forward);

            switch (m_State)
            {
                case AIState.Idle:
                    Idle();
                    break;
                case AIState.Chasing:
                    Chasing();
                    break;
                case AIState.Attacking:
                    Attack();
                    break;
                default:
                    Idle();
                    break;
            }
        }

        transform.position = Vector3.Lerp(transform.position, m_TargetPosition, m_MoveSpeed * Time.deltaTime);
    }

    public void TakeDamage(int amount)
    {
        m_Health.CurrentValue -= amount;
    }

    public void Idle()
    {
        // Check if player is in sight
        Vector3 dirToPlayer = (GameManager.Instance.Player.transform.position - transform.position).normalized;

        RaycastHit hit;
        Ray ray = new Ray(transform.position, dirToPlayer);
        if (Physics.Raycast(ray, out hit))
        {
            if(hit.transform.CompareTag("Player"))
            {
                m_State = AIState.Chasing;
            }
        }
    }

    public void Chasing()
    {
        if (IsMoving() == false)
        {
            if (m_MoveTimer > 0)
            {
                m_MoveTimer -= Time.deltaTime;
            }
            else
            {
                m_MoveTimer = m_MoveRate;

                // m_Dungeon.m_Grid.WorldToTilePosition((int)m_Dungeon.m_Player.transform.position.x , (int)m_Dungeon.m_Player.transform.position.z)
                Vector2 nextTile = m_Dungeon.TilePathFinder.GetNextClosestTile(m_Dungeon.m_Grid.WorldToTilePosition(m_TargetPosition.x, m_TargetPosition.z));
                if (m_Dungeon.GetPlayerTile() == nextTile)
                {
                    m_State = AIState.Attacking;
                }

                if (m_Dungeon.m_Grid.GetTile((int)nextTile.x, (int)nextTile.y).IsBlocked() == false)
                {
                    // get the next tile we need too move too and set it too our target position
                    m_Dungeon.m_Grid.SetTileBlockedFromWorld(m_TargetPosition.x, m_TargetPosition.z, false);
                    m_TargetPosition = m_Dungeon.m_Grid.TileToWorldPosition(nextTile);
                    m_Dungeon.m_Grid.SetTileBlockedFromWorld(m_TargetPosition.x, m_TargetPosition.z, true);
                }
            }
        }
    }

    public void Attack()
    {
        Vector3 dirToPlayer = (GameManager.Instance.Player.transform.position - transform.position);
        if (dirToPlayer.magnitude >= 3)
        {
            m_State = AIState.Chasing;
            return;
        }

        if (m_AttackTimer > 0)
        {
            m_AttackTimer -= Time.deltaTime;
        }
        else
        {
            m_AttackTimer = m_AttackRate;
            GameManager.Instance.Player.TakeDamage(m_Damage);
            GameManager.Instance.Player.AddStatusEffect(StatusEffect.GetStatusEffect(m_StatusOnAttack));
        }
    }

    public void Die()
    {
        // Do death
        m_Dungeon.m_Grid.SetTileBlockedFromWorld(m_TargetPosition.x, m_TargetPosition.z, false);
        // Delete when death finished
        Destroy(gameObject);
    }

    public bool IsMoving()
    {
        float dist = (m_TargetPosition - transform.position).sqrMagnitude;
        if (dist > 0.5f)
        {
            return true;
        }
        return false;
    }

    public void SetPosition(Vector3 position)
    {
        m_TargetPosition   = position;
        transform.position = position;
    }

}
