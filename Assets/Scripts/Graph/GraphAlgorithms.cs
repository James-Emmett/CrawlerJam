using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class GraphMinSpanningTree
{
    Graph              m_Graph;
    List<float>        m_CostToNode;
    List<Edge>         m_SpanningTree;
    List<Edge>         m_Fringe;
    List<Edge>         m_CullEdge;

    public GraphMinSpanningTree(Graph graph, int source = -1)
    {
        m_Graph = graph;
        // Hope this fills them all with null?
        m_SpanningTree = new List<Edge>();
        m_Fringe = new List<Edge>();
        m_CostToNode = new List<float>();
        m_CullEdge = new List<Edge>();

        // Fill lists with default data ebcause C# doesnt have a defaul intiialisation -_-
        for (int i = 0; i < m_Graph.NodeCount(); ++i)
        {
            m_SpanningTree.Add(null);
            m_Fringe.Add(null);
            m_CostToNode.Add(-1);
        }

        // We have too loop untill we find a source node that 
        // connects too all nodes.
        if (source < 0)
        {
            for (int nd = 0; nd < m_Graph.NodeCount(); ++nd)
            {
                if (m_SpanningTree[nd] is null)
                {
                    Search(nd);
                }
            }
        }
        else
        {
            Search(source);
        }
    }

    public void Search(int source)
    {
        m_CullEdge.Clear();

        // Orders a queue based on lowest cost, in this game jam cost is all 1
        IndexedPriorityQueueLow queue = new IndexedPriorityQueueLow(m_CostToNode, m_Graph.NodeCount());

        // Start with source
        queue.Insert(source);

        // Pop and add too spanning tree untill no nodes left
        while (queue.Empty() == false)
        {
            // Get node with smallest cost from previous node.
            int best = queue.Pop();

            // Add this node too our spannig tree
            m_SpanningTree[best] = m_Fringe[best];

            // Loop through each edge coonected too this node and evaluate it
            foreach (Edge edge in m_Graph.GetNodeEdgeList(best))
            {
                float cost = edge.m_Cost;

                // If the node isnt on the fringe its first check so add it
                if(m_Fringe[edge.m_End] is null)
                {
                    // Mark the cost of this node
                    m_CostToNode[edge.m_End] = cost;

                    // Insert into the queue
                    queue.Insert(edge.m_End);

                    // Add the edge too the fringe for searching
                    m_Fringe[edge.m_End] = edge;
                }
                // If the cost is cheaper from this node than previously then replace it and adjust
                // but only if its not already on the spanning tree, otherwsise its already sorted
                else if (cost < m_CostToNode[edge.m_End] && m_SpanningTree[edge.m_End] is null)
                {
                    m_CostToNode[edge.m_End] = cost;
                    queue.ChangePriority(edge.m_End);

                    // Inform the fringe of the new "better" edge we found.
                    m_Fringe[edge.m_End] = edge;
                }
                else if (m_SpanningTree[edge.m_End] is null)
                {
                    m_CullEdge.Add(edge);
                }
            }
        }
    }

    public List<Edge> SpanningTree()
    {
        return m_SpanningTree;
    }

    public List<Edge> CullEdges()
    {
        return m_CullEdge;
    }
}