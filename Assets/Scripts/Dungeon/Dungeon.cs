using System.Collections.Generic;
using UnityEngine;

public class Dungeon : MonoBehaviour
{
    // add spanning tree to identify leaf nodes etc.
    public List<Room>       m_Rooms;    // Contains room meta data
    public Graph            m_Graph;
    private int[]           m_Data;     // Contains the actual tile data
    public int              m_Width;
    public int              m_Height;
    public int              m_MinRoomWidth;
    public int              m_MaxRoomWidth;
    public int              m_MinRoomHeight;
    public int              m_MaxRoomHeight;
    public float            m_Threshold;
    public int              m_Attempts;
    GraphMinSpanningTree     graphTree;
    int                     m_VisCount = 2;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Generate(m_Width, m_Height, m_MinRoomWidth, m_MaxRoomWidth, m_MinRoomHeight, m_MaxRoomHeight, m_Attempts);
        }

        if(Input.GetKeyDown(KeyCode.T))
        {
            m_VisCount++;
            if (m_VisCount >= 3) { m_VisCount = 0; }
        }
    }

    public void Generate(int width, int height, int minRoomWidth, int maxRoomWidth, int minRoomHeight, int maxRoomHeight, int attempts)
    {
        m_Width = width;
        m_Height = height;
        m_MinRoomWidth = minRoomWidth;
        m_MaxRoomWidth = maxRoomWidth;
        m_MinRoomHeight = minRoomHeight;
        m_MaxRoomHeight = maxRoomHeight;
        m_Attempts = attempts;
        m_Data = new int[m_Width * m_Height];

        // Add ranom rooms too the dungeon
        AddRooms();
        CullRooms();

        // Delaunay Triangulation
        List<Node> nodes = new List<Node>();
        for (int i = 0; i < m_Rooms.Count; i++)
        {
            Vector3 pos = (m_Rooms[i].GetCenter());
            nodes.Add(new Node(new Vector2(pos.x, pos.z), m_Rooms[i].m_RoomID, i));
        }

       List<Triangle> triangleList = DelaunayTriangulation.Triangulate(nodes, m_Width, m_Height);

        m_Graph = new Graph(true);

        for (int i = 0; i < nodes.Count; ++i)
        {
            m_Graph.AddNode(nodes[i]);
        }

        // Add the triangle edges too the graph
        foreach (Triangle triangle in triangleList)
        {
            m_Graph.AddEdge(new Edge(triangle.m_PointA.m_Index, triangle.m_PointB.m_Index));
            m_Graph.AddEdge(new Edge(triangle.m_PointB.m_Index, triangle.m_PointC.m_Index));
            m_Graph.AddEdge(new Edge(triangle.m_PointC.m_Index, triangle.m_PointA.m_Index));
        }

        graphTree = new GraphMinSpanningTree(m_Graph, -1);

        // Now clear all edges from graph, add spanning tree edges AND 15% of the culled edges back too the graph.
        m_Graph.ClearEdges();

        foreach (Edge edge in graphTree.SpanningTree())
        {
            m_Graph.AddEdge(edge);
        }

        foreach (Edge edge in graphTree.CullEdges())
        {
            // 16% chance too add loop back
            if (Random.Range(1, 6) <= 2)
            {
                m_Graph.AddEdge(edge);
            }
        }
    }

    void AddRooms()
    {
        // Re-init the room list deleting previous in the process (yay garbage hitch)
        m_Rooms = new List<Room>();

        // I prefer this over tiny Keep as the size in tiny keep is non determinastic
        // this one although filled with redundant placment we decide map size upfront.
        for (int i = 0; i < m_Attempts; ++i)
        {
            int width = Random.Range(m_MinRoomWidth, m_MaxRoomWidth);
            int height = Random.Range(m_MinRoomHeight, m_MaxRoomHeight);
            int x = Random.Range(1, m_Width - width);
            int y = Random.Range(1, m_Height - height);

            Room room = new Room(x, y, width, height, m_Rooms.Count);
            bool overlapped = false;
            // See if we collisde with an already placed room
            for (int r = 0; r < m_Rooms.Count; ++r)
            {
                if (m_Rooms[r].Intersects(room))
                {
                    overlapped = true;
                    break;
                }
            }

            if (overlapped)
            {
                continue;
            }

            m_Rooms.Add(room);
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

        int tWidth = (int)(meanWidth * 0.95f);
        int tHeight = (int)(meanHeight * 0.95f);

        List<Room> cullList = new List<Room>();
        for (int i = 0; i < m_Rooms.Count; ++i)
        {
            if (m_Rooms[i].m_Width < tWidth || m_Rooms[i].m_Height < tHeight)
            {
                cullList.Add(m_Rooms[i]);
            }
        }

        for (int i = 0; i < cullList.Count; ++i)
        {
            m_Rooms.Remove(cullList[i]);
        }
    }

    public void OnDrawGizmos()
    {
        if (m_Rooms != null)
        {
            for (int i = 0; i < m_Rooms.Count; ++i)
            {
                Gizmos.color = new Color((float)i / m_Rooms.Count, (float)i / m_Rooms.Count, (float)i / m_Rooms.Count);
                Gizmos.DrawCube(m_Rooms[i].GetCenter(), m_Rooms[i].GetSize());
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(m_Rooms[i].GetCenter(), 1);
            }
        }

        if (m_Graph != null && m_Graph.NodeCount() > 0 && (graphTree is null) == false)
        {
            if (m_VisCount == 0 && (graphTree.SpanningTree() is null) == false)
            {
                foreach (Edge edge in graphTree.SpanningTree())
                {
                    if ((edge is null) == false)
                    {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawLine(new Vector3(m_Graph.GetNode(edge.m_Start).m_Position.x, 0, m_Graph.GetNode(edge.m_Start).m_Position.y), new Vector3(m_Graph.GetNode(edge.m_End).m_Position.x, 0, m_Graph.GetNode(edge.m_End).m_Position.y));
                    }
                }
            }
            else if (m_VisCount == 1)
            {
                NodeIterator itr = new NodeIterator(m_Graph);
                for (Node node = itr.Begin(); itr.End() == false; node = itr.Next())
                {
                    foreach (Edge edge in m_Graph.GetNodeEdgeList(node.m_Index))
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawLine(new Vector3(m_Graph.GetNode(edge.m_Start).m_Position.x, 0, m_Graph.GetNode(edge.m_Start).m_Position.y), new Vector3(m_Graph.GetNode(edge.m_End).m_Position.x, 0, m_Graph.GetNode(edge.m_End).m_Position.y));
                    }
                }
            }
            else if (m_VisCount == 2 && (graphTree.CullEdges() is null) == false)
            {
                List<Edge> edges = graphTree.CullEdges();
                for (int i = 0; i < edges.Count; ++i)
                {
                    Edge edge = edges[i];
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawLine(new Vector3(m_Graph.GetNode(edge.m_Start).m_Position.x, 0, m_Graph.GetNode(edge.m_Start).m_Position.y), new Vector3(m_Graph.GetNode(edge.m_End).m_Position.x, 0, m_Graph.GetNode(edge.m_End).m_Position.y));

                }
            }
        }
    }
}
