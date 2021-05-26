using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTeleporter : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform player;
    public Transform receiver;
    private Vector3 AutzenLocation;
    public GameObject duck;
    public int test;
    public GameObject healthBar;
    private bool playerIsOverlapping = false;
    // Update is called once per frame

    void Awake()
    {
        healthBar = GameObject.Find("Duck Health").gameObject;
        duck = GameObject.Find("Duck").gameObject;
        player = GameObject.Find("Player").transform;
        AutzenLocation = new Vector3(-346f, 0.5f, 32f);
        healthBar.SetActive(false);
        duck.SetActive(false);
    }
    void Update()
    {
        if (playerIsOverlapping)
        {
            Debug.Log("In portal");
            print("portal touch");
            //Vector3 portalToPlayer = player.position - transform.position;
            //float dotProduct = Vector3.Dot(transform.up, portalToPlayer);

            /*if (dotProduct < 0f)
            {
                float rotDiff = -Quaternion.Angle(transform.rotation, receiver.rotation);
                rotDiff = rotDiff + 180;
                player.Rotate(Vector3.up, rotDiff);

                Vector3 positionOffset = Quaternion.Euler(0f, rotDiff, 0f) * portalToPlayer;
                player.position = receiver.position + positionOffset;
                playerIsOverlapping = false;
            }*/
            duck.SetActive(true);
            healthBar.SetActive(true);
            player.position = AutzenLocation;
            playerIsOverlapping = false;
        }
        
    }

    public void SetReceiver(Transform receive)
    {
        receiver = receive;
    }

    public void SetPlayer(Transform playerPos)
    {
        player = playerPos;
    }

    void OnTriggerEnter(Collider other)
    {
        print("touch");
        if (other.tag == "Player")
        {
            playerIsOverlapping = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerIsOverlapping = false;
        }
    }
}
