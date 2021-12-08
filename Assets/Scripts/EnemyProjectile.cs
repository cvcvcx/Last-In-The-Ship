using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    
    private MovementTransform movement;
    private float projectileDistance = 30;
    private int damage = 20;
    public void Setup(Vector3 position)
    {
        movement = GetComponent<MovementTransform>();
        StartCoroutine("OnMove", position);
    }

    private IEnumerator OnMove(Vector3 targetPosition)
    {
        Vector3 start = transform.position;
        movement.MoveTo((targetPosition - transform.position).normalized*0.5f);
        while (true)
        {
            if (Vector3.Distance(transform.position, start) >= projectileDistance)
            {
                DestoryProjectile();
                yield break;
            }
            yield return null;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().TakeDamage(damage);

            DestoryProjectile();
        }
     }
    public void DestoryProjectile()
    {
        EnemyFSM.enemyBullets.Remove(gameObject);
        
        Destroy(gameObject);
    }
}
