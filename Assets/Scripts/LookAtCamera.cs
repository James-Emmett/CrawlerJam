using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forward = (GameManager.Instance.Player.transform.position - transform.position).normalized;
        forward.y = 0;
        if (forward == Vector3.zero) { forward = Vector3.forward; }
        transform.rotation = Quaternion.LookRotation(forward);
    }
}
