using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable] // Put this or it dont show up in inspector
public struct Config
{
    public int Width;
    public int Height;
    public int ChunkSize;
    public int ChunkOffset;

    public int ChunksWide()
    {
        return (int)Mathf.Ceil(Width / ChunkSize); //.Ceil rounds Up /.Floor Rounds down no matter the number at the end "1.1"
    }

    public int ChunksHigh()
    {
        return (int)Mathf.Ceil(Height / ChunkSize);
    }

    public int GetTotalCorners()
    {
        return (Width) * (Height);
    }

    public int GetCornersPerRow()
    {
        return Width;
    }

    public int GetCornersPerCol()
    {
        return Height;
    }
}

public class DungeonMesh : MonoBehaviour
{
    private Config m_Config;

    public  Chunk               prefab;
    public  Material            m_DungeonMaterial;
    public  List<MeshFilter>    m_MeshList = new List<MeshFilter>();
    private List<Chunk>         m_Chunks = new List<Chunk>();
    private Grid2D              m_Grid;

    public void Create(Grid2D grid, int chunkSize)
    {
        m_Grid                  = grid;
        m_Config.Width          = grid.m_Width;
        m_Config.Height         = grid.m_Height;
        m_Config.ChunkSize      = chunkSize;
        m_Config.ChunkOffset    = grid.m_TileSize;

        // Generate Chunks/Mesh
        CreateChunks();
    }

    public void SetTileFromWorldPoint(float x, float y, TileType n)
    {
        Vector2 point = m_Grid.WorldToTilePosition(x, y);

        // Gets which chunk where in
        int chunkX = (int)(point.y / m_Config.ChunkSize);
        int chunkY = (int)(point.y / m_Config.ChunkSize);

        // Use modulo to get which tile where are within the chunk itself.
        int chunkTileX = ((int)point.x % m_Config.ChunkSize); //% gets the remainder
        int chunkTileY = ((int)point.y % m_Config.ChunkSize);

        // Checks if its outside map i.e negative tile index
        if (point.x > m_Config.Width || point.x < 0 || point.y < 0 || point.y > m_Config.Height || m_Grid.GetTile((int)point.x, (int)point.y).m_Type == n)
        {
            // Nothign to change outside map bounds!!!
            // Maybe block cursor going out of bounds moron (james)?
            return;
        }

        // Edit map and update chunk
        m_Grid.GetTile((int)point.x, (int)point.y).m_Type = n;

        // Update the actual chunk in center.
        RegenerateChunk(m_Chunks[(chunkY * m_Config.ChunksWide()) + chunkX]);

        // Check X's
        if (chunkTileX == 0)
        {
            RegenerateChunk(GetChunk(chunkX - 1, chunkY));
        }
        else if (chunkTileX == m_Config.ChunkSize - 1)
        {
            RegenerateChunk(GetChunk(chunkX + 1, chunkY));
        }

        // Check Y's
        if (chunkTileY == 0)
        {
            RegenerateChunk(GetChunk(chunkX, chunkY - 1));
        }
        else if (chunkTileY == m_Config.ChunkSize - 1)
        {
            RegenerateChunk(GetChunk(chunkX, chunkY + 1));
        }

        // Check Left side diagonals
        if (chunkTileX == 0)
        {
            if (chunkTileY == 0)
            {
                RegenerateChunk(GetChunk(chunkX - 1, chunkY - 1));
            }
            else if (chunkTileY == m_Config.ChunkSize - 1)
            {
                RegenerateChunk(GetChunk(chunkX - 1, chunkY + 1));
            }
        }

        // check Right Side Diagonals
        if (chunkTileX == m_Config.ChunkSize - 1)
        {
            if (chunkTileY == 0)
            {
                RegenerateChunk(GetChunk(chunkX + 1, chunkY - 1));
            }
            else if (chunkTileY == m_Config.ChunkSize - 1)
            {
                RegenerateChunk(GetChunk(chunkX + 1, chunkY + 1));
            }
        }
    }

    public void CreateChunks()
    {
        if (m_Chunks.Count >= 1)
        {
            for (int i = 0; i < m_Chunks.Count; i++)
            {
                Destroy(m_Chunks[i].gameObject);
            }
            m_Chunks.Clear();
        }

        // Loop through each chun kand create them.
        int cw = m_Config.ChunksWide();
        int ch = m_Config.ChunksHigh();
        for (int y = 0; y < ch; ++y)
        {
            for (int x = 0; x < cw; ++x)
            {
                Chunk chunk = Instantiate(prefab);
                chunk.transform.parent = transform;
                chunk.Initialize(x, y, new Vector2(x * m_Config.ChunkSize, y * m_Config.ChunkSize) * 2);
                chunk.m_Renderer.material = m_DungeonMaterial;
                RegenerateChunk(chunk);
                m_Chunks.Add(chunk);
            }
        }
    }

    public void RegenerateChunk(Chunk chunk)
    {
        if (chunk == null) { return; }
        MeshFilter chunkMesh = chunk.m_MeshFilter;

        // The combined result for this mesh chunk.!.
        List<CombineInstance> combine = new List<CombineInstance>();

        // Loop through the correct chunk and its data
        int startY  = Mathf.Clamp(chunk.m_ChunkY * m_Config.ChunkSize, 0, m_Config.Height - 1);
        int startX  = Mathf.Clamp(chunk.m_ChunkX * m_Config.ChunkSize, 0, m_Config.Width - 1);
        int endY    = Mathf.Clamp(startY + m_Config.ChunkSize, 0, m_Config.Height - 1);
        int endX    = Mathf.Clamp(startX + m_Config.ChunkSize, 0, m_Config.Width - 1);

        for (int tileY = startY; tileY < endY; ++tileY)
        {
            for (int tileX = startX; tileX < endX; ++tileX)
            {
                int southWest = (tileY * m_Config.GetCornersPerRow()) + tileX;
                int northWest = southWest + m_Config.GetCornersPerRow();
                int northEast = northWest + 1;
                int southEast = southWest + 1;

                int meshID = 1 * (int)m_Grid[northWest].m_Type + 2 * (int)m_Grid[northEast].m_Type + 4 * (int)m_Grid[southEast].m_Type + 8 * (int)m_Grid[southWest].m_Type;

                if (meshID != 0)
                {
                    CombineInstance instance = new CombineInstance();
                    instance.mesh = m_MeshList[meshID].sharedMesh;
                    instance.transform = Matrix4x4.Translate(chunk.transform.InverseTransformPoint(new Vector3(tileX * m_Config.ChunkOffset, 0, tileY * m_Config.ChunkOffset)));
                    combine.Add(instance);
                }
            }
        }

        // Upload the new mesh too this chunk.
        chunkMesh.mesh.CombineMeshes(combine.ToArray());
        chunk.m_MeshCollider.sharedMesh = chunkMesh.sharedMesh;
    }

    public Chunk GetChunk(int x, int y)
    {
        // Check if chunk x and y is within the 2D chunk map.
        if (x >= 0 && x < m_Config.ChunksWide() && y >= 0 && y < m_Config.ChunksHigh())
        {
            // Get the chunk in a linear map.
            return m_Chunks[(y * m_Config.ChunksWide()) + x];
        }

        return null;
    }

    public void Clear()
    {
        m_Config = new Config();
        m_Chunks.Clear();
        m_Grid = null;
    }
}