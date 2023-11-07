using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeType_", menuName = "Scriptable Objects/Dungeon/Room Node Type")]//右键创建资产选项中菜单中添加子菜单：脚本对象房间类型
public class RoomNodeTypeSO : ScriptableObject
{
    public string roomNodeTypeName;//节点类型名，用下面的布尔值来确定具体类型

    #region Header
    [Header("Only flag the RoomNodeTypes that should be visible in the editor")]
    #endregion Header
    public bool displayInNodeGraphEditor = true;//是否应当被显示在节点编辑器中，默认为真
    #region Header
    [Header("One Type Should Be A Corridor")]
    #endregion Header
    public bool isCorridor;
    #region Header
    [Header("One Type Should Be A CorridorNS ")]
    #endregion Header
    public bool isCorridorNS;
    #region Header
    [Header("One Type Should Be A CorridorEW")]
    #endregion Header
    public bool isCorridorEW;
    #region Header
    [Header("One Type Should Be An Entrance")]
    #endregion Header
    public bool isEntrance;
    #region Header
    [Header("One Type Should Be A Boss Room")]
    #endregion Header
    public bool isBossRoom;
    #region Header
    [Header("One Type Should Be None (Unassigned)")]
    #endregion Header
    public bool isNone;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(roomNodeTypeName), roomNodeTypeName);
    }
#endif
    #endregion
}
