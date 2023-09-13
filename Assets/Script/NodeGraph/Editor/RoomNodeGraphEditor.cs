using UnityEditor.Callbacks;
using UnityEngine;
using UnityEditor;
using System;
using System.Diagnostics;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;
    private static RoomNodeGraphSO currentRoomNodeGraph;
    private RoomNodeSO currentRoomNode = null;
    private RoomNodeTypeListSO roomNodeTypeList;

    //Node layout values（节点布局数值）
    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 75;
    private const int nodeBorder = 12;

    [MenuItem("Room Node Graph Editor",menuItem ="Window/Dungon Editor/Room Node Graph Editor")]//菜单项属性，使编辑器窗口出现在Unity编辑器的菜单上
    private static void OpenWindow()
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");//获取窗口，传入想要打开的编辑器窗口类型（类名）
    }

    private void OnEnable()//定义节点布局的风格
    {
        //Define node layout style
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;//加载预定义的资产（节点1），与unity编辑器配合使用，作为纹理输入
        roomNodeStyle.normal.textColor = Color.white;//文本颜色
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.padding = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        //Load Room node types
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    /// <summary>
    /// Open the room node graph editor window if a room node graph scriptable object asset is double clicked in the inspector
    /// </summary>
    /// 
    [OnOpenAsset(0)] // Need the namespace UnityEditor.Callbacks
    public static bool OnDoubleClickAsset(int instanceID, int line)//在编辑器中点击资产时调用
    {
        RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;//使用实例id进行实例化

        if (roomNodeGraph != null)//在编辑器中点击节点时
        {
            OpenWindow();//打开窗口

            currentRoomNodeGraph = roomNodeGraph;//使当前节点变为该节点

            return true;
        }
        return false;
    }

    /// <summary>
    /// Draw Editor Gui
    /// </summary>
    private void OnGUI()
    {
        #region
        //GUILayout.BeginArea(new Rect(new Vector2(100f, 100f), new Vector2(nodeWidth, nodeHeight)), roomNodeStyle);//开始布局区域
        //EditorGUILayout.LabelField("Node 1");//标签字段
        //GUILayout.EndArea();//结束布局区域
        //
        //GUILayout.BeginArea(new Rect(new Vector2(300f, 300f), new Vector2(nodeWidth, nodeHeight)), roomNodeStyle);//开始布局区域
        //EditorGUILayout.LabelField("Node 2");//标签字段
        //GUILayout.EndArea();//结束布局区域
        #endregion
        // If a scriptable object of typeRoomNodeGraphS0 has been selected then process
        if (currentRoomNodeGraph != null)
        {
            // Process Events
            ProcessEvents(Event.current);

            // Draw Room Nodes
            DrawRoomNodes();
        }
        if (GUI.changed) 
            Repaint();
    }

    private void ProcessEvents(Event currentEvent)
    {
        // Get room node that mouse is over if it's null or not currently being dragged
        if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)//检测鼠标是否在房间节点上按下拖动
        {
            currentRoomNode = IsMouseOverRoomNode(currentEvent);//事件，返回当前房间节点
        }

        // if mouse isn't over a room node
        if (currentRoomNode == null)
        {
            ProcessRoomNodeGraphEvents(currentEvent);//处理房间节点图的事件
        }
        // else process room node events
        else
        {
            // process room node events
            currentRoomNode.ProcessEvents(currentEvent);
        }
    }

    /// <summary>
    /// Check to see to mouse is over a room node - if so then return the room node else return null
    /// </summary>
    private RoomNodeSO IsMouseOverRoomNode(Event currentEvent)
    {
        for (int i = currentRoomNodeGraph.roomNodeList.Count - 1; i >= 0; i--)//列表循环、确定当前节点
        {
            if (currentRoomNodeGraph.roomNodeList[i].rect.Contains(currentEvent.mousePosition))
            {
                return currentRoomNodeGraph.roomNodeList[i];
            }
        }
        return null;
    }

    /// <summary>
    /// Process Room Node Graph Events
    /// </summary>
    private void ProcessRoomNodeGraphEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            // Process Mouse Down Events
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);//鼠标按下时触发事件
                break;

            default: 
                break;
        }
    }

    /// <summary>
    /// Process mouse down events on the room node graph (not over a node)
    /// </summary>
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        // Process right click mouse down on graph event (show context menu)
        if (currentEvent.button == 1)//1是鼠标右键
        {
            ShowContextMenu(currentEvent.mousePosition);//显示上下文菜单
        }
    }

    /// <summary>
    /// Show the context menu
    /// </summary>
    private void ShowContextMenu(Vector2 mousePosition)//传入鼠标位置
    {
        GenericMenu menu = new GenericMenu();//创建一个新的菜单

        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);//往菜单里传入函数、激活状态，菜单位置为鼠标位置

        menu.ShowAsContext();
    }

    /// <summary>
    /// Create a room node at the mouse position
    /// </summary>
    private void CreateRoomNode(object mousePositionObject)
    {
        CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));//谓词，寻找鼠标选择的房间节点类型，传给下面的重载函数
    }

    private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;

        // create room node scriptable object asset
        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();//创建房间节点实例

        // add room node to current room node graph room node list
        currentRoomNodeGraph.roomNodeList.Add(roomNode);//将创建的房间节点实例保存到列表

        //set room node values
        roomNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph, roomNodeType);//初始化房间节点

        // add room node to roon node graph scriptable object asset database
        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);//将创建的房间节点和当前的房间节点图添加到资产

        AssetDatabase.SaveAssets();//保存资产
    }

    /// <summary>
    /// Draw room nodes in the graph window
    /// </summary>
    private void DrawRoomNodes()
    {
        // Loop through all room nodes and draw them
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)//循环浏览所有房间节点，添加到节点图中
        {
            roomNode.Draw(roomNodeStyle);//绘制房间节点
        }
        GUI.changed = true;
    }
}
