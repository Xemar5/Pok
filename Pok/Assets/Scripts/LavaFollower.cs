using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LavaFollower : Follower
{
    [SerializeField]
    private PlayerController playerController = null;
    [SerializeField]
    private float minDeltaFollow;
    [SerializeField]
    private float increment = 0.1f;
    private float deltafollow;
    private float lastDeltaFollow;

    public bool IsStarted { get; set; }

    private void Awake()
    {
        deltafollow = base.offset;
        playerController.OnJump += StartLava;
    }

    private void StartLava(PlayerController obj)
    { 
        IsStarted = true;
    }

    private void Update()
    {
        if (IsStarted == false)
        {
            return;
        }
        lastDeltaFollow = deltafollow;
        deltafollow = transform.position.y - followedTransform.transform.position.y - lastDeltaFollow + base.offset + increment;
        deltafollow = Mathf.Max(minDeltaFollow, deltafollow);
        base.offset = deltafollow;
    }
}