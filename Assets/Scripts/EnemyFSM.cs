﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum EnemyState { None= -1, Idle =0, Wander,Pursuit,Attack,}
public class EnemyFSM : MonoBehaviour
{
    public static List<GameObject> enemyBullets = new List<GameObject>();

    [Header("Audio Clips")]    
    [SerializeField]
    public AudioClip audioClipFire;
    [SerializeField]
    public AudioClip attackedClip;
    [SerializeField]
    public AudioClip deadClip;


    [Header("Pursuit")]
    [SerializeField]
    private float targetRecognitionRange = 8;
    [SerializeField]
    private float pursuitLimitRange = 10;
    private float rotateSpeed = 10;

    [Header("Attack")]
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private Transform projectileSpawnPoint;
    [SerializeField]
    private float attackRange = 7;
    [SerializeField]
    private float attackRate = 1;

    private EnemyState enemyState = EnemyState.None;
    private float lastAttackTime = 0;

    [SerializeField]
    private SkinnedMeshRenderer myMesh;

    private Status status;
    private NavMeshAgent navMeshAgent;
    private Transform target;
    private EnemyMemoryPool enemyMemoryPool;
    private EnemyAnimatorController animator;
    private AudioSource audioSource;    
    private bool lookAt=false;
    public EnemyState EnemyState{ get;private set; }

    //private void Awake()
    public void Setup(Transform target,EnemyMemoryPool enemyMemoryPool)
    {
        status = GetComponent<Status>();
        animator = GetComponentInChildren<EnemyAnimatorController>();
        animator.target = target;        
        navMeshAgent = GetComponent<NavMeshAgent>();
        this.target = target;        
        this.enemyMemoryPool = enemyMemoryPool;
        audioSource = GetComponent<AudioSource>();

        navMeshAgent.updateRotation = false;
    }
    
    private void LateUpdate()
    {
        if (lookAt)
            animator.SpineLookAt();
        else
            animator.Wander();


    }
    private void OnEnable()
    {
        ChangeState(EnemyState.Idle);
    }
    private void OnDisable()
    {
        StopCoroutine(enemyState.ToString());
        enemyState = EnemyState.None;
    }

    public void ChangeState(EnemyState newState)
    {
        if (enemyState == newState) return;
        StopCoroutine(enemyState.ToString());
        enemyState = newState;
        StartCoroutine(enemyState.ToString());
        
    }

    private IEnumerator Idle()
    {
        StartCoroutine("AutoChangeFromIdleToWander");
        while (true)
        {
            CalculateDistanceToTargetAndSelectState();
            yield return null;
        }
    }

    private IEnumerator AutoChangeFromIdleToWander()
    {
        int changeTime = Random.Range(1, 5);
        yield return new WaitForSeconds(changeTime);

        ChangeState(EnemyState.Wander);
    }
    private IEnumerator Wander()
    {
        float currentTime = 0;
        float maxTime = 10;
        
        navMeshAgent.speed = status.WalkSpeed;
        animator.MoveSpeed = 0.5f;        
        if (audioSource.isPlaying==false)
        {
            audioSource.loop = true;
            audioSource.Play();
        }
        navMeshAgent.SetDestination(CalculateWanderPosition());        
        Vector3 to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
        transform.rotation = Quaternion.LookRotation(to - from);
        while (true)
        {
            currentTime += Time.deltaTime;

            to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
            from =  new Vector3(transform.position.x, 0, transform.position.z);
            
            if ((to - from).sqrMagnitude < 0.01f || currentTime >= maxTime) 
            {
                ChangeState(EnemyState.Idle);
            }
            CalculateDistanceToTargetAndSelectState();
            yield return null;
        }
    }
    

    private Vector3 CalculateWanderPosition()
    {
        float wanderRadius = 10;
        int wanderJitter = 0;
        int wanderJitterMin = 0;
        int wanderJitterMax = 360;

        Vector3 rangePosition = Vector3.zero;
        Vector3 rangeScale = Vector3.one * 100.0f;

        wanderJitter = Random.Range(wanderJitterMin, wanderJitterMax);
        Vector3 targetPosition = transform.position + SetAngle(wanderRadius, wanderJitter);

        targetPosition.x = Mathf.Clamp(targetPosition.x, rangePosition.x - rangeScale.x * 0.5f, rangePosition.x + rangeScale.x * 0.5f);
        targetPosition.y = 0.0f;
        targetPosition.z = Mathf.Clamp(targetPosition.z, rangePosition.z - rangeScale.z * 0.5f, rangePosition.z + rangeScale.z * 0.5f);
        return targetPosition;
        
    }

    private Vector3 SetAngle(float radius,int angle) 
    {
        Vector3 position = Vector3.zero;

        position.x = Mathf.Cos(angle) * radius;
        position.z = Mathf.Sin(angle) * radius;
        return position;
    }
    private IEnumerator Pursuit()
    {
        while (true)
            
        {
            navMeshAgent.speed = status.RunSpeed;
            animator.MoveSpeed = 1.0f;
            navMeshAgent.SetDestination(target.position);
            LookRotationToTarget();
            CalculateDistanceToTargetAndSelectState();
            yield return null;
        }
    }
    private IEnumerator Attack()
    {
        navMeshAgent.ResetPath();
        animator.MoveSpeed = 0.0f;
        navMeshAgent.speed = 0.0f;
        while (true)
        {

            

            CalculateDistanceToTargetAndSelectState();
            if(Time.time - lastAttackTime > attackRate)
            {
                lastAttackTime = Time.time;
                animator.Attack();
                LookRotationToTarget();
                GameObject clone = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                enemyBullets.Add(clone);
                audioSource.volume = 0.25f;
                audioSource.PlayOneShot(audioClipFire);
                
                clone.GetComponent<EnemyProjectile>().Setup(target.position);
            }
            yield return null;
        }
    }
    private void LookRotationToTarget()
    {
        Vector3 to = new Vector3(target.position.x, 0, target.position.z);
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
        transform.rotation =Quaternion.LookRotation(to - from);

        //        Quaternion rotation = Quaternion.LookRotation(to - from);
        //      transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.01f);
    }
    private void CalculateDistanceToTargetAndSelectState()
    {
        if (target == null) return;
        float distance = Vector3.Distance(target.position, transform.position);
        if(distance<= attackRange)
        {
            lookAt = true;
            ChangeState(EnemyState.Attack);
        } 
        else if(distance<= targetRecognitionRange)
        {
            lookAt = true;
            ChangeState(EnemyState.Pursuit);
        }
        else if (distance >= pursuitLimitRange)
        {
            lookAt = false;
            ChangeState(EnemyState.Wander);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, navMeshAgent.destination - transform.position);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetRecognitionRange);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pursuitLimitRange); 

        Gizmos.color = new Color(0.39f,0.04f,0.04f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    public void TakeDamage(int damage)
    {
        audioSource.volume = 1.0f;
        bool isDie = status.DecreaseHP(damage);
        audioSource.PlayOneShot(attackedClip);
        if (isDie== true)
        {            
            audioSource.PlayOneShot(deadClip);
            enemyMemoryPool.DestroyEnemy(gameObject);
        }
    }
   
}
