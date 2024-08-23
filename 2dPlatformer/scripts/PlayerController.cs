using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof (Damageable))]
public class PlayerController : MonoBehaviour
{

    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float airWalkSpeed = 3f;
    public float jumpImpulse = 10f;
    Vector2 moveInput;
    TouchingDirections touchingDirections;
    Damageable damageable;
   
  

    public float CurrentMoveSpeed { get
        {   
            if(CanMove) 
            { 
                if(IsMoving && !touchingDirections.isOnWall)
            {
                    if(touchingDirections.isGrounded)

            {
                        if(IsRunning)
                {
                             return runSpeed;
                }
                       else

                {
                             return walkSpeed;
                }
            }      
             else
            {
                //Air STate
                        return airWalkSpeed;
            }
            
            }   else
            {
                // Idle speed 0
                    return 0;
            }

            
           
        }  else
        {  // Movement locked
            return 0;
        }
        }

    }

    [SerializeField]
    private bool _isMoving = false;
    public bool IsMoving { get
    {
        return _isMoving;
    } 
    private set
     
    {

        _isMoving = value;
        animator.SetBool(AnimationStrings.isMoving, value);
        touchingDirections = GetComponent<TouchingDirections>();
    }
}

    [SerializeField]
    private bool _isRunning = false;
    
    public bool IsRunning 
    {
        get
        {
            return _isRunning;
        }
        set
        {
            _isRunning = value;
            animator.SetBool(AnimationStrings.isRunning, value);
        }
    }
    
    public bool _isFacingRight = true;


    public bool IsFacingRight { get { return _isFacingRight;  }   private set{
        // Flip only if value is new

        if(_isFacingRight != value)

        {
            //Flip the local scale to make the player face the opposite direction
            transform.localScale *= new Vector2(-1, 1);
        }


        _isFacingRight = value;

    } }
    
    public bool CanMove { get 
    {
        return animator.GetBool(AnimationStrings.canMove);
        }
    }
    
    public bool IsAlive 
    {
        get
        {
            return animator.GetBool(AnimationStrings.isAlive);       
        }
        
    }

    public bool LockVelocity { get
    {
        return animator.GetBool(AnimationStrings.lockVelocity);
    }
    set
    {
        animator.SetBool(AnimationStrings.lockVelocity, value);
    }
     }

    Rigidbody2D rb;
    Animator animator;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();
    }

  
    private void FixedUpdate()
    {
        if(!damageable.LockVelocity)
             rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);

        animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);
    }
    // walk speed may need deltatime /\
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if(IsAlive)
        {
            IsMoving = moveInput != Vector2.zero;
            SetFacingDirection(moveInput);
        }else
        {
            IsMoving = false;
        }

        
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
       if(moveInput.x > 0 && !IsFacingRight)
       {
            // Face the right
            IsFacingRight = true;
       } else if (moveInput.x < 0 && IsFacingRight)
       {
        // Face the left
            IsFacingRight = false;
       }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsRunning = true;
           
         } else if(context.canceled)
            {
                IsRunning  = false;
            }
        
    }
    public void OnJump(InputAction.CallbackContext context)
    {   // ToDO Check if alive 
        if (context.started && touchingDirections.isGrounded && CanMove)
        {
            animator.SetTrigger(AnimationStrings.jumpTrigger);
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
        }
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            animator.SetTrigger(AnimationStrings.attackTrigger);
        }
    }

      public void OnRangedAttack(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            animator.SetTrigger(AnimationStrings.rangedAttackTrigger);
        }
    }
        public void onHit(int damage, Vector2 knockback)
        {   
           
            rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
        }
}
