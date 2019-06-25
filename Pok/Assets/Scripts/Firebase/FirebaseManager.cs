using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    private static FirebaseManager instance;
    public static FirebaseManager Instance
    {
        get
        {
            if (instance == null && Application.isPlaying == true)
            {
                instance = new GameObject("FirebaseManager").AddComponent<FirebaseManager>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }

    private class ReverseComparer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return y.CompareTo(x);
        }
    }

    private List<ScoreEntryData> scores = new List<ScoreEntryData>();


    public string UserGuid { get; set; }

    public event Action OnInitialized;

    public void Initialize()
    {
        string scoresJson = PlayerPrefs.GetString("Scores", "{}");
        scores = JsonConvert.DeserializeObject<List<ScoreEntryData>>(scoresJson);
    }


    public GuidData GetId()
    {
        GuidData data = new GuidData();
        int id = PlayerPrefs.GetInt("PlayerGuid", 0);
        data.guid = id.ToString();
        return data;
    }

    public ScoreResponse SendScoreAndGetRanking(ScoreQuery localEntryData)
    {
        SendScore(localEntryData);
        return GetRanking(localEntryData);
    }

    private bool SendScore(ScoreQuery localEntryData)
    {
        ScoreEntryData newEntry = new ScoreEntryData();
        newEntry.guid = localEntryData.guid;
        newEntry.score = localEntryData.score;
        newEntry.invscore = -localEntryData.score;
        newEntry.username = localEntryData.username;
        scores.Add(newEntry);
        scores = scores.OrderBy(x => x.score, new ReverseComparer()).Take(5).ToList();
        int rank = 1;
        for (int i = 0; i < scores.Count; i++)
        {
            scores[i].rank = rank++;
        }
        string scoresJson = JsonConvert.SerializeObject(scores);
        PlayerPrefs.SetString("Scores", scoresJson);
        return true;
    }

    public ScoreResponse GetRanking(ScoreQuery localEntryData)
    {
        ScoreResponse response = new ScoreResponse();
        response.entries = scores.Take(5).ToList();
        int index = response.entries.FindIndex(x => x.guid == localEntryData.guid);
        if (index == -1)
        {
            ScoreEntryData newEntry = scores.First(x => x.guid == localEntryData.guid);
            response.entries.Add(newEntry);
            response.senderIndex = 5;
        }
        else
        {
            response.senderIndex = index;
        }
        return response;

    }

}