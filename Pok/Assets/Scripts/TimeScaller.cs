using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TimeScaller : MonoBehaviour
{
    [SerializeField]
    private MenuManager menuManager = null;
    [SerializeField]
    private float initialTimeScale = 1;
    [SerializeField]
    private float increment = 0.01f;
    public float TimeScale { get; set; }

    private void Awake()
    {
        TimeScale = initialTimeScale;
    }
    private void Update()
    {
        if (menuManager.GameStarted == false)
        {
            return;
        }
        TimeScale += increment * Time.deltaTime;
    }
}