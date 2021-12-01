using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    private Animator animator;
    public Transform target;
    public Vector3 relativeVec;
    public Vector3 offsetVec;
    public Transform spine;
    public Quaternion oriSpine;
    

    private EnemyFSM enemyFSM;
    public bool lookAt;
    private void Awake()
    {
        oriSpine = spine.rotation;
        animator = GetComponentInChildren<Animator>();
        enemyFSM = GetComponentInParent<EnemyFSM>();
        
        lookAt = false;
    }

    public float MoveSpeed
    {
        set => animator.SetFloat("movementSpeed", value);
        get => animator.GetFloat("movementSpeed");
    }
    


    public void SpineLookAt()
    {       

        spine.LookAt(target.position);
        spine.rotation = spine.rotation * Quaternion.Euler(relativeVec);
    }
 
    public void Wander()
    {
        spine.rotation = spine.rotation * Quaternion.Euler(offsetVec);
        
    }
 
    public void Attack()
    {
        animator.SetTrigger("Attack");

    } 

    public void Play(string stateName, int layer, float normalizedTime)
    {
        animator.CrossFade(stateName,0.3f, layer, normalizedTime);
    }
}
