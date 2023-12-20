using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(WeaponFiredEvent))]

[DisallowMultipleComponent]
public class FireWeapon : MonoBehaviour
{
    private float firePreChargeTimer = 0f;
    private float fireRateCoolDownTimer = 0f;
    private ActiveWeapon activeWeapon;
    private FireWeaponEvent fireWeaponEvent;
    private ReloadWeaponEvent reloadWeaponEvent;
    private WeaponFiredEvent weaponFiredEvent;

    private void Awake()
    {
        // Load components.
        activeWeapon = GetComponent<ActiveWeapon>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
    }

    private void OnEnable()
    {
        // Subscribe to fire weapon event.
        fireWeaponEvent.OnFireWeapon += FireWeaponEvent_OnFireWeapon;
    }

    private void OnDisable()
    {
        // Unsubscribe from fire weapon event.
        fireWeaponEvent.OnFireWeapon -= FireWeaponEvent_OnFireWeapon;
    }

    private void Update()
    {
        // Decrease cooldown timer. ����������ʱ���ʱ��
        fireRateCoolDownTimer -= Time.deltaTime;
    }


    /// <summary>
    /// Handle fire weapon event.
    /// </summary>
    private void FireWeaponEvent_OnFireWeapon(FireWeaponEvent fireWeaponEvent, FireWeaponEventArgs fireWeaponEventArgs)
    {
        WeaponFire(fireWeaponEventArgs);
    }

    /// <summary>
    /// Fire weapon.
    /// </summary>
    private void WeaponFire(FireWeaponEventArgs fireWeaponEventArgs)
    {
        // Handle weapon precharge timer. ����������ܶ�ʱ����
        WeaponPreCharge(fireWeaponEventArgs);

        // Weapon fire.
        if (fireWeaponEventArgs.fire)
        {
            // Test if weapon is ready to fire. ȷ�Ͽ�������������ӵ�
            if (IsWeaponReadyToFire())
            {
                FireAmmo(fireWeaponEventArgs.aimAngle, fireWeaponEventArgs.weaponAimAngle, fireWeaponEventArgs.weaponAimDirectionVector);

                ResetCoolDownTimer();//���һ���ӵ�������������ʱ���ʱ��

                ResetPrechargeTimer();//���һ���ӵ���������������ʱ���ʱ��
            }
        }
    }

    /// <summary>
    /// Handle weapon precharge.
    /// </summary>
    private void WeaponPreCharge(FireWeaponEventArgs fireWeaponEventArgs)
    {
        // Weapon precharge.
        if (fireWeaponEventArgs.firePreviousFrame)
        {
            // Decrease precharge timer if fire button held previous frame. �������ť��ס��������������ܼ�ʱ����
            firePreChargeTimer -= Time.deltaTime;
        }
        else
        {
            // else reset the precharge timer. �ɿ������ö�ʱ����
            ResetPrechargeTimer();
        }
    }

    /// <summary>
    /// Returns true if the weapon is ready to fire, else returns false. ȷ���Ƿ�������
    /// </summary>
    private bool IsWeaponReadyToFire()
    {
        // if there is no ammo and weapon doesn't have infinite ammo then return false. ���û�е�ҩ������Ҳû�����޵�ҩ����ô����false��
        if (activeWeapon.GetCurrentWeapon().weaponRemainingAmmo <= 0 && !activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteAmmo)
            return false;

        // if the weapon is reloading then return false. ���������������װ�����򷵻�false��
        if (activeWeapon.GetCurrentWeapon().isWeaponReloading)
            return false;

        // If the weapon is cooling down then return false. ���������������������û�г��ܵ������򷵻�false��
        if (firePreChargeTimer > 0f || fireRateCoolDownTimer > 0f)
            return false;

        // if no ammo in the clip and the weapon doesn't have infinite clip capacity then return false. ���������û�е�ҩ����������û�����޵ĵ��������򷵻�false��
        if (!activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteClipCapacity && activeWeapon.GetCurrentWeapon().weaponClipRemainingAmmo <= 0)
        {
            // trigger a reload weapon event. ��������װ�������¼���
            reloadWeaponEvent.CallReloadWeaponEvent(activeWeapon.GetCurrentWeapon(), 0);

            return false;
        }
        // weapon is ready to fire - return true ����׼������-����true
        return true;

    }

    /// <summary>
    /// Set up ammo using an ammo gameobject and component from the object pool. ʹ�õ�ҩ��Ϸ����Ͷ�����е�������õ�ҩ��
    /// </summary>
    private void FireAmmo(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        AmmoDetailsSO currentAmmo = activeWeapon.GetCurrentAmmo();

        if (currentAmmo != null)
        {
            // Get ammo prefab from array �������л�ȡԤ�Ƶ�ҩ
            GameObject ammoPrefab = currentAmmo.ammoPrefabArray[Random.Range(0, currentAmmo.ammoPrefabArray.Length)];

            // Get random speed value ��ȡ�������ֵ
            float ammoSpeed = Random.Range(currentAmmo.ammoSpeedMin, currentAmmo.ammoSpeedMax);

            // Get Gameobject with IFireable component ʹ��IFireable�����ȡGameobject
            IFireable ammo = (IFireable)PoolManager.Instance.ReuseComponent(ammoPrefab, activeWeapon.GetShootPosition(), Quaternion.identity);

            // Initialise Ammo ��ʼ����ҩ
            ammo.InitialiseAmmo(currentAmmo, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector);

            // Reduce ammo clip count if not infinite clip capacity ����������޵�������������ٵ�ҩ��������
            if (!activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteClipCapacity)
            {
                activeWeapon.GetCurrentWeapon().weaponClipRemainingAmmo--;
                activeWeapon.GetCurrentWeapon().weaponRemainingAmmo--;
            }
            // Call weapon fired event �������������¼�
            weaponFiredEvent.CallWeaponFiredEvent(activeWeapon.GetCurrentWeapon());
        }
    }

    /// <summary>
    /// Reset cooldown timer
    /// </summary>
    private void ResetCoolDownTimer()
    {
        // Reset cooldown timer ����������ʱ���ʱ��
        fireRateCoolDownTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponFireRate;
    }

    /// <summary>
    /// Reset precharge timers
    /// </summary>
    private void ResetPrechargeTimer()
    {
        // Reset precharge timer �����������ʱ���ʱ��
        firePreChargeTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponPrechargeTime;
    }
}
