using UnityEngine;
[System.Serializable]
public class Doorway 
{
    public Vector2Int position;
    public Orientation orientation;
    public GameObject doorPrefab;
    #region Header
    [Header("The Upper Left Position To Start Copying From：开始复制的左上角位置")]
    #endregion
    public Vector2Int doorwayStartCopyPosition;
    #region Header
    [Header("The width of tiles in the doorway to copy over：门口要复制的瓷砖宽度")]
    #endregion
    public int doorwayCopyTileWidth;
    #region Header
    [Header("The height of tiles in the doorway to copy over：要复制的门口瓷砖的高度")]
    #endregion
    public int doorwayCopyTileHeight;
    [HideInInspector]
    public bool isConnected = false;
    [HideInInspector]
    public bool isUnavailable = false;
}
