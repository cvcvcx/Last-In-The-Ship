using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


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

    [Header("UI_Image")]
    [SerializeField]
    private GameObject image_AutoAim_Skill;
    [SerializeField]
    private GameObject image_UsingAutoAim;
    [SerializeField]
    private GameObject image_AutoAim_CoolTime_GameObj;
    [SerializeField]
    private Image image_AutoAim_CoolTime_Skill;

    [SerializeField]
    private GameObject image_SlowMode_Skill;
    [SerializeField]
    private GameObject image_UsingSlowMode;
    [SerializeField]
    private GameObject image_SlowMode_CoolTime_Skill_GameObj;
    [SerializeField]
    private Image image_SlowMode_CoolTime_Skill;

    [SerializeField]
    private TextMeshProUGUI text_AutoAim_CoolTime;
    [SerializeField]
    private TextMeshProUGUI text_SlowMode_CoolTime;

    private bool isDie;
    private bool isAutoAim = false;
    private bool isSlowMode = false;
    private bool canAutoAim = false;
    private bool canSlowMode = true;
    private WeaponBase weapon;


    public bool IsAutoAim { get { return isAutoAim; } set { isAutoAim = value; } }
    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        rotateToMouse = GetComponent<RotateToMouse>();
        movement = GetComponent<MovementCharacterController>();
        status = GetComponent<Status>();

        audioSource = GetComponent<AudioSource>();


    }
    private void Start()
    {
       StartCoroutine(FeverModeCoolTime(5.0f));
        
    }
    private void Update()
    {
        GameManager.Instance.UpdatePauseGame();
        if (GameManager.Instance.isPause == true) return;
        if (GameManager.Instance.isGameClear == true) return;
        if (canAutoAim&&isSlowMode==false)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                IsAutoAim = true;
                StartCoroutine(OnFeverMode(10.0f));
            }
        }
        if (canSlowMode&&isAutoAim==false)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                StartCoroutine(OnSlowMode(5.0f));
            }
        }
        //불렛타임일때 움직일수없고, 로테이트도 고정한상태에서 적에게 총 발사
        UpdateWeaponAction();
        UpdateMove();
        UpdateJump();
        UpdateRotate();

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

            if (audioSource.isPlaying == false)
            {
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            movement.MoveSpeed = 0;
            weapon.Animator.MoveSpeed = 0;

            if (audioSource.isPlaying == true)
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
    public void TakeDamage(int damage)
    {
        isDie = status.DecreaseHP(damage);
        if (isDie == true)
        {
            GameManager.Instance.isPause = true;
            GameManager.Instance.Pause();
            GameManager.Instance.DieMenuSetActive(true);
            Debug.Log("GameOver");
        }
    }


    public void SwitchingWeapon(WeaponBase newWeapon)
    {
        weapon = newWeapon;
    }

    IEnumerator FeverModeCoolTime(float cool)
    {


        while (cool > 0.1f)
            //1초보다 많이 남아있으면, 스킬 쿨타임이 점점 줄어듬
        {
            cool -= Time.deltaTime;
            image_AutoAim_CoolTime_Skill.fillAmount = (1.0f / cool);
            text_AutoAim_CoolTime.text = Mathf.Round(cool).ToString();
            yield return new WaitForFixedUpdate();
        }        
        canAutoAim = true;
        image_AutoAim_Skill.SetActive(true);
        image_AutoAim_CoolTime_GameObj.SetActive(false);
    } 
    IEnumerator OnFeverMode(float time)
    {
        isAutoAim = true;
        image_UsingAutoAim.SetActive(true);
        Time.timeScale = 1.5f;
        //오토에임 스타트
        while (time > 0.1f)
        {
            time -= Time.unscaledDeltaTime;
            yield return new WaitForFixedUpdate();
        }//유지시간 측정
        isAutoAim = false;
        Time.timeScale = 1.0f;
        image_AutoAim_Skill.SetActive(false);
        image_AutoAim_CoolTime_GameObj.SetActive(true);
        image_UsingAutoAim.SetActive(false);

        //오토에임 끔
        StartCoroutine(FeverModeCoolTime(5.0f));
    }  
    IEnumerator OnSlowMode(float time)
    {
        isSlowMode = true;
        Time.timeScale = 0.2f;
        image_UsingSlowMode.SetActive(true);
        //슬로우 스타트
        while (time > 0.1f)
        {
            time -= Time.unscaledDeltaTime;
            yield return new WaitForFixedUpdate();
        }//유지시간 측정
        isSlowMode = false;
        Time.timeScale = 1.0f;
        image_SlowMode_Skill.SetActive(false);
        image_SlowMode_CoolTime_Skill_GameObj.SetActive(true);
        image_UsingSlowMode.SetActive(false);
        //슬로우모드 끔
        StartCoroutine(SlowModeCoolTime(5.0f));
    } 
    IEnumerator SlowModeCoolTime(float cool)
    {
        while (cool > 0.1f)
            //1초보다 많이 남아있으면, 스킬 쿨타임이 점점 줄어듬
        {
            cool -= Time.deltaTime;
            image_SlowMode_CoolTime_Skill.fillAmount = (1.0f / cool);
            text_SlowMode_CoolTime.text = Mathf.Round(cool).ToString();
            yield return new WaitForFixedUpdate();
        }
        image_SlowMode_Skill.SetActive(true);
        image_SlowMode_CoolTime_Skill_GameObj.SetActive(false);
        canSlowMode = true;
    }
}
