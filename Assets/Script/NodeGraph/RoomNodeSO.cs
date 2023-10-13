using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomNodeSO : ScriptableObject
{
    [HideInInspector] public string id;//����ڵ�id����GUID����
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

    public void Initialise(Rect rect, RoomNodeGraphSO nodeGraph, RoomNodeTypeSO roomNodeType)//���䴴��ʱ������һ���ڵ�ͼ��һ����������
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();//����һ��Ψһ��GUID
        this.name = "RoomNode";
        this.roomNodeGraph = nodeGraph;
        this.roomNodeType = roomNodeType;

        // Load room node type list
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;//ʹ����Ϸ��Դ�����ط���ڵ������б���Ա������
    }

    public void Draw(GUIStyle nodeStyle)//���Ʒ���
    {
        // Draw Node Box Using Begin Area
        GUILayout.BeginArea(rect, nodeStyle);//���ƽڵ��

        // Start Region To Detect Popup Selection Changes
        EditorGUI.BeginChangeCheck();//���仯

        //if the room node has a parent or is of type entrance then display a label else display a popup
        if (parentRoomNodeIDList.Count > 0 || roomNodeType.isEntrance)
        {
            // Display a label that can't be changed
            EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);//���ڵ����ߺ󣬲����ٸ��Ľڵ㷿������
        }

        else
        {
            // Display a popup using the Roomlodetype name values that can be selected from (default to the currently set roomilodetype)
            int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);//���ѡ���ˣ������仯������ʹ��ν��ָ������ڵ����ͣ��б������뷿��ڵ�������ͬ�ģ�
            int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());//����һ���������ڣ���ʾ����ڵ����͵��ַ������飬��ѡ��ķ���ڵ�������������
            roomNodeType = roomNodeTypeList.list[selection];//��������������ѡ�ķ�������

            // If the room type selection has changed making child connections potentially invalid
            if (roomNodeTypeList.list[selected].isCorridor && !roomNodeTypeList.list[selection].isCorridor 
                || !roomNodeTypeList.list[selected].isCorridor && roomNodeTypeList.list[selection].isCorridor 
                || !roomNodeTypeList.list[selected].isBossRoom && roomNodeTypeList.list[selection].isBossRoom)//ѡ��ķ��䷢���仯�󣬽���Լ��
            {
                // if a room node type has been chanped and it already has children then delete the parent child links since we need to revalidate any
                if (childRoomNodeIDList.Count > 0)//�ӽڵ�ĸ��ڵ㱻ɾ���󣬶Ըýڵ��������
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
            EditorUtility.SetDirty(this);//�仯�������б仯����SetDirty�����������ı仯

        GUILayout.EndArea();
    }

    /// <summary>
    /// Populate a string array with the room node types to display that can be selected
    /// </summary>
    public string[] GetRoomNodeTypesToDisplay()
    {
        string[] roomArray = new string[roomNodeTypeList.list.Count];//��������Ϊ�ڵ����ͳ���һ�µĿ�����

        for (int i = 0; i < roomNodeTypeList.list.Count; i++)//ѭ�������ڵ����ͼ�������
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
        switch (currentEvent.type)//ȷ���༭���з�����ʲô���͵Ľ��������¡��ɿ����϶���
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
    private void ProcessMouseDownEvent(Event currentEvent)//��갴���¼�
    {
        // left click down
        if (currentEvent.button == 0)
        {
            ProcessLeftClickDownEvent();//�����갴�£��Ƿ�Ϊ���0��
        }
        else if(currentEvent.button == 1)
        {
            ProcessRightClickDownEvent(currentEvent);//�����갴�£��Ƿ�Ϊ�Ҽ�1��
        }
    }

    /// <summary>
    /// Process left click down event
    /// </summary>
    private void ProcessLeftClickDownEvent()
    {
        //Selection.activeObject = this;

        // Toggle node selection
        if (isSelected == true)//�л��Ƿ�ѡ��
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
    private void ProcessRightClickDownEvent(Event currentEvent)//�Ҽ������¼�
    {
        roomNodeGraph.SetNodeToDrawConnectionLinefrom(this, currentEvent.mousePosition);//�����λ�û���
    }

    /// <summary>
    /// Process mouse up events
    /// </summary>
    private void ProcessMouseUpEvent(Event currentEvent)//���̧���¼�
    {
        // left click up
        if (currentEvent.button == 0)
        {
            ProcessLeftClickUpEvent();//������̧���Ƿ�Ϊ���0��
        }
    }

    /// <summary>
    /// Process left click up event
    /// </summary>
    private void ProcessLeftClickUpEvent()
    {
        if (isLeftClickDragging)//�ͷ��϶�״̬
        {
            isLeftClickDragging = false;
        }
    }

    /// <summary>
    /// Process mouse up events
    /// </summary>
    private void ProcessMouseDragEvent(Event currentEvent)//����϶��¼�
    {
        // left click up
        if (currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent);//�������϶����Ƿ�Ϊ���0��
        }
    }

    /// <summary>
    /// Process left Mouse Drag event
    /// </summary>
    private void ProcessLeftMouseDragEvent(Event currentEvent)
    {
        isLeftClickDragging = true;//�����϶��¼�Ϊtrue

        DragNode(currentEvent.delta);//���뵱ǰ���λ��
        GUI.changed = true;
    }

    /// <summary>
    /// Drag node
    /// </summary>
    public void DragNode(Vector2 delta)//���ƽڵ�λ��
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
        if (IsChildRoomValid(childID))//����ӽڵ�id�Ƿ���Ч
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
        bool isConnectedBossNodeAlready = false;//���boss���Ƿ��ѱ�����
        // Check if there is there already a connected boss room in the node graph
        foreach (RoomNodeSO roomNode in roomNodeGraph.roomNodeList)
        {
            if (roomNode.roomNodeType.isBossRoom && roomNode.parentRoomNodeIDList.Count > 0)//ÿ��ֻ��Ҫһ��boss����
                isConnectedBossNodeAlready = true;
        }

        // if the child node has a type of boss room and there is already a connected boss room node then return false
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isBossRoom && isConnectedBossNodeAlready)//����ӽڵ���boss���������䲻���ٱ�����
            return false;

        // If the child node has a type of none then return false
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isNone)//ȷ���ӽڵ��Ƿ�Ϊ��
           return false;

        // If the node already has a child with this child ID return false
        if (childRoomNodeIDList.Contains(childID))//ȷ�����ӽڵ㲻�ᱻ�ظ�����
            return false;

        // If this node ID and the child ID are the same return false
        if (id == childID)//�ӽڵ㲻����������
            return false;

        // If the node already has a child with this parent ID return false
        if (parentRoomNodeIDList.Contains(childID))//ȷ�����ӽڵ㲢δ����Ϊ���ڵ�
            return false;

        // If the child node already has a parent return false
        if (roomNodeGraph.GetRoomNode(childID).parentRoomNodeIDList.Count > 0)//ȷ���ӽڵ�ֻ��һ�����ڵ�
            return false;

        // If child is a corridor and this node is a corridor return false
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && roomNodeType.isCorridor)//���������Ȳ�������
            return false;

        // If child is not a corridor and this node is not a corridor return false
        if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && !roomNodeType.isCorridor)//�����뷿�䲻��ֱ������
            return false;

        // If adding a corridor check that this node has < the maximum permitted child corridors
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count >= Settings.maxChildCorridors)//ȷ����������Լ����Χ��
            return false;

        // if the child room is an entrance return false - the entrance must always be the top level parent node
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isEntrance)//��ڷ��䲻����Ϊ�ӽڵ�
            return false;

        // If adding a room to a corridor check that this corridor node doesn't already have a room added
        if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count > 0)//ȷ�����Ⱥ�ֻ����һ������
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
    public bool RemoveChildRoomNodeIDFromRoomNode(string childID)//ɾ���ӽڵ�id
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
    public bool RemoveParentRoomNodeIDFromRoomNode(string parentID)//ɾ�����ڵ�id
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
