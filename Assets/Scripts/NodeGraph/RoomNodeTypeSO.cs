using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeType_", menuName = "Scriptable Objects/Dungeon/Room Node Type")]//�Ҽ������ʲ�ѡ���в˵�������Ӳ˵����ű����󷿼�����
public class RoomNodeTypeSO : ScriptableObject
{
    public string roomNodeTypeName;//�ڵ���������������Ĳ���ֵ��ȷ����������

    #region Header
    [Header("Only flag the RoomNodeTypes that should be visible in the editor")]
    #endregion Header
    public bool displayInNodeGraphEditor = true;//�Ƿ�Ӧ������ʾ�ڽڵ�༭���У�Ĭ��Ϊ��
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
