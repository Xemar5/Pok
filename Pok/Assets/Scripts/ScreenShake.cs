using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    private static ScreenShake instance;
    public static ScreenShake Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ScreenShake>();
            }
            return instance;
        }
    }

    [SerializeField]
    private float duration = 0.2f;
    [SerializeField]
    private float force = 5;

    private Vector3 cameraLocalPosition;
    private float currentForce = 0;


    private void Awake()
    {
        cameraLocalPosition = Camera.main.transform.localPosition;
    }


    public static void Play()
    {
        Instance.StartCoroutine(Instance.PlayShake());
    }

    private IEnumerator PlayShake()
    {
        float time = 0;
        currentForce += force;
        while (time < duration)
        {
            time += Time.deltaTime;
            Vector3 pos = Camera.main.transform.localPosition;
            pos.x = Random.Range(-currentForce, +currentForce) * Time.deltaTime;
            pos.y = Random.Range(-currentForce, +currentForce) * Time.deltaTime;
            Camera.main.transform.localPosition = pos;

            yield return null;
        }
        currentForce -= force;
        Camera.main.transform.localPosition = cameraLocalPosition;
    }


}
