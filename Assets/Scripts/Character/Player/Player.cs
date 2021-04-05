using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    public string   m_CharacterName = "Hero";
    public int      m_Level = 1;
    public int      m_ExperiencePoints = 0;

    //--State--
    public Attribute    m_Health;
    public Attribute    m_Energy;
    public Attribute    m_Food;
    public Attribute    m_CaryWeight;

    //--Stats--
    public Attribute m_Strength;
    public Attribute m_Dexterity;
    public Attribute m_Vitality;
    public Attribute m_Willpower;

    public float m_MoveSpeed = 5.0f;
    public float m_RotSpeed = 5.0f;
    public int   m_CellSize = 4; // Maybe fetch this from a map.
    public Inventory m_Inventory;


    // Movment Targets
    private Vector3     m_TargetPosition;
    private Quaternion  m_TargetRotation;
    private MouseLook   m_Camera;

    // Start is called before the first frame update
    void Start()
    {
        m_TargetPosition = transform.position;
        m_TargetRotation = transform.rotation;
        m_Camera = Camera.main.GetComponent<MouseLook>();
        m_Inventory.Initlaize();
    }

    // Update is called once per frame
    void Update()
    {
        // Update buffs

        UpdateRotation();
        UpdateMovment();

        // Check for Combat stuffs?
    }

    public float AttackPower()
    {
        // retrun some attack power which is how mauch damage we want o deal
        // The damage "reciver" will use damage resistance
        return 0;
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
            m_TargetPosition += transform.TransformDirection(dir);
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
}
