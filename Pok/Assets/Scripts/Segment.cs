

using System;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour
{
    [SerializeField]
    private float top = 0;
    [SerializeField]
    private float bottom = 0;

    public float Top => top;
    public float Bottom => bottom;
    public Segment Prefab { get; set; }

    [ContextMenu("Calculate Height")]
    private void CalculateHeight()
    {
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

        List<Collider2D> colliderList = new List<Collider2D>();
        GetComponentsInChildren<Collider2D>(colliderList);
        foreach (Collider2D collider in colliderList)
        {
            bounds.Encapsulate(collider.bounds);
        }

        List<SpriteRenderer> rendererList = new List<SpriteRenderer>();
        GetComponentsInChildren<SpriteRenderer>(rendererList);
        foreach (SpriteRenderer renderer in rendererList)
        {
            bounds.Encapsulate(renderer.bounds);
        }

        bottom = bounds.min.y - transform.position.y;
        top = bounds.max.y - transform.position.y;
    }

    public void SetPosition(float totalHeight)
    {
        Vector3 position = transform.localPosition;
        position.y = totalHeight - bottom;
        transform.localPosition = position;
    }
}