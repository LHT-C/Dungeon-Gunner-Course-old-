using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    #region DUNGEON BUILD SETTINGS
    public const int maxDungeonRebuildAttemptsForRoomGraph = 1000;//尝试重建节点的最大次数
    public const int maxDungeonBuildAttempts = 10;//尝试建造地牢的最大次数
    #endregion

    #region ROOM SETTINGS

    public const int maxChildCorridors = 3; //一个房间最多连接三条走廊

    #endregion
}
