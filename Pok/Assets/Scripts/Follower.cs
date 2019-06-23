using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    [SerializeField]
    private Transform followedTransform = null;
    [SerializeField]
    private float ratio = 0.01f;

    public float offset = 0;


    private void LateUpdate()
    {
        Vector3 position = transform.position;
        float y = Mathf.Max(position.y, followedTransform.position.y + offset);
        Vector3 newPosition = new Vector3(position.x, y, position.z);

        transform.position = Vector3.Lerp(position, newPosition, ratio);
    }

    [ContextMenu("Set Offset")]
    private void SetOffset()
    {
        if (followedTransform == null)
        {
            Debug.LogError("Followed Transform field not set.");
        }
        else
        {
            offset = followedTransform.position.y - transform.position.y;
        }
    }

}
