using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour,IDamageable
{
    [SerializeField]private float initialHealth = 100f;
    public float health { get; protected set;}

    protected bool dead;

    public event Action onDead;

    protected virtual void Start()
    {
        
    }

    public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        TakeDamage(damage);
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        dead = true;

        if (onDead != null)
        {
            onDead();
        }
        
        Destroy(gameObject);
    }
}
