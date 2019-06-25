using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LavaFollower : Follower
{
    [SerializeField]
    private MenuManager menuManager = null;
    [SerializeField]
    private TimeScaller timeScaller = null;
    [SerializeField]
    private float minDeltaFollow;
    [SerializeField]
    private float increment = 0.1f;
    private float deltafollow;
    private float lastDeltaFollow;
    private float maxDeltaFollow;


    private void Awake()
    {
        deltafollow = base.offset;
        maxDeltaFollow = Camera.main.ViewportToWorldPoint(Vector3.up).y * 1.5f;
    }

    private void Update()
    {
        if (menuManager.GameStarted == false)
        {
            return;
        }
        lastDeltaFollow = deltafollow;
        deltafollow = transform.position.y - followedTransform.transform.position.y - lastDeltaFollow + base.offset + increment * timeScaller.TimeScale;
        deltafollow = Mathf.Clamp(deltafollow, minDeltaFollow, maxDeltaFollow);
        base.offset = deltafollow;
    }
}