using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float speed = 0;
    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    private Rigidbody rb;
    private int count;
    private float movementX;
    private float movementY;
    private int jumps;
    public SphereCollider col;
    public float jumpForce = 7;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        SetCountText();
        winTextObject.SetActive(false);
        jumps = 0;
        col = GetComponent<SphereCollider>();
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        if (count >= 12)
        {
            winTextObject.SetActive(true);
        }
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);
        
        if (Input.GetKeyDown("space") && jumps < 2)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumps++;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            Debug.Log("Picked up cube");
            count++;
            SetCountText();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            Debug.Log(("test"));
            jumps = 0;
        }
    }

    // Update is called once per frame
    // void Update()
    // {
    //     
    // }
}
