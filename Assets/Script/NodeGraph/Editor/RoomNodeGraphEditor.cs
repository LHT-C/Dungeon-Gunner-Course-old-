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

    //Node layout values���ڵ㲼����ֵ��
    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 75;
    private const int nodeBorder = 12;

    // connecting line values
    private const float connectingLineWidth = 3f;
    private const float connectingLineArrowSize = 6f;

    [MenuItem("Room Node Graph Editor",menuItem ="Window/Dungon Editor/Room Node Graph Editor")]//�˵������ԣ�ʹ�༭�����ڳ�����Unity�༭���Ĳ˵���
    private static void OpenWindow()
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");//��ȡ���ڣ�������Ҫ�򿪵ı༭���������ͣ�������
    }

    private void OnEnable()
    {
        // Subscribe to the inspector selection changed event
        Selection.selectionChanged += InspectorSelectionChanged;//����ѡ����ı�ʱ���¼�

        //Define node layout style
        roomNodeStyle = new GUIStyle();//����ڵ㲼�ֵķ��
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;//����Ԥ������ʲ����ڵ�1������unity�༭�����ʹ�ã���Ϊ��������
        roomNodeStyle.normal.textColor = Color.white;//�ı���ɫ
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.padding = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        //Define selected node style
        roomNodeSelectedStyle = new GUIStyle();//���屻ѡ�к�Ľڵ㲼�ֵķ��
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
    public static bool OnDoubleClickAsset(int instanceID, int line)//�ڱ༭���е���ʲ�ʱ����
    {
        RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;//ʹ��ʵ��id����ʵ����

        if (roomNodeGraph != null)//�ڱ༭���е���ڵ�ʱ
        {
            OpenWindow();//�򿪴���

            currentRoomNodeGraph = roomNodeGraph;//ʹ��ǰ�ڵ��Ϊ�ýڵ�

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
        //GUILayout.BeginArea(new Rect(new Vector2(100f, 100f), new Vector2(nodeWidth, nodeHeight)), roomNodeStyle);//��ʼ��������
        //EditorGUILayout.LabelField("Node 1");//��ǩ�ֶ�
        //GUILayout.EndArea();//������������
        //
        //GUILayout.BeginArea(new Rect(new Vector2(300f, 300f), new Vector2(nodeWidth, nodeHeight)), roomNodeStyle);//��ʼ��������
        //EditorGUILayout.LabelField("Node 2");//��ǩ�ֶ�
        //GUILayout.EndArea();//������������
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
                currentRoomNodeGraph.roomNodeToDrawLinefrom.rect.center, currentRoomNodeGraph.linePosition, Color.white, null, connectingLineWidth);//ʵ�ʻ����ߵķ��������������ߵĿ�Ⱥ���ɫ�����ԣ��ߵ���յ��λ�ô�RoomNodeGraphSO��ȡ
        }
    }


    private void ProcessEvents(Event currentEvent)
    {
        // Get room node that mouse is over if it's null or not currently being dragged
        if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)//�������Ƿ��ڷ���ڵ��ϰ����϶�
        {
            currentRoomNode = IsMouseOverRoomNode(currentEvent);//�¼������ص�ǰ����ڵ�
        }

        // if mouse isn't over a room node or we are currently drapzing a line from the room node then process praph events
        if (currentRoomNode == null || currentRoomNodeGraph.roomNodeToDrawLinefrom!= null)//�������Ƿ��Ҽ��϶�
        {
            ProcessRoomNodeGraphEvents(currentEvent);//������ڵ�ͼ���¼�
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
        for (int i = currentRoomNodeGraph.roomNodeList.Count - 1; i >= 0; i--)//�б�ѭ����ȷ����ǰ�ڵ�
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
                ProcessMouseDownEvent(currentEvent);//��갴��ʱ�����¼�
                break;

            // Process Mouse Up Events
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);//���̧��ʱ�����¼�
                break;

            // Process Mouse Drag Events
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);//�����קʱ�����¼�
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
        if (currentEvent.button == 1)//1������Ҽ�
        {
            ShowContextMenu(currentEvent.mousePosition);//��ʾ�����Ĳ˵�
        }
        // Process left mouse down on graph event
        else if (currentEvent.button == 0)//0��������
        {
            ClearLineDrag();
            ClearAllSelectedRoomNodes();
        }
    }

    /// <summary>
    /// Show the context menu
    /// </summary>
    private void ShowContextMenu(Vector2 mousePosition)//�������λ��
    {
        GenericMenu menu = new GenericMenu();//����һ���µĲ˵�

        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);//���˵��ﴫ�뺯��������״̬���˵�λ��Ϊ���λ��

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
            CreateRoomNode(new Vector2(200f, 200f), roomNodeTypeList.list.Find(x => x.isEntrance));//������һ���ڵ�ʱ�����ⴴ��һ����ڽڵ�
        }

        CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));//ν�ʣ�Ѱ�����ѡ��ķ���ڵ����ͣ�������������غ���
    }

    /// <summary>
    /// Create a room node at the mouse position - overloaded to also pass in RoomNodeType
    /// </summary>
    private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;

        // create room node scriptable object asset
        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();//��������ڵ�ʵ��

        // add room node to current room node graph room node list
        currentRoomNodeGraph.roomNodeList.Add(roomNode);//�������ķ���ڵ�ʵ�����浽�б�

        //set room node values
        roomNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph, roomNodeType);//��ʼ������ڵ�

        // add room node to roon node graph scriptable object asset database
        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);//�������ķ���ڵ�͵�ǰ�ķ���ڵ�ͼ��ӵ��ʲ�

        AssetDatabase.SaveAssets();//�����ʲ�

        // Reflesh graph node dictonary
        currentRoomNodeGraph.OnValidate();
    }

    /// <summary>
    /// Clear selection from all room nodes
    /// </summary>
    private void ClearAllSelectedRoomNodes()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)//�������λ��ʱ��ȡ��ѡ�нڵ�
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

            if (roomNode != null)//���������˵ķ���ڵ㣬�������ӽڵ��ϵ�������б���id��
            {
                // if so set it as a child of the parent room node if it can be added
                if (currentRoomNodeGraph.roomNodeToDrawLinefrom.AddChildRoomNodeIDToRoomNode(roomNode.id))
                {
                    // Set parent ID in child room node
                    roomNode.AddParentRoomNodeIDToRoomNode(currentRoomNodeGraph.roomNodeToDrawLinefrom.id);
                }
            }

            ClearLineDrag();//�����Ч�϶���
        }
    }

    /// <summary>
    /// Process mouse drag event
    /// </summary>
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        // process right click drag event - draw line
        if (currentEvent.button == 1)//���
        {
            ProcessRightMouseDragEvent(currentEvent);
        }
    }

    /// <summary>
    /// Process right mouse drag event - draw line
    /// </summary>
    private void ProcessRightMouseDragEvent(Event currentEvent)
    {
        if (currentRoomNodeGraph.roomNodeToDrawLinefrom != null)//ִ�л��߷�����gui״̬��Ϊ�ı�
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
        currentRoomNodeGraph.linePosition += delta;//�������λ�øı���λ��
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
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)//ѭ�����з���ڵ㣬����key���ֵ��л�ȡ�����ӷ���ڵ����Ϣ��ȷʵΪ�ӷ���ڵ��򻭳�������
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
    private void DrawConnectionLine(RoomNodeSO parentRoomNode, RoomNodeSO childRoomNode)//ʵ���ڸ��ӽڵ�֮����ƽڵ�������ߵķ���
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
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)//ѭ��������з���ڵ㣬��ӵ��ڵ�ͼ��
        {
            if (roomNode.isSelected)
            {
                roomNode.Draw(roomNodeSelectedStyle);
            }
            else
            {
                roomNode.Draw(roomNodeStyle);//���Ʒ���ڵ�
            }
        }
        GUI.changed = true;
    }

    /// <summary>
    /// Selection changed in the inspector
    /// </summary>
    private void InspectorSelectionChanged()//ѡ��Ľڵ�ͼ�ı�ʱ�����ı༭���е���ʾ����
    {
        RoomNodeGraphSO roomNodeGraph = Selection.activeObject as RoomNodeGraphSO;

        if (roomNodeGraph != null)
        {
            currentRoomNodeGraph = roomNodeGraph;
            GUI.changed = true;
        }
    }
}
