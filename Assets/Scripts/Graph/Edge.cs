using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Edge
{
    // Hopefully these nodes are just pointers? not sure how C# works
    // in that sense???
    public int  m_Start;
    public int  m_End;
    public int  m_Cost = 1;

    public Edge(int start, int end, int cost = 1)
    {
        m_Start = start;
        m_End   = end;
        m_Cost  = cost;
    }

    public static bool operator ==(Edge A, Edge B)
    {
        return  ((A.m_Start == B.m_Start && A.m_End == B.m_End) ||
                (A.m_Start == B.m_End && A.m_End == B.m_Start)) && A.m_Cost == B.m_Cost;
    }

    public static bool operator !=(Edge A, Edge B)
    {
        return  A.m_Start != B.m_Start && A.m_End != B.m_End ||
                A.m_Start != B.m_End && A.m_End != B.m_Start;
    }

    public override bool Equals(object obj)
    {
        return this == (Edge)obj;
    }

    public override int GetHashCode()
    {
        return m_Start.GetHashCode() ^ m_End.GetHashCode();
    }
}