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

    //Node layout values���ڵ㲼����ֵ��
    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 75;
    private const int nodeBorder = 12;

    [MenuItem("Room Node Graph Editor",menuItem ="Window/Dungon Editor/Room Node Graph Editor")]//�˵������ԣ�ʹ�༭�����ڳ�����Unity�༭���Ĳ˵���
    private static void OpenWindow()
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");//��ȡ���ڣ�������Ҫ�򿪵ı༭���������ͣ�������
    }

    private void OnEnable()//����ڵ㲼�ֵķ��
    {
        //Define node layout style
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;//����Ԥ������ʲ����ڵ�1������unity�༭�����ʹ�ã���Ϊ��������
        roomNodeStyle.normal.textColor = Color.white;//�ı���ɫ
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
        if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)//�������Ƿ��ڷ���ڵ��ϰ����϶�
        {
            currentRoomNode = IsMouseOverRoomNode(currentEvent);//�¼������ص�ǰ����ڵ�
        }

        // if mouse isn't over a room node
        if (currentRoomNode == null)
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
        CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));//ν�ʣ�Ѱ�����ѡ��ķ���ڵ����ͣ�������������غ���
    }

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
    }

    /// <summary>
    /// Draw room nodes in the graph window
    /// </summary>
    private void DrawRoomNodes()
    {
        // Loop through all room nodes and draw them
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)//ѭ��������з���ڵ㣬��ӵ��ڵ�ͼ��
        {
            roomNode.Draw(roomNodeStyle);//���Ʒ���ڵ�
        }
        GUI.changed = true;
    }
}
