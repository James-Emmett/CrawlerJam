using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph
{
    public const int invalid_Node = -1;
    List<Node>       m_Nodes;
    List<List<Edge>> m_AdjacentList;
    int              m_NextIndex;
    bool             m_IsDirected = true;

    public Graph(bool isDirected = true)
    {
        m_IsDirected = isDirected;
        m_Nodes = new List<Node>();
        m_AdjacentList = new List<List<Edge>>();
    }

    public int GetNextFreeNode()
    {
        return m_NextIndex;
    }

    // Adds a new node too the graph for you to use
    public int AddNode(Node node)
    {
        if (node.m_Index < m_Nodes.Count)
        {
            if (node.m_Index != invalid_Node) { throw new Exception("Attempting too replace existing node"); }

            m_Nodes[node.m_Index] = node;
            return m_NextIndex;
        }
        else
        {
            if(node.m_Index == invalid_Node)
            {
                throw new Exception("Attempting Too Add Invalid Node");
            }

            m_Nodes.Add(node);
            m_AdjacentList.Add(new List<Edge>());

            return m_NextIndex++;
        }
    }

    public Node GetNode(int index)
    {
        if(index >= m_Nodes.Count || m_Nodes[index].m_Index == invalid_Node)
        {
            //throw new Exception("Invalid Node");
        }

        return m_Nodes[index];
    }

    public int NodeCount()
    {
        return m_Nodes.Count;
    }

    public List<Node> GetNodeList()
    {
        return m_Nodes;
    }

    public List<Edge> GetNodeEdgeList(int node)
    {
        if (node >= m_Nodes.Count || m_Nodes[node].m_Index == invalid_Node)
        {
            throw new Exception("Invalid Node");
        }
        return m_AdjacentList[node];
    }

    public void RemoveNode(int node)
    {
        if(node >= m_Nodes.Count) { throw new Exception("Invalid Node Index"); } 
        
        // Theres a chance it will be reactivated
        m_Nodes[node].m_Index = invalid_Node;

        if(m_IsDirected == false)
        {
            // Loop through each neighbour
            foreach (Edge edge in m_AdjacentList[node])
            {
                // Loop through each neighbors edges and remove any too removed node
                foreach (Edge item in m_AdjacentList[edge.m_End])
                {
                    if(item.m_End == node)
                    {
                        m_AdjacentList[item.m_Start].Remove(item);
                        break;
                    }
                }
            }
            // Clear for next nodes use!
            m_AdjacentList[node].Clear();
        }
        else
        {
            // Slow removal of node edges.
            RemoveInvalidEdges();
        }
    }

    public void AddEdge(Edge edge)
    {
        if (edge.m_Start >= m_NextIndex && edge.m_End >= m_NextIndex)
        {
            throw new Exception("Invalid Edge Detected");
        }

        if (edge.m_Start != invalid_Node && edge.m_End != invalid_Node)
        {
            if (IsUniqueEdge(edge.m_Start, edge.m_End))
            {
                m_AdjacentList[edge.m_Start].Add(edge);
            }

            // If the graph is not directed we must add a second edge going in the other direction.!!!
            if (m_IsDirected == false)
            {
                if (IsUniqueEdge(edge.m_End, edge.m_Start))
                {
                    m_AdjacentList[edge.m_End].Add(new Edge(edge.m_End, edge.m_Start));
                }
            }
        }
    }

    public void RemoveEdge(int start, int end)
    {
        if ((start >= m_NextIndex || end >= m_NextIndex) && (start == invalid_Node || end == invalid_Node))
        {
            throw new Exception("Invalid Edge Indexs");
        }

        // Remove Double edge from end too start node if not direced.
        if(m_IsDirected == false)
        {
            foreach (Edge edge in m_AdjacentList[end])
            {
                if(edge.m_End == start)
                {
                    m_AdjacentList[end].Remove(edge);
                    break;
                }
            }
        }

        // Remove directed edge
        foreach (Edge edge in m_AdjacentList[start])
        {
            if(edge.m_End == end)
            {
                m_AdjacentList[start].Remove(edge);
                break;
            }
        }
    }

    public bool IsUniqueEdge(int start, int end)
    {
        foreach (Edge edge in m_AdjacentList[start])
        {
            if(edge.m_End == end)
            {
                return false;
            }
        }
        return true;
    }

    // SLOOOOOW
    private void RemoveInvalidEdges()
    {
        // Not sure how effecient iterator is in C#
        // Negative list iteration might be more optimal?
        foreach (List<Edge> edgeList in m_AdjacentList)
        {
            foreach (Edge edge in edgeList)
            {
                Node A = m_Nodes[edge.m_Start];
                Node B = m_Nodes[edge.m_End];

                if(A.m_Index == invalid_Node || B.m_Index == invalid_Node)
                {
                    // Remove the edge
                    edgeList.Remove(edge);
                }
            }
        }
    }

    public void ClearAll()
    {
        m_AdjacentList.Clear();
        m_Nodes.Clear();
    }

    public void ClearEdges()
    {
        foreach (List<Edge> edgeList in m_AdjacentList)
        {
            edgeList.Clear();
        }
    }

    public void ClearNodes()
    {
        ClearAll();
    }
}

class NodeIterator
{
    private Graph        m_Graph;
    private List<Node>   m_Nodes;
    private int          m_CurrentNode;

    public NodeIterator(Graph graph)
    {
        m_Graph = graph;
        m_Nodes = graph.GetNodeList();
        m_CurrentNode = 0;
    }

    // Get FirstActive Node
    public Node Begin()
    {
        // Start at first index, iterate untill an active node is found
        // Then return the current node which is next active.!!!
        GetNextValid(0);
        return m_Graph.GetNodeList()[m_CurrentNode];
    }

    // Return Next Active Node
    public Node Next()
    {
        if (GetNextValid(++m_CurrentNode))
        {
            return m_Graph.GetNode(m_CurrentNode);
        }

        return null;
    }

    public bool End()
    {
        if (m_CurrentNode == m_Nodes.Count)
        {
            return true;
        }

        return false;
    }

    bool GetNextValid(int index)
    {
        if(m_CurrentNode == m_Nodes.Count) 
        {
            return false; 
        }

        while (m_Nodes[index].m_Index == Graph.invalid_Node)
        {
            index++;

            if(index == m_Nodes.Count - 1)
            {
                break;
            }
        }

        // Get the handle of what ever we landed on
        m_CurrentNode = m_Nodes[index].m_Index;
        return true;
    }
}