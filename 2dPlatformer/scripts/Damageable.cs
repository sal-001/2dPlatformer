using System;
using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class Damageable : MonoBehaviour
{   
    public UnityEvent<int, Vector2> damageableHit;
    public UnityEvent damageableDeath;
    Animator animator;
    [SerializeField]
    private int _maxHealth = 100;
    public int MaxHealth{
        get{
                return _maxHealth;
        } 
         set
        {
                _maxHealth = value;
        }
     }


    [SerializeField]
    private int _health = 100;
    public int Health
    {
        get
        {
            return _health;
        }
        set
        {
                _health = value;

                // if health drops below 0, character is no longer alive
                if(_health <= 0)
                {
                    IsAlive = false; 
                }
        }
    }
    [SerializeField]
    private bool _isAlive = true;
    [SerializeField]
    private bool isInvincible = false;
   
    private float timeSinceHit = 0;
    public float invincibilityTime = 0.25f;

    public bool IsAlive 
    {
        get{
            return _isAlive ;
            }
            set
            {
                _isAlive = value;
                animator.SetBool(AnimationStrings.isAlive, value);
                Debug.Log("IsAlive Set" + value);
                
                if (value == false)
                {
                    damageableDeath.Invoke();
                }

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



    private void Awake()

    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(isInvincible)
        {
            if(timeSinceHit > invincibilityTime)

            {   // Remove invincibility
                isInvincible = false;
                timeSinceHit = 0;
            }
            timeSinceHit += Time.deltaTime;
        
        }
        
    }


    // Returns if damage occured
   public bool Hit(int damage, Vector2 knockback)
   {
    if(IsAlive && !isInvincible)
    {
        Health -= damage;
        isInvincible = true;

        // applys knockback when hit occurs 
        animator.SetTrigger(AnimationStrings.hitTrigger);
        LockVelocity = true;
        damageableHit?.Invoke(damage, knockback);
        CharacterEvents.characterDamaged.Invoke(gameObject, damage);



        return true;
    }
    // Unable to hit
    return false;

   }
   // Returns if character was healed or not
   public bool Heal(int healthRestore)
   {
    if(IsAlive && Health < MaxHealth)
    {
        int maxHeal = Mathf.Max(MaxHealth - Health, 0);
        int actualHeal = Mathf.Min(maxHeal, healthRestore);
        Health += actualHeal;
        CharacterEvents.characterHealed(gameObject, actualHeal);
        return true;


    }

    return false;
   }
}
