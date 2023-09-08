using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;

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

    }

    /// <summary>
    /// Draw Editor Gui
    /// </summary>
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(new Vector2(100f, 100f), new Vector2(nodeWidth, nodeHeight)), roomNodeStyle);//��ʼ��������
        EditorGUILayout.LabelField("Node 1");//��ǩ�ֶ�
        GUILayout.EndArea();//������������

        GUILayout.BeginArea(new Rect(new Vector2(300f, 300f), new Vector2(nodeWidth, nodeHeight)), roomNodeStyle);//��ʼ��������
        EditorGUILayout.LabelField("Node 2");//��ǩ�ֶ�
        GUILayout.EndArea();//������������
    }
}
