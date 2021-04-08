using UnityEngine;

// Dummy class toos tore access too some stuffs, and do initial position.
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    public MeshFilter   m_MeshFilter;
    public MeshRenderer m_Renderer;
    public MeshCollider m_MeshCollider;
    public int m_ChunkX;
    public int m_ChunkY;

    public void Initialize(int x, int y, Vector2 position)
    {
        m_ChunkX            = x;
        m_ChunkY            = y;
        m_MeshFilter        = GetComponent<MeshFilter>();
        m_Renderer          = GetComponent<MeshRenderer>();
        m_MeshCollider      = GetComponent<MeshCollider>();
        transform.position  = new Vector3(position.x, 0, position.y);
    }
}
