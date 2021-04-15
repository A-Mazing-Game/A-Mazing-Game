using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Hearing : MonoBehaviour
{
    public Transform player;
    
    private SphereCollider hearingVolume;
    private NavMeshAgent _ghostNav;

    private Vector2 _playerVelocity;
    private Vector2 _prevPos;
    private Vector3 _angles;
    private float _playerDistanceFraction;
    private float _waitTime = 3.0f;
    private float _timer;
    private float _maxTurnSpeed = 3.0f;
    private float _minTurnSpeed = 0.1f;
    private float _interpolatedSpeed;
    private float _angle;
    private bool _IsPlayerInRange;

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == player)
        {
            _IsPlayerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == player)
        {
            _IsPlayerInRange = false;
        }
    }
    
    void Start()
    {
        _prevPos = new Vector2(player.position.x, player.position.z);
        hearingVolume = GetComponent<SphereCollider>();
        _ghostNav = GetComponent<NavMeshAgent>();
        _angles = new Vector3(0.0f,1.0f,0.0f);
    }
    
    void FixedUpdate()
    {
        // Add to the running timer for the ghost's pause duration
        _timer += Time.deltaTime;
        // Calculate the current player velocity
        _playerVelocity = (new Vector2(player.position.x, player.position.z) - _prevPos)/Time.deltaTime;
        _prevPos = new Vector2(player.position.x, player.position.z);
        // Determine if the player is in the hearing range of the ghost
        if (_IsPlayerInRange)
        {
            // If so, determine the exact fraction of the players distance from the edge of the ghost's hearing volume to
            // the ghost itself
            _playerDistanceFraction =
                1.2f - (Vector2.Distance(player.position, transform.position) / hearingVolume.radius);
            // Interpolate the turning speed of the ghost corresponding to the player distance fraction
            // (The closer the player is to the ghost, the faster the ghost will turn around)
            _interpolatedSpeed =
                (1.0f - _playerDistanceFraction) * _minTurnSpeed + _playerDistanceFraction * _maxTurnSpeed;
        }
        
        // Check if the player is in the hearing range and if the player is currently moving 
        if (_IsPlayerInRange && _playerVelocity != Vector2.zero)
        {
            // Stops the NavMeshAgent movement
            // (simulates the ghost hearing the player and stopping to listen)
            _ghostNav.isStopped = true;

            // Calculate the unit direction to the player
            Vector3 direction = player.position - transform.position;
            direction.Normalize();

            // Calculates the relative angle between the player and the ghost
            _angle = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(Vector3.forward, direction));

            // Rotates the ghost over some time using Quaternion interpolation until the dot product reaches ~ 0.8
            // (simulates the ghost hearing the player and turning in the direction of the sound)
            float dotProd = Vector3.Dot(Vector3.forward, direction);
            if (dotProd < 0.8f)
            {
                Quaternion rot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, _interpolatedSpeed * Time.deltaTime);
            }
            // This next part is 100% added in here only as a way for me to implement the dot product like we did in class.
            // There are many easier ways to accomplish the same desired effect...
            else
            {
                // eulerAngles calculation takes over and tracks the player's movement, resets the transform.rotation
                transform.rotation = Quaternion.identity;
                _angles.y = _angle;
                transform.eulerAngles = _angles;
            }
            
            // Negates the angle if the angle exceeds a certain threshold
            Vector3 cross = Vector3.Cross(Vector3.forward, direction);
            if (cross.y < 0.0f)
            {
                _angle = -_angle;
            }
        }

        // Check if the player has been stationary for long enough
        if (_timer > _waitTime)
        {
            // If so, unfreeze the ghost and continue its AI navigation
            _timer -= _waitTime;
            _ghostNav.isStopped = false;
        }
        
    }
}
