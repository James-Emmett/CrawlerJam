using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator
{
    public List<Room>       m_Rooms;
    public Graph            m_Graph;
    public int              m_Width;
    public int              m_Height;
    public int              m_MinRoomWidth;
    public int              m_MaxRoomWidth;
    public int              m_MinRoomHeight;
    public int              m_MaxRoomHeight;
    public int              m_Attempts;

    private GraphMinSpanningTree m_GraphTree;
    private Grid2D               m_Grid;

    public void Generate(Grid2D grid, int minRoomWidth, int minRoomHeight, int maxRoomWidth, int maxRoomHeight, int attempts)
    {
        m_Grid   = grid;
        m_Width  = grid.m_Width;
        m_Height = grid.m_Height;
        m_MinRoomWidth = minRoomWidth;
        m_MaxRoomWidth = maxRoomWidth;
        m_MinRoomHeight = minRoomHeight;
        m_MaxRoomHeight = maxRoomHeight;
        m_Attempts = attempts;

        m_Rooms = new List<Room>();
        m_Graph = new Graph(true);

        // Add ranom rooms too the dungeon
        AddRooms();
        CullRooms();
        Triangulate();
        BuildHallways();
        FillGrid();
    }

    void AddRooms()
    {
        for (int i = 0; i < m_Attempts; ++i)
        {
            int width = Random.Range(m_MinRoomWidth, m_MaxRoomWidth);
            int height = Random.Range(m_MinRoomHeight, m_MaxRoomHeight);
            Room room = new Room(Random.Range(2, m_Width - width - 1), Random.Range(2, m_Height - height - 1), width, height, m_Rooms.Count);

            bool overlapped = false;
            for (int r = 0; r < m_Rooms.Count; ++r)
            {
                if (m_Rooms[r].IntersectPadding(room))
                {
                    overlapped = true;
                    break;
                }
            }

            if (overlapped)
            {
                continue;
            }
            else
            {
                m_Rooms.Add(room);
            }
        }
    }

    void CullRooms()
    {
        // get the mean widths
        int meanWidth = 0;
        int meanHeight = 0;
        for (int i = 0; i < m_Rooms.Count; ++i)
        {
            meanWidth += m_Rooms[i].m_Width;
            meanHeight += m_Rooms[i].m_Height;
        }

        meanWidth /= m_Rooms.Count;
        meanHeight /= m_Rooms.Count;

        int tWidth = (int)(meanWidth * 0.7f);
        int tHeight = (int)(meanHeight * 0.7f);

        for (int i = m_Rooms.Count - 1; i >= 0; --i)
        {
            if (m_Rooms[i].m_Width < tWidth || m_Rooms[i].m_Height < tHeight)
            {
                m_Rooms.RemoveAt(i);
            }
        }

        // Sort rooms by smallest distance too furthest.
        RoomCompererMin roomCompererMin = new RoomCompererMin();
        m_Rooms.Sort(roomCompererMin);
    }

    void Triangulate()
    {
        // Delaunay Triangulation
        List<Node> nodes = new List<Node>();
        for (int i = 0; i < m_Rooms.Count; i++)
        {
            nodes.Add(new Node(m_Rooms[i].GetCenter(), i, i));
        }

        List<Triangle> triangleList = DelaunayTriangulation.Triangulate(nodes, m_Width, m_Height);

        // Add the nodes from the tiangulation too our graph
        for (int i = 0; i < nodes.Count; ++i)
        {
            m_Graph.AddNode(nodes[i]);
        }

        // Add the edges from the triangulation too our graph
        foreach (Triangle triangle in triangleList)
        {
            m_Graph.AddEdge(new Edge(triangle.m_PointA.m_Index, triangle.m_PointB.m_Index));
            m_Graph.AddEdge(new Edge(triangle.m_PointB.m_Index, triangle.m_PointC.m_Index));
            m_Graph.AddEdge(new Edge(triangle.m_PointC.m_Index, triangle.m_PointA.m_Index));
        }
    }

    void BuildHallways()
    {
        // Create Spannign Tree from our graph
        m_GraphTree = new GraphMinSpanningTree(m_Graph, -1);

        // Add the spannign tree result back too our graph
        m_Graph.ClearEdges();
        foreach (Edge edge in m_GraphTree.SpanningTree())
        {
            m_Graph.AddEdge(edge);
        }

        // Randomly add the culled edges back too graph
        foreach (Edge edge in m_GraphTree.CullEdges())
        {
            // 16% chance too add loop back
            if (Random.Range(1, 6) <= 1)
            {
                m_Graph.AddEdge(edge);
            }
        }
    }

    void FillGrid()
    {
        Tile tile = null;

        // Fill Rooms
        foreach (Room room in m_Rooms)
        {
            for (int y = room.Bottom(); y <= room.Top(); ++y)
            {
                for (int x = room.Left(); x <= room.Right(); ++x)
                {
                    tile = m_Grid.GetTile(x, y);
                    tile.m_Type = TileType.Room;
                    tile.m_IsMoveBlocked = false;
                }
            }
        }

        // Fill Hallways
        // Yay another loop within a loop, with a loop inside the inner loops loop
        foreach (List<Edge> edgeList in m_Graph.AdjacentList())
        {
            if (edgeList != null)
            {
                for (int e = 0; e < edgeList.Count; ++e)
                {
                    Edge edge = edgeList[e];
                    Room startRoom = m_Rooms[m_Graph.GetNode(edge.m_Start).m_RoomID];
                    Room endRoom = m_Rooms[m_Graph.GetNode(edge.m_End).m_RoomID];
                    Vector2 pointA = Vector2.zero;
                    Vector2 pointB = Vector2.zero;

                    bool isVertical = true;

                    if (startRoom.Right() < endRoom.Left() || endRoom.Right() < startRoom.Left())
                    {
                        isVertical = true;
                    }

                    if (startRoom.Top() < endRoom.Bottom() || endRoom.Top() < startRoom.Bottom())
                    {
                        isVertical = false;
                    }

                    if (isVertical)
                    {
                        // Point A picks from its top pointB picks from bottom
                        if (startRoom.Top() < endRoom.Bottom())
                        {
                            pointA = new Vector2(Mathf.Ceil(startRoom.m_X), Mathf.Ceil(startRoom.Top()));
                            pointB = new Vector2(Mathf.Ceil(endRoom.m_X), Mathf.Ceil(endRoom.Bottom()));
                        }
                        // Point A picks from bottom and pointB picks from top
                        else
                        {
                            pointA = new Vector2(Mathf.Ceil(startRoom.m_X), Mathf.Ceil(startRoom.Bottom()));
                            pointB = new Vector2(Mathf.Ceil(endRoom.m_X), Mathf.Ceil(endRoom.Top()));
                        }

                        int min = (int)Mathf.Min(pointA.y, pointB.y);
                        int max = (int)Mathf.Max(pointA.y, pointB.y);
                        int mid = (int)(min + (max - min) * 0.5f);

                        if (pointA.y < mid)
                        {
                            for (int i = (int)pointA.y; i < mid; ++i)
                            {
                                tile = m_Grid.GetTile((int)pointA.x, i);
                                tile.m_Type = TileType.Room;
                                tile.m_IsMoveBlocked = false;
                            }

                            for (int i = (int)mid; i < pointB.y; ++i)
                            {
                                tile = m_Grid.GetTile((int)pointB.x, i);
                                tile.m_Type = TileType.Room;
                                tile.m_IsMoveBlocked = false;
                            }

                        }
                        else
                        {
                            for (int i = mid; i < (int)pointA.y; ++i)
                            {
                                tile = m_Grid.GetTile((int)pointA.x, i);
                                tile.m_Type = TileType.Room;
                                tile.m_IsMoveBlocked = false;
                            }

                            for (int i = (int)pointB.y; i < mid; ++i)
                            {
                                tile = m_Grid.GetTile((int)pointB.x, i);
                                tile.m_Type = TileType.Room;
                                tile.m_IsMoveBlocked = false;
                            }
                        }

                        if (pointA.x < pointB.x)
                        {
                            for (int i = (int)pointA.x; i <= pointB.x; ++i)
                            {
                                tile = m_Grid.GetTile(i, mid);
                                tile.m_Type = TileType.Room;
                                tile.m_IsMoveBlocked = false;
                            }
                        }
                        else
                        {
                            for (int i = (int)pointB.x; i <= pointA.x; ++i)
                            {
                                tile = m_Grid.GetTile(i, mid);
                                tile.m_Type = TileType.Room;
                                tile.m_IsMoveBlocked = false;
                            }
                        }

                    }
                    else
                    {
                        // Point A picks from its Right pointB picks from Left
                        if (startRoom.Right() < endRoom.Left())
                        {
                            pointA = new Vector2(Mathf.Ceil(startRoom.Right()), Mathf.Ceil(startRoom.m_Y));
                            pointB = new Vector2(Mathf.Ceil(endRoom.Left()), Mathf.Ceil(endRoom.m_Y));
                        }
                        // Point A picks from Left and pointB picks from Right
                        else
                        {
                            pointA = new Vector2(Mathf.Ceil(startRoom.Left()), Mathf.Ceil(startRoom.m_Y));
                            pointB = new Vector2(Mathf.Ceil(endRoom.Right()), Mathf.Ceil(endRoom.m_Y));
                        }

                        int min = (int)Mathf.Min(pointA.x, pointB.x);
                        int max = (int)Mathf.Max(pointA.x, pointB.x);
                        int mid = (int)Mathf.Ceil(min + (max - min) * 0.5f);

                        if (pointA.x < mid)
                        {
                            for (int i = (int)pointA.x; i < mid; ++i)
                            {
                                tile = m_Grid.GetTile(i, (int)pointA.y);
                                tile.m_Type = TileType.Room;
                                tile.m_IsMoveBlocked = false;
                            }

                            for (int i = (int)mid; i < pointB.x; ++i)
                            {
                                tile = m_Grid.GetTile(i, (int)pointB.y);
                                tile.m_Type = TileType.Room;
                                tile.m_IsMoveBlocked = false;
                            }

                        }
                        else
                        {
                            for (int i = mid; i < (int)pointA.x; ++i)
                            {
                                tile = m_Grid.GetTile(i, (int)pointA.y);
                                tile.m_Type = TileType.Room;
                                tile.m_IsMoveBlocked = false;
                            }

                            for (int i = (int)pointB.x; i < mid; ++i)
                            {
                                tile = m_Grid.GetTile(i, (int)pointB.y);
                                tile.m_Type = TileType.Room;
                                tile.m_IsMoveBlocked = false;

                            }
                        }

                        if (pointA.y < pointB.y)
                        {
                            for (int i = (int)pointA.y; i <= pointB.y; ++i)
                            {
                                tile = m_Grid.GetTile(mid, i);
                                tile.m_Type = TileType.Room;
                                tile.m_IsMoveBlocked = false;
                            }
                        }
                        else
                        {
                            for (int i = (int)pointB.y; i <= pointA.y; ++i)
                            {
                                tile = m_Grid.GetTile(mid, i);
                                tile.m_Type = TileType.Room;
                                tile.m_IsMoveBlocked = false;
                            }
                        }
                    }
                }
            }
        }
    }
}
