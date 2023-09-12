using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomNodeSO : ScriptableObject
{
    [HideInInspector] public string id;//����ڵ�id����GUID����
    [HideInInspector] public List<RoomNodeSO> parebtRoomNodeIDList = new List<RoomNodeSO>();
    [HideInInspector] public List<RoomNodeSO> childRoomNodeIDList = new List<RoomNodeSO>();
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    public RoomNodeTypeSO roomNodeType;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;

    #region Editor Code

    // the following code should only be run in the Unity Editor
#if UNITY_EDITOR

    [HideInInspector] public Rect rect;

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

        // Display a popup using the Roomlodetype name values that can be selected from (default to the currently set roomilodetype)

        int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);//���ѡ���ˣ������仯������ʹ��ν��ָ������ڵ����ͣ��б������뷿��ڵ�������ͬ�ģ�

        int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());//����һ���������ڣ���ʾ����ڵ����͵��ַ������飬��ѡ��ķ���ڵ�������������

        roomNodeType = roomNodeTypeList.list[selection];//��������������ѡ�ķ�������

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

#endif

    #endregion Editor Code
}
