using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntry : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvasGroup = null;
    [SerializeField]
    private TMP_Text rank = null;
    [SerializeField]
    private TMP_Text username = null;
    [SerializeField]
    private TMP_Text score = null;
    [SerializeField]
    private Image background = null;

    public void SetScoreEntryData(ScoreEntryData data, bool isLocal, float fadeDuration)
    {
        canvasGroup.DOFade(1, fadeDuration);
        rank.text = data.rank.ToString();
        username.text = data.username;
        score.text = data.score.ToString();
        if (isLocal == true)
        {
            background.gameObject.SetActive(true);
            rank.color = Color.black;
            username.color = Color.black;
            score.color = Color.black;
        }
        else
        {
            background.gameObject.SetActive(false);
            rank.color = Color.white;
            username.color = Color.white;
            score.color = Color.white;
        }
    }

    public void Disable()
    {
        canvasGroup.DOFade(0, 0);
    }
}
