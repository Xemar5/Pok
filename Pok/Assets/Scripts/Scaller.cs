using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scaller : MonoBehaviour
{
    [SerializeField]
    private float targetWidth = 10;

    private void Awake()
    {
        float ortho = (float)Screen.height / (float)Screen.width;
        Camera.main.orthographicSize = targetWidth * ortho;
    }
}
