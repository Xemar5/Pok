using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private TouchHandler leftTouch = null;
    [SerializeField]
    private TouchHandler rightTouch = null;
    [SerializeField]
    private new Rigidbody2D rigidbody2D = null;
    [SerializeField]
    private float upwardVelocity = 10;
    [SerializeField]
    private float sidewardVelocity = 10;
    [SerializeField]
    private int resetJumps = 2;
    [SerializeField]
    private float minJumpInterval = 0.1f;
    [SerializeField]
    private float maxBufferDuration = 0.2f;

    private TouchHandler lastInput = null;
    private Collider2D lastCollider = null;
    private int jumpsLeft = 0;
    private float lastJumpTime = 0;
    private TouchHandler bufferedTouch = null;
    private float bufferedTouchTime = 0;

    public event Action<PlayerController> OnKill;
    public event Action<PlayerController> OnJump;

    public bool CanMove { get; set; }

    private void Awake()
    {
        rightTouch.OnTouchDown += OnTouchHandle;
        leftTouch.OnTouchDown += OnTouchHandle;
        jumpsLeft = resetJumps;
    }
    private void Update()
    {
        float time = Time.time;
        if (bufferedTouch != null && lastJumpTime + minJumpInterval <= time && jumpsLeft > 0)
        {
            OnTouchHandle(bufferedTouch);
            bufferedTouch = null;
        }
        if (bufferedTouch != null && time - bufferedTouchTime > maxBufferDuration)
        {
            bufferedTouch = null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (lastCollider != collision.collider)
        {
            //rigidbody2D.velocity = Vector2.zero;
        }
        jumpsLeft = resetJumps;
        lastCollider = collision.collider;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Hazard hazard = collision.GetComponent<Hazard>();
        if (hazard != null)
        {
            Kill();
        }
    }

    public void Kill()
    {
        OnKill?.Invoke(this);
    }

    private void OnTouchHandle(TouchHandler touchHandler)
    {
        if (CanMove == false)
        {
            return;
        }
        if (lastInput == touchHandler)
        {
            return;
        }
        if (jumpsLeft == 0)
        {
            bufferedTouch = touchHandler;
            bufferedTouchTime = Time.time;
            return;
        }
        if (lastJumpTime + minJumpInterval > Time.time)
        {
            bufferedTouch = touchHandler;
            bufferedTouchTime = Time.time;
            return;
        }
        lastJumpTime = Time.time;
        lastInput = touchHandler;

        Vector2 velocity = new Vector2(touchHandler.Direction * sidewardVelocity, upwardVelocity);
        rigidbody2D.velocity = velocity;
        jumpsLeft -= 1;
        OnJump?.Invoke(this);
    }

}
