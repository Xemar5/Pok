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
    [SerializeField]
    private Color lightColor = new Color();
    [SerializeField]
    private Color darkColor = new Color();

    public void SetScoreEntryData(ScoreEntryData data, bool isLocal, float fadeDuration)
    {
        canvasGroup.DOFade(1, fadeDuration);
        rank.text = data.rank.ToString();
        username.text = data.username;
        score.text = data.score.ToString();
        if (isLocal == true)
        {
            background.gameObject.SetActive(true);
            rank.color = darkColor;
            username.color = darkColor;
            score.color = darkColor;
        }
        else
        {
            background.gameObject.SetActive(false);
            rank.color = lightColor;
            username.color = lightColor;
            score.color = lightColor;
        }
    }

    public void Disable()
    {
        canvasGroup.DOFade(0, 0);
    }
}
