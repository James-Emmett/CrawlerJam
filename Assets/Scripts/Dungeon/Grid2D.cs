using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum TileType { None, Room}

public class Tile
{
    public TileType m_Type;
    public Vector2  m_Position;
    public bool     m_IsMoveBlocked;

    public Tile(int x, int y, TileType type, bool moveBlocked = false)
    {
        m_Position = new Vector2(x, y);
        m_Type = type;
        m_IsMoveBlocked = moveBlocked;
    }

    public bool IsWall()
    {
        // Its nothing so it has too be blocked
        if (m_Type == TileType.None)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsBlocked()
    {
        if (m_Type == TileType.None)
        {
            return true;
        }

        return m_IsMoveBlocked;
    }

}

// This grid is for a MARCHING sqaures algorithm so everything is offset by half of the visualised tile size!!!
// May seem confusing but its how Marching Sqaures Works!!!
public class Grid2D
{
    public int m_Width;
    public int m_Height;
    public int m_TileSize;
    private Tile[] m_Tiles;

    public Grid2D(int width, int height, int tileSize)
    {
        m_Width = width;
        m_Height = height;
        m_TileSize = tileSize;
        m_Tiles = new Tile[width * height];

        for (int y = 0; y < m_Height; ++y)
        {
            for (int x = 0; x < m_Width; ++x)
            {
                m_Tiles[(y * m_Width) + x] = new Tile(x, y, TileType.None, true);
            }
        }
    }

    public Vector3 TileToWorldPosition(float x, float y)
    {
        Vector3 point = Vector2.zero;
        point.x = (int)((x * m_TileSize) - (m_TileSize * 0.5f));
        point.z = (int)((y * m_TileSize) - (m_TileSize * 0.5f));
        point.y = 0;
        return point;
    }

    public Vector3 TileToWorldPosition(Vector2 point)
    {
        Vector3 worldPoint = Vector2.zero;
        worldPoint.x = (int)((point.x * m_TileSize) - (m_TileSize * 0.5f));
        worldPoint.z = (int)((point.y * m_TileSize) - (m_TileSize * 0.5f));
        worldPoint.y = 0;
        return worldPoint;
    }

    public Vector2 WorldToTilePosition(float x, float y)
    {
        Vector2 point = Vector2.zero;
        point.x = (int)((x / m_TileSize) + (m_TileSize * 0.5f));
        point.y = (int)((y / m_TileSize) + (m_TileSize * 0.5f));
        return point;
    }

    public void SetTileFromWorldPoint(float x, float y, Tile tile)
    {
        Vector2 tileIndex = WorldToTilePosition(x, y);
        m_Tiles[(int)(tileIndex.y * m_Width) + (int)tileIndex.x] = tile;
    }

    public void SetTile(int x, int y, Tile tile)
    {
        m_Tiles[(y * m_Width) + x] = tile;
    }

    public void SetTileBlockedFromWorld(float x, float y, bool value)
    {
        Vector2 tileIndex = WorldToTilePosition(x, y);
        m_Tiles[(int)(tileIndex.y * m_Width) + (int)tileIndex.x].m_IsMoveBlocked = value;
    }

    public void SetTileBlocked(int x, int y, bool value)
    {
        m_Tiles[(y * m_Width) + x].m_IsMoveBlocked = value;
    }

    public Tile GetTileFromWorld(float x, float y)
    {
        Vector2 tileIndex = WorldToTilePosition(x, y);
        return m_Tiles[(int)(tileIndex.y * m_Width) + (int)tileIndex.x];
    }

    public Tile GetTile(int x, int y)
    {
        if(x <= 0 || x >= m_Width || y <= 0 || y >= m_Height) 
        { 
            return null; 
        }

        return m_Tiles[(y * m_Width) + x];
    }

    public Tile this[int i]
    {
        get { return m_Tiles[i]; }
        set { m_Tiles[i] = value; }
    }

    public int TilePointToIndex(int x, int y)
    {
        return (y * m_Width) + x;
    }

    public List<Tile> Neighbours(Vector2 point)
    {
        Tile tile;
        List<Tile> tiles = new List<Tile>();

        tile = GetTile((int)point.x - 1, (int)point.y);

        if (tile != null)
        {
            tiles.Add(tile);
        }

        tile = GetTile((int)point.x + 1, (int)point.y);

        if (tile != null)
        {
            tiles.Add(tile);
        }

        tile = GetTile((int)point.x, (int)point.y + 1);
        if (tile != null)
        {
            tiles.Add(tile);
        }

        tile = GetTile((int)point.x, (int)point.y - 1);
        if (tile != null)
        {
            tiles.Add(tile);
        }

        return tiles;
    }
}