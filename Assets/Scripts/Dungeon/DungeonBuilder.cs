using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
public class DungeonBuilder : SingletonMonobehaviour<DungeonBuilder>
{
    public Dictionary<string, Room> dungeonBuilderRoomDictionary = new Dictionary<string, Room>();
    private Dictionary<string, RoomTemplateSO> roomTemplateDictionary = new Dictionary<string, RoomTemplateSO>();
    private List<RoomTemplateSO> roomTemplateList = null;
    private RoomNodeTypeListSO roomNodeTypeList;
    private bool dungeonBuildSuccessful;

    private void OnEnable()
    {
        // Set dimmed material to off：将暗淡的材质设置为关闭
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 0f);//设置房间alpha值（亮度）
    }

    private void OnDisable()
    {
        // Set dimmed material to fully visible：将变暗的材质设置为完全可见
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 1f);
    }

    protected override void Awake()
    {
        base.Awake();

        // Load the room node type list：加载房间节点类型列表
        LoadRoomNodeTypeList();
    }

    /// <summary>
    /// Load the room node type list：加载房间节点类型列表
    /// </summary>
    private void LoadRoomNodeTypeList()
    {
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    /// <summary>
    /// Generate random dungeon, returns true if dungeon built, false if failed：生成随机地牢，如果地牢已建立则返回true，如果失败则返回false
    /// </summary>
    public bool GenerateDungeon(DungeonLevelSO currentDungeonLevel)
    {
        roomTemplateList = currentDungeonLevel.roomTemplateList;

        // Load the scriptable object room templates into the dictionary：将可编写脚本的对象房间模板加载到字典中
        LoadRoomTemplatesIntoDictionary();

        dungeonBuildSuccessful = false;
        int dungeonBuildAttempts = 0;

        while (!dungeonBuildSuccessful && dungeonBuildAttempts < Settings.maxDungeonBuildAttempts)//重复尝试生成地牢
        {
            dungeonBuildAttempts++;

            // Select a random room node graph from the list：从列表中选择随机房间节点图
            RoomNodeGraphSO roomNodeGraph = SelectRandomRoomNodeGraph(currentDungeonLevel.roomNodeGraphList);

            int dungeonRebuildAttemptsForNodeGraph = 0;
            dungeonBuildSuccessful = false;

            // Loop until dungeon successfully built or more than max attempts for node graph：循环直到地牢成功构建或节点图的最大尝试次数超过为止
            while (!dungeonBuildSuccessful && dungeonRebuildAttemptsForNodeGraph <= Settings.maxDungeonRebuildAttemptsForRoomGraph)//节点生成失败时，清空地牢中的节点
            {
                // Clear dungeon room gameobjects and dungeon room dictionary：清除地牢房间游戏对象和地牢房间词典
                ClearDungeon();

                dungeonRebuildAttemptsForNodeGraph++;

                // Attempt To Build A Random Dungeon For The Selected room node graph：尝试为所选房间节点图构建随机地牢
                dungeonBuildSuccessful = AttemptToBuildRandomDungeon(roomNodeGraph);//尝试重建地牢房间节点
            }


            if (dungeonBuildSuccessful)
            {
                // Instantiate Room Gameobjects：实例化房间游戏对象
                InstantiateRoomGameobjects();
            }
        }

        return dungeonBuildSuccessful;
    }

    /// <summary>
    /// Load the room templates into the dictionary：将房间模板加载到词典中
    /// </summary>
    private void LoadRoomTemplatesIntoDictionary()
    {
        // Clear room template dictionary：清除房间模板词典
        roomTemplateDictionary.Clear();

        // Load room template list into dictionary：将所有房间模板加入字典，如果重复则用控制台提示
        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
        {
            if (!roomTemplateDictionary.ContainsKey(roomTemplate.guid))
            {
                roomTemplateDictionary.Add(roomTemplate.guid, roomTemplate);
            }
            else
            {
                Debug.Log("Duplicate Room Template Key In " + roomTemplateList);
            }
        }
    }

    /// <summary>
    /// Attempt to randomly build the dungeon for the specified room nodeGraph. Returns true if a successful random layout was generated, else returns false if a problem was encoutered and another attempt is required.
    /// 尝试为指定的房间nodeGraph随机构建地牢。如果生成了成功的随机布局，则返回true；如果出现问题并需要再次尝试，则返回false。
    /// </summary>
    private bool AttemptToBuildRandomDungeon(RoomNodeGraphSO roomNodeGraph)//假设能够正常生成节点（没有重叠）的情况下，开始建立房间节点，并记录重叠次数，进行判定
    {

        // Create Open Room Node Queue：创建打开的房间节点队列
        Queue<RoomNodeSO> openRoomNodeQueue = new Queue<RoomNodeSO>();

        // Add Entrance Node To Room Node Queue From Room Node Graph：从房间节点图将入口节点添加到房间节点队列
        RoomNodeSO entranceNode = roomNodeGraph.GetRoomNode(roomNodeTypeList.list.Find(x => x.isEntrance));

        if (entranceNode != null)
        {
            openRoomNodeQueue.Enqueue(entranceNode);
        }
        else
        {
            Debug.Log("No Entrance Node");
            return false;  // Dungeon Not Built：未建造的地牢
        }

        // Start with no room overlaps：从没有房间重叠开始
        bool noRoomOverlaps = true;


        // Process open room nodes queue：处理打开的房间节点队列
        noRoomOverlaps = ProcessRoomsInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue, noRoomOverlaps);

        // If all the room nodes have been processed and there hasn't been a room overlap then return true：如果所有房间节点都已处理，并且没有房间重叠，则返回true
        if (openRoomNodeQueue.Count == 0 && noRoomOverlaps)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    /// <summary>
    /// Process rooms in the open room node queue, returning true if there are no room overlaps：处理开放房间节点队列中的房间，如果没有房间重叠，则返回true
    /// </summary>
    private bool ProcessRoomsInOpenRoomNodeQueue(RoomNodeGraphSO roomNodeGraph, Queue<RoomNodeSO> openRoomNodeQueue, bool noRoomOverlaps)//生成房间，循环遍历子节点，根据节点类型进行处理
    {

        // While room nodes in open room node queue & no room overlaps detected：当打开的房间节点队列中的房间节点时&未检测到房间重叠。
        while (openRoomNodeQueue.Count > 0 && noRoomOverlaps == true)
        {
            // Get next room node from open room node queue：从打开的房间节点队列中获取下一个房间节点。
            RoomNodeSO roomNode = openRoomNodeQueue.Dequeue();

            // Add child Nodes to queue from room node graph (with links to this parent Room)：将子节点添加到文件室节点图中的队列（带有指向此父文件室的链接）
            foreach (RoomNodeSO childRoomNode in roomNodeGraph.GetChildRoomNodes(roomNode))
            {
                openRoomNodeQueue.Enqueue(childRoomNode);
            }

            // if the room is the entrance mark as positioned and add to room dictionary：如果房间是定位的入口标记，则添加到房间词典中
            if (roomNode.roomNodeType.isEntrance)
            {
                RoomTemplateSO roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);

                Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

                room.isPositioned = true;

                // Add room to room dictionary：将房间添加到房间词典
                dungeonBuilderRoomDictionary.Add(room.id, room);
            }

            // else if the room type isn't an entrance：否则，如果房间不是入口
            else
            {
                // Else get parent room for node：否则为节点获取父空间
                Room parentRoom = dungeonBuilderRoomDictionary[roomNode.parentRoomNodeIDList[0]];

                // See if room can be placed without overlaps：查看房间是否可以无重叠放置
                noRoomOverlaps = CanPlaceRoomWithNoOverlaps(roomNode, parentRoom);
            }
        }
        return noRoomOverlaps;
    }


    /// <summary>
    /// Attempt to place the room node in the dungeon - if room can be placed return the room, else return null：尝试将房间节点放置在地牢中-如果可以放置房间，则返回房间，否则返回null
    /// </summary>
    private bool CanPlaceRoomWithNoOverlaps(RoomNodeSO roomNode, Room parentRoom)//处理房间出现重叠的情况
    {

        // initialise and assume overlap until proven otherwise.：初始化并假设房间有重叠，直到证明不存在重叠。
        bool roomOverlaps = true;

        // Do While Room Overlaps - try to place against all available doorways of the parent until the room is successfully placed without overlap.
        // 在房间重叠时执行-尝试靠着父对象的所有可用门口放置，直到房间成功放置而不重叠为止。
        while (roomOverlaps)
        {
            // Select random unconnected available doorway for Parent：随机选择父房间的未连接的可用门口
            List<Doorway> unconnectedAvailableParentDoorways = GetUnconnectedAvailableDoorways(parentRoom.doorWayList).ToList();

            if (unconnectedAvailableParentDoorways.Count == 0)
            {
                // If no more doorways to try then overlap failure：如果没有更多的门口可以尝试，那么重叠失败。
                return false; // room overlaps：房间重叠
            }

            Doorway doorwayParent = unconnectedAvailableParentDoorways[UnityEngine.Random.Range(0, unconnectedAvailableParentDoorways.Count)];//将两个未连接的门洞随机进行合理连接（通过房间类型和父子节点关系判断）

            // Get a random room template for room node that is consistent with the parent door orientation：//为房间节点获取与父门方向一致的随机房间模板
            RoomTemplateSO roomtemplate = GetRandomTemplateForRoomConsistentWithParent(roomNode, doorwayParent);

            // Create a room：创建房间
            Room room = CreateRoomFromRoomTemplate(roomtemplate, roomNode);

            // Place the room - returns true if the room doesn't overlap：放置房间-如果房间没有重叠，则返回true
            if (PlaceTheRoom(parentRoom, doorwayParent, room))
            {
                // If room doesn't overlap then set to false to exit while loop：如果房间没有重叠，则设置为false以退出while循环
                roomOverlaps = false;

                // Mark room as positioned：将房间标记为已定位
                room.isPositioned = true;

                // Add room to dictionary：将房间添加到词典
                dungeonBuilderRoomDictionary.Add(room.id, room);
            }
            else
            {
                roomOverlaps = true;
            }
        }

        return true;  // no room overlaps：无房间重叠

    }

    /// <summary>
    /// Get random room template for room node taking into account the parent doorway orientation.：为房间节点获取随机房间模板，同时考虑父门口方向。
    /// </summary>
    private RoomTemplateSO GetRandomTemplateForRoomConsistentWithParent(RoomNodeSO roomNode, Doorway doorwayParent)
    {
        RoomTemplateSO roomtemplate = null;

        // If room node is a corridor then select random correct Corridor room template based on parent doorway orientation
        // 如果房间节点是道路，则根据父门口方向随机选择正确的道路房间模板
        if (roomNode.roomNodeType.isCorridor)
        {
            switch (doorwayParent.orientation)
            {
                case Orientation.north:
                case Orientation.south:
                    roomtemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorNS));
                    break;


                case Orientation.east:
                case Orientation.west:
                    roomtemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorEW));
                    break;


                case Orientation.none:
                    break;

                default:
                    break;
            }
        }
        // Else select random room template：否则选择随机房间模板
        else
        {
            roomtemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
        }
        return roomtemplate;
    }

    /// <summary>
    /// Place the room - returns true if the room doesn't overlap, false otherwise：放置房间-如果房间没有重叠，则返回true，否则返回fals
    /// </summary>
    private bool PlaceTheRoom(Room parentRoom, Doorway doorwayParent, Room room)//实际根据位置放置房间的方法
    {

        // Get current room doorway position：获取当前房间门口位置
        Doorway doorway = GetOppositeDoorway(doorwayParent, room.doorWayList);

        // Return if no doorway in room opposite to parent doorway：如果父门口对面的房间没有门口，则返回
        if (doorway == null)
        {
            // Just mark the parent doorway as unavailable so we don't try and connect it again：将父门口标记为不可用，这样我们就不会再尝试连接它
            doorwayParent.isUnavailable = true;

            return false;
        }

        // Calculate 'world' grid parent doorway position：计算“世界”网格中父门口位置
        Vector2Int parentDoorwayPosition = parentRoom.lowerBounds + doorwayParent.position - parentRoom.templateLowerBounds;

        Vector2Int adjustment = Vector2Int.zero;

        // Calculate adjustment position offset based on room doorway position that we are trying to connect (e.g. if this doorway is west then we need to add (1,0) to the east parent doorway)
        // 根据我们试图连接的房间门口位置计算调整位置偏移（例如，如果这个门口在西面，那么我们需要将（1,0）添加到东面的父门口）

        switch (doorway.orientation)
        {
            case Orientation.north:
                adjustment = new Vector2Int(0, -1);
                break;

            case Orientation.east:
                adjustment = new Vector2Int(-1, 0);
                break;

            case Orientation.south:
                adjustment = new Vector2Int(0, 1);
                break;

            case Orientation.west:
                adjustment = new Vector2Int(1, 0);
                break;

            case Orientation.none:
                break;

            default:
                break;
        }

        // Calculate room lower bounds and upper bounds based on positioning to align with parent doorway：通过计算坐标来对齐连接
        room.lowerBounds = parentDoorwayPosition + adjustment + room.templateLowerBounds - doorway.position;
        room.upperBounds = room.lowerBounds + room.templateUpperBounds - room.templateLowerBounds;

        Room overlappingRoom = CheckForRoomOverlap(room);

        if (overlappingRoom == null)
        {
            // mark doorways as connected & unavailable：将门口标记为已连接和不可用
            doorwayParent.isConnected = true;
            doorwayParent.isUnavailable = true;

            doorway.isConnected = true;
            doorway.isUnavailable = true;

            // return true to show rooms have been connected with no overlap：返回true以显示房间已连接且没有重叠
            return true;
        }
        else
        {
            // Just mark the parent doorway as unavailable so we don't try and connect it again：只需将父门口标记为不可用，这样我们就不会再尝试连接它
            doorwayParent.isUnavailable = true;

            return false;
        }
    }

    /// <summary>
    /// Get the doorway from the doorway list that has the opposite orientation to doorway：从门口列表中获取与门口方向相反的门口
    /// </summary>
    private Doorway GetOppositeDoorway(Doorway parentDoorway, List<Doorway> doorwayList)//检查门洞方向方法
    {

        foreach (Doorway doorwayToCheck in doorwayList)
        {
            if (parentDoorway.orientation == Orientation.east && doorwayToCheck.orientation == Orientation.west)
            {
                return doorwayToCheck;
            }
            else if (parentDoorway.orientation == Orientation.west && doorwayToCheck.orientation == Orientation.east)
            {
                return doorwayToCheck;
            }
            else if (parentDoorway.orientation == Orientation.north && doorwayToCheck.orientation == Orientation.south)
            {
                return doorwayToCheck;
            }
            else if (parentDoorway.orientation == Orientation.south && doorwayToCheck.orientation == Orientation.north)
            {
                return doorwayToCheck;
            }
        }
        return null;
    }

    /// <summary>
    /// Check for rooms that overlap the upper and lower bounds parameters, and if there are overlapping rooms then return room else return null
    /// 检查是否有与上限和下限参数重叠的房间，如果有重叠的房间则返回room-else返回null
    /// </summary>
    private Room CheckForRoomOverlap(Room roomToTest)
    {
        // Iterate through all rooms：遍历所有房间
        foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBuilderRoomDictionary)
        {
            Room room = keyvaluepair.Value;

            // skip if same room as room to test or room hasn't been positioned：如果与要测试的房间相同的房间或房间尚未定位，则跳过
            if (room.id == roomToTest.id || !room.isPositioned)
                continue;

            // If room overlaps：如果房间重叠
            if (IsOverLappingRoom(roomToTest, room))
            {
                return room;
            }
        }
        // Return
        return null;
    }


    /// <summary>
    /// Check if 2 rooms overlap each other - return true if they overlap or false if they don't overlap：检查两个房间是否重叠-如果重叠则返回true，如果不重叠则返回false
    /// </summary>
    private bool IsOverLappingRoom(Room room1, Room room2)
    {
        bool isOverlappingX = IsOverLappingInterval(room1.lowerBounds.x, room1.upperBounds.x, room2.lowerBounds.x, room2.upperBounds.x);

        bool isOverlappingY = IsOverLappingInterval(room1.lowerBounds.y, room1.upperBounds.y, room2.lowerBounds.y, room2.upperBounds.y);

        if (isOverlappingX && isOverlappingY)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Check if interval 1 overlaps interval 2 - this method is used by the IsOverlappingRoom method：检查间隔1是否与间隔2重叠-IsOverlappingRoom方法使用此方法
    /// </summary>
    private bool IsOverLappingInterval(int imin1, int imax1, int imin2, int imax2)
    {
        if (Mathf.Max(imin1, imin2) <= Mathf.Min(imax1, imax2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Get a random room template from the roomtemplatelist that matches the roomType and return it (return null if no matching room templates found).
    /// 从roomtemplatelist中获取与roomType匹配的随机房间模板并返回（如果找不到匹配的房间模板，则返回null）
    /// </summary>
    private RoomTemplateSO GetRandomRoomTemplate(RoomNodeTypeSO roomNodeType)
    {
        List<RoomTemplateSO> matchingRoomTemplateList = new List<RoomTemplateSO>();

        // Loop through room template list：循环浏览房间模板列表
        foreach (RoomTemplateSO roomTemplate in roomTemplateList)//将模板添加到列表
        {
            // Add matching room templates：添加匹配的房间模板
            if (roomTemplate.roomNodeType == roomNodeType)
            {
                matchingRoomTemplateList.Add(roomTemplate);
            }
        }

        // Return null if list is zero：如果列表为零，则返回null
        if (matchingRoomTemplateList.Count == 0)
            return null;

        // Select random room template from list and return：从列表中随机选择房间模板并返回
        return matchingRoomTemplateList[UnityEngine.Random.Range(0, matchingRoomTemplateList.Count)];//从列表中随机获得房间模板
    }

    /// <summary>
    /// Get unconnected doorways：获取未连接的门口
    /// </summary>
    private IEnumerable<Doorway> GetUnconnectedAvailableDoorways(List<Doorway> roomDoorwayList)
    {
        // Loop through doorway list：循环门口列表
        foreach (Doorway doorway in roomDoorwayList)
        {
            if (!doorway.isConnected && !doorway.isUnavailable)
                yield return doorway;
        }
    }

    /// <summary>
    /// Create room based on roomTemplate and layoutNode, and return the created room：基于roomTemplate和layoutNode创建房间，并返回创建的房间
    /// </summary>
    private Room CreateRoomFromRoomTemplate(RoomTemplateSO roomTemplate, RoomNodeSO roomNode)//根据房间模板创建房间
    {
        // Initialise room from template：根据模板初始化房间
        Room room = new Room();

        room.templateID = roomTemplate.guid;
        room.id = roomNode.id;
        room.prefab = roomTemplate.prefab;
        room.roomNodeType = roomTemplate.roomNodeType;
        room.lowerBounds = roomTemplate.lowerBounds;
        room.upperBounds = roomTemplate.upperBounds;
        room.spawnPositionArray = roomTemplate.spawnPositionArray;
        room.templateLowerBounds = roomTemplate.lowerBounds;
        room.templateUpperBounds = roomTemplate.upperBounds;
        room.childRoomIDList = CopyStringList(roomNode.childRoomNodeIDList);
        room.doorWayList = CopyDoorwayList(roomTemplate.doorwayList);

        // Set parent ID for room：设置房间的父ID
        if (roomNode.parentRoomNodeIDList.Count == 0) // Entrance
        {
            room.parentRoomID = "";
            room.isPreviouslyVisited = true;

            //Set entrance in game manager：在游戏管理器中设置入口
            GameManager.Instance.SetCurrentRoom(room);
        }
        else
        {
            room.parentRoomID = roomNode.parentRoomNodeIDList[0];//非入口节点的情况下，重新设置父节点
        }
        return room;
    }

    /// <summary>
    /// Select a random room node graph from the list of room node graphs：从房间节点图列表中随机选择一个房间节点图
    /// </summary>
    private RoomNodeGraphSO SelectRandomRoomNodeGraph(List<RoomNodeGraphSO> roomNodeGraphList)
    {
        if (roomNodeGraphList.Count > 0)
        {
            return roomNodeGraphList[UnityEngine.Random.Range(0, roomNodeGraphList.Count)];//从房间节点列表中随机选择房间节点
        }
        else
        {
            Debug.Log("No room node graphs in list");
            return null;
        }
    }

    /// <summary>
    /// Create deep copy of doorway list：创建门口列表的深度副制
    /// </summary>
    private List<Doorway> CopyDoorwayList(List<Doorway> oldDoorwayList)//重新添加子节点时，将门洞同时深度复制
    {
        List<Doorway> newDoorwayList = new List<Doorway>();

        foreach (Doorway doorway in oldDoorwayList)
        {
            Doorway newDoorway = new Doorway();

            newDoorway.position = doorway.position;
            newDoorway.orientation = doorway.orientation;
            newDoorway.doorPrefab = doorway.doorPrefab;
            newDoorway.isConnected = doorway.isConnected;
            newDoorway.isUnavailable = doorway.isUnavailable;
            newDoorway.doorwayStartCopyPosition = doorway.doorwayStartCopyPosition;
            newDoorway.doorwayCopyTileWidth = doorway.doorwayCopyTileWidth;
            newDoorway.doorwayCopyTileHeight = doorway.doorwayCopyTileHeight;

            newDoorwayList.Add(newDoorway);
        }
        return newDoorwayList;
    }

    /// <summary>
    /// Create deep copy of string list：创建字符串列表的深度复制
    /// </summary>
    private List<string> CopyStringList(List<string> oldStringList)//通过深度复制字符串来重新添加子节点
    {
        List<string> newStringList = new List<string>();

        foreach (string stringValue in oldStringList)
        {
            newStringList.Add(stringValue);
        }
        return newStringList;
    }

    /// <summary>
    /// Instantiate the dungeon room gameobjects from the prefabs：从预制件中实例化地牢房间游戏对象
    /// </summary>
    private void InstantiateRoomGameobjects()//最终实际实例化房间资源的方法
    {
        // Iterate through all dungeon rooms：遍历所有地牢房间
        foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBuilderRoomDictionary)//遍历字典
        {
            Room room = keyvaluepair.Value;//返回对应的键值对来提取房间

            // Calculate room position (remember the room instantiatation position needs to be adjusted by the room template lower bounds)
            // 计算房间位置（记住房间实例化位置需要通过房间模板下限进行调整）
            Vector3 roomPosition = new Vector3(room.lowerBounds.x - room.templateLowerBounds.x, room.lowerBounds.y - room.templateLowerBounds.y, 0f);//计算房间的坐标

            // Instantiate room：实例化房间
            GameObject roomGameobject = Instantiate(room.prefab, roomPosition, Quaternion.identity, transform);

            // Get instantiated room component from instantiated prefab：从实例化的预制件中获取实例化的房间组件
            InstantiatedRoom instantiatedRoom = roomGameobject.GetComponentInChildren<InstantiatedRoom>();

            instantiatedRoom.room = room;

            // Initialise The Instantiated Room：初始化实例化的房间
            instantiatedRoom.Initialise(roomGameobject);

            // Save gameobject reference：保存游戏对象引用
            room.instantiatedRoom = instantiatedRoom;
        }
    }

    /// <summary>
    /// Get a room template by room template ID, returns null if ID doesn't exist：根据房间模板ID获取房间模板，如果ID不存在则返回null
    /// </summary>
    public RoomTemplateSO GetRoomTemplate(string roomTemplateID)//获取房间模板的辅助方法
    {
        if (roomTemplateDictionary.TryGetValue(roomTemplateID, out RoomTemplateSO roomTemplate))
        {
            return roomTemplate;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Get room by roomID, if no room exists with that ID return null：按房间ID获取房间，如果不存在具有该ID的房间，则返回null
    /// </summary>
    public Room GetRoomByRoomID(string roomID)//通过房间id获取房间的辅助方法
    {
        if (dungeonBuilderRoomDictionary.TryGetValue(roomID, out Room room))
        {
            return room;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Clear dungeon room gameobjects and dungeon room dictionary：清除地牢房间游戏对象和地牢房间词典
    /// </summary>
    private void ClearDungeon()//清空地牢方法
    {
        // Destroy instantiated dungeon gameobjects and clear dungeon manager room dictionary：销毁实例化的地牢游戏对象并清除地牢管理员房间词典
        if (dungeonBuilderRoomDictionary.Count > 0)
        {
            foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBuilderRoomDictionary)
            {
                Room room = keyvaluepair.Value;

                if (room.instantiatedRoom != null)
                {
                    Destroy(room.instantiatedRoom.gameObject);
                }
            }
            dungeonBuilderRoomDictionary.Clear();
        }
    }
}