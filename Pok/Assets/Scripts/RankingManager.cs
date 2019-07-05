using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class RankingManager : MonoBehaviour
{
    [SerializeField]
    private MenuManager menuManager = null;
    [SerializeField]
    private CanvasGroup canvasGroup = null;
    [SerializeField]
    private float maxAwaitTime = 5f;

    [Header("Info Components")]
    [SerializeField]
    private CanvasGroup waitingForScoresText = null;
    [SerializeField]
    private CanvasGroup timeoutText = null;

    [Header("Entries")]
    [SerializeField]
    private CanvasGroup entriesCanvasGroup = null;
    [SerializeField]
    private List<LeaderboardEntry> entries = new List<LeaderboardEntry>();


    private ScoreResponse response = null;



    private void Awake()
    {
        for (int i = 0; i < entries.Count; i++)
        {
            entries[i].Disable();
        }
        entriesCanvasGroup.DOFade(0, 0);
        waitingForScoresText.DOFade(0, 0);
        timeoutText.DOFade(0, 0);
        canvasGroup.DOFade(0, 0);

        StartCoroutine(FirebaseManager.Instance.Initialize());
    }

    //private IEnumerator Firebase()
    //{
    //    string userGuid = PlayerPrefs.GetString("PlayerGuid", string.Empty);
    //    while (string.IsNullOrEmpty(userGuid) == true)
    //    {
    //        yield return StartCoroutine(FirebaseManager.Instance.GetId(response =>
    //        {
    //            PlayerPrefs.SetString("PlayerGuid", response.guid);
    //            userGuid = response.guid;
    //        }, () =>
    //        {
    //            Debug.LogWarning("Couldn't get user GUID. Retrying in 3 seconds.");
    //        }));

    //        if (string.IsNullOrEmpty(userGuid) == true)
    //        {
    //            yield return new WaitForSeconds(3);
    //        }
    //    }
    //    FirebaseManager.Instance.UserGuid = userGuid;
    //}


    public void SendScoreAndGetRanking(ScoreQuery localEntryData)
    {
        StartCoroutine(FirebaseManager.Instance.SendScoreAndGetRanking(localEntryData, response =>
        {
            this.response = response;
        }, () =>
        {
            Debug.LogWarning("Couldn't send the score or get leaderboards");
        }));
    }
    public void GetRanking(ScoreQuery localEntryData)
    {
        StartCoroutine(FirebaseManager.Instance.GetRanking(localEntryData, (response) =>
        {
            this.response = response;
        }, () =>
        {
            Debug.LogWarning("Couldn't get leaderboards");
        }));
    }


    public IEnumerator Show(float fadeDuration)
    {
        canvasGroup.DOFade(1, fadeDuration);
        float waitTimeStart = Time.time;
        if (response == null)
        {
            ShowWaitingForScores(fadeDuration);
            yield return new WaitWhile(() => response == null && Time.time - waitTimeStart < maxAwaitTime);
        }
        if (response != null)
        {
            StartCoroutine(Populate(response, fadeDuration));
            response = null;
        }
        else
        {
            StartCoroutine(ShowTimeout(fadeDuration));
        }

    }


    private void ShowWaitingForScores(float fadeDuration)
    {
        waitingForScoresText.DOFade(1, fadeDuration);
    }
    private IEnumerator ShowTimeout(float fadeDuration)
    {
        waitingForScoresText.DOFade(0, fadeDuration);
        timeoutText.DOFade(1, fadeDuration);
        yield return new WaitForSeconds(fadeDuration * 4f);
        menuManager.Hide();
    }

    private IEnumerator Populate(ScoreResponse response, float fadeDuration)
    {
        waitingForScoresText.DOFade(0, fadeDuration);
        entriesCanvasGroup.DOFade(1, fadeDuration);
        List<ScoreEntryData> orderedEntries = response.entries.OrderBy(x => x.rank).ToList();
        for (int i = 0; i < orderedEntries.Count; i++)
        {
            ScoreEntryData entryData = orderedEntries[i];
            entries[i].SetScoreEntryData(entryData, i == response.senderIndex, fadeDuration);
            yield return new WaitForSeconds(fadeDuration / 2f);
        }
        yield return new WaitForSeconds(fadeDuration * 10f);
        menuManager.Hide();
    }

}
