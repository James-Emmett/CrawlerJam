using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TileEdge
{
    public Vector2 m_Parent;
    public Vector2 m_Tile;
    public int     m_ParentIndex;
    public int     m_TileIndex;

    public TileEdge(Vector2 parent, Vector2 tile, int parentIndex, int tileIndex)
    {
        m_Parent        = parent;
        m_Tile          = tile;
        m_ParentIndex   = parentIndex;
        m_TileIndex     = tileIndex;
    }
}

public class TilePathFinder
{
    Grid2D          m_Grid = null;
    TileEdge[]      m_ShortestPathTree;

    public TilePathFinder(Grid2D grid)
    {
        m_Grid              = grid;
    }

    public bool FindPath(Vector2 start)
    {
        // So expensive :/
        m_ShortestPathTree  = new TileEdge[m_Grid.m_Width * m_Grid.m_Height];
        int startIndex      = m_Grid.TilePointToIndex((int)start.x, (int)start.y);

        // Find next node
        Queue<int> queue = new Queue<int>();

        // Put the start tile index in our priority queue
        queue.Enqueue(startIndex);

        // While not empty keep finding a path
        while (queue.Count != 0)
        {
            // Get enxt node from queue
            int current = queue.Dequeue();

            // Loop through each neighbouring node
            List<Tile> neighbourNodes = m_Grid.Neighbours(m_Grid[current].m_Position);
            foreach (Tile tile in neighbourNodes)
            {
                if (tile.IsWall()) { continue; }
                int tileIndex = m_Grid.TilePointToIndex((int)tile.m_Position.x, (int)tile.m_Position.y);

                // Add node too frontier if not already on frontier
                if(m_ShortestPathTree[tileIndex] == null)
                {
                    queue.Enqueue(tileIndex);

                    // Add edge so we know parent for when we add edge too the shortest path otehrwsie we cant follow path back to make path.!!!
                    m_ShortestPathTree[tileIndex] = new TileEdge(m_Grid[current].m_Position, tile.m_Position, current, tileIndex);
                }
            }
        }
   
        return false;
    }

    public float ManHatDistance(Vector2 A, Vector2 B)
    {
        return (B.x - A.x) + (B.y - A.y);
    }

    public Vector2 GetNextClosestTile(Vector2 position)
    {
        if (m_ShortestPathTree != null)
        {
            if (m_ShortestPathTree[m_Grid.TilePointToIndex((int)position.x, (int)position.y)] != null)
            {
                return m_ShortestPathTree[m_Grid.TilePointToIndex((int)position.x, (int)position.y)].m_Parent;
            }
        }
        return position;
    }

    public TileEdge[] PathTree()
    {
        return m_ShortestPathTree;
    }
}
