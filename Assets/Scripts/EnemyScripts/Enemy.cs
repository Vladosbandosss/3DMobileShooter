using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Enemy : LivingEntity
{
    public enum State
    {
        Idle,
        Chasing,
        Attacking
    }
    private State currentState;

    [SerializeField] private ParticleSystem deadEffect;
    private ParticleSystem.MainModule deadEffectMain;

    public static event Action onDeadStatic;

    [SerializeField] private NavMeshAgent pathFinder;

    private Transform target;
    private LivingEntity targetEnetity;
    private Material skinMaterial;
    private Color originalColor;

    [SerializeField] private float refreshAfter = 0.5f;
    [SerializeField] private float attackDistance = 0.5f;

    private float timeBetweenAtttack = 1f;
    private float nextAttackTime = 0f;
    private float myCollisionRadius;
    private float targetCollisionRadius;
    private bool hasTarget;

    [SerializeField] private float damage = 1f;

    [SerializeField] private AudioClip[] hurtClips;
    [SerializeField] private AudioClip[] deathClips;
    [SerializeField] private AudioClip enemyAttackClip;

    private void Awake()
    {
        target = GameObject.FindWithTag(TagManager.PLAYERTAG).transform;

        if (target)
        {
            hasTarget = true;
            targetEnetity = target.GetComponent<LivingEntity>();
            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
        }
        
        //test
         //SetCharacteristic(3.5f,5,3,Color.blue);
    }

    protected override void Start()
    {
        base.Start();

        if (hasTarget)
        {
            currentState = State.Chasing;
            targetEnetity.onDead += OnTargetDead;
            StartCoroutine(nameof(UpdatePath));
        }
    }

    private void Update()
    {
        Debug.Log(targetEnetity.health);
        if (hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                float sqDstToTarget = (target.position - transform.position).sqrMagnitude;
                float playerDist = 4f;

                if (sqDstToTarget < Mathf.Pow(attackDistance + myCollisionRadius + targetCollisionRadius, 2))
                {
                    nextAttackTime = Time.time + timeBetweenAtttack;
                    AudioManager.instance.PlaySound(enemyAttackClip,transform.position);
                    StartCoroutine(nameof(Attack));
                }
            }
        }
    }

    public void SetCharacteristic(float moveSpeed,int hitsToKillPlayer,float enemyHealth,Color skinColor)
    {
        pathFinder.speed = moveSpeed;

        if (hasTarget)
        {
            damage = Mathf.Ceil(targetEnetity.initialHealth / hitsToKillPlayer);
        }

        initialHealth = enemyHealth;

        deadEffectMain = deadEffect.main;
        deadEffectMain.startColor = new Color(skinColor.r, skinColor.g, skinColor.b, 1f);

        skinMaterial = GetComponent<Renderer>().material;
        skinMaterial.color = skinColor;
        originalColor = skinMaterial.color;
    }

    private IEnumerator UpdatePath()
    {
        while (hasTarget)
        {
            if (currentState == State.Chasing)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;

                Vector3 targetPos = target.position -
                                    dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistance / 2f);

                if (!dead)
                {
                    pathFinder.SetDestination(targetPos);
                }
            }
            
            yield return new WaitForSeconds(refreshAfter);
        }
    }

    private void OnTargetDead()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    private IEnumerator Attack()
    {
        currentState = State.Attacking;

        pathFinder.enabled = false;

        Vector3 originalPos = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPos = target.position - dirToTarget * myCollisionRadius;

        float percent = 0f;
        float attackSpeed = 3f;
        
        skinMaterial.color=Color.red;
        bool hasApliedDamage = false;

        while (percent<=1)
        {
            if (percent >= 0.5f && !hasApliedDamage)
            {
                hasApliedDamage = true;
                targetEnetity.TakeDamage(damage);
            }

            percent += Time.deltaTime * attackSpeed;

            float interpolation = (-Mathf.Pow(percent, 2) + percent)*4f;

            transform.position = Vector3.Lerp(originalPos, attackPos, interpolation);

            yield return null;
        }

        skinMaterial.color = originalColor;
        currentState = State.Chasing;
        pathFinder.enabled = true;
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        AudioManager.instance.PlaySound(hurtClips[Random.Range(0,hurtClips.Length)],transform.position);

        if (damage >= health)
        {
            if (onDeadStatic != null)
            {
                onDeadStatic();
            }

            AudioManager.instance.PlaySound(deathClips[Random.Range(0,deathClips.Length)],transform.position);

                GameObject deadParticles = Instantiate(deadEffect.gameObject, hitPoint,
                    Quaternion.FromToRotation(Vector3.forward, hitDirection));
                
                Destroy(deadParticles,deadEffectMain.startLifetimeMultiplier);
        }
        
        base.TakeDamage(damage);
    }
}
