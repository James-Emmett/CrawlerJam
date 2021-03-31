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

    // Movment Targets
    private Vector3 m_TargetPosition;
    private Quaternion m_TargetRotation;


    // Start is called before the first frame update
    void Start()
    {
        m_TargetPosition = transform.position;
        m_TargetRotation = transform.rotation;
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
        if (IsMoving() == false && IsRotating() == false)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                m_TargetRotation *= Quaternion.Euler(new Vector3(0,-90,0));
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                m_TargetRotation *= Quaternion.Euler(new Vector3(0, 90, 0));
            }
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, m_TargetRotation, m_RotSpeed * Time.deltaTime);
    }

    public void UpdateMovment()
    {
        if (IsRotating() == false && IsMoving() == false)
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

    public bool IsRotating()
    {
        float angle = Quaternion.Angle(transform.rotation, m_TargetRotation);
        if (angle > 0.45f)
        {
            return true;
        }
        return false;
    }
}
