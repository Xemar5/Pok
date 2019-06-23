using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField]
    private TMP_Text scoreText = null;
    [SerializeField]
    private PlayerController player = null;
    [SerializeField]
    private CanvasGroup canvasGroup = null;
    [SerializeField]
    private Animator animator = null;
    [SerializeField]
    private string stateParam = "State";
    private float startTime = -1;
    private bool started = false;

    public int CurrentScore { get; private set; }
    public float HighestScore { get; private set; }
    public float HighestScoreTime { get; private set; }

    private void Awake()
    {
        player.OnJump += FirstJump;
        player.OnKill += Player_OnKill;
        int best = PlayerPrefs.GetInt("PlayerScore", 0);
        scoreText.text = best.ToString();
        canvasGroup.DOFade(1, 0);
    }

    private void Player_OnKill(PlayerController obj)
    {
        bool newHighScore = PlayerPrefs.GetInt("PlayerScore", 0) < CurrentScore;
        if (newHighScore)
        {
            animator.SetInteger(stateParam, 2);
        }
        started = false;
    }

    private void Update()
    {
        if (started == false)
        {
            return;
        }
        float time = Time.time - startTime;
        float score = player.transform.position.y;
        if (score > CurrentScore)
        {
            HighestScore = score;
            HighestScoreTime = time;
            CurrentScore = (int)score - (int)time;
            scoreText.text = CurrentScore.ToString();

        }
    }

    private void FirstJump(PlayerController obj)
    {
        animator.SetInteger(stateParam, 1);
        player.OnJump -= FirstJump;
        CurrentScore = 0;
        HighestScore = 0;
        HighestScoreTime = 0;
        scoreText.text = CurrentScore.ToString();
        startTime = Time.time;
        started = true;
    }

    public void Hide(float fadeDuration)
    {
        canvasGroup.DOFade(0, fadeDuration);
    }
}