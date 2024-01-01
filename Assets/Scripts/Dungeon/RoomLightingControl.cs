using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(InstantiatedRoom))]
public class RoomLightingControl : MonoBehaviour
{
    private InstantiatedRoom instantiatedRoom;

    private void Awake()
    {
        // Load components：加载组件
        instantiatedRoom = GetComponent<InstantiatedRoom>();
    }

    private void OnEnable()
    {
        //subscribe to room changed event：订阅房间更改事件
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        //unsubscribe from room changed event：取消订阅房间更改事件
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    /// <summary>
    /// Handle room changed event：处理房间更改事件
    /// </summary>
    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        // If this is the room entered and the room isn't already lit, then fade in the room lighting：如果是已进入的房间，而房间还没有照明，则将房间的照明减弱
        if (roomChangedEventArgs.room == instantiatedRoom.room && !instantiatedRoom.room.isLit)
        {
            // Fade in room：淡入房间
            FadeInRoomLighting();

            // Fade in the room doors lighting：房间门照明褪色
            FadeInDoors();

            instantiatedRoom.room.isLit = true;
        }
    }

    /// <summary>
    /// Fade in the room lighting：房间照明褪色
    /// </summary>
    private void FadeInRoomLighting()
    {
        // Fade in the lighting for the room tilemaps：淡化房间瓷砖贴图的照明
        StartCoroutine(FadeInRoomLightingRoutine(instantiatedRoom));
    }

    /// <summary>
    /// Fade in the room lighting coroutine：淡出室内照明协程
    /// </summary>
    private IEnumerator FadeInRoomLightingRoutine(InstantiatedRoom instantiatedRoom)
    {
        // Create new material to fade in：创建要淡入的新材质
        Material material = new Material(GameResources.Instance.variableLitShader);

        instantiatedRoom.groundTilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.decoration1Tilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.decoration2Tilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.frontTilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.minimapTilemap.GetComponent<TilemapRenderer>().material = material;

        for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime)
        {
            material.SetFloat("Alpha_Slider", i);
            yield return null;
        }

        // Set material back to lit material：将材质设置回点亮的材质
        instantiatedRoom.groundTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.decoration1Tilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.decoration2Tilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.frontTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.minimapTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
    }

    /// <summary>
    /// Fade in the doors：门褪色
    /// </summary>
    private void FadeInDoors()
    {
        Door[] doorArray = GetComponentsInChildren<Door>();

        foreach (Door door in doorArray)
        {
            DoorLightingControl doorLightingControl = door.GetComponentInChildren<DoorLightingControl>();

            doorLightingControl.FadeInDoor(door);
        }

    }
}
