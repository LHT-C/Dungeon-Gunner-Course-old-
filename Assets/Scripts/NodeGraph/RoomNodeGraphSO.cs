using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName ="RoomNodeGraph", menuName = "Scriptable Objects/Dungeon/Room Node Graph")]//�Ҽ������ʲ�ѡ���в˵�������Ӳ˵����ű����󷿼�ڵ�ͼ
public class RoomNodeGraphSO : ScriptableObject
{
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
    [HideInInspector] public List<RoomNodeSO> roomNodeList = new List<RoomNodeSO>();
    [HideInInspector] public Dictionary<string, RoomNodeSO> roomNodeDictionary = new Dictionary<string, RoomNodeSO>();

    private void Awake()
    {
        LoadRoomNodeDictionary();
    }

    /// <summary>
    /// Load the room node dictionary from the room node list.
    /// </summary>
    private void LoadRoomNodeDictionary()//�����з���ڵ���뵽�ֵ���
    {
        roomNodeDictionary.Clear();

        // Populate dictionary
        foreach (RoomNodeSO node in roomNodeList)
        {
            roomNodeDictionary[node.id] = node;
        }
    }

    /// <summary>
    /// Get room node by room nodeID
    /// </summary>
    public RoomNodeSO GetRoomNode(string roomNodeID)
    {
        if (roomNodeDictionary.TryGetValue(roomNodeID, out RoomNodeSO roomNode))//�ýڵ�id���ֵ��в��ҷ���ڵ�
        {
            return roomNode;
        }
        return null;
    }

    #region Editor Code

    // The following code should only run in the Unity Editor
#if UNITY_EDITOR

    [HideInInspector] public RoomNodeSO roomNodeToDrawLinefrom = null;
    [HideInInspector] public Vector2 linePosition;

    // Repopulate node dictionary every time a change is made in the editor
    public void OnValidate()
    {
        LoadRoomNodeDictionary();
    }

    public void SetNodeToDrawConnectionLinefrom(RoomNodeSO node, Vector2 position)//����ڵ�λ�ú����λ����Ϊ�ж����ݺ�����յ�
    {
        roomNodeToDrawLinefrom = node;
        linePosition = position;
    }

#endif

    #endregion Editor Code
}
