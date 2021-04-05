using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class IndexedPriorityQueueLow
{
    List<float> m_Keys;
    List<int>   m_Heap;
    List<int>   m_IndexToHeap; // Table of index too heap ID's
    int m_Count;
    int m_Max;

    public IndexedPriorityQueueLow(List<float> keys, int maxSize)
    {
        m_Keys = keys;
        m_Count = 0;
        m_Max = maxSize;

        // + 1 as 2/2 is 1 not 0
        m_Heap = new List<int>(m_Max + 1);
        m_IndexToHeap = new List<int>(m_Max + 1);

        for (int i = 0; i < m_Max + 1; i++)
        {
            m_Heap.Add(0);
            m_IndexToHeap.Add(-1);
        }
    }

    private void Swap(int a, int b)
    {
        int temp = m_Heap[a];
        m_Heap[a] = m_Heap[b];
        m_Heap[b] = temp;

        m_IndexToHeap[m_Heap[a]] = a;
        m_IndexToHeap[m_Heap[b]] = b;
    }

    public void Insert(int index)
    {
        if(m_Count >= m_Max + 1) { throw new Exception("Priority Queue Overflow"); }

        ++m_Count;
        m_Heap[m_Count] = index; // set the item too the next avlible poisiton
        m_IndexToHeap[index] = m_Count; // Map the index too its heap position
        ReorderUpwards(m_Count); // Shuffle the index too its correct position
    }

    public int Pop()
    {
        // Swap first and last, reorder and remove
        Swap(1, m_Count);
        ReorderDownwards(1, m_Count - 1);
        return m_Heap[m_Count--];
    }

    public bool Empty() { return m_Count == 0; }

    public void ReorderUpwards(int index)
    {
        // Whilst were not the first node and parent is more than us swap
        while ((index > 1) && m_Keys[m_Heap[(int)(index * 0.5f)]] > m_Keys[m_Heap[index]])
        {
            Swap((int)(index * 0.5f), index);
        }
    }

    public void ReorderDownwards(int index, int heapSize)
    {
        // Keep swapping untill where last node
        while(2 * index <= heapSize)
        {
            int leaf = 2 * index;

            // set leaf too the right node if its less than the left.
            if((leaf < heapSize) && (m_Keys[m_Heap[leaf]] > m_Keys[m_Heap[leaf + 1]]))
            {
                ++leaf;
            }

            // If parent greater than child then swap.
            if(m_Keys[m_Heap[index]] > m_Keys[m_Heap[leaf]])
            {
                Swap(leaf, index);
                index = leaf;
            }
            else
            {
                // Where finished ordering now so break.
                break;
            }
        }
    }

    public void ChangePriority(int index)
    {
        // Just reorder upwards bottom half doesnt matter for popping
        // and will be fixed on next pop
        ReorderUpwards(m_IndexToHeap[index]);
    }
}