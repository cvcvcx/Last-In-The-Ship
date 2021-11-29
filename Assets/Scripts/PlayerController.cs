﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Input KeyCodes")]
    [SerializeField]
    private KeyCode keyCodeRun = KeyCode.LeftShift;
    private KeyCode keyCodeJump = KeyCode.Space;
    private KeyCode keyCodeReload = KeyCode.R;

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipWalk;
    [SerializeField]
    private AudioClip audioClipRun;

    private RotateToMouse rotateToMouse;
    private MovementCharacterController movement;
    private Status status;    
    private AudioSource audioSource;
    private WeaponBase weapon;
    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        rotateToMouse = GetComponent<RotateToMouse>();
        movement = GetComponent<MovementCharacterController>();
        status = GetComponent<Status>();
        
        audioSource = GetComponent<AudioSource>();
        

}
    private void Update()
    {
        UpdateRotate();
        UpdateMove();
        UpdateJump();
        UpdateWeaponAction();
    }
    private void UpdateRotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotateToMouse.UpdateRotate(mouseX, mouseY);
    }
    private void UpdateMove()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 moveVelocity = new Vector3(x, 0, z).normalized;
        if (x != 0 || z != 0)
        {
            bool isRun = false;
            if (z > 0) isRun = Input.GetKey(keyCodeRun);
            movement.MoveSpeed = isRun == true ? status.RunSpeed : status.WalkSpeed;
            weapon.Animator.MoveSpeed = isRun == true ? 1 : 0.5f;
            audioSource.clip = isRun == true ? audioClipRun : audioClipWalk;

            if(audioSource.isPlaying == false)
            {
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            movement.MoveSpeed = 0;
            weapon.Animator.MoveSpeed = 0;

            if(audioSource.isPlaying == true)
            {
                audioSource.Stop();
            }
        }
        movement.MoveTo(moveVelocity);
    }
    private void UpdateJump()
    {
        if (Input.GetKeyDown(keyCodeJump))
        {
            movement.Jump();
        }
    }
    private void UpdateWeaponAction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            weapon.StartWeaponAction(0);

        }
        else if (Input.GetMouseButtonUp(0))

        {
            weapon.StopWeaponAction(0);
        }
        if (Input.GetMouseButtonDown(1))
        {
            weapon.StartWeaponAction(1);

        }
        else if (Input.GetMouseButtonUp(1))

        {
            weapon.StopWeaponAction(1);
        }

        if (Input.GetKeyDown(keyCodeReload))
        {
            weapon.StartReload();
        }
    }
    public  void TakeDamage(int damage)
    {
        bool isDie = status.DecreaseHP(damage);
        if(isDie == true)
        {
            Debug.Log("GameOver");
        }
    }
    public void SwitchingWeapon(WeaponBase newWeapon)
    {
        weapon = newWeapon;
    }
}
