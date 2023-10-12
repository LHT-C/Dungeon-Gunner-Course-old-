using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeTypeListSO", menuName = "Scriptable Objects/Dungeon/Room Node Type List")]//�Ҽ������ʲ�ѡ���в˵�������Ӳ˵����ű����󷿼�ڵ�ͼ
public class RoomNodeTypeListSO : ScriptableObject
{
    #region Header ROOM NODE TYPE LIST
    [Space(10)]
    [Header("ROOM NODE TYPE LIST")]
    #endregion
    #region Tooltip
    [Tooltip("This list should be populated with all the RoomNodeTypeS0 for the game - it is used instead of an enum")]//������ʾ����
    #endregion
    public List<RoomNodeTypeSO> list;

    #region Validation
#if UNITY_EDITOR//�༭��ָ�hash if����ֻ��unity�༭����ִ��
    private void OnValidate()//���ڼ��ֵ�ı仯���Ƿ���ڿ��б�
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(list), list);
    }
#endif
    #endregion
}
