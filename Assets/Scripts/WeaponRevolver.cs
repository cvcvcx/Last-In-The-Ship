using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class WeaponRevolver : WeaponBase
{
	List<GameObject> shootableEnemy = new List<GameObject>();
	
	[Header("Fire Effects")]
	[SerializeField]
	private GameObject muzzleFlashEffect;

	[Header("Spawn Points")]	
	[SerializeField]
	private Transform bulletSpawnPoint;

	[Header("Audio Clips")]
	[SerializeField]
	private AudioClip audioClipTakeOutWeapon;
	[SerializeField]
	private AudioClip audioClipFire;
	[SerializeField]
	private AudioClip audioClipReload;

	[Header("Aim UI")]
	[SerializeField]
	private Image imageAim;

	private GameObject autoAimtarget;
	public string enemyTag = "ImpactEnemy";

	public LayerMask whatIsTarget; // 추적 대상 레이어



	Quaternion oriQua;
	private float defaultModeFov = 60;
	private float aimModeFov = 60;	
	

	public static bool isBulletAttack;
	//나중에 수정할것

	private ImpactMemoryPool impactMemoryPool;
	private Camera mainCamera;
	PlayerController playerController;

	private bool hasTarget => autoAimtarget != null;

	private void OnEnable()
    {
		PlaySound(audioClipTakeOutWeapon);
		muzzleFlashEffect.SetActive(false);
		onMagazineEvent.Invoke(weaponSetting.currentMagazine);
		onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
		ResetVariables();
	}
    private void Awake()
    {
		base.Setup();
		playerController = GetComponentInParent<PlayerController>();
		impactMemoryPool = GetComponent<ImpactMemoryPool>();
		mainCamera = Camera.main;		
		weaponSetting.currentMagazine = weaponSetting.maxMagazine;
		weaponSetting.currentAmmo = weaponSetting.maxAmmo;
			
    }
    

    public override void StartWeaponAction(int type = 0)
    {
		if (isReload == true) return;
		if (isModeChange == true) return;
		if (type == 0)
		{
				OnAttack();
		}
		else
		{			
			StartCoroutine("OnModeChange");
		}
    }

	
	private bool IsTargetOnSight(Transform target)
	{
		RaycastHit hit;

		var direction = target.position - transform.position;

		direction.y = transform.forward.y;

		if (Vector3.Angle(direction, transform.forward) > 50.0f * 0.5f)
		{
			return false;
		}

		if (Physics.Raycast(transform.position, direction, out hit, whatIsTarget))
		{
			if (hit.transform == target) return true;
		}

		return false;
	}

	public void OnAttack()
	{

		if (isBulletAttack == true) return;
		 if (Time.time - lastAttackTime > weaponSetting.attackRate)
		{
			if (animator.MoveSpeed > 0.5f)
			{
				return;
			}
			if (weaponSetting.currentAmmo <= 0)
			{
				return;
			}

			lastAttackTime = Time.time;
			weaponSetting.currentAmmo--;
			onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
			//animator.Play("Fire", -1, 0);
			Attack();
			TwoStepRaycast();

		}
	}
	private void Attack() 
	{
		string animation = animator.AimModeIs == true ? "AimFire" : "Fire";
		animator.Play(animation, -1, 0);
		StartCoroutine("OnMuzzleFlashEffect");
		PlaySound(audioClipFire);
		
	}
	private IEnumerator OnMuzzleFlashEffect()
	{
		muzzleFlashEffect.SetActive(true);
		yield return new WaitForSeconds(weaponSetting.attackRate * 0.1f);
		muzzleFlashEffect.SetActive(false);
	}
	private IEnumerator OnReload()
	{
		isReload = true;
		animator.OnReload();
		PlaySound(audioClipReload);
		while (true)
		{
			if (animator.CurrentAnimationIs("Movement"))//오디오 플레이 중일때도 리로드 가능하게 함
			{
				isReload = false;

				weaponSetting.currentMagazine--;
				onMagazineEvent.Invoke(weaponSetting.currentMagazine);

				weaponSetting.currentAmmo = weaponSetting.maxAmmo;
				onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
				yield break;
			}
			yield return null;
		}
	}
	private void TwoStepRaycast()
	{
		Ray ray;
		RaycastHit hit;
		Vector3 targetPoint = Vector3.zero;

		ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);
		if (Physics.Raycast(ray, out hit, weaponSetting.attackDistance))
		{
			targetPoint = hit.point;
		}
		else
		{
			targetPoint = ray.origin + ray.direction * weaponSetting.attackDistance;
		}
		Debug.DrawRay(ray.origin, ray.direction * weaponSetting.attackDistance, Color.red);

		Vector3 attackDirection = (targetPoint - bulletSpawnPoint.position).normalized;
		if (Physics.Raycast(bulletSpawnPoint.position, attackDirection, out hit, weaponSetting.attackDistance))
		{			
			impactMemoryPool.SpawnImpact(hit);
            if (hit.transform.GetComponent<EnemyProjectile>() != null)
            {
				EnemyProjectile enemyBullet = hit.transform.GetComponent<EnemyProjectile>();
				enemyBullet.DestoryProjectile();
            }
			if (hit.transform.CompareTag("ImpactEnemy"))
			{
				hit.transform.GetComponent<EnemyFSM>().TakeDamage(weaponSetting.damage);
			}
		}
		Debug.DrawRay(bulletSpawnPoint.position, attackDirection * weaponSetting.attackDistance, Color.blue);
	}

	
	public override void StopWeaponAction(int type = 0)
    {
		isAttack = false;
    }
    public override void StartReload()
    {
		if (isReload == true || weaponSetting.currentMagazine <= 0) return;
		StopWeaponAction();
		StartCoroutine("OnReload");
    }
	private IEnumerator OnModeChange()
	{
				
		float current = 0;
		float percent = 0;
		float time = 0.35f;
		animator.AimModeIs = !animator.AimModeIs;
		imageAim.enabled = !imageAim.enabled;		
		float start = mainCamera.fieldOfView;
		
		

		float end = animator.AimModeIs == true ? aimModeFov : defaultModeFov;		
		isModeChange = true;		
		while (percent < 1)
		{

			current += Time.deltaTime;
			percent = current / time;
			mainCamera.fieldOfView = Mathf.Lerp(start, end, percent);		
			
			yield return null;
		}		
		if(percent>=1) isModeChange = false;
        
	}


	
	

	private void ResetVariables()
    {
		isReload = false;
		isAttack = false;
    }
  
}
