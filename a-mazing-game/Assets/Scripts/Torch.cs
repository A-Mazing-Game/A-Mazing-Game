using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour
{
    public float range = 10;
    public GameObject torch;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            PlaceTorch();
        }
    }


    // Update is called once per frame
    public void PlaceTorch ()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position,transform.forward, out hit, range))
        {
            print("torched");
            Debug.Log("idk");
            //Vector3 pos = hit.transform.position;
            Instantiate(torch, hit.point, Quaternion.LookRotation(hit.normal));
        }
        

    }
}
