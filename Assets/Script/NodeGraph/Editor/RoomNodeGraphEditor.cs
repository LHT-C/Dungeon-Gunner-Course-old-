using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;

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

    }

    /// <summary>
    /// Draw Editor Gui
    /// </summary>
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(new Vector2(100f, 100f), new Vector2(nodeWidth, nodeHeight)), roomNodeStyle);//开始布局区域
        EditorGUILayout.LabelField("Node 1");//标签字段
        GUILayout.EndArea();//结束布局区域

        GUILayout.BeginArea(new Rect(new Vector2(300f, 300f), new Vector2(nodeWidth, nodeHeight)), roomNodeStyle);//开始布局区域
        EditorGUILayout.LabelField("Node 2");//标签字段
        GUILayout.EndArea();//结束布局区域
    }
}
