using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { North, East, South, West}
public class Dungeon : MonoBehaviour
{
    public Player           m_Player;
    public DungeonGenerator m_DungeonGenerator = new DungeonGenerator();
    public DungeonMesh      m_DungeonMesh;
    public GameObject       m_Exit = null;
    public GameObject       m_TorchPrefab = null;
    public Grid2D           m_Grid;
    public int              m_Width;
    public int              m_Height;

    public EnemyFactory     m_EnemyFactory;
    private TilePathFinder  m_PathFinder;
    private ObjectPool      m_TorchPool;
    private List<GameObject> m_PotentialObjects;


    public TilePathFinder TilePathFinder
    {
        get { return m_PathFinder; }
    }

    private void Start()
    {
        m_TorchPool = new ObjectPool(m_TorchPrefab, 20);
        GenerateNewDungeon();
    }

    public void GenerateNewDungeon()
    {
        if(m_Grid != null) { m_Grid = null; }
        m_Grid = new Grid2D(m_Width, m_Height, 2);
        m_DungeonGenerator.Generate(m_Grid, 4, 4, 8, 8, 250);
        m_DungeonMesh.Create(m_Grid, 8);
        m_PathFinder = new TilePathFinder(m_Grid);

        PlaceTorchesAndObjects();
        SetPlayerSpawn();
        CreateExit();
        PlaceEnemies();
    }

    private void CreateExit()
    {
        if(m_Exit == null)
        {
            throw new Exception("Forgot tooa dd exit dingy bat.");
        }

        m_Exit.transform.position = m_Grid.TileToWorldPosition(m_DungeonGenerator.m_Rooms[m_DungeonGenerator.m_Rooms.Count - 1].GetCenter());

    }

    private void SetPlayerSpawn()
    {
        m_Player.SetPosition(m_Grid.TileToWorldPosition(m_DungeonGenerator.m_Rooms[0].GetCenter()) + new Vector3(0,1,0));
        m_PathFinder.FindPath(m_Grid.WorldToTilePosition(m_Player.transform.position.x, m_Player.transform.position.z));
    }

    // place torches and objects in rooms
    public void PlaceTorchesAndObjects()
    {
        m_TorchPool.ResetPool();

        if (m_PotentialObjects != null)
        {
            for (int i = 0; i < m_PotentialObjects.Count; ++i)
            {
                if (m_PotentialObjects[i] != null)
                {
                    GameObject.Destroy(m_PotentialObjects[i]);
                }
            }
        }
        else
        {
            m_PotentialObjects = new List<GameObject>();
        }

        foreach (Room room in m_DungeonGenerator.m_Rooms)
        {
            int count = UnityEngine.Random.Range(1, 4);

            for (int i = 0; i < count; ++i)
            {
                // Get random point in room
                Vector2 point = new Vector2(UnityEngine.Random.Range(room.Left(), room.Right()), UnityEngine.Random.Range(room.Bottom(), room.Top()));
                GameManager.Instance.ItemFactory.InstantiateWorldObjectExisting(GameManager.Instance.ItemFactory.GetRandomConsumeable(), m_Grid.TileToWorldPosition(point.x, point.y) + new Vector3(0,1,0), Vector3.zero);
            }

            count = UnityEngine.Random.Range(0, 2);
            for (int i = 0; i < count; ++i)
            {
                // Get random point in room
                Vector2 point = new Vector2(UnityEngine.Random.Range(room.Left(), room.Right()), UnityEngine.Random.Range(room.Bottom(), room.Top()));
                GameManager.Instance.ItemFactory.InstantiateWorldObjectExisting(GameManager.Instance.ItemFactory.GetRandomWeapon(), m_Grid.TileToWorldPosition(point.x, point.y) + new Vector3(0, 1, 0), Vector3.zero);
            }
        }

        foreach (Room room in m_DungeonGenerator.m_Rooms)
        {
            int quaterWidth  = (int)(room.m_Width * 0.25f);
            int quaterHeight = (int)(room.m_Height * 0.25f);

            // Place Northern Torchs
            PlaceSingleTorch(new Vector2(room.Left() + quaterWidth, room.Top()), Direction.North);
            PlaceSingleTorch(new Vector2(room.Right() - quaterWidth, room.Top()), Direction.North);

            // Place Eastern Torches
            PlaceSingleTorch(new Vector2(room.Right(), room.Bottom() + quaterHeight), Direction.East);
            PlaceSingleTorch(new Vector2(room.Right(), room.Top() - quaterHeight), Direction.East);

            // Place Southern Torches
            PlaceSingleTorch(new Vector2(room.Left() + quaterWidth, room.Bottom()), Direction.South);
            PlaceSingleTorch(new Vector2(room.Right() - quaterWidth, room.Bottom()), Direction.South);

            // Place Western Torches
            PlaceSingleTorch(new Vector2(room.Left(), room.Bottom() + quaterHeight), Direction.West);
            PlaceSingleTorch(new Vector2(room.Left(), room.Top() - quaterHeight), Direction.West);
        }
    }

    public void PlaceSingleTorch(Vector2 tilePosition, Direction direction)
    {
        if (m_Grid.GetTile((int)tilePosition.x, (int)tilePosition.y).m_Type == TileType.Room)
        {
            float rot = 0;

            if(direction == Direction.North)
            {
                rot = 90;
                if(m_Grid.GetTile((int)tilePosition.x, (int)tilePosition.y + 1).m_Type == TileType.Room)
                {
                    return;
                }
            }

            if (direction == Direction.East)
            {
                rot = 180;
                if (m_Grid.GetTile((int)tilePosition.x + 1, (int)tilePosition.y).m_Type == TileType.Room)
                {
                    return;
                }
            }


            if (direction == Direction.South)
            {
                rot = 270;
                if (m_Grid.GetTile((int)tilePosition.x, (int)tilePosition.y - 1).m_Type == TileType.Room)
                {
                    return;
                }
            }

            if (direction == Direction.West)
            {
                rot = 0;
                if (m_Grid.GetTile((int)tilePosition.x - 1, (int)tilePosition.y).m_Type == TileType.Room)
                {
                    return;
                }
            }


            GameObject torch = m_TorchPool.GetPooledObject();
            torch.transform.position = m_Grid.TileToWorldPosition(tilePosition.x, tilePosition.y);
            torch.transform.rotation = Quaternion.Euler(0, rot, 0);
        }
    }

    public void PlaceEnemies()
    {
        m_EnemyFactory.Reset();

        for (int i = 1; i < m_DungeonGenerator.m_Rooms.Count; ++i)
        {
            Room room = m_DungeonGenerator.m_Rooms[i];
            int count = UnityEngine.Random.Range(1, 2);

            for (int j = 0; j < count; ++j)
            {
                // Get random point in room
                Vector2 point = new Vector2(UnityEngine.Random.Range(room.Left(), room.Right()), UnityEngine.Random.Range(room.Bottom(), room.Top()));
                Enemy enemy = m_EnemyFactory.CreateRandomEnemy();
                enemy.SetPosition(m_Grid.TileToWorldPosition(point));
                m_Grid.SetTileBlockedFromWorld(enemy.transform.position.x, enemy.transform.position.z, true);
            }
        }
    }

    public Vector2 GetPlayerTile()
    {
        return m_Grid.WorldToTilePosition(m_Player.transform.position.x, m_Player.transform.position.z);
    }
}