using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This ios bad code, use pooling but had 4 hrs left :/
public class EnemyFactory : MonoBehaviour
{
    public List<Enemy> m_Enemies;
    private Dictionary<string, ObjectPool> m_EnemyMap = new Dictionary<string, ObjectPool>();

    public void Start()
    {
        for (int i = 0; i < m_Enemies.Count; ++i)
        {
            m_EnemyMap.Add(m_Enemies[i].m_CharacterName, new ObjectPool(m_Enemies[i].gameObject, 20));
        }
    }

    // Instantiaites a NEW enemy no pooling urggg :/
    public Enemy CreateEnemy(string name, Vector3 position)
    {
        Enemy enemy = null;
        ObjectPool pool = null;
        if (m_EnemyMap.TryGetValue(name, out pool))
        {
            enemy = pool.GetPooledObject().GetComponent<Enemy>();
            enemy.transform.position = position;
        }

        return enemy;
    }


    public Enemy CreateRandomEnemy()
    {
        if (m_Enemies.Count >= 1)
        {
            return Instantiate(m_Enemies[Random.Range(0, m_Enemies.Count)]);
        }

        return null;
    }

    public void Reset()
    {
        foreach(KeyValuePair<string, ObjectPool> pool in m_EnemyMap)
{
            pool.Value.ResetPool();
        }
    }
}
