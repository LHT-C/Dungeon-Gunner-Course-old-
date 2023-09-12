using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomNodeSO : ScriptableObject
{
    [HideInInspector] public string id;//房间节点id，由GUID生成
    [HideInInspector] public List<RoomNodeSO> parebtRoomNodeIDList = new List<RoomNodeSO>();
    [HideInInspector] public List<RoomNodeSO> childRoomNodeIDList = new List<RoomNodeSO>();
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    public RoomNodeTypeSO roomNodeType;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;

    #region Editor Code

    // the following code should only be run in the Unity Editor
#if UNITY_EDITOR

    [HideInInspector] public Rect rect;

    public void Initialise(Rect rect, RoomNodeGraphSO nodeGraph, RoomNodeTypeSO roomNodeType)//房间创建时，输入一个节点图和一个房间类型
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();//设置一个唯一的GUID
        this.name = "RoomNode";
        this.roomNodeGraph = nodeGraph;
        this.roomNodeType = roomNodeType;

        // Load room node type list
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;//使用游戏资源来加载房间节点类型列表到成员变量中
    }

    public void Draw(GUIStyle nodeStyle)//绘制方法
    {
        // Draw Node Box Using Begin Area
        GUILayout.BeginArea(rect, nodeStyle);//绘制节点框

        // Start Region To Detect Popup Selection Changes
        EditorGUI.BeginChangeCheck();//检查变化

        // Display a popup using the Roomlodetype name values that can be selected from (default to the currently set roomilodetype)

        int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);//如果选择了（发生变化），则使用谓词指定房间节点类型（列表索引与房间节点类型相同的）

        int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());//创建一个弹出窗口，显示房间节点类型的字符串数组，按选择的房间节点类型填入索引

        roomNodeType = roomNodeTypeList.list[selection];//利用索引返回所选的房间类型

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);//变化检查如果有变化，则SetDirty，保存所作的变化

        GUILayout.EndArea();
    }

    /// <summary>
    /// Populate a string array with the room node types to display that can be selected
    /// </summary>
    public string[] GetRoomNodeTypesToDisplay()
    {
        string[] roomArray = new string[roomNodeTypeList.list.Count];//创建长度为节点类型长度一致的空数组

        for (int i = 0; i < roomNodeTypeList.list.Count; i++)//循环，将节点类型加入数组
        {
            if (roomNodeTypeList.list[i].displayInNodeGraphEditor)
            {
                roomArray[i] = roomNodeTypeList.list[i].roomNodeTypeName;
            }
        }
        return roomArray;
    }

#endif

    #endregion Editor Code
}
