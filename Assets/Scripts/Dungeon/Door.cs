using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class Door : MonoBehaviour
{
    #region Header OBJECT REFERENCES
    [Space(10)]
    [Header("OBJECT REFERENCES：对象引用")]
    #endregion

    #region Tooltip
    [Tooltip("Populate this with the BoxCollider2D component on the DoorCollider gameobject：使用DoorCollider游戏对象上的BoxCollider2D组件填充")]
    #endregion
    [SerializeField] private BoxCollider2D doorCollider;

    [HideInInspector] public bool isBossRoomDoor = false;
    private BoxCollider2D doorTrigger;
    private bool isOpen = false;
    private bool previouslyOpened = false;
    private Animator animator;

    private void Awake()
    {
        // disable door collider by default：默认情况下禁用门碰撞器
        doorCollider.enabled = false;

        // Load components：加载组件
        animator = GetComponent<Animator>();
        doorTrigger = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag || collision.tag == Settings.playerWeapon)//角色或武器接触到门时，开门
        {
            OpenDoor();
        }
    }

    private void OnEnable()
    {
        // When the parent gameobject is disabled (when the player moves far enough away from the room) the animator state gets reset. Therefore we need to restore the animator state.
        // 当禁用父游戏对象时（当玩家移动到离房间足够远的地方时），动画师状态将重置。因此，我们需要恢复动画师的状态。
        animator.SetBool(Settings.open, isOpen);
    }


    /// <summary>
    /// Open the door：开门
    /// </summary>
    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            previouslyOpened = true;
            doorCollider.enabled = false;
            doorTrigger.enabled = false;

            // Set open parameter in animator
            animator.SetBool(Settings.open, true);
        }
    }

    /// <summary>
    /// Lock the door：锁门
    /// </summary>
    public void LockDoor()
    {
        isOpen = false;
        doorCollider.enabled = true;
        doorTrigger.enabled = false;

        // set open to false to close door：将open设置为false关闭门
        animator.SetBool(Settings.open, false);
    }

    /// <summary>
    /// Unlock the door：解锁门
    /// </summary>
    public void UnlockDoor()
    {
        doorCollider.enabled = false;
        doorTrigger.enabled = true;

        if (previouslyOpened == true)
        {
            isOpen = false;
            OpenDoor();
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorCollider), doorCollider);
    }
#endif
    #endregion

}
