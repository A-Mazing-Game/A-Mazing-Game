using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float turnSpeed = 20f;
    public ParticleSystem dust;
    
    Vector3 m_Movement;
    Quaternion m_rotation = Quaternion.identity;
    Animator m_Animator;
    Rigidbody m_rigidbody;
    AudioSource m_AudioSource;


    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_AudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");  // a or d key left arrow / right arrow
        float vertical = Input.GetAxis("Vertical"); // w or s key, up or down arrow

        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);

        bool isWalking = hasHorizontalInput || hasVerticalInput;
        m_Animator.SetBool("IsWalking", isWalking);

        if (isWalking)
        {
            
            m_rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX |
                                      RigidbodyConstraints.FreezeRotationY;
            dust.Play();
            if (!m_AudioSource.isPlaying)
            {
                m_AudioSource.Play();
            }
        }
        else
        {
            m_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            m_AudioSource.Stop();
        }

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement,
            turnSpeed * Time.deltaTime, 0f);
        m_rotation = Quaternion.LookRotation(desiredForward);
    }

    private void OnAnimatorMove()
    {
        m_rigidbody.MovePosition(m_rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude);
        m_rigidbody.MoveRotation(m_rotation);
    }

}
