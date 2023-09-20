using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName ="RoomNodeGraph", menuName = "Scriptable Objects/Dungeon/Room Node Graph")]//右键创建资产选项中菜单中添加子菜单：脚本对象房间节点图
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
    private void LoadRoomNodeDictionary()//将所有房间节点加入到字典中
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
        if (roomNodeDictionary.TryGetValue(roomNodeID, out RoomNodeSO roomNode))//用节点id从字典中查找房间节点
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

    public void SetNodeToDrawConnectionLinefrom(RoomNodeSO node, Vector2 position)//传入节点位置和鼠标位置作为判断依据和起点终点
    {
        roomNodeToDrawLinefrom = node;
        linePosition = position;
    }

#endif

    #endregion Editor Code
}
