using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum RoomType { None, Spawn, Exit}
// Its basically a rectangle with an id and type...
[System.Serializable]
public class Room
{
    public RoomType m_RoomType;
    public int      m_RoomID;
    public int      m_X;
    public int      m_Y;
    public int      m_Width;
    public int      m_Height;

    public Room(int x, int y, int width, int height, int ID, RoomType type = RoomType.None)
    {
        m_X         = x;
        m_Y         = y;
        m_Width     = width;
        m_Height    = height;
        m_RoomID    = ID;
        m_RoomType  = type;
    }

    public int Left()
    {
        return m_X;
    }

    public int Right()
    {
        return m_X + m_Width;
    }

    public int Bottom()
    {
        return m_Y;
    }

    public int Top()
    {
        return m_Y + m_Height;
    }

    public bool Intersects(Room other)
    {
        if (Right() < other.Left() || Left() > other.Right()) { return false; }
        if (Top() < other.Bottom() || Bottom() > other.Top()) { return false; }
        return true;
    }

    public Vector3 GetCenter()
    {
        return new Vector3(m_X + (m_Width * 0.5f), 0, m_Y + (m_Height * 0.5f));
    }

    public Vector3 GetExtents()
    {
        return new Vector3(m_Width * 0.5f, 0, m_Height * 0.5f);
    }

    public Vector3 GetSize()
    {
        return new Vector3(m_Width, 0, m_Height);
    }
}