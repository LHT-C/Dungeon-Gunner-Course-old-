using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    #region Tooltip

    [Tooltip("The player WeaponShootPosition gameobject in the hieracrchy")]

    #endregion Tooltip

    [SerializeField] private Transform weaponShootPosition;

    private Player player;

    private void Awake()
    {
        // Load components
        player = GetComponent<Player>();
    }

    private void Update()
    {
        // Process the player movement input
        MovementInput();

        // Process the player weapon input
        WeaponInput();
    }

    /// <summary>
    /// Player movement input
    /// </summary>
    private void MovementInput()
    {
        player.idleEvent.CallIdleEvent();
    }

    /// <summary>
    /// Weapon Input
    /// </summary>
    private void WeaponInput()
    {
        Vector3 weaponDirection;
        float weaponAngleDegrees, playerAngleDegrees;
        AimDirection playerAimDirection;

        // Aim weapon input
        AimWeaponInput(out weaponDirection, out weaponAngleDegrees, out playerAngleDegrees, out playerAimDirection);
    }

    private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegrees, out float playerAngleDegrees, out AimDirection playerAimDirection)
    {

        // Get mouse world position
        Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();//获取当前鼠标位置的坐标

        // Calculate direction vector of mouse cursor from weapon shoot position
        weaponDirection = (mouseWorldPosition - weaponShootPosition.position);//武器朝向为鼠标位置减武器位置得到的向量

        // Calculate direction vector of mouse cursor from player transform position
        Vector3 playerDirection = (mouseWorldPosition - transform.position);//角色朝向为鼠标位置减角色位置得到的向量

        // Get weapon to cursor angle
        weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);//根据坐标计算武器朝向弧度

        // Get player to cursor angle
        playerAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirection);//根据坐标计算人物朝向弧度

        // Set player aim direction
        playerAimDirection = HelperUtilities.GetAimDirection(playerAngleDegrees);//根据角度，判断瞄准方向

        // Trigger weapon aim event
        player.aimWeaponEvent.CallAimWeaponEvent(playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
    }
}
