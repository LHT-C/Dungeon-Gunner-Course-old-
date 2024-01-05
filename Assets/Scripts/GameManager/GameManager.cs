using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]//��ֹ������ͬһ���
public class GameManager : SingletonMonobehaviour<GameManager>
{
    #region Header DUNGEON LEVELS

    [Space(10)]
    [Header("DUNGEON LEVELS�����β㼶")]

    #endregion Header DUNGEON LEVELS

    #region Tooltip

    [Tooltip("Populate with the dungeon level scriptable objects��ʹ�õ��β㼶�ɱ�д�ű��Ķ������")]

    #endregion Tooltip

    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;

    #region Tooltip

    [Tooltip("Populate with the starting dungeon level for testing , first level = 0���ÿ�ʼ�ĵ��β㼶����Խ��в��ԣ���һ�㼶=0")]

    #endregion Tooltip

    [SerializeField] private int currentDungeonLevelListIndex = 0;
    private Room currentRoom;
    private Room previousRoom;
    private PlayerDetailsSO playerDetails; 
    private Player player;

    [HideInInspector] public GameState gameState;

    protected override void Awake()
    {
        // Call base class�����û���
        base.Awake();

        // Set player details - saved in current player scriptable object from the main menu�����������ϸ��Ϣ-�����˵������ڵ�ǰ��ҽű�������
        playerDetails = GameResources.Instance.currentPlayer.playerDetails;

        // Instantiate player��ʵ�������
        InstantiatePlayer();
    }

    /// <summary>
    /// Create player in scene at position���ڳ����е�λ�ô������
    /// </summary>
    private void InstantiatePlayer()//�������꣬ʵ������ɫ
    {
        // Instantiate player��ʵ�������
        GameObject playerGameObject = Instantiate(playerDetails.playerPrefab);

        // Instantiate player��ʵ�������
        player = playerGameObject.GetComponent<Player>();

        player.Initialize(playerDetails);
    }

    private void OnEnable()
    {
        // Subscribe to room changed event�����ķ�������¼�
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        // Unsubscribe from room changed event��ȡ�����ĸ��ķ����¼�
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    /// <summary>
    /// Handle room changed event������������¼�
    /// </summary>
    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        SetCurrentRoom(roomChangedEventArgs.room);//����ʱ�����õ�ǰ����
    }

    // Start is called before the first frame update���ڵ�һ֡����֮ǰ����Start
    private void Start()
    {
        gameState = GameState.gameStarted;
    }

    // Update is called once per frame��ÿ֡����һ�θ���
    private void Update()
    {
        HandleGameState();

        // For testing
        //if (Input.GetKeyDown(KeyCode.R))//����R���ؿ���Ϸ���������������ͼ��
        //{
        //    gameState = GameState.gameStarted;
        //}

    }

    /// <summary>
    /// Handle game state��������Ϸ״̬
    /// </summary>
    private void HandleGameState()
    {
        // Handle game state��������Ϸ״̬
        switch (gameState)
        {
            case GameState.gameStarted:

                // Play first level�����һ��
                PlayDungeonLevel(currentDungeonLevelListIndex);

                gameState = GameState.playingLevel;

                break;
        }
    }

    /// <summary>
    /// Set the current room the player in in������������ڵĵ�ǰ����
    /// </summary>
    public void SetCurrentRoom(Room room)//���õ�ǰ����
    {
        previousRoom = currentRoom;
        currentRoom = room;
    }

    private void PlayDungeonLevel(int dungeonLevelListIndex)
    {
        // Build dungeon for level Ϊ��ǰ�㼶���ɷ���
        bool dungeonBuiltSucessfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[dungeonLevelListIndex]);//����DungeonBuilder�����ɵ���

        if (!dungeonBuiltSucessfully)
        {
            Debug.LogError("Couldn't build dungeon from specified rooms and node graphs");
        }

        // Call static event that room has changed������ǰ������Ϊ�ѱ仯
        StaticEventHandler.CallRoomChangedEvent(currentRoom);

        // Set player roughly mid-room�������λ�����ڷ������м�
        player.gameObject.transform.position = new Vector3((currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2f, (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) / 2f,0f);

        // Get nearest spawn point in room nearest to player����ȡ����������ˢ��λ��
        player.gameObject.transform.position = HelperUtilities.GetSpawnPositionNearestToPlayer(player.gameObject.transform.position);
    }

    /// <summary>
    /// Get the player����ȡ��ǰ��ɫ
    /// </summary>
    public Player GetPlayer()
    {
        return player;
    }

    /// <summary>
    /// Get the player minimap icon����ȡ��ҵ�С��ͼͼ��
    /// </summary>
    public Sprite GetPlayerMiniMapIcon()
    {
        return playerDetails.playerMiniMapIcon;
    }

    /// <summary>
    /// Get the current room the player is in����ȡ������ڵĵ�ǰ����
    /// </summary>
    public Room GetCurrentRoom()
    {
        return currentRoom;
    }

    #region Validation

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }

#endif

    #endregion Validation

}

