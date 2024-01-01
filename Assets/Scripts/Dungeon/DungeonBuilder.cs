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
        // Set dimmed material to off���������Ĳ�������Ϊ�ر�
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 0f);//���÷���alphaֵ�����ȣ�
    }

    private void OnDisable()
    {
        // Set dimmed material to fully visible�����䰵�Ĳ�������Ϊ��ȫ�ɼ�
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 1f);
    }

    protected override void Awake()
    {
        base.Awake();

        // Load the room node type list�����ط���ڵ������б�
        LoadRoomNodeTypeList();
    }

    /// <summary>
    /// Load the room node type list�����ط���ڵ������б�
    /// </summary>
    private void LoadRoomNodeTypeList()
    {
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    /// <summary>
    /// Generate random dungeon, returns true if dungeon built, false if failed������������Σ���������ѽ����򷵻�true�����ʧ���򷵻�false
    /// </summary>
    public bool GenerateDungeon(DungeonLevelSO currentDungeonLevel)
    {
        roomTemplateList = currentDungeonLevel.roomTemplateList;

        // Load the scriptable object room templates into the dictionary�����ɱ�д�ű��Ķ��󷿼�ģ����ص��ֵ���
        LoadRoomTemplatesIntoDictionary();

        dungeonBuildSuccessful = false;
        int dungeonBuildAttempts = 0;

        while (!dungeonBuildSuccessful && dungeonBuildAttempts < Settings.maxDungeonBuildAttempts)//�ظ��������ɵ���
        {
            dungeonBuildAttempts++;

            // Select a random room node graph from the list�����б���ѡ���������ڵ�ͼ
            RoomNodeGraphSO roomNodeGraph = SelectRandomRoomNodeGraph(currentDungeonLevel.roomNodeGraphList);

            int dungeonRebuildAttemptsForNodeGraph = 0;
            dungeonBuildSuccessful = false;

            // Loop until dungeon successfully built or more than max attempts for node graph��ѭ��ֱ�����γɹ�������ڵ�ͼ������Դ�������Ϊֹ
            while (!dungeonBuildSuccessful && dungeonRebuildAttemptsForNodeGraph <= Settings.maxDungeonRebuildAttemptsForRoomGraph)//�ڵ�����ʧ��ʱ����յ����еĽڵ�
            {
                // Clear dungeon room gameobjects and dungeon room dictionary��������η�����Ϸ����͵��η���ʵ�
                ClearDungeon();

                dungeonRebuildAttemptsForNodeGraph++;

                // Attempt To Build A Random Dungeon For The Selected room node graph������Ϊ��ѡ����ڵ�ͼ�����������
                dungeonBuildSuccessful = AttemptToBuildRandomDungeon(roomNodeGraph);//�����ؽ����η���ڵ�
            }


            if (dungeonBuildSuccessful)
            {
                // Instantiate Room Gameobjects��ʵ����������Ϸ����
                InstantiateRoomGameobjects();
            }
        }

        return dungeonBuildSuccessful;
    }

    /// <summary>
    /// Load the room templates into the dictionary��������ģ����ص��ʵ���
    /// </summary>
    private void LoadRoomTemplatesIntoDictionary()
    {
        // Clear room template dictionary���������ģ��ʵ�
        roomTemplateDictionary.Clear();

        // Load room template list into dictionary�������з���ģ������ֵ䣬����ظ����ÿ���̨��ʾ
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
    /// ����Ϊָ���ķ���nodeGraph����������Ρ���������˳ɹ���������֣��򷵻�true������������Ⲣ��Ҫ�ٴγ��ԣ��򷵻�false��
    /// </summary>
    private bool AttemptToBuildRandomDungeon(RoomNodeGraphSO roomNodeGraph)//�����ܹ��������ɽڵ㣨û���ص���������£���ʼ��������ڵ㣬����¼�ص������������ж�
    {

        // Create Open Room Node Queue�������򿪵ķ���ڵ����
        Queue<RoomNodeSO> openRoomNodeQueue = new Queue<RoomNodeSO>();

        // Add Entrance Node To Room Node Queue From Room Node Graph���ӷ���ڵ�ͼ����ڽڵ���ӵ�����ڵ����
        RoomNodeSO entranceNode = roomNodeGraph.GetRoomNode(roomNodeTypeList.list.Find(x => x.isEntrance));

        if (entranceNode != null)
        {
            openRoomNodeQueue.Enqueue(entranceNode);
        }
        else
        {
            Debug.Log("No Entrance Node");
            return false;  // Dungeon Not Built��δ����ĵ���
        }

        // Start with no room overlaps����û�з����ص���ʼ
        bool noRoomOverlaps = true;


        // Process open room nodes queue������򿪵ķ���ڵ����
        noRoomOverlaps = ProcessRoomsInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue, noRoomOverlaps);

        // If all the room nodes have been processed and there hasn't been a room overlap then return true��������з���ڵ㶼�Ѵ�������û�з����ص����򷵻�true
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
    /// Process rooms in the open room node queue, returning true if there are no room overlaps�������ŷ���ڵ�����еķ��䣬���û�з����ص����򷵻�true
    /// </summary>
    private bool ProcessRoomsInOpenRoomNodeQueue(RoomNodeGraphSO roomNodeGraph, Queue<RoomNodeSO> openRoomNodeQueue, bool noRoomOverlaps)//���ɷ��䣬ѭ�������ӽڵ㣬���ݽڵ����ͽ��д���
    {

        // While room nodes in open room node queue & no room overlaps detected�����򿪵ķ���ڵ�����еķ���ڵ�ʱ&δ��⵽�����ص���
        while (openRoomNodeQueue.Count > 0 && noRoomOverlaps == true)
        {
            // Get next room node from open room node queue���Ӵ򿪵ķ���ڵ�����л�ȡ��һ������ڵ㡣
            RoomNodeSO roomNode = openRoomNodeQueue.Dequeue();

            // Add child Nodes to queue from room node graph (with links to this parent Room)�����ӽڵ���ӵ��ļ��ҽڵ�ͼ�еĶ��У�����ָ��˸��ļ��ҵ����ӣ�
            foreach (RoomNodeSO childRoomNode in roomNodeGraph.GetChildRoomNodes(roomNode))
            {
                openRoomNodeQueue.Enqueue(childRoomNode);
            }

            // if the room is the entrance mark as positioned and add to room dictionary����������Ƕ�λ����ڱ�ǣ�����ӵ�����ʵ���
            if (roomNode.roomNodeType.isEntrance)
            {
                RoomTemplateSO roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);

                Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

                room.isPositioned = true;

                // Add room to room dictionary����������ӵ�����ʵ�
                dungeonBuilderRoomDictionary.Add(room.id, room);
            }

            // else if the room type isn't an entrance������������䲻�����
            else
            {
                // Else get parent room for node������Ϊ�ڵ��ȡ���ռ�
                Room parentRoom = dungeonBuilderRoomDictionary[roomNode.parentRoomNodeIDList[0]];

                // See if room can be placed without overlaps���鿴�����Ƿ�������ص�����
                noRoomOverlaps = CanPlaceRoomWithNoOverlaps(roomNode, parentRoom);
            }
        }
        return noRoomOverlaps;
    }


    /// <summary>
    /// Attempt to place the room node in the dungeon - if room can be placed return the room, else return null�����Խ�����ڵ�����ڵ�����-������Է��÷��䣬�򷵻ط��䣬���򷵻�null
    /// </summary>
    private bool CanPlaceRoomWithNoOverlaps(RoomNodeSO roomNode, Room parentRoom)//����������ص������
    {

        // initialise and assume overlap until proven otherwise.����ʼ�������跿�����ص���ֱ��֤���������ص���
        bool roomOverlaps = true;

        // Do While Room Overlaps - try to place against all available doorways of the parent until the room is successfully placed without overlap.
        // �ڷ����ص�ʱִ��-���Կ��Ÿ���������п����ſڷ��ã�ֱ������ɹ����ö����ص�Ϊֹ��
        while (roomOverlaps)
        {
            // Select random unconnected available doorway for Parent�����ѡ�񸸷����δ���ӵĿ����ſ�
            List<Doorway> unconnectedAvailableParentDoorways = GetUnconnectedAvailableDoorways(parentRoom.doorWayList).ToList();

            if (unconnectedAvailableParentDoorways.Count == 0)
            {
                // If no more doorways to try then overlap failure�����û�и�����ſڿ��Գ��ԣ���ô�ص�ʧ�ܡ�
                return false; // room overlaps�������ص�
            }

            Doorway doorwayParent = unconnectedAvailableParentDoorways[UnityEngine.Random.Range(0, unconnectedAvailableParentDoorways.Count)];//������δ���ӵ��Ŷ�������к������ӣ�ͨ���������ͺ͸��ӽڵ��ϵ�жϣ�

            // Get a random room template for room node that is consistent with the parent door orientation��//Ϊ����ڵ��ȡ�븸�ŷ���һ�µ��������ģ��
            RoomTemplateSO roomtemplate = GetRandomTemplateForRoomConsistentWithParent(roomNode, doorwayParent);

            // Create a room����������
            Room room = CreateRoomFromRoomTemplate(roomtemplate, roomNode);

            // Place the room - returns true if the room doesn't overlap�����÷���-�������û���ص����򷵻�true
            if (PlaceTheRoom(parentRoom, doorwayParent, room))
            {
                // If room doesn't overlap then set to false to exit while loop���������û���ص���������Ϊfalse���˳�whileѭ��
                roomOverlaps = false;

                // Mark room as positioned����������Ϊ�Ѷ�λ
                room.isPositioned = true;

                // Add room to dictionary����������ӵ��ʵ�
                dungeonBuilderRoomDictionary.Add(room.id, room);
            }
            else
            {
                roomOverlaps = true;
            }
        }

        return true;  // no room overlaps���޷����ص�

    }

    /// <summary>
    /// Get random room template for room node taking into account the parent doorway orientation.��Ϊ����ڵ��ȡ�������ģ�壬ͬʱ���Ǹ��ſڷ���
    /// </summary>
    private RoomTemplateSO GetRandomTemplateForRoomConsistentWithParent(RoomNodeSO roomNode, Doorway doorwayParent)
    {
        RoomTemplateSO roomtemplate = null;

        // If room node is a corridor then select random correct Corridor room template based on parent doorway orientation
        // �������ڵ��ǵ�·������ݸ��ſڷ������ѡ����ȷ�ĵ�·����ģ��
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
        // Else select random room template������ѡ���������ģ��
        else
        {
            roomtemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
        }
        return roomtemplate;
    }

    /// <summary>
    /// Place the room - returns true if the room doesn't overlap, false otherwise�����÷���-�������û���ص����򷵻�true�����򷵻�fals
    /// </summary>
    private bool PlaceTheRoom(Room parentRoom, Doorway doorwayParent, Room room)//ʵ�ʸ���λ�÷��÷���ķ���
    {

        // Get current room doorway position����ȡ��ǰ�����ſ�λ��
        Doorway doorway = GetOppositeDoorway(doorwayParent, room.doorWayList);

        // Return if no doorway in room opposite to parent doorway��������ſڶ���ķ���û���ſڣ��򷵻�
        if (doorway == null)
        {
            // Just mark the parent doorway as unavailable so we don't try and connect it again�������ſڱ��Ϊ�����ã��������ǾͲ����ٳ���������
            doorwayParent.isUnavailable = true;

            return false;
        }

        // Calculate 'world' grid parent doorway position�����㡰���硱�����и��ſ�λ��
        Vector2Int parentDoorwayPosition = parentRoom.lowerBounds + doorwayParent.position - parentRoom.templateLowerBounds;

        Vector2Int adjustment = Vector2Int.zero;

        // Calculate adjustment position offset based on room doorway position that we are trying to connect (e.g. if this doorway is west then we need to add (1,0) to the east parent doorway)
        // ����������ͼ���ӵķ����ſ�λ�ü������λ��ƫ�ƣ����磬�������ſ������棬��ô������Ҫ����1,0����ӵ�����ĸ��ſڣ�

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

        // Calculate room lower bounds and upper bounds based on positioning to align with parent doorway��ͨ��������������������
        room.lowerBounds = parentDoorwayPosition + adjustment + room.templateLowerBounds - doorway.position;
        room.upperBounds = room.lowerBounds + room.templateUpperBounds - room.templateLowerBounds;

        Room overlappingRoom = CheckForRoomOverlap(room);

        if (overlappingRoom == null)
        {
            // mark doorways as connected & unavailable�����ſڱ��Ϊ�����ӺͲ�����
            doorwayParent.isConnected = true;
            doorwayParent.isUnavailable = true;

            doorway.isConnected = true;
            doorway.isUnavailable = true;

            // return true to show rooms have been connected with no overlap������true����ʾ������������û���ص�
            return true;
        }
        else
        {
            // Just mark the parent doorway as unavailable so we don't try and connect it again��ֻ�轫���ſڱ��Ϊ�����ã��������ǾͲ����ٳ���������
            doorwayParent.isUnavailable = true;

            return false;
        }
    }

    /// <summary>
    /// Get the doorway from the doorway list that has the opposite orientation to doorway�����ſ��б��л�ȡ���ſڷ����෴���ſ�
    /// </summary>
    private Doorway GetOppositeDoorway(Doorway parentDoorway, List<Doorway> doorwayList)//����Ŷ����򷽷�
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
    /// ����Ƿ��������޺����޲����ص��ķ��䣬������ص��ķ����򷵻�room-else����null
    /// </summary>
    private Room CheckForRoomOverlap(Room roomToTest)
    {
        // Iterate through all rooms���������з���
        foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBuilderRoomDictionary)
        {
            Room room = keyvaluepair.Value;

            // skip if same room as room to test or room hasn't been positioned�������Ҫ���Եķ�����ͬ�ķ���򷿼���δ��λ��������
            if (room.id == roomToTest.id || !room.isPositioned)
                continue;

            // If room overlaps����������ص�
            if (IsOverLappingRoom(roomToTest, room))
            {
                return room;
            }
        }
        // Return
        return null;
    }


    /// <summary>
    /// Check if 2 rooms overlap each other - return true if they overlap or false if they don't overlap��������������Ƿ��ص�-����ص��򷵻�true��������ص��򷵻�false
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
    /// Check if interval 1 overlaps interval 2 - this method is used by the IsOverlappingRoom method�������1�Ƿ�����2�ص�-IsOverlappingRoom����ʹ�ô˷���
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
    /// ��roomtemplatelist�л�ȡ��roomTypeƥ����������ģ�岢���أ�����Ҳ���ƥ��ķ���ģ�壬�򷵻�null��
    /// </summary>
    private RoomTemplateSO GetRandomRoomTemplate(RoomNodeTypeSO roomNodeType)
    {
        List<RoomTemplateSO> matchingRoomTemplateList = new List<RoomTemplateSO>();

        // Loop through room template list��ѭ���������ģ���б�
        foreach (RoomTemplateSO roomTemplate in roomTemplateList)//��ģ����ӵ��б�
        {
            // Add matching room templates�����ƥ��ķ���ģ��
            if (roomTemplate.roomNodeType == roomNodeType)
            {
                matchingRoomTemplateList.Add(roomTemplate);
            }
        }

        // Return null if list is zero������б�Ϊ�㣬�򷵻�null
        if (matchingRoomTemplateList.Count == 0)
            return null;

        // Select random room template from list and return�����б������ѡ�񷿼�ģ�岢����
        return matchingRoomTemplateList[UnityEngine.Random.Range(0, matchingRoomTemplateList.Count)];//���б��������÷���ģ��
    }

    /// <summary>
    /// Get unconnected doorways����ȡδ���ӵ��ſ�
    /// </summary>
    private IEnumerable<Doorway> GetUnconnectedAvailableDoorways(List<Doorway> roomDoorwayList)
    {
        // Loop through doorway list��ѭ���ſ��б�
        foreach (Doorway doorway in roomDoorwayList)
        {
            if (!doorway.isConnected && !doorway.isUnavailable)
                yield return doorway;
        }
    }

    /// <summary>
    /// Create room based on roomTemplate and layoutNode, and return the created room������roomTemplate��layoutNode�������䣬�����ش����ķ���
    /// </summary>
    private Room CreateRoomFromRoomTemplate(RoomTemplateSO roomTemplate, RoomNodeSO roomNode)//���ݷ���ģ�崴������
    {
        // Initialise room from template������ģ���ʼ������
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

        // Set parent ID for room�����÷���ĸ�ID
        if (roomNode.parentRoomNodeIDList.Count == 0) // Entrance
        {
            room.parentRoomID = "";
            room.isPreviouslyVisited = true;

            //Set entrance in game manager������Ϸ���������������
            GameManager.Instance.SetCurrentRoom(room);
        }
        else
        {
            room.parentRoomID = roomNode.parentRoomNodeIDList[0];//����ڽڵ������£��������ø��ڵ�
        }
        return room;
    }

    /// <summary>
    /// Select a random room node graph from the list of room node graphs���ӷ���ڵ�ͼ�б������ѡ��һ������ڵ�ͼ
    /// </summary>
    private RoomNodeGraphSO SelectRandomRoomNodeGraph(List<RoomNodeGraphSO> roomNodeGraphList)
    {
        if (roomNodeGraphList.Count > 0)
        {
            return roomNodeGraphList[UnityEngine.Random.Range(0, roomNodeGraphList.Count)];//�ӷ���ڵ��б������ѡ�񷿼�ڵ�
        }
        else
        {
            Debug.Log("No room node graphs in list");
            return null;
        }
    }

    /// <summary>
    /// Create deep copy of doorway list�������ſ��б����ȸ���
    /// </summary>
    private List<Doorway> CopyDoorwayList(List<Doorway> oldDoorwayList)//��������ӽڵ�ʱ�����Ŷ�ͬʱ��ȸ���
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
    /// Create deep copy of string list�������ַ����б����ȸ���
    /// </summary>
    private List<string> CopyStringList(List<string> oldStringList)//ͨ����ȸ����ַ�������������ӽڵ�
    {
        List<string> newStringList = new List<string>();

        foreach (string stringValue in oldStringList)
        {
            newStringList.Add(stringValue);
        }
        return newStringList;
    }

    /// <summary>
    /// Instantiate the dungeon room gameobjects from the prefabs����Ԥ�Ƽ���ʵ�������η�����Ϸ����
    /// </summary>
    private void InstantiateRoomGameobjects()//����ʵ��ʵ����������Դ�ķ���
    {
        // Iterate through all dungeon rooms���������е��η���
        foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBuilderRoomDictionary)//�����ֵ�
        {
            Room room = keyvaluepair.Value;//���ض�Ӧ�ļ�ֵ������ȡ����

            // Calculate room position (remember the room instantiatation position needs to be adjusted by the room template lower bounds)
            // ���㷿��λ�ã���ס����ʵ����λ����Ҫͨ������ģ�����޽��е�����
            Vector3 roomPosition = new Vector3(room.lowerBounds.x - room.templateLowerBounds.x, room.lowerBounds.y - room.templateLowerBounds.y, 0f);//���㷿�������

            // Instantiate room��ʵ��������
            GameObject roomGameobject = Instantiate(room.prefab, roomPosition, Quaternion.identity, transform);

            // Get instantiated room component from instantiated prefab����ʵ������Ԥ�Ƽ��л�ȡʵ�����ķ������
            InstantiatedRoom instantiatedRoom = roomGameobject.GetComponentInChildren<InstantiatedRoom>();

            instantiatedRoom.room = room;

            // Initialise The Instantiated Room����ʼ��ʵ�����ķ���
            instantiatedRoom.Initialise(roomGameobject);

            // Save gameobject reference��������Ϸ��������
            room.instantiatedRoom = instantiatedRoom;
        }
    }

    /// <summary>
    /// Get a room template by room template ID, returns null if ID doesn't exist�����ݷ���ģ��ID��ȡ����ģ�壬���ID�������򷵻�null
    /// </summary>
    public RoomTemplateSO GetRoomTemplate(string roomTemplateID)//��ȡ����ģ��ĸ�������
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
    /// Get room by roomID, if no room exists with that ID return null��������ID��ȡ���䣬��������ھ��и�ID�ķ��䣬�򷵻�null
    /// </summary>
    public Room GetRoomByRoomID(string roomID)//ͨ������id��ȡ����ĸ�������
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
    /// Clear dungeon room gameobjects and dungeon room dictionary��������η�����Ϸ����͵��η���ʵ�
    /// </summary>
    private void ClearDungeon()//��յ��η���
    {
        // Destroy instantiated dungeon gameobjects and clear dungeon manager room dictionary������ʵ�����ĵ�����Ϸ����������ι���Ա����ʵ�
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