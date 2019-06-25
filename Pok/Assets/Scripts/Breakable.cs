using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem destroyParticles = null;
    [SerializeField]
    private float fadeDuration = 0.2f;
    [SerializeField]
    private Collider2D[] colliders = null;
    [SerializeField]
    private SpriteRenderer[] renderers = null;
    private bool isDestroying = false;

    private void Awake()
    {
        BreakableManager.Instance.InitializeBreakable(OnRushEnabled, OnRushDisabled);
    }

    private void OnEnable()
    {
        ResetDestroyAnimation();
    }

    private void OnRushEnabled(PlayerController playerController)
    {
        foreach (Collider2D collider in colliders)
        {
            collider.isTrigger = true;
        }
    }

    private void OnRushDisabled(PlayerController playerController)
    {
        foreach (Collider2D collider in colliders)
        {
            collider.isTrigger = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController playerController = collision.GetComponent<PlayerController>();
        if (playerController != null)
        {
            PlayDestroyAnimation();
            MusicPlayer.Instance.PlayHitBreakable();
            playerController.DisableRush();
        }
    }

    private void PlayDestroyAnimation()
    {
        Debug.Log("Destroyed");
        isDestroying = true;
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }
        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.DOFade(0, fadeDuration);
        }
        destroyParticles?.Play();
    }
    private void ResetDestroyAnimation()
    {
        isDestroying = false;
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = true;
        }
        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.DOFade(1, 0);
        }
        destroyParticles?.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }


}
