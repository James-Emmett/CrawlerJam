using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Node
{
    public int          m_Index;
    public int          m_RoomID;
    public Vector2      m_Position;
    public bool         m_IsActive;

    public Node(Vector2 position, int roomID, int index)
    {
        m_Position  = position;
        m_RoomID    = roomID;
        m_Index     = index;
        m_IsActive  = true;
    }

    public static bool operator ==(Node A, Node B)
    {
        return A.m_Position == B.m_Position;// && A.m_Index == B.m_Index;
    }

    public static bool operator !=(Node A, Node B)
    {
        return A.m_Position != B.m_Position;// && A.m_Index != B.m_Index;
    }

    public override bool Equals(object obj)
    {
        return this == (Node)obj;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public bool PositionEqual(Node node)
    {
        return m_Position == node.m_Position;
    }
}