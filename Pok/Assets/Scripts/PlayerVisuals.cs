using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{

    [SerializeField]
    private SpriteRenderer spriteRenderer = null;
    [SerializeField]
    private TrailRenderer trailRenderer = null;
    [SerializeField]
    private float fadeDuration = 0.2f;
    [SerializeField]
    private Color rushColorColor = new Color();
    [SerializeField]
    private Gradient trailRushColorGradient = new Gradient();

    [SerializeField]
    private ParticleSystem flyParticles = null;
    [SerializeField]
    private ParticleSystem jumpParticles = null;
    [SerializeField]
    private ParticleSystem hitParticles = null;
    [SerializeField]
    private ParticleSystem deathParticles = null;

    [SerializeField]
    private ParticleSystem rushParticles = null;
    [SerializeField]
    private ParticleSystem rushChargedParticles = null;

    private Color startingColor;
    private Gradient trailStartingColorGradient;

    private void Awake()
    {
        startingColor = spriteRenderer.color;
        trailStartingColorGradient = trailRenderer.colorGradient;
    }

    public void PlayJumpParticles(Vector2 direction)
    {
        RotateParticlesTowardsDirection(direction, jumpParticles);
        jumpParticles.Play();
    }

    public void PlayHitParticles(Vector2 direction)
    {
        RotateParticlesTowardsDirection(direction, hitParticles);
        hitParticles.Play();
    }

    public void PlayDeathParticles()
    {
        rushChargedParticles.Stop();
        rushParticles.Stop();
        spriteRenderer.DOFade(0, fadeDuration);
        deathParticles.Play();
        ScreenShake.Play();
    }

    public void PlayRushChargedParticles()
    {
        rushChargedParticles.Play();
        spriteRenderer.DOColor(rushColorColor, fadeDuration);
    }

    public void PlayRushParticles()
    {
        ScreenShake.Play();
        rushChargedParticles.Stop();
        flyParticles.Stop();
        rushParticles.Play();
        trailRenderer.colorGradient = trailRushColorGradient;
    }

    public void StopRushParticles()
    {
        flyParticles.Play();
        rushParticles.Stop();
        spriteRenderer.DOColor(startingColor, fadeDuration);
        trailRenderer.colorGradient = trailStartingColorGradient;
    }

    private void RotateParticlesTowardsDirection(Vector2 direction, ParticleSystem particles)
    {
        Quaternion rotation = particles.transform.rotation;
        Vector3 euler = rotation.eulerAngles;
        euler.z = Vector2.SignedAngle(Vector2.right, direction);
        rotation.eulerAngles = euler;
        particles.transform.rotation = rotation;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ScreenShake.Play();
    }
}