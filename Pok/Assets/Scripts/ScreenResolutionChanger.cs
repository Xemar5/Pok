using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if UNITY_ANDROID
using System.Threading.Tasks;
#endif
using UnityEngine;

public class ScreenResolutionChanger : MonoBehaviour
{
#if UNITY_ANDROID
    private static ScreenResolutionChanger instance;
    private void Awake()
    {

        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        Screen.SetResolution((int)((float)Screen.width * 0.5f), (int)((float)Screen.height * 0.5f), true);
        DontDestroyOnLoad(this.gameObject);
    }
#endif
}