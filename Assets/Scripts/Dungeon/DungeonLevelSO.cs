using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevel_", menuName = "Scriptable Objects/Dungeon/Dungeon Level")]
public class DungeonLevelSO : ScriptableObject
{
    #region Header BASIC LEVEL DETAILS

    [Space(10)]
    [Header("BASIC LEVEL DETAILS：基本层级详细信息")]

    #endregion Header BASIC LEVEL DETAILS

    #region Tooltip

    [Tooltip("The name for the level：层级的名称")]

    #endregion Tooltip

    public string levelName;

    #region Header ROOM TEMPLATES FOR LEVEL

    [Space(10)]
    [Header("ROOM TEMPLATES FOR LEVEL：层级房间模板")]

    #endregion Header ROOM TEMPLATES FOR LEVEL

    #region Tooltip

    [Tooltip("Populate the list with the room templates that you want to be part of the level.  You need to ensure that room templates are included for all room node types that are specified in the Room Node Graphs for the level：使用要成为层级一部分的房间模板填充列表。您需要确保在层级的“房间节点图”中指定的所有房间节点类型都包含房间模板。")]

    #endregion Tooltip

    public List<RoomTemplateSO> roomTemplateList;

    #region Header ROOM NODE GRAPHS FOR LEVEL

    [Space(10)]
    [Header("ROOM NODE GRAPHS FOR LEVEL：层级的房间节点图")]

    #endregion Header ROOM NODE GRAPHS FOR LEVEL

    #region Tooltip

    [Tooltip("Populate this list with the room node grpahs which should be randomly selected from for the level：使用房间节点节点填充此列表，该节点应为级别随机选择。")]

    #endregion Tooltip

    public List<RoomNodeGraphSO> roomNodeGraphList;

    #region Validation

#if UNITY_EDITOR

    // Validate scriptable object details enetered：验证插入的可编写脚本的对象详细信息
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(levelName), levelName);
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomTemplateList), roomTemplateList))
            return;
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomNodeGraphList), roomNodeGraphList))
            return;

        // Check to make sure that room templates are specified for all the node types in the specified node graphs
        // 检查以确保为指定节点图中的所有节点类型指定了房间模板

        // First check that north/south corridor, east/west corridor and entrance types have been specified：首先检查是否已指定北/南走廊、东/西走廊和入口类型
        bool isEWCorridor = false;
        bool isNSCorridor = false;
        bool isEntrance = false;

        // Loop through all room templates to check that this node type has been specified：循环浏览所有房间模板以检查是否已指定此节点类型
        foreach (RoomTemplateSO roomTemplateSO in roomTemplateList)
        {
            if (roomTemplateSO == null)
                return;

            if (roomTemplateSO.roomNodeType.isCorridorEW)
                isEWCorridor = true;

            if (roomTemplateSO.roomNodeType.isCorridorNS)
                isNSCorridor = true;

            if (roomTemplateSO.roomNodeType.isEntrance)
                isEntrance = true;
        }

        if (isEWCorridor == false)
        {
            Debug.Log("In " + this.name.ToString() + " : No E/W Corridor Room Type Specified");
        }

        if (isNSCorridor == false)
        {
            Debug.Log("In " + this.name.ToString() + " : No N/S Corridor Room Type Specified");
        }

        if (isEntrance == false)
        {
            Debug.Log("In " + this.name.ToString() + " : No Entrance Corridor Room Type Specified");
        }

        // Loop through all node graphs：遍历所有节点图
        foreach (RoomNodeGraphSO roomNodeGraph in roomNodeGraphList)
        {
            if (roomNodeGraph == null)
                return;

            // Loop through all nodes in node graph：遍历节点图中的所有节点
            foreach (RoomNodeSO roomNodeSO in roomNodeGraph.roomNodeList)
            {
                if (roomNodeSO == null)
                    continue;

                // Check that a room template has been specified for each roomNode type：检查是否已为每个房间节点类型指定了房间模板
                // Corridors and entrance already checked：走廊和入口已检查
                if (roomNodeSO.roomNodeType.isEntrance || roomNodeSO.roomNodeType.isCorridorEW || roomNodeSO.roomNodeType.isCorridorNS || roomNodeSO.roomNodeType.isCorridor || roomNodeSO.roomNodeType.isNone)
                    continue;

                bool isRoomNodeTypeFound = false;

                // Loop through all room templates to check that this node type has been specified：循环浏览所有房间模板以检查是否已指定此节点类型
                foreach (RoomTemplateSO roomTemplateSO in roomTemplateList)
                {

                    if (roomTemplateSO == null)
                        continue;

                    if (roomTemplateSO.roomNodeType == roomNodeSO.roomNodeType)
                    {
                        isRoomNodeTypeFound = true;
                        break;
                    }
                }
                if (!isRoomNodeTypeFound)
                    Debug.Log("In " + this.name.ToString() + " : No room template " + roomNodeSO.roomNodeType.name.ToString() + " found for node graph " + roomNodeGraph.name.ToString());
            }
        }
    }

#endif

    #endregion Validation
}