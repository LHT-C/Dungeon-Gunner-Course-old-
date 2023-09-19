using UnityEditor.Callbacks;
using UnityEngine;
using UnityEditor;
using System;
using System.Diagnostics;
using static UnityEditor.Graphs.Styles;
using Color = UnityEngine.Color;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;
    private GUIStyle roomNodeSelectedStyle;
    private static RoomNodeGraphSO currentRoomNodeGraph;
    private RoomNodeSO currentRoomNode = null;
    private RoomNodeTypeListSO roomNodeTypeList;

    //Node layout values（节点布局数值）
    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 75;
    private const int nodeBorder = 12;

    // connecting line values
    private const float connectingLineWidth = 3f;
    private const float connectingLineArrowSize = 6f;

    [MenuItem("Room Node Graph Editor",menuItem ="Window/Dungon Editor/Room Node Graph Editor")]//菜单项属性，使编辑器窗口出现在Unity编辑器的菜单上
    private static void OpenWindow()
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");//获取窗口，传入想要打开的编辑器窗口类型（类名）
    }

    private void OnEnable()
    {
        // Subscribe to the inspector selection changed event
        Selection.selectionChanged += InspectorSelectionChanged;//订阅选择类改变时的事件

        //Define node layout style
        roomNodeStyle = new GUIStyle();//定义节点布局的风格
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;//加载预定义的资产（节点1），与unity编辑器配合使用，作为纹理输入
        roomNodeStyle.normal.textColor = Color.white;//文本颜色
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.padding = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        //Define selected node style
        roomNodeSelectedStyle = new GUIStyle();//定义被选中后的节点布局的风格
        roomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
        roomNodeSelectedStyle.normal.textColor = Color.white;
        roomNodeSelectedStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeSelectedStyle.padding = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        //Load Room node types
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    private void OnDisable()
    {
        // Unsubscribe to the inspector selection changed event
        Selection.selectionChanged -= InspectorSelectionChanged;
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
            // Draw line if being dragged
            DrawDraggedLine();

            // Process Events
            ProcessEvents(Event.current);

            // Draw Connections Between Room nodes
            DrawRoomConnections();

            // Draw Room Nodes
            DrawRoomNodes();
        }
        if (GUI.changed) 
            Repaint();
    }

    private void DrawDraggedLine()
    {
        if(currentRoomNodeGraph.linePosition != Vector2.zero)
        {
            //Draw line from node to line position
            Handles.DrawBezier(currentRoomNodeGraph.roomNodeToDrawLinefrom.rect.center, currentRoomNodeGraph.linePosition,
                currentRoomNodeGraph.roomNodeToDrawLinefrom.rect.center, currentRoomNodeGraph.linePosition, Color.white, null, connectingLineWidth);//实际绘制线的方法，并设置了线的宽度和颜色等属性，七点和终点的位置从RoomNodeGraphSO获取
        }
    }


    private void ProcessEvents(Event currentEvent)
    {
        // Get room node that mouse is over if it's null or not currently being dragged
        if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)//检测鼠标是否在房间节点上按下拖动
        {
            currentRoomNode = IsMouseOverRoomNode(currentEvent);//事件，返回当前房间节点
        }

        // if mouse isn't over a room node or we are currently drapzing a line from the room node then process praph events
        if (currentRoomNode == null || currentRoomNodeGraph.roomNodeToDrawLinefrom!= null)//检测鼠标是否右键拖动
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

            // Process Mouse Up Events
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);//鼠标抬起时触发事件
                break;

            // Process Mouse Drag Events
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);//鼠标拖拽时触发事件
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
        // Process left mouse down on graph event
        else if (currentEvent.button == 0)//0是鼠标左键
        {
            ClearLineDrag();
            ClearAllSelectedRoomNodes();
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
        // if current node graph empty then add entrance room node first
        if (currentRoomNodeGraph.roomNodeList.Count == 0 )
        {
            CreateRoomNode(new Vector2(200f, 200f), roomNodeTypeList.list.Find(x => x.isEntrance));//创建第一个节点时，额外创建一个入口节点
        }

        CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));//谓词，寻找鼠标选择的房间节点类型，传给下面的重载函数
    }

    /// <summary>
    /// Create a room node at the mouse position - overloaded to also pass in RoomNodeType
    /// </summary>
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

        // Reflesh graph node dictonary
        currentRoomNodeGraph.OnValidate();
    }

    /// <summary>
    /// Clear selection from all room nodes
    /// </summary>
    private void ClearAllSelectedRoomNodes()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)//点击其他位置时，取消选中节点
        {
            if (roomNode.isSelected)
            {
                roomNode.isSelected = false;

                GUI.changed = true;
            }
        }
    }

    /// <summary>
    /// Process right mouse up event 
    /// </summary>
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        // if releasing the right mouse button and currently dragging a line
        if (currentEvent.button == 1 && currentRoomNodeGraph.roomNodeToDrawLinefrom != null)
        {
            // check if over a room node
            RoomNodeSO roomNode = IsMouseOverRoomNode(currentEvent);

            if (roomNode != null)//链接线两端的房间节点，建立父子节点关系（向父子列表传入id）
            {
                // if so set it as a child of the parent room node if it can be added
                if (currentRoomNodeGraph.roomNodeToDrawLinefrom.AddChildRoomNodeIDToRoomNode(roomNode.id))
                {
                    // Set parent ID in child room node
                    roomNode.AddParentRoomNodeIDToRoomNode(currentRoomNodeGraph.roomNodeToDrawLinefrom.id);
                }
            }

            ClearLineDrag();//清除无效拖动线
        }
    }

    /// <summary>
    /// Process mouse drag event
    /// </summary>
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        // process right click drag event - draw line
        if (currentEvent.button == 1)//检测
        {
            ProcessRightMouseDragEvent(currentEvent);
        }
    }

    /// <summary>
    /// Process right mouse drag event - draw line
    /// </summary>
    private void ProcessRightMouseDragEvent(Event currentEvent)
    {
        if (currentRoomNodeGraph.roomNodeToDrawLinefrom != null)//执行划线方法，gui状态改为改变
        {
            DragConnectingLine(currentEvent.delta);
            GUI.changed = true;
        }
    }

    /// <summary>
    /// Drag connecting line from node
    /// </summary>
    public void DragConnectingLine(Vector2 delta)
    {
        currentRoomNodeGraph.linePosition += delta;//根据鼠标位置改变线位置
    }

    /// <summary>
    /// Clear line drag from a room node
    /// </summary>
    private void ClearLineDrag()
    {
        currentRoomNodeGraph.roomNodeToDrawLinefrom = null;
        currentRoomNodeGraph.linePosition = Vector2.zero;
        GUI.changed = true;
    }

    /// <summary>
    /// Draw connections in the graph window between room nodes
    /// </summary>
    private void DrawRoomConnections()
    {
        // Loop through all room nodes
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)//循环所有房间节点，利用key从字典中获取所有子房间节点的信息，确实为子房间节点则画出连接线
        {
            if (roomNode.childRoomNodeIDList.Count > 0)
            {
                // Loop through child room nodes
                foreach (string childRoomNodeID in roomNode.childRoomNodeIDList)
                {
                    // get child room node from dictionary
                    if (currentRoomNodeGraph.roomNodeDictionary.ContainsKey(childRoomNodeID))
                    {
                        DrawConnectionLine(roomNode, currentRoomNodeGraph.roomNodeDictionary[childRoomNodeID]);

                        GUI.changed = true;
                    }

                }
            }
        }
    }

    /// <summary>
    /// Draw connection line between the parent room node and child room node
    /// </summary>
    private void DrawConnectionLine(RoomNodeSO parentRoomNode, RoomNodeSO childRoomNode)//实际在父子节点之间绘制节点间连接线的方法
    {
        // get line start and end position
        Vector2 startPosition = parentRoomNode.rect.center;
        Vector2 endPosition = childRoomNode.rect.center;

        // Calulate midway point
        Vector2 midPosition = (endPosition + startPosition) / 2f;

        // vector from start to end position of line
        Vector2 direction = endPosition - startPosition;

        // Calulate normalisedperpendicular positions from the mid point
        Vector2 arrowTailPoint1 = midPosition - new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;
        Vector2 arrowTailPoint2 = midPosition + new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;

        // Calculate mid point offset position for arrow head
        Vector2 arrowHeadPoint = midPosition + direction.normalized * connectingLineArrowSize;

        // Draw Arrow
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, connectingLineWidth);
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null, connectingLineWidth);

        // Draw line
        Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition, Color.white, null, connectingLineWidth);

            GUI.changed = true;
    }

    /// <summary>
    /// Draw room nodes in the graph window
    /// </summary>
    private void DrawRoomNodes()
    {
        // Loop through all room nodes and draw them
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)//循环浏览所有房间节点，添加到节点图中
        {
            if (roomNode.isSelected)
            {
                roomNode.Draw(roomNodeSelectedStyle);
            }
            else
            {
                roomNode.Draw(roomNodeStyle);//绘制房间节点
            }
        }
        GUI.changed = true;
    }

    /// <summary>
    /// Selection changed in the inspector
    /// </summary>
    private void InspectorSelectionChanged()//选择的节点图改变时，更改编辑器中的显示内容
    {
        RoomNodeGraphSO roomNodeGraph = Selection.activeObject as RoomNodeGraphSO;

        if (roomNodeGraph != null)
        {
            currentRoomNodeGraph = roomNodeGraph;
            GUI.changed = true;
        }
    }
}
