using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomNodeSO : ScriptableObject
{
    [HideInInspector] public string id;//房间节点id，由GUID生成
    [HideInInspector] public List<string> parentRoomNodeIDList = new List<string>();
    [HideInInspector] public List<string> childRoomNodeIDList = new List<string>();
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    public RoomNodeTypeSO roomNodeType;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;

    #region Editor Code

    // the following code should only be run in the Unity Editor
#if UNITY_EDITOR

    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClickDragging = false;
    [HideInInspector] public bool isSelected = false;

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

        //if the room node has a parent or is of type entrance then display a label else display a popup
        if (parentRoomNodeIDList.Count > 0 || roomNodeType.isEntrance)
        {
            // Display a label that can't be changed
            EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);//当节点连线后，不能再更改节点房间类型
        }

        else
        {
            // Display a popup using the Roomlodetype name values that can be selected from (default to the currently set roomilodetype)
            int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);//如果选择了（发生变化），则使用谓词指定房间节点类型（列表索引与房间节点类型相同的）
            int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());//创建一个弹出窗口，显示房间节点类型的字符串数组，按选择的房间节点类型填入索引
            roomNodeType = roomNodeTypeList.list[selection];//利用索引返回所选的房间类型

            // If the room type selection has changed making child connections potentially invalid
            if (roomNodeTypeList.list[selected].isCorridor && !roomNodeTypeList.list[selection].isCorridor 
                || !roomNodeTypeList.list[selected].isCorridor && roomNodeTypeList.list[selection].isCorridor 
                || !roomNodeTypeList.list[selected].isBossRoom && roomNodeTypeList.list[selection].isBossRoom)//选择的房间发生变化后，进行约束
            {
                // if a room node type has been chanped and it already has children then delete the parent child links since we need to revalidate any
                if (childRoomNodeIDList.Count > 0)//子节点的父节点被删除后，对该节点进行限制
                {
                    for (int i = childRoomNodeIDList.Count - 1; i >= 0; i--)
                    {
                        // Get child room node
                        RoomNodeSO childRoomNode = roomNodeGraph.GetRoomNode(childRoomNodeIDList[i]);

                        // If the child room node is not null
                        if (childRoomNode!= null)
                        {
                            // Remove childID from parent room node
                            RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);

                            // Remove parentiD from child room node
                            childRoomNode.RemoveParentRoomNodeIDFromRoomNode(id);
                        }
                    }
                }
            }
        }
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

    /// <summary>
    /// Process events for the node
    /// </summary>
    public void ProcessEvents(Event currentEvent)
    {
        switch (currentEvent.type)//确定编辑器中发生了什么类型的交互（按下、松开、拖动）
        {
            // Process Mouse Down Events
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;

            // Process Mouse Up Events
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;

            // Process Mouse Drag Events
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Process mouse down events
    /// </summary>
    private void ProcessMouseDownEvent(Event currentEvent)//鼠标按下事件
    {
        // left click down
        if (currentEvent.button == 0)
        {
            ProcessLeftClickDownEvent();//检测鼠标按下（是否为左键0）
        }
        else if(currentEvent.button == 1)
        {
            ProcessRightClickDownEvent(currentEvent);//检测鼠标按下（是否为右键1）
        }
    }

    /// <summary>
    /// Process left click down event
    /// </summary>
    private void ProcessLeftClickDownEvent()
    {
        //Selection.activeObject = this;

        // Toggle node selection
        if (isSelected == true)//切换是否选择
        {
            isSelected = false;
        }
        else
        {
            isSelected = true;
        }
    }

    /// <summary>
    /// Process right click down event
    /// </summary>
    private void ProcessRightClickDownEvent(Event currentEvent)//右键按下事件
    {
        roomNodeGraph.SetNodeToDrawConnectionLinefrom(this, currentEvent.mousePosition);//在鼠标位置画线
    }

    /// <summary>
    /// Process mouse up events
    /// </summary>
    private void ProcessMouseUpEvent(Event currentEvent)//鼠标抬起事件
    {
        // left click up
        if (currentEvent.button == 0)
        {
            ProcessLeftClickUpEvent();//检测鼠标抬起（是否为左键0）
        }
    }

    /// <summary>
    /// Process left click up event
    /// </summary>
    private void ProcessLeftClickUpEvent()
    {
        if (isLeftClickDragging)//释放拖动状态
        {
            isLeftClickDragging = false;
        }
    }

    /// <summary>
    /// Process mouse up events
    /// </summary>
    private void ProcessMouseDragEvent(Event currentEvent)//鼠标拖动事件
    {
        // left click up
        if (currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent);//检测鼠标拖动（是否为左键0）
        }
    }

    /// <summary>
    /// Process left Mouse Drag event
    /// </summary>
    private void ProcessLeftMouseDragEvent(Event currentEvent)
    {
        isLeftClickDragging = true;//设置拖动事件为true

        DragNode(currentEvent.delta);//传入当前鼠标位置
        GUI.changed = true;
    }

    /// <summary>
    /// Drag node
    /// </summary>
    public void DragNode(Vector2 delta)//控制节点位置
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }

    /// <summary>
    /// Add childID to the node (returns true if the node has been added, false otherwise)
    /// </summary>
    public bool AddChildRoomNodeIDToRoomNode(string childID)
    {
        // Check child node can be added validly to parent
        if (IsChildRoomValid(childID))//检查子节点id是否有效
        {
            childRoomNodeIDList.Add(childID);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Check the child node can be validly added to the parent node - return true if it can otherwise return false
    /// </summary>
    public bool IsChildRoomValid(string childID)
    {
        bool isConnectedBossNodeAlready = false;//检查boss房是否已被连接
        // Check if there is there already a connected boss room in the node graph
        foreach (RoomNodeSO roomNode in roomNodeGraph.roomNodeList)
        {
            if (roomNode.roomNodeType.isBossRoom && roomNode.parentRoomNodeIDList.Count > 0)//每层只需要一个boss房间
                isConnectedBossNodeAlready = true;
        }

        // if the child node has a type of boss room and there is already a connected boss room node then return false
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isBossRoom && isConnectedBossNodeAlready)//如果子节点是boss房，则让其不能再被连接
            return false;

        // If the child node has a type of none then return false
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isNone)//确认子节点是否为空
           return false;

        // If the node already has a child with this child ID return false
        if (childRoomNodeIDList.Contains(childID))//确保该子节点不会被重复连接
            return false;

        // If this node ID and the child ID are the same return false
        if (id == childID)//子节点不能自我连接
            return false;

        // If the node already has a child with this parent ID return false
        if (parentRoomNodeIDList.Contains(childID))//确保该子节点并未已作为父节点
            return false;

        // If the child node already has a parent return false
        if (roomNodeGraph.GetRoomNode(childID).parentRoomNodeIDList.Count > 0)//确保子节点只有一个父节点
            return false;

        // If child is a corridor and this node is a corridor return false
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && roomNodeType.isCorridor)//走廊与走廊不能连接
            return false;

        // If child is not a corridor and this node is not a corridor return false
        if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && !roomNodeType.isCorridor)//房间与房间不能直接连接
            return false;

        // If adding a corridor check that this node has < the maximum permitted child corridors
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count >= Settings.maxChildCorridors)//确保走廊数在约束范围内
            return false;

        // if the child room is an entrance return false - the entrance must always be the top level parent node
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isEntrance)//入口房间不能作为子节点
            return false;

        // If adding a room to a corridor check that this corridor node doesn't already have a room added
        if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count > 0)//确保走廊后只连接一个房间
            return false;

        return true;
    }

    /// <summary>
    /// Add parentID to the node (returns true if the node has been added, false otherwise)
    /// </summary>
    public bool AddParentRoomNodeIDToRoomNode(string parentID)
    {
        parentRoomNodeIDList.Add(parentID);
        return true;
    }

    /// <summary>
    /// Remove childID from the node (returns true if the node has been removed, false otherwise)
    /// </summary>
    public bool RemoveChildRoomNodeIDFromRoomNode(string childID)//删除子节点id
    {
        // if the node contains the child ID then remove it
        if (childRoomNodeIDList.Contains(childID))
        {
            childRoomNodeIDList.Remove(childID);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Remove parentID from the node (returns true if the node has been remove, false otherwise
    /// </summary>
    public bool RemoveParentRoomNodeIDFromRoomNode(string parentID)//删除父节点id
    {
        // if the node contains the parent ID then remove it
        if (parentRoomNodeIDList.Contains(parentID))
        {
            parentRoomNodeIDList.Remove(parentID);
            return true;
        }
        return false;
    }

#endif

    #endregion Editor Code
}
