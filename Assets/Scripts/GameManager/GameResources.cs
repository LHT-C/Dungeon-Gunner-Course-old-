using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameResources : MonoBehaviour
{
    private static GameResources instance;

    public static GameResources Instance//��ʵ������ӷ���
    {
        get
        {
            if (instance == null)//�����Ƿ�Ϊ��
            {
                instance = Resources.Load<GameResources>("GameResources");//��Ԥ�Ƽ���GameResources->Resources�ļ����м��ص���Ϸ��Դ���Ͷ���
            }
            return instance;
        }
    }

    #region Header DUNGEON
    [Space(10)]
    [Header("DUNGEON������")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the dungeon RoomNodeTypeListSO���õ��ε�RoomNodeTypeListSO���")]
    #endregion
    public RoomNodeTypeListSO roomNodeTypeList;

    #region Header PLAYER
    [Space(10)]
    [Header("PLAYER�����")]
    #endregion Header PLAYER
    #region Tooltip
    [Tooltip("The current player scriptable object - this is used to reference the current player between scenes����ǰ��ҿɽű�������-�����ڳ���֮�����õ�ǰ���")]
    #endregion Tooltip
    public CurrentPlayerSO currentPlayer;

    #region Header SOUNDS
    [Space(10)]
    [Header("SOUNDS������")]
    #endregion Header
    #region Tooltip
    [Tooltip("Populate with the sounds master mixer group��ʹ�������������������")]
    #endregion
    public AudioMixerGroup soundsMasterMixerGroup;
    #region Tooltip
    [Tooltip("Door open close sound effect")]
    #endregion Tooltip
    public SoundEffectSO doorOpenCloseSoundEffect;

    #region Header MATERIALS
    [Space(10)]
    [Header("MATERIALS������")]
    #endregion
    #region Tooltip
    [Tooltip("Dimmed Material���䰵�Ĳ���")]
    #endregion
    public Material dimmedMaterial;

    #region Tooltip
    [Tooltip("Sprite-Lit-Default Material����������Ĭ�ϲ���")]
    #endregion
    public Material litMaterial;

    #region Tooltip
    [Tooltip("Populate with the Variable Lit Shader��ʹ�ÿɱ�ƹ���ɫ�����")]
    #endregion
    public Shader variableLitShader;

    #region Header UI
    [Space(10)]
    [Header("UI")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with ammo icon prefab���õ�ҩͼ��Ԥ�����")]
    #endregion
    public GameObject ammoIconPrefab;

    #region Validation
#if UNITY_EDITOR
    // Validate the scriptable object details entered
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(roomNodeTypeList), roomNodeTypeList);
        HelperUtilities.ValidateCheckNullValue(this, nameof(currentPlayer), currentPlayer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundsMasterMixerGroup), soundsMasterMixerGroup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorOpenCloseSoundEffect), doorOpenCloseSoundEffect);
        HelperUtilities.ValidateCheckNullValue(this, nameof(litMaterial), litMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(dimmedMaterial), dimmedMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(variableLitShader), variableLitShader);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoIconPrefab), ammoIconPrefab);
    }

#endif
    #endregion
}
