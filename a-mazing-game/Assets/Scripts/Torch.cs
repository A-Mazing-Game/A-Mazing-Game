using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour
{
    public float range = 0.001f;
    public GameObject torch;
    public Transform fakeParent;
    private Vector3 offset;

    void Start()
    {
        //int layerMask = 3 << 8;
        //layerMask = ~layerMask;
        offset = new Vector3 (0.0f, 1.5f, 0.0f);
    }
    void LateUpdate()
    {
        //RaycastHit hit;
        //var directionOfRay = transform.TransformDirection(Vector3.forward);
        //Debug.DrawRay(transform.position, directionOfRay * range, Color.red, 5f);
        transform.rotation = fakeParent.rotation;
        transform.position = fakeParent.position + offset;
        //transform.position.y = fakeParent.position.y + 2;
        if (Input.GetKeyDown(KeyCode.T))
        { 
            PlaceTorch();
        }
    }


    // Update is called once per frame
    public void PlaceTorch ()
    {
        Debug.Log("Pressed T");
        RaycastHit hit;
        //Debug.DrawRay(transform.position, transform.forward * range, Color.green, 5f);
        if (Physics.Raycast(transform.position, transform.forward, out hit, range))
        {
            
            print("torched");
            Debug.Log(hit.transform.name);
            //Vector3 pos = hit.transform.position;
            Vector3 pos = hit.point;
            pos.y = pos.y + 2;
            Instantiate(torch, hit.point, transform.rotation);
            //torch.transform.LookAt(hit.point + hit.normal);

        }
        

    }
}
