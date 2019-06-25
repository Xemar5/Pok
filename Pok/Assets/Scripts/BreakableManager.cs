using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableManager : MonoBehaviour
{
    private static BreakableManager instance;
    public static BreakableManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<BreakableManager>();
            }
            return instance;
        }
    }


    [SerializeField]
    private PlayerController playerController = null;
    private bool isRushEnabled = false;


    private void Awake()
    {
        playerController.OnRushStart += OnRushStart;
        playerController.OnRushEnd += OnRushEnd;
    }

    private void OnRushStart(PlayerController obj)
    {
        isRushEnabled = true;
    }
    private void OnRushEnd(PlayerController obj)
    {
        isRushEnabled = false;
    }


    public void InitializeBreakable(Action<PlayerController> onRushEnabled, Action<PlayerController> onRushDisabled)
    {
        playerController.OnRushStart += onRushEnabled;
        playerController.OnRushEnd += onRushDisabled;
        if (isRushEnabled == true)
        {
            onRushEnabled.Invoke(playerController);
        }
        else
        {
            onRushDisabled.Invoke(playerController);
        }
    }

}
