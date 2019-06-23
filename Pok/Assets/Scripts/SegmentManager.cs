using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SegmentManager : MonoBehaviour
{
    [SerializeField]
    private Transform segmentParent = null;
    [SerializeField]
    private Segment startingSegmentPrefab = null;
    [SerializeField]
    private List<Segment> segmentPrefabs = new List<Segment>();

    private Dictionary<Segment, Queue<Segment>> pooledSegments = new Dictionary<Segment, Queue<Segment>>();
    private Queue<Segment> activeSegments = new Queue<Segment>();
    private float totalHeight = 0;

    private void Awake()
    {
        for (int i = 0; i < segmentPrefabs.Count; i++)
        {
            Segment segment = segmentPrefabs[i];
            if (segment.Top - segment.Bottom <= 0)
            {
                Debug.LogError("Segment with height equal to or less than 0; removing");
                segmentPrefabs.RemoveAt(i);
                i -= 1;
            }
        }
        if (segmentPrefabs.Count == 0)
        {
            Debug.LogError("No prefabs exist.");
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        float topCameraY = Camera.main.ViewportToWorldPoint(Vector3.up).y;
        float bottomCameraY = Camera.main.ViewportToWorldPoint(Vector3.down).y;
        int debugExit = 0;
        while (topCameraY >= totalHeight)
        {
            int segmentIndex = UnityEngine.Random.Range(0, segmentPrefabs.Count);
            Segment segment = UseSegment(segmentPrefabs[segmentIndex]);
            if (totalHeight == 0)
            {
                DisplaceFirstSegment(segment);
            }
            totalHeight += segment.Top - segment.Bottom;

            debugExit += 1;
            if (debugExit > 1000)
            {
                Debug.LogError("Infinite Loop Detected");
                gameObject.SetActive(false);
                return;
            }
        }
        debugExit = 0;
        while (activeSegments.Count > 0 &&  bottomCameraY > activeSegments.Peek().Top + activeSegments.Peek().transform.localPosition.y)
        {
            ReturnBottomSegment();

            debugExit += 1;
            if (debugExit > 1000)
            {
                Debug.LogError("Infinite Loop Detected");
                gameObject.SetActive(false);
                return;
            }
        }
    }

    private void DisplaceFirstSegment(Segment segment)
    {
        segment.transform.localPosition += Vector3.up * segment.Bottom;
        totalHeight += segment.Bottom;
    }

    private Segment UseSegment(Segment prefab)
    {
        if (pooledSegments.TryGetValue(prefab, out Queue<Segment> queue) == true && queue.Count > 0)
        {
            Segment segment = queue.Dequeue();
            segment.gameObject.SetActive(true);
            segment.SetPosition(totalHeight);
            activeSegments.Enqueue(segment);
            return segment;
        }
        else
        {
            Segment segment = Instantiate(prefab, segmentParent, false);
            segment.Prefab = prefab;
            segment.SetPosition(totalHeight);
            activeSegments.Enqueue(segment);
            return segment;
        }
    }


    private void ReturnBottomSegment()
    {
        Segment segment = activeSegments.Dequeue();
        segment.gameObject.SetActive(false);
        ReturnToPooled(segment.Prefab, segment);
    }
    private void ReturnToPooled(Segment prefab, Segment segment)
    {
        if (pooledSegments.TryGetValue(prefab, out Queue<Segment> list) == true)
        {
            list.Enqueue(segment);
        }
        else
        {
            Queue<Segment> segments = new Queue<Segment>();
            segments.Enqueue(segment);
            pooledSegments.Add(prefab, segments);
        }
    }

}