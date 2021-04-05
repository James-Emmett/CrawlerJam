using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseLook : MonoBehaviour
{
    public Player m_Player;
    private Quaternion m_TargetRotation;

    float x = 0;
    float y = 0;

    private void Start()
    {
        float x = transform.eulerAngles.x;
        float y = transform.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        x += -Input.GetAxis("Mouse Y");
        y += Input.GetAxis("Mouse X");
        x = Mathf.Clamp(x, -55, 45);

        // Apply 90 degree keyboard rotation
        if (m_Player.IsMoving() == false)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                y = Quaternion.LookRotation(GetCardinalForward()).eulerAngles.y;
                y += -90;
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                y = Quaternion.LookRotation(GetCardinalForward()).eulerAngles.y;
                y += 90;
            }
        }

        transform.eulerAngles = new Vector3(x,y,0);

        // Sync camera too player
        transform.position = m_Player.transform.position;

        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out hit))
        {
            if(hit.transform.tag == "Item")
            {
                // tell game manager to display pick up message?
                if(Input.GetKey(KeyCode.R) && m_Player.m_Inventory.HasFreSpace())
                {
                   hit.transform.GetComponent<ItemWorld>().OnPickUp();
                }
            }
        }
    }

    public Vector3 GetCardinalForward()
    {
        // align ourself with the largets of the x,y axis in either direction
        Vector3 forward = transform.forward.normalized;

        // Lock the rotation too cardinal direction?
        if (Mathf.Abs(forward.x) > Mathf.Abs(forward.z))
        {
            forward.z = 0; forward.y = 0;
            forward.x = 1 * Mathf.Sign(forward.x);
        }
        else
        {
            forward.x = 0; forward.y = 0;
            forward.z = 1 * Mathf.Sign(forward.z);
        }

        return forward;
    }
}
