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
    private TimeScaller timeScaller = null;
    [SerializeField]
    private PlayerVisuals playerVisuals = null;

    [Space]
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

    [Space]
    [SerializeField]
    private float rushHoldDuration = 0.5f;
    [SerializeField]
    private float rushForceMultiplier = 1.5f;
    [SerializeField]
    private float rushBreakDuration = 0.5f;

    private TouchHandler lastInput = null;
    private int jumpsLeft = 0;
    private float lastJumpTime = 0;
    private TouchHandler bufferedTouch = null;
    private float bufferedTouchTime = 0;

    private float rushStartTime = -1;
    private TouchHandler rushTouch = null;
    private bool rushVisualsPlayed = false;
    private float rushBreakTimeLeft = 0;

    public event Action<PlayerController> OnKill;
    public event Action<PlayerController> OnJump;
    public event Action<PlayerController> OnTryJump;
    public event Action<PlayerController> OnRushStart;
    public event Action<PlayerController> OnRushEnd;

    public bool CanMove { get; set; }

    private void Awake()
    {
        rightTouch.OnTouchUp += OnTouchHandle;
        leftTouch.OnTouchUp += OnTouchHandle;
        rightTouch.OnTouchDown += OnStartRushCharge;
        leftTouch.OnTouchDown += OnStartRushCharge;
        jumpsLeft = resetJumps;
    }

    private void Update()
    {
        float time = Time.time;
        if (bufferedTouch != null && lastJumpTime + minJumpInterval <= time && (jumpsLeft > 0 || jumpsLeft == -1))
        {
            OnTouchHandle(bufferedTouch);
            bufferedTouch = null;
        }
        if (bufferedTouch != null && time - bufferedTouchTime > maxBufferDuration)
        {
            bufferedTouch = null;
        }

        if (rushTouch != null)
        {
            //rigidbody2D.velocity = Vector2.zero;
        }
        if (rushTouch != null && rushVisualsPlayed == false && time - rushStartTime >= rushHoldDuration)
        {
            playerVisuals.PlayRushChargedParticles();
            MusicPlayer.Instance.PlayRushCharged();
            rushVisualsPlayed = true;
        }
        if (rushBreakTimeLeft > 0)
        {
            rushBreakTimeLeft -= Time.deltaTime;
            if (rushBreakTimeLeft <= 0)
            {
                DisableRush();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Hazard hazard = collision.gameObject.GetComponent<Hazard>();
        if (hazard != null)
        {
            Kill();
            return;
        }
        jumpsLeft = resetJumps;
        playerVisuals.PlayHitParticles(collision.relativeVelocity.normalized);
        MusicPlayer.Instance.PlayHit();

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
        if (CanMove == false)
        {
            return;
        }
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.isKinematic = true;
        CanMove = false;
        playerVisuals.PlayDeathParticles();
        MusicPlayer.Instance.PlayKill();
        OnKill?.Invoke(this);
    }

    private void OnTouchHandle(TouchHandler touchHandler)
    {
        OnTryJump?.Invoke(this);
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
        bool rushing = OnStopRushCharge(touchHandler);

        lastJumpTime = Time.time;
        lastInput = touchHandler;

        float rushScale = rushing ? rushForceMultiplier : 1;

        Vector2 velocity = new Vector2(
            touchHandler.Direction * sidewardVelocity * timeScaller.TimeScale * rushScale,
            upwardVelocity * timeScaller.TimeScale * rushScale);
        rigidbody2D.velocity = velocity;
        if (jumpsLeft > 0)
        {
            jumpsLeft -= 1;
        }
        playerVisuals.PlayJumpParticles(-velocity.normalized);
        MusicPlayer.Instance.PlayJump();
        OnJump?.Invoke(this);
    }

    private void OnStartRushCharge(TouchHandler touchHandler)
    {
        if (CanMove == false)
        {
            return;
        }
        if (rushTouch != null || lastInput == touchHandler)
        {
            return;
        }
        rushTouch = touchHandler;
        rushStartTime = Time.time;
    }
    private bool OnStopRushCharge(TouchHandler touchHandler)
    {
        if (rushTouch == null || rushTouch != touchHandler)
        {
            return false;
        }
        float duration = Time.time - rushStartTime;
        bool rushing = false;
        if (touchHandler == rushTouch && duration >= rushHoldDuration)
        {
            rushing = true;
            EnableRush();
        }
        rushStartTime = -1;
        rushTouch = null;
        rushVisualsPlayed = false;
        return rushing;
    }




    private void EnableRush()
    {
        rushBreakTimeLeft = rushBreakDuration;
        playerVisuals.PlayRushParticles();
        MusicPlayer.Instance.PlayRush();
        OnRushStart?.Invoke(this);
    }

    public void DisableRush()
    {
        rushBreakTimeLeft = 0;
        playerVisuals.StopRushParticles();
        OnRushEnd?.Invoke(this);
    }


}
