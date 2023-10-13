using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [Header("DUNGEON")]
    #endregion
    #region Tooltip
    [Tooltip("Populated with dungeon RoomNodeTypeS0")]//������ʾ����
    #endregion

    public RoomNodeTypeListSO roomNodeTypeList;
}
