using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AmmoDetails_", menuName = "Scriptable Objects/Weapons/Ammo Details")]
public class AmmoDetailsSO : ScriptableObject
{
    #region Header BASIC AMMO DETAILS
    [Space(10)]
    [Header("BASIC AMMO DETAILS 基本弹药细节")]
    #endregion
    #region Tooltip
    [Tooltip("Name for the ammo 弹药名称")]
    #endregion
    public string ammoName;
    public bool isPlayerAmmo;

    #region Header AMMO SPRITE, PREFAB & MATERIALS
    [Space(10)]
    [Header("AMMO SPRITE, PREFAB & MATERIALS 弹药精灵，预制件和材料")]
    #endregion
    #region Tooltip
    [Tooltip("Sprite to be used for the ammo 用于弹药的精灵")]
    #endregion
    public Sprite ammoSprite;
    #region Tooltip
    [Tooltip("Populate with the prefab to be used for the ammo.  If multiple prefabs are specified then a random prefab from the array will be selecetd.  The prefab can be an ammo pattern - as long as it conforms to the IFireable interface. 用预制弹药填充。如果指定了多个预制件，则将从阵列中选择一个随机预制件。预制件可以是弹药模式——只要它符合IFireable接口")]
    #endregion
    public GameObject[] ammoPrefabArray;
    #region Tooltip
    [Tooltip("The material to be used for the ammo 用于弹药的材料")]
    #endregion
    public Material ammoMaterial;
    #region Tooltip
    [Tooltip("If the ammo should 'charge' briefly before moving then set the time in seconds that the ammo is held charging after firing before release 如果弹药在移动前应短暂“充电”，则设置弹药在发射后充电后释放的时间（以秒为单位）")]
    #endregion
    public float ammoChargeTime = 0.1f;
    #region Tooltip
    [Tooltip("If the ammo has a charge time then specify what material should be used to render the ammo while charging 如果弹药有充电时间，则指定充电时应使用何种材料渲染弹药")]
    #endregion
    public Material ammoChargeMaterial;

    #region Header AMMO BASE PARAMETERS
    [Space(10)]
    [Header("AMMO BASE PARAMETERS 弹药库参数")]
    #endregion
    #region Tooltip
    [Tooltip("The damage each ammo deals 每枚弹药造成的伤害")]
    #endregion
    public int ammoDamage = 1;
    #region Tooltip
    [Tooltip("The minimum speed of the ammo - the speed will be a random value between the min and max 弹药的最小速度-速度将是最小值和最大值之间的随机值")]
    #endregion
    public float ammoSpeedMin = 20f;
    #region Tooltip
    [Tooltip("The maximum speed of the ammo - the speed will be a random value between the min and max 弹药的最大速度-速度将是最小值和最大值之间的随机值")]
    #endregion
    public float ammoSpeedMax = 20f;
    #region Tooltip
    [Tooltip("The range of the ammo (or ammo pattern) in unity units 单位中弹药（或弹药模式）的射程")]
    #endregion
    public float ammoRange = 20f;
    #region Tooltip
    [Tooltip("The rotation speed in degrees per second of the ammo pattern 弹药模式的旋转速度（以度/秒为单位）")]
    #endregion
    public float ammoRotationSpeed = 1f;

    #region Header AMMO SPREAD DETAILS
    [Space(10)]
    [Header("AMMO SPREAD DETAILS 弹药散布细节")]
    #endregion
    #region Tooltip
    [Tooltip("This is the  minimum spread angle of the ammo.  A higher spread means less accuracy. A random spread is calculated between the min and max values. 这是弹药的最小展开角。更高的排列意味着更低的准确性。在最小值和最大值之间计算随机排列")]
    #endregion
    public float ammoSpreadMin = 0f;
    #region Tooltip
    [Tooltip(" This is the  maximum spread angle of the ammo.  A higher spread means less accuracy. A random spread is calculated between the min and max values. 这是弹药的最大展开角。更高的排列意味着更低的准确性。在最小值和最大值之间计算随机排列")]
    #endregion
    public float ammoSpreadMax = 0f;

    #region Header AMMO SPAWN DETAILS
    [Space(10)]
    [Header("AMMO SPAWN DETAILS 弹药生成详细信息")]
    #endregion
    #region Tooltip
    [Tooltip("This is the minimum number of ammo that are spawned per shot. A random number of ammo are spawned between the minimum and maximum values. 这是每次射击产生的最小弹药数量。在最小值和最大值之间产生随机数量的弹药")]
    #endregion
    public int ammoSpawnAmountMin = 1;
    #region Tooltip
    [Tooltip("This is the maximum number of ammo that are spawned per shot. A random number of ammo are spawned between the minimum and maximum values. 这是每次射击产生的最大弹药数量。在最小值和最大值之间产生随机数量的弹药")]
    #endregion
    public int ammoSpawnAmountMax = 1;
    #region Tooltip
    [Tooltip("Minimum spawn interval time. The time interval in seconds between spawned ammo is a random value between the minimum and maximum values specified. 最短生成间隔时间。生成弹药之间的时间间隔（秒）是指定的最小值和最大值之间的随机值")]
    #endregion
    public float ammoSpawnIntervalMin = 0f;
    #region Tooltip
    [Tooltip("Maximum spawn interval time. The time interval in seconds between spawned ammo is a random value between the minimum and maximum values specified. 最大生成间隔时间。生成弹药之间的时间间隔（秒）是指定的最小值和最大值之间的随机值")]
    #endregion
    public float ammoSpawnIntervalMax = 0f;


    #region Header AMMO TRAIL DETAILS
    [Space(10)]
    [Header("AMMO TRAIL DETAILS 弹药踪迹详细信息")]
    #endregion
    #region Tooltip
    [Tooltip("Selected if an ammo trail is required, otherwise deselect.  If selected then the rest of the ammo trail values should be populated. 如果需要弹药轨迹，则选择，否则取消选择。如果选中，则应填充其余弹药轨迹值")]
    #endregion
    public bool isAmmoTrail = false;
    #region Tooltip
    [Tooltip("Ammo trail lifetime in seconds. 弹药轨迹寿命（秒）")]
    #endregion
    public float ammoTrailTime = 3f;
    #region Tooltip
    [Tooltip("Ammo trail material. 弹药痕迹材料")]
    #endregion
    public Material ammoTrailMaterial;
    #region Tooltip
    [Tooltip("The starting width for the ammo trail. 弹药轨迹的起始宽度")]
    #endregion
    [Range(0f, 1f)] public float ammoTrailStartWidth;
    #region Tooltip
    [Tooltip("The ending width for the ammo trail 弹药轨迹的结束宽度")]
    #endregion
    [Range(0f, 1f)] public float ammoTrailEndWidth;

    #region Validation
#if UNITY_EDITOR
    // Validate the scriptable object details entered
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(ammoName), ammoName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoSprite), ammoSprite);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(ammoPrefabArray), ammoPrefabArray);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoMaterial), ammoMaterial);
        if (ammoChargeTime > 0)
            HelperUtilities.ValidateCheckNullValue(this, nameof(ammoChargeMaterial), ammoChargeMaterial);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoDamage), ammoDamage, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpeedMin), ammoSpeedMin, nameof(ammoSpeedMax), ammoSpeedMax, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoRange), ammoRange, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpreadMin), ammoSpreadMin, nameof(ammoSpreadMax), ammoSpreadMax, true);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpawnAmountMin), ammoSpawnAmountMin, nameof(ammoSpawnAmountMax), ammoSpawnAmountMax, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpawnIntervalMin), ammoSpawnIntervalMin, nameof(ammoSpawnIntervalMax), ammoSpawnIntervalMax, true);
        if (isAmmoTrail)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailTime), ammoTrailTime, false);
            HelperUtilities.ValidateCheckNullValue(this, nameof(ammoTrailMaterial), ammoTrailMaterial);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailStartWidth), ammoTrailStartWidth, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailEndWidth), ammoTrailEndWidth, false);
        }
    }

#endif
    #endregion
}
