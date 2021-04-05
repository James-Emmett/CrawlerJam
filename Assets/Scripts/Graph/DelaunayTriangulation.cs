using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public struct Circle
{
    public Vector2  m_Center;
    public float    m_Radius;

    public Circle(Vector2 center, float radius)
    {
        m_Center = center;
        m_Radius = radius;
    }

    public bool Contain(Vector2 point)
    {
        return (point - m_Center).SqrMagnitude() < (m_Radius * m_Radius);
    }

    public bool Contain(int x, int y)
    {
        return (x - m_Center.x) * (x - m_Center.x) + (y - m_Center.y) * (y - m_Center.y) < (m_Radius * m_Radius);
    }
}

public class Triangle
{
    public Node m_PointA;
    public Node m_PointB;
    public Node m_PointC;
    public Circle  m_Circle;

    public Triangle(Node A, Node B, Node C)
    {
        if(IsCounterClockwise(A.m_Position,B.m_Position,C.m_Position) == false)
        {
            m_PointA = A;
            m_PointB = C;
            m_PointC = B;
        }
        else
        {
            m_PointA = A;
            m_PointB = B;
            m_PointC = C;
        }

        UpdateCircumCircle();
    }

    // Im still a bit "unsure" how they derived the aux1, aux2 or what they are, 
    // it makes sense how circumcircle is derived using perpendicular vectors but this
    // seems like something different?
    // http://www.ctralie.com/Teaching/COMPSCI290/Lectures/3_SegmentIntersection_Circumcenter/index.html
    // https://codefound.wordpress.com/2013/02/21/how-to-compute-a-circumcircle/#more-58
    // https://www.ics.uci.edu/~eppstein/junkyard/circumcenter.html
    // https://youtu.be/_qeoJnjpyRQ
    void UpdateCircumCircle()
    {
        // Get the Sqaured length of each vector pointing too triangles
        float A = m_PointA.m_Position.sqrMagnitude;
        float B = m_PointB.m_Position.sqrMagnitude;
        float C = m_PointC.m_Position.sqrMagnitude;

        var aux1 =  (A * (m_PointC.m_Position.y -  m_PointB.m_Position.y) + B * (m_PointA.m_Position.y - m_PointC.m_Position.y) + C  * (m_PointB.m_Position.y - m_PointA.m_Position.y));
        var aux2 = -(A * (m_PointC.m_Position.x -  m_PointB.m_Position.x) + B * (m_PointA.m_Position.x - m_PointC.m_Position.x) + C  * (m_PointB.m_Position.x - m_PointA.m_Position.x));
        var div  =  (2 * (m_PointA.m_Position.x * (m_PointC.m_Position.y - m_PointB.m_Position.y) + m_PointB.m_Position.x * (m_PointA.m_Position.y - m_PointC.m_Position.y) + m_PointC.m_Position.x * (m_PointB.m_Position.y - m_PointA.m_Position.y)));

        if (div == 0)
        {
            throw new DivideByZeroException();
        }

        m_Circle.m_Center = new Vector2(aux1 / div, aux2 / div);
        // standard radius from circle calculation just uses point A all poitns radius is the same though !
        m_Circle.m_Radius = Mathf.Sqrt((m_Circle.m_Center.x - m_PointA.m_Position.x) * (m_Circle.m_Center.x - m_PointA.m_Position.x) + (m_Circle.m_Center.y - m_PointA.m_Position.y) * (m_Circle.m_Center.y - m_PointA.m_Position.y));
    }

    public bool SharesNode(Triangle triangle)
    {
        if (m_PointA == triangle.m_PointA) { return true; }
        if (m_PointA == triangle.m_PointB) { return true; }
        if (m_PointA == triangle.m_PointC) { return true; }

        if (m_PointB == triangle.m_PointA) { return true; }
        if (m_PointB == triangle.m_PointB) { return true; }
        if (m_PointB == triangle.m_PointC) { return true; }

        if (m_PointC == triangle.m_PointA) { return true; }
        if (m_PointC == triangle.m_PointB) { return true; }
        if (m_PointC == triangle.m_PointC) { return true; }

        return false;
    }

    // Checks if the slopes are same, greater or less
    // http://myitlearnings.com/checking-collinearity-of-3-points-and-their-orientation/
    // (y2 – y1)/(x2 – x1) = (y3 – y2)/(x3 – x2)
    // == (y2 – y1)*(x3 – x2) = (y3 – y2)*(x2 – x1)
    public static bool IsCounterClockwise(Vector2 A, Vector2 B, Vector2 C)
    {
        return ((B.x - A.x) * (C.y - A.y)) - (C.x - A.x) * (B.y - A.y) > 0;
    }
}

public class DelaunayTriangulation
{
    public static List<Triangle> Triangulate(List<Node> points, float maxX, float maxY)
    {
        if (points.Count < 3)
        {
            throw new Exception("More than 3 Points required");
        }

        // Add Super Triangle Containing All Points, It Is Used Too Start The Algorithm
        // Connect First Point Too Super Triangle And Repeat Till Done Etc.
        Triangle superTriangle = SuperTriangle(points, maxX, maxY);
        List<Triangle> triangles = new List<Triangle>
        {
            superTriangle
        };

        // Loop through each point ignoreing the super triangle, andd add too triangulation
        for (int i = 0; i < points.Count - 3; ++i)
        {
            // Edge buffer Containg Any Edges That Need remaking
            List<Edge> edgeBuffer = new List<Edge>();

            // Find Triangles where Circumcircle overlaps this point
            // Add its edges too the edge buffer for reconstruction later
            // Loop backwards so removal from list doesnt mess up loop
            for (int j = triangles.Count - 1; j >= 0; j--)
            {
                Triangle t = triangles[j];
                if (t.m_Circle.Contain(points[i].m_Position))
                {
                    edgeBuffer.Add(new Edge(t.m_PointA.m_Index, t.m_PointB.m_Index));
                    edgeBuffer.Add(new Edge(t.m_PointB.m_Index, t.m_PointC.m_Index));
                    edgeBuffer.Add(new Edge(t.m_PointC.m_Index, t.m_PointA.m_Index));
                    triangles.RemoveAt(j);
                }
            }

            // remove Duplicate edges, you remove both only keep unique ones
            // we have too loop backwards then loop through already looped portion
            // otherwise it messes up the loop counters and the list resizes dynamically!!!
            for (int j = edgeBuffer.Count - 2; j >= 0; --j)
            {
                for (int k = edgeBuffer.Count - 1; k >= j + 1; --k)
                {
                    if (edgeBuffer[j] == edgeBuffer[k])
                    {
                        edgeBuffer.RemoveAt(k);
                        edgeBuffer.RemoveAt(j);
                        k--; // Dont want to check the one we just checked
                        continue;
                    }
                }
            }

            // Generate new triangles using the edge buffer too fill in holes.
            for (int j = 0; j < edgeBuffer.Count; j++)
            {
                // Just grab an old Non-Duplicate edge and connect too the offending point! simples :D.
                triangles.Add(new Triangle(points[edgeBuffer[j].m_Start], points[edgeBuffer[j].m_End], points[i]));
            }
        }

        // Remove any triangles that attach too the SuperTriangle as these arent required for
        // the final triangulated mesh just temp too start algorithem with a triangle and guarenteed offending point!
        for (int i = triangles.Count - 1; i >= 0; i--)
        {
            if (triangles[i].SharesNode(superTriangle))
            {
                triangles.RemoveAt(i);
            }
        }

        // remove the 3 super triangle nodes
        points.RemoveAt(points.Count - 1);
        points.RemoveAt(points.Count - 1);
        points.RemoveAt(points.Count - 1);

        return triangles;
    }

    private static Triangle SuperTriangle(List<Node> nodes, float maxX, float maxY)
    {
        // Get the longest axis and just make it big, guarntees 
        // surrounded triangle even if it looks ugly.
        float scaledMaxAxis = Mathf.Max(maxX, maxY) * 10;
        nodes.Add(new Node(new Vector2(scaledMaxAxis, -50), 0, nodes.Count));
        nodes.Add(new Node(new Vector2(-50, scaledMaxAxis), 0, nodes.Count));
        nodes.Add(new Node(new Vector2(-scaledMaxAxis, -scaledMaxAxis), 0, nodes.Count));
        return new Triangle(nodes[nodes.Count - 3], nodes[nodes.Count - 2], nodes[nodes.Count - 1]);
    }
}