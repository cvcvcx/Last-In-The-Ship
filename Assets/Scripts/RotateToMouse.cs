using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToMouse : MonoBehaviour
{
    [SerializeField]
    private float rotCamXAxisSpeed = 5;
    [SerializeField]
    private float rotCamYAxisSpeed = 3;
    private float limitMinX = -80;
    private float limitMaxX = 50;
    private float eulerAngleX;
    private float eulerAngleY;
    public LayerMask whatIsTarget; // 추적 대상 레이어 
    PlayerController playerController;
    public void UpdateRotate(float mouseX, float mouseY)
    {



        eulerAngleY += mouseX * rotCamYAxisSpeed;
        eulerAngleX -= mouseY * rotCamXAxisSpeed;

        eulerAngleX = ClampAngle(eulerAngleX, limitMinX, limitMaxX);
        if (playerController.IsSlowMode)
            transform.localRotation = Quaternion.Euler(eulerAngleX, eulerAngleY, 0);

        if (playerController.IsAutoAim == false)
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(eulerAngleX, eulerAngleY, 0), Time.deltaTime * 20f);
        else
        {

            if (EnemyMemoryPool.enemies.Count > 0)
            {
                float radius = 8;
                Collider[] cols = Physics.OverlapSphere(transform.position, radius, whatIsTarget);
                GameObject clossedEnemy = null;
                float closestDistance = Mathf.Infinity;

                if (cols != null)
                {
                    foreach (Collider col in cols)
                    {
                        float distance = Vector3.Distance(col.transform.position, transform.position);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            clossedEnemy = col.gameObject;
                        }
                    }
                    if (clossedEnemy != null)
                    {
                        Vector3 dir = clossedEnemy.transform.position + Vector3.up - transform.position;
                        dir.Normalize();
                        var difference = dir - transform.forward;
                        float vectorOffset = difference.magnitude;
                        var localDifference = transform.InverseTransformDirection(dir);
                        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);
                    }
                    else
                    {
                        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(eulerAngleX, eulerAngleY, 0), Time.deltaTime * 5f); 
                    }

                }

            }
            else            
            {
                transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(eulerAngleX, eulerAngleY, 0), Time.deltaTime * 5f);
                
            }
           
        }


    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    IEnumerator ChangeToMouseAim()
    {
        while (Quaternion.Angle(transform.localRotation,Quaternion.Euler(eulerAngleX,eulerAngleY,0))>1f)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(eulerAngleX, eulerAngleY, 0), Time.deltaTime * 10.0f);
        }
        transform.localRotation = Quaternion.Euler(eulerAngleX, eulerAngleY, 0);
        yield return null;
    }
    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }
}