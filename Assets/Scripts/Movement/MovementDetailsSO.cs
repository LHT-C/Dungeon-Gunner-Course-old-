using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementDetails_", menuName = "Scriptable Objects/Movement/MovementDetails")]
public class MovementDetailsSO : ScriptableObject
{
    #region Header MOVEMENT DETAILS
    [Space(10)]
    [Header("MOVEMENT DETAILS：移动细节")]
    #endregion Header
    #region Tooltip
    [Tooltip("The minimum move speed. The GetMoveSpeed method calculates a random value between the minimum and maximum：最小移动速度。GetMoveSpeed方法计算最小值和最大值之间的随机值")]
    #endregion Tooltip
    public float minMoveSpeed = 8f;
    #region Tooltip
    [Tooltip("The maximum move speed. The GetMoveSpeed method calculates a random value between the minimum and maximum：最大移动速度。GetMoveSpeed方法计算最小值和最大值之间的随机值")]
    #endregion Tooltip
    public float maxMoveSpeed = 8f;
    #region Tooltip
    [Tooltip("If there is a roll movement- this is the roll speed：如果有翻滚运动，这就是翻滚速度")]
    #endregion
    public float rollSpeed; // for player
    #region Tooltip
    [Tooltip("If there is a roll movement - this is the roll distance：如果有翻滚运动，这就是翻滚距离")]
    #endregion
    public float rollDistance; // for player
    #region Tooltip
    [Tooltip("If there is a roll movement - this is the cooldown time in seconds between roll actions：如果有翻滚运动，这就是翻滚动作之间的冷却时间（以秒为单位）")]
    #endregion
    public float rollCooldownTime; // for player

    /// <summary>
    /// Get a random movement speed between the minimum and maximum values：获取最小值和最大值之间的随机移动速度
    /// </summary>
    public float GetMoveSpeed()
    {
        if (minMoveSpeed == maxMoveSpeed)
        {
            return minMoveSpeed;
        }
        else
        {
            return Random.Range(minMoveSpeed, maxMoveSpeed);//在最大速度和最小速度间随机生成速度值
        }
    }

    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(minMoveSpeed), minMoveSpeed, nameof(maxMoveSpeed), maxMoveSpeed, false);

        if (rollDistance != 0f || rollSpeed != 0 || rollCooldownTime != 0)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollDistance), rollDistance, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollSpeed), rollSpeed, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollCooldownTime), rollCooldownTime, false);
        }
    }

#endif
    #endregion Validation
}
