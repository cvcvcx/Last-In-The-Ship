using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitchSystem : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private PlayerHUD playerHUD;

    [SerializeField]
    private WeaponBase[] weapons;

    private WeaponBase currentWeapon;
    private WeaponBase previousWeapon;

    private void Awake()
    {
        playerHUD.SetupAllWeapons(weapons);
        for (int i = 0; i < weapons.Length; ++i)
        {
            if (weapons[i].gameObject != null)
            {
                weapons[i].gameObject.SetActive(false);
            }
        }
        SwitchingWeapon(WeaponType.Main);

    }
    

    void Update()
    {
        UpdateSwitch();    
    }

    private void UpdateSwitch()
    {
        if (!Input.anyKeyDown) return;
        int inputIndex = 0;
        if(int.TryParse(Input.inputString, out inputIndex) && (inputIndex > 0 && inputIndex < 3))
        {
            SwitchingWeapon((WeaponType)(inputIndex - 1));
        }
    }

    private void SwitchingWeapon(WeaponType weaponType)
    {
        if (weapons[(int)weaponType] == null)
        {
            return;
        }
        if (currentWeapon != null)
        {
            previousWeapon = currentWeapon;
        }
        currentWeapon = weapons[(int)weaponType];
        if (currentWeapon == previousWeapon)
        {
            return;
        }
        playerController.SwitchingWeapon(currentWeapon);
        playerHUD.SwichingWeapon(currentWeapon);

        if (previousWeapon != null)
        {
            previousWeapon.gameObject.SetActive(false);
        }
            currentWeapon.gameObject.SetActive(true);
    }
}
