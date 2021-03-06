using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerHUD : MonoBehaviour
{

    private static PlayerHUD instance;

    public static PlayerHUD Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<PlayerHUD>();

            return instance;
        }
    }

    private WeaponBase weapon;
    [Header("Components")]
    
    [SerializeField]
    private Status status;
    
    [Header("Weapon Base")]
    [SerializeField]
    private Image imageWeaponIcon;
    [SerializeField]
    private TextMeshProUGUI textWeaponName;
    [SerializeField]
    private Sprite[] spriteWeaponIcons;
    [SerializeField]
    private Vector2[] sizeWeaponIcons;
    
    [Header("Ammo")]
    [SerializeField]
    private TextMeshProUGUI textAmmo;
    
    [Header("Magazine")]
    [SerializeField]
    private GameObject magazineUIPrefab;
    [SerializeField]
    private Transform magazineParent;
    [SerializeField]
    private int maxMagazineCount;
    [SerializeField]
    private List<GameObject> magazineList;

    [Header("HP & BloodScreen UI")]
    [SerializeField]
    private TextMeshProUGUI textHP;
    [SerializeField]
    private Image imageBloodScreen;
    [SerializeField]
    private AnimationCurve curveBloodScreen;

    [Header("Score & Wave& GameOver UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private GameObject gameoverUI;
    [SerializeField] private GameObject waveEnterStart;
    [SerializeField] private GameObject clearUI;


    private void Awake()
    {
        status.onHPEvent.AddListener(UpdateHPHUD);
    }
    public void SetupAllWeapons(WeaponBase[] weapons)
    {
        SetupMagazine();
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].onAmmoEvent.AddListener(UpdateAmmoHUD);
            weapons[i].onMagazineEvent.AddListener(UpdateMagazineHUD);
        }
    }
    public void SwichingWeapon(WeaponBase newWeapon) 
    {
        weapon = newWeapon;
        SetupWeapon();
    }
    
    private void SetupWeapon()
    {
        textWeaponName.text = weapon.WeaponName.ToString();
        imageWeaponIcon.sprite = spriteWeaponIcons[(int)weapon.WeaponName];
        imageWeaponIcon.rectTransform.sizeDelta = sizeWeaponIcons[(int)weapon.WeaponName];
    }

    private void UpdateAmmoHUD(int currentAmmo, int maxAmmo)
    {
        textAmmo.text = $"<size=40>{currentAmmo}/</size>{maxAmmo}";
    }

    public void UpdateScoreText(int newScore)
    {
        scoreText.text = "Score : " + newScore;
    }

    public void UpdateWaveText(int waves, int count)
    {
        waveText.text = "Wave : " + waves + "\nEnemy Left : " + count;
    }

    public void SetActiveGameoverUI(bool active)
    {
        gameoverUI.SetActive(active);
    } 
    public void SetActiveEnterStartWaveUI(bool active)
    {
        waveEnterStart.SetActive(active);
    }
    public void SetActiveClearUI(bool active)
    {
        clearUI.SetActive(active);
    }

    private void SetupMagazine()
    {
        magazineList = new List<GameObject>();
        for (int i = 0; i < maxMagazineCount; ++i)
        {
            GameObject clone = Instantiate(magazineUIPrefab);
            clone.transform.SetParent(magazineParent);
            clone.SetActive(false);

            magazineList.Add(clone);
        }
    }

    private void UpdateMagazineHUD(int currentMagazine)
    {
        for (int i = 0; i < magazineList.Count; i++)
        {
            magazineList[i].SetActive(false);
        }
        for (int i = 0; i < currentMagazine; i++)
        {
            magazineList[i].SetActive(true);
        }
    }
    private void UpdateHPHUD(int previous, int current)
    {
        textHP.text = "HP " + current;
        if(previous - current > 0)
        {
            StopCoroutine("OnBloodScreen");
            StartCoroutine("OnBloodScreen");
        }
    }

    public void GameRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator OnBloodScreen()
    {
        float percent = 0;
        while (percent<1)
        {
            percent += Time.deltaTime;
            Color color = imageBloodScreen.color;
            color.a = Mathf.Lerp(1,0, curveBloodScreen.Evaluate(percent));
            imageBloodScreen.color = color;
            yield return null;
        }
    }
}
