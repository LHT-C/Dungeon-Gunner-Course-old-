<<<<<<< Updated upstream
using UnityEngine;
[System.Serializable]

=======
﻿using UnityEngine;
[System.Serializable]
>>>>>>> Stashed changes
public class Doorway 
{
    public Vector2Int position;
    public Orientation orientation;
    public GameObject doorPrefab;
<<<<<<< Updated upstream

    #region Header
    [Header("The Upper Left Position To Start Copying From")]
    #endregion
    public Vector2Int doorwaystartCopyPosition;
    #region Header
    [Header("The width of tiles in the doorway to copy over")]
    #endregion
    public int donmycopytilewidth;
    #region Header
    [Header("The height of tiles in the doorway to copy over")]
    #endregion
    public int doorwayCopyTileeight;
=======
    #region Header
    [Header("The Upper Left Position To Start Copying From")]
    #endregion
    public Vector2Int doorwayStartCopyPosition;
    #region Header
    [Header("The width of tiles in the doorway to copy over")]
    #endregion
    public int doorwayCopyTileWidth;
    #region Header
    [Header("The height of tiles in the doorway to copy over")]
    #endregion
    public int doorwayCopyTileHeight;
>>>>>>> Stashed changes
    [HideInInspector]
    public bool isConnected = false;
    [HideInInspector]
    public bool isUnavailable = false;
}
