using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private static MusicPlayer instance;
    public static MusicPlayer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MusicPlayer>();
            }
            return instance;
        }
    }

    [SerializeField]
    private AudioSource sfxSource = null;
    [SerializeField]
    private AudioClip jump = null;
    [SerializeField]
    private AudioClip kill = null;
    [SerializeField]
    private AudioClip rushCharged = null;
    [SerializeField]
    private AudioClip rush = null;
    [SerializeField]
    private AudioClip hitBreakable = null;
    [SerializeField]
    private AudioClip hit = null;

    [Space]
    [SerializeField]
    private float minPitch = 0.5f;
    [SerializeField]
    private float maxPitch = 1.5f;

    private void Awake()
    {

        if (instance != this && instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void PlayJump()
    {
        sfxSource.pitch = Random.Range(minPitch, maxPitch);
        sfxSource.PlayOneShot(jump);
    }
    public void PlayKill()
    {
        sfxSource.pitch = Random.Range(minPitch, maxPitch);
        sfxSource.PlayOneShot(kill);
    }
    public void PlayRushCharged()
    {
        sfxSource.pitch = Random.Range(minPitch, maxPitch);
        sfxSource.PlayOneShot(rushCharged);
    }
    public void PlayRush()
    {
        sfxSource.pitch = Random.Range(minPitch, maxPitch);
        sfxSource.PlayOneShot(rush);
    }
    public void PlayHit()
    {
        sfxSource.pitch = Random.Range(minPitch, maxPitch);
        sfxSource.PlayOneShot(hit);
    }
    public void PlayHitBreakable()
    {
        sfxSource.pitch = Random.Range(minPitch, maxPitch);
        sfxSource.PlayOneShot(hitBreakable);
    }
}
