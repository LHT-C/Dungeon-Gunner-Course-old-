using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementDetails_", menuName = "Scriptable Objects/Movement/MovementDetails")]
public class MovementDetailsSO : ScriptableObject
{
    #region Header MOVEMENT DETAILS
    [Space(10)]
    [Header("MOVEMENT DETAILS���ƶ�ϸ��")]
    #endregion Header
    #region Tooltip
    [Tooltip("The minimum move speed. The GetMoveSpeed method calculates a random value between the minimum and maximum����С�ƶ��ٶȡ�GetMoveSpeed����������Сֵ�����ֵ֮������ֵ")]
    #endregion Tooltip
    public float minMoveSpeed = 8f;
    #region Tooltip
    [Tooltip("The maximum move speed. The GetMoveSpeed method calculates a random value between the minimum and maximum������ƶ��ٶȡ�GetMoveSpeed����������Сֵ�����ֵ֮������ֵ")]
    #endregion Tooltip
    public float maxMoveSpeed = 8f;
    #region Tooltip
    [Tooltip("If there is a roll movement- this is the roll speed������з����˶�������Ƿ����ٶ�")]
    #endregion
    public float rollSpeed; // for player
    #region Tooltip
    [Tooltip("If there is a roll movement - this is the roll distance������з����˶�������Ƿ�������")]
    #endregion
    public float rollDistance; // for player
    #region Tooltip
    [Tooltip("If there is a roll movement - this is the cooldown time in seconds between roll actions������з����˶�������Ƿ�������֮�����ȴʱ�䣨����Ϊ��λ��")]
    #endregion
    public float rollCooldownTime; // for player

    /// <summary>
    /// Get a random movement speed between the minimum and maximum values����ȡ��Сֵ�����ֵ֮�������ƶ��ٶ�
    /// </summary>
    public float GetMoveSpeed()
    {
        if (minMoveSpeed == maxMoveSpeed)
        {
            return minMoveSpeed;
        }
        else
        {
            return Random.Range(minMoveSpeed, maxMoveSpeed);//������ٶȺ���С�ٶȼ���������ٶ�ֵ
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
