using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNodeSO : ScriptableObject
{
    [HideInInspector] public string id;//房间节点id，由GUID生成
    [HideInInspector] public List<RoomNodeSO> parebtRoomNodeIDList = new List<RoomNodeSO>();
    [HideInInspector] public List<RoomNodeSO> childRoomNodeIDList = new List<RoomNodeSO>();
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    public RoomNodeTypeSO roomNodeType;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
}
