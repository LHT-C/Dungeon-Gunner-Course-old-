using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class StaticEventHandler //静态事件类（常时广播）
{
    // Room changed event
    public static event Action<RoomChangedEventArgs> OnRoomChanged;

    public static void CallRoomChangedEvent(Room room)
    {
        OnRoomChanged?.Invoke(new RoomChangedEventArgs() { room = room });
    }
}

public class RoomChangedEventArgs : EventArgs
{
    public Room room;
}
