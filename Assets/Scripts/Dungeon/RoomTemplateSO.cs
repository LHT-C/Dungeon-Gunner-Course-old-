using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Room_", menuName = "Scriptable Objects/Dungeon/Room")]
public class RoomTemplateSO : ScriptableObject
{
    [HideInInspector] public string guid;

    #region Header ROOM PREFAB

    [Space(10)]
    [Header("ROOM PREFAB：房间预制件")]

    #endregion Header ROOM PREFAB

    #region Tooltip

    [Tooltip("The gameobject prefab for the room (this will contain all the tilemaps for the room and environment game objects：房间的游戏对象预制件（这将包含房间和环境游戏对象的所有瓷砖贴图")]

    #endregion Tooltip

    public GameObject prefab;

    [HideInInspector] public GameObject previousPrefab; // this is used to regenerate the guid if the so is copied and the prefab is changed：如果复制了so并更改了预制件，则此选项用于重新生成guid


    #region Header ROOM CONFIGURATION

    [Space(10)]
    [Header("ROOM CONFIGURATION：房间配置")]

    #endregion Header ROOM CONFIGURATION

    #region Tooltip

    [Tooltip("The room node type SO. The room node types correspond to the room nodes used in the room node graph.  The exceptions being with corridors.  In the room node graph there is just one corridor type 'Corridor'.  For the room templates there are 2 corridor node types - CorridorNS and CorridorEW：房间节点类型SO。房间节点类型对应于房间节点图中使用的房间节点。走廊除外。在房间节点图中，只有一种走廊类型“走廊”。对于房间样板，有两种道路节点类型-道路NS和道路EW。")]

    #endregion Tooltip

    public RoomNodeTypeSO roomNodeType;

    #region Tooltip

    [Tooltip("If you imagine a rectangle around the room tilemap that just completely encloses it, the room lower bounds represent the bottom left corner of that rectangle. This should be determined from the tilemap for the room (using the coordinate brush pointer to get the tilemap grid position for that bottom left corner (Note: this is the local tilemap position and NOT world position：如果在房间瓷砖贴图周围想象一个完全包围它的矩形，则房间下限表示该矩形的左下角。这应该根据房间的瓷砖贴图来确定（使用坐标刷指针来获得左下角的瓷砖贴图网格位置）（注意：这是局部瓷砖贴图位置，而不是世界位置）")]

    #endregion Tooltip

    public Vector2Int lowerBounds;

    #region Tooltip

    [Tooltip("If you imagine a rectangle around the room tilemap that just completely encloses it, the room upper bounds represent the top right corner of that rectangle. This should be determined from the tilemap for the room (using the coordinate brush pointer to get the tilemap grid position for that top right corner (Note: this is the local tilemap position and NOT world position：如果在房间瓷砖贴图周围想象一个完全包围它的矩形，则房间上限表示该矩形的右上角。这应该根据房间的瓷砖贴图来确定（使用坐标刷指针来获得右上角的瓷砖贴图网格位置）（注意：这是局部瓷砖贴图位置，而不是世界位置）")]

    #endregion Tooltip

    public Vector2Int upperBounds;

    #region Tooltip

    [Tooltip("There should be a maximum of four doorways for a room - one for each compass direction.  These should have a consistent 3 tile opening size, with the middle tile position being the doorway coordinate 'position'：一个房间最多应该有四个门口，每个指南针方向一个。这些应具有一致的3瓷砖开口尺寸，中间瓷砖位置为门口坐标“位置”")]

    #endregion Tooltip

    [SerializeField] public List<Doorway> doorwayList;

    #region Tooltip

    [Tooltip("Each possible spawn position (used for enemies and chests) for the room in tilemap coordinates should be added to this array：瓷砖贴图坐标中房间的每个可能的刷怪位置（用于敌人和箱子）都应该添加到此阵列中")]

    #endregion Tooltip

    public Vector2Int[] spawnPositionArray;

    /// <summary>
    /// Returns the list of Entrances for the room template：返回房间模板的入口列表
    /// </summary>
    public List<Doorway> GetDoorwayList()
    {
        return doorwayList;
    }

    #region Validation

#if UNITY_EDITOR

    // Validate SO fields
    private void OnValidate()
    {
        // Set unique GUID if empty or the prefab changes
        if (guid == "" || previousPrefab != prefab)
        {
            guid = GUID.Generate().ToString();
            previousPrefab = prefab;
            EditorUtility.SetDirty(this);
        }

        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(doorwayList), doorwayList);

        // Check spawn positions populated
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(spawnPositionArray), spawnPositionArray);
    }

#endif

    #endregion Validation
}