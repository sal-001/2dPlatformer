using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class FlyingEye : MonoBehaviour
{
    public float flightSpeed = 2f;
    public float waypointReachedDistance = 0.1f;
    public DetectionZone biteDetectionZone;
    public Collider2D deathCollider;
    public List<Transform> waypoints;
    


    Animator animator;
    Rigidbody2D rb;
    Damageable damageable;
    Transform nextWaypoint;


    int waypointNum = 0;
    public bool _hasTarget = false;
   

    public bool HasTarget { 
        get { return _hasTarget;} 
        
        private set
    {
        _hasTarget = value;
        animator.SetBool(AnimationStrings.hasTarget, value);
    }
     }
     public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }
    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        damageable = GetComponent<Damageable>();
    
    }
    private void Start()
    {
        nextWaypoint = waypoints[waypointNum];
    }
    private void OnEnable()
    {
        damageable.damageableDeath.AddListener(OnDeath);
    }
    
    // Update is called once per frame
    void Update()
    {
        HasTarget = biteDetectionZone.detectedColliders.Count > 0 ;
    }
    private void FixedUpdate()
    {
        if(damageable.IsAlive)
        {
            if(CanMove)
            {
                Flight();
            } 
            else
            {
                rb.velocity = Vector3.zero;           
             }  
        }

            
    }

    private void Flight()
    {
        // Fly to next waypoint
        Vector2 directionToWaypoint = (nextWaypoint.position - transform.position).normalized;
        // Check if we have reached the waypoint
        float distance = Vector2.Distance(nextWaypoint.position, transform.position);

        rb.velocity = directionToWaypoint * flightSpeed;
        updateDirection();
        // See if we need to switch waypoints

        if(distance <= waypointReachedDistance)
        {
            // Switch to next waypoint
            waypointNum++;

            if(waypointNum >= waypoints.Count)
            {
                // Loop back to original waypoint
                waypointNum = 0;
            }
        
            nextWaypoint = waypoints[waypointNum];
        }
    }

    private void updateDirection()
    {
        Vector3 locScale = transform.localScale;
        if(transform.localScale.x > 0)
        {
             // facing right
             if (rb.velocity.x < 0)
             {
                transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z); 
             }
        }
           
        else
        {
            // facing left
        if (rb.velocity.x > 0)
             {
                transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z);
             }
        }
    }
    public void OnDeath()
    {
        // Dead flyer falls to ground
            rb.gravityScale = 2f;
            rb.velocity = new Vector2(0, rb.velocity.y);
            deathCollider.enabled = true;
    }
 }

