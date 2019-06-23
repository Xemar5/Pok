using Firebase;
using Firebase.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    private static FirebaseManager instance;
    public static FirebaseManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("FirebaseManager").AddComponent<FirebaseManager>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }


    private FirebaseApp app;
    private FirebaseFunctions functions;

    public event Action OnInitialized;

    public bool IsInitialized => app != null;
    public string UserGuid { get; set; }

    public async void Initialize()
    {
        if (app != null)
        {
            Debug.Log("Firebase already initialized");
            return;
        }
        try
        {
            DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (status == DependencyStatus.Available)
            {
                app = FirebaseApp.DefaultInstance;
                functions = FirebaseFunctions.GetInstance(app);
                OnInitialized?.Invoke();
            }
            else
            {
                throw new FirebaseException(0, "Couldn't check and fix dependencies with resulting status: " + status);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Couldn't initialize firebase: " + e);
            Debug.Log("Retrying in 3 sec...");
            await Task.Delay(3000);
            Initialize();
        }
    }


    public async Task<GuidData> GetId()
    {
        try
        {
            HttpsCallableResult response = await functions.GetHttpsCallable("getUniqueGuid").CallAsync();
            GuidData data = new GuidData();
            data.guid = (string)response.Data;
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError("Couldn't get the user GUID: " + e);
            return null;
        }
    }

    public async Task<ScoreResponse> SendScoreAndGetRanking(ScoreQuery localEntryData)
    {
        string json = JsonUtility.ToJson(localEntryData);
        try
        {
            await functions.GetHttpsCallable("setScore").CallAsync(json);
        }
        catch (Exception e)
        {
            Debug.LogError("Couldn't send user score: " + e);
            return null;
        }

        try
        {
            HttpsCallableResult response = await functions.GetHttpsCallable("getRanking").CallAsync(json);
            ScoreResponse data = JsonUtility.FromJson<ScoreResponse>((string)response.Data);
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError("Couldn't receive leaderboards: " + e);
            return null;
        }
    }
}