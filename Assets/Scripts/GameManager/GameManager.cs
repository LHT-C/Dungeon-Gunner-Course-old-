using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]//防止多次添加同一组件
public class GameManager : SingletonMonobehaviour<GameManager>
{
    #region Header DUNGEON LEVELS

    [Space(10)]
    [Header("DUNGEON LEVELS")]

    #endregion Header DUNGEON LEVELS

    #region Tooltip

    [Tooltip("Populate with the dungeon level scriptable objects")]

    #endregion Tooltip

    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;

    #region Tooltip

    [Tooltip("Populate with the starting dungeon level for testing , first level = 0")]

    #endregion Tooltip

    [SerializeField] private int currentDungeonLevelListIndex = 0;
    private Room currentRoom;
    private Room previousRoom;
    private PlayerDetailsSO playerDetails; 
    private Player player;

    [HideInInspector] public GameState gameState;

    protected override void Awake()
    {
        // Call base class
        base.Awake();

        // Set player details - saved in current player scriptable object from the main menu
        playerDetails = GameResources.Instance.currentPlayer.playerDetails;

        // Instantiate player
        InstantiatePlayer();
    }

    /// <summary>
    /// Create player in scene at position
    /// </summary>
    private void InstantiatePlayer()//根据坐标，实例化角色
    {
        // Instantiate player
        GameObject playerGameObject = Instantiate(playerDetails.playerPrefab);

        // Instantiate player
        player = playerGameObject.GetComponent<Player>();

        player.Initialize(playerDetails);
    }

    private void OnEnable()
    {
        // Subscribe to room changed event.
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        // Unsubscribe from room changed event
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;

    }

    /// <summary>
    /// Handle room changed event
    /// </summary>
    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        SetCurrentRoom(roomChangedEventArgs.room);//启动时，设置当前房间
    }

    // Start is called before the first frame update
    private void Start()
    {
        gameState = GameState.gameStarted;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleGameState();

        // For testing
        if (Input.GetKeyDown(KeyCode.R))//按下R键重开游戏（重新生成随机地图）
        {
            gameState = GameState.gameStarted;
        }

    }

    /// <summary>
    /// Handle game state
    /// </summary>
    private void HandleGameState()//游戏状态设置
    {
        // Handle game state
        switch (gameState)
        {
            case GameState.gameStarted:

                // Play first level
                PlayDungeonLevel(currentDungeonLevelListIndex);

                gameState = GameState.playingLevel;

                break;
        }
    }

    /// <summary>
    /// Set the current room the player in in
    /// </summary>
    public void SetCurrentRoom(Room room)//设置当前房间
    {
        previousRoom = currentRoom;
        currentRoom = room;
    }

    private void PlayDungeonLevel(int dungeonLevelListIndex)
    {
        // Build dungeon for level 为当前层级生成房间
        bool dungeonBuiltSucessfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[dungeonLevelListIndex]);//调用DungeonBuilder来生成地牢

        if (!dungeonBuiltSucessfully)
        {
            Debug.LogError("Couldn't build dungeon from specified rooms and node graphs");
        }

        // Call static event that room has changed.将当前房间设为已变化
        StaticEventHandler.CallRoomChangedEvent(currentRoom);

        // Set player roughly mid-room 将玩家位置设于房间正中间
        player.gameObject.transform.position = new Vector3((currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2f, (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) / 2f,0f);

        // Get nearest spawn point in room nearest to player 获取离玩家最近的刷新位置
        player.gameObject.transform.position = HelperUtilities.GetSpawnPositionNearestToPlayer(player.gameObject.transform.position);
    }

    /// <summary>
    /// Get the player
    /// </summary>
    public Player GetPlayer()//获取当前角色
    {
        return player;
    }

    /// <summary>
    /// Get the current room the player is in
    /// </summary>
    public Room GetCurrentRoom()//获取当前房间
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

