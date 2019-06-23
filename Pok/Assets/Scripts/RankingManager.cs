using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        FirebaseManager.Instance.OnInitialized += OnFirebaseInitialized;
        FirebaseManager.Instance.Initialize();
    }

    private async void OnFirebaseInitialized()
    {
        string userGuid = PlayerPrefs.GetString("PlayerGuid", string.Empty);
        if (string.IsNullOrEmpty(userGuid) == true)
        {
            GuidData data = null;
            while (data == null)
            {
                data = await FirebaseManager.Instance.GetId();
                if (data == null)
                {
                    await Task.Delay(3000); // wait for 3s and retry
                    if (Application.isPlaying == false)
                    {
                        return;
                    }
                }
                else
                {
                    PlayerPrefs.SetString("PlayerGuid", data.guid);
                    userGuid = data.guid;
                }
            }
        }
        FirebaseManager.Instance.UserGuid = userGuid;
    }


    public async void SendScoreAndGetRanking(ScoreQuery localEntryData)
    {
        this.response = await FirebaseManager.Instance.SendScoreAndGetRanking(localEntryData);
        if (this.response == null)
        {
            Debug.LogWarning("Couldn't send the score or get leaderboards");
        }
    }
    public async void GetRanking(ScoreQuery localEntryData)
    {
        string json = JsonUtility.ToJson(localEntryData);
        this.response = await FirebaseManager.Instance.GetRanking(json);
        if (this.response == null)
        {
            Debug.LogWarning("Couldn't get leaderboards");
        }
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
