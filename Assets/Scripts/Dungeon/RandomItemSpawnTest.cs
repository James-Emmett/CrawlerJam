using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomItemSpawnTest : MonoBehaviour
{
    bool hasRan = false;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(hasRan == false)
        {
            ItemFactory factory = GameManager.Instance.ItemFactory;
            int amount = Random.Range(10, 40);
            for (int i = 0; i < amount; i++)
            {
                factory.InstantiateWorldObjectExisting(factory.GetRandomConsumeable(), new Vector3(Random.Range(-20, 20), 1f, Random.Range(-20, 20)), Vector3.zero);
                factory.InstantiateWorldObjectExisting(factory.GetRandomWeapon(), new Vector3(Random.Range(-20, 20), 1f, Random.Range(-20, 20)), Vector3.zero);
            }
            hasRan = true;
        }
    }
}
