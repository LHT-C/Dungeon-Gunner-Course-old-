using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResources : MonoBehaviour
{
    private static GameResources instance;

    public static GameResources Instance//在实例中添加方法
    {
        get
        {
            if (instance == null)//检验是否为空
            {
                instance = Resources.Load<GameResources>("GameResources");//将预制件从GameResources->Resources文件夹中加载到游戏资源类型对象
            }
            return instance;
        }
    }

    #region Header DUNGEON
    [Space(10)]
    [Header("DUNGEON")]
    #endregion
    #region Tooltip
    [Tooltip("Populated with dungeon RoomNodeTypeS0")]//工具提示属性
    #endregion

    public RoomNodeTypeListSO roomNodeTypeList;
}
