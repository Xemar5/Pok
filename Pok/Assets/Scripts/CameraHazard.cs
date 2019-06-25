using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHazard : MonoBehaviour
{
    [SerializeField]
    private PlayerController player = null;

    private void Update()
    {
        if (player.CanMove == false)
        {
            return;
        }
        float maxX = Camera.main.ViewportToWorldPoint(Vector3.right).x;

        if (player.transform.position.x > maxX ||
            player.transform.position.x < -maxX)
        {
            player.Kill();
        }
        
    }

}
