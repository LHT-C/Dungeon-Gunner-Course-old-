using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(IdleEvent))]
[DisallowMultipleComponent]
public class Idle : MonoBehaviour
{
    private Rigidbody2D rigidBody2D;
    private IdleEvent idleEvent;

    private void Awake()
    {
        // Load components：加载组件
        rigidBody2D = GetComponent<Rigidbody2D>();
        idleEvent = GetComponent<IdleEvent>();
    }

    private void OnEnable()
    {
        // Subscribe to idle event：订阅空闲事件
        idleEvent.OnIdle += IdleEvent_OnIdle;
    }

    private void OnDisable()
    {
        // Subscribe to idle event：取消订阅空闲事件
        idleEvent.OnIdle -= IdleEvent_OnIdle;
    }

    private void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
        MoveRigidBody();
    }

    /// <summary>
    /// Move the rigidbody component：移动刚体组件
    /// </summary>
    private void MoveRigidBody()
    {
        // ensure the rb collision detection is set to continuous：确保rb碰撞检测设置为连续
        rigidBody2D.velocity = Vector2.zero;//站立时，停止角色刚体的移动
    }
}
