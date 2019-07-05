using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

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

    //private List<ScoreEntryData> scores = new List<ScoreEntryData>();


    public string UserGuid { get; set; }

    public event Action OnInitialized;

    public IEnumerator Initialize()
    {
        string userGuid = PlayerPrefs.GetString("PlayerGuid", string.Empty);
        while (string.IsNullOrEmpty(userGuid) == true)
        {
            yield return StartCoroutine(FirebaseManager.Instance.GetId(response =>
            {
                PlayerPrefs.SetString("PlayerGuid", response.guid);
                userGuid = response.guid;
            }, () =>
            {
                Debug.LogWarning("Couldn't get user GUID. Retrying in 3 seconds.");
            }));

            if (string.IsNullOrEmpty(userGuid) == true)
            {
                yield return new WaitForSeconds(3);
            }
        }
        FirebaseManager.Instance.UserGuid = userGuid;
    }


    public IEnumerator GetId(Action<GuidData> onSuccess, Action onFailure)
    {
        using (UnityWebRequest request = UnityWebRequest.Get("https://us-central1-pok-game.cloudfunctions.net/getUniqueGuidRequest"))
        {
            yield return request.SendWebRequest();

            if (request.isNetworkError == true)
            {
                Debug.LogError("An error has occured while getting user GUID: " + request.error);
                onFailure?.Invoke();
            }
            else
            {
                Debug.Log($"Successfully retrieved GUID: {request.downloadHandler.text}");
                GuidData data = new GuidData();
                data.guid = request.downloadHandler.text;
                onSuccess?.Invoke(data);
            }
        }
    }

    public IEnumerator SendScoreAndGetRanking(ScoreQuery localEntryData, Action<ScoreResponse> onSuccess, Action onFailure)
    {
        bool sendScoreGood = false;
        yield return StartCoroutine(SendScore(localEntryData, () => sendScoreGood = true, onFailure));
        if (sendScoreGood == true)
        {
            yield return StartCoroutine(GetRanking(localEntryData, onSuccess, onFailure));
        }
    }

    private IEnumerator SendScore(ScoreQuery localEntryData, Action onSuccess, Action onFailure)
    {
        string queryJson = JsonConvert.SerializeObject(localEntryData);
        byte[] queryBytes = new UTF8Encoding().GetBytes(queryJson);
        using (UnityWebRequest request = new UnityWebRequest("https://us-central1-pok-game.cloudfunctions.net/setScoreRequest", "POST"))
        {
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(queryBytes);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();
            if (request.isNetworkError == true)
            {
                Debug.LogError("An error has occured while puting Score data: " + request.error);
                onFailure?.Invoke();
            }
            else
            {
                Debug.Log("Successfully send score");
                onSuccess?.Invoke();
            }
        }

    }

    public IEnumerator GetRanking(ScoreQuery localEntryData, Action<ScoreResponse> onSuccess, Action onFailure)
    {
        Debug.Log("getRank: " + localEntryData.guid);
        string queryJson = JsonConvert.SerializeObject(localEntryData);
        byte[] queryBytes = new UTF8Encoding().GetBytes(queryJson);
        Debug.Log("getRank: " + queryJson);
        using (UnityWebRequest request = new UnityWebRequest("https://us-central1-pok-game.cloudfunctions.net/getRankingRequest", "POST"))
        {
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(queryBytes);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();
            if (request.isNetworkError == true)
            {
                Debug.LogError("An error has occured while puting Rank request: " + request.error);
                onFailure?.Invoke();
            }
            else
            {
                Debug.Log($"Successfully retrieved Ranks: {request.downloadHandler.text}");
                ScoreResponse data = JsonConvert.DeserializeObject<ScoreResponse>(request.downloadHandler.text);
                onSuccess?.Invoke(data);
            }
        }
    }

}


//using Firebase;
//using Firebase.Functions;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//public class FirebaseManager : MonoBehaviour
//{
//    private static FirebaseManager instance;
//    public static FirebaseManager Instance
//    {
//        get
//        {
//            if (instance == null && Application.isPlaying == true)
//            {
//                instance = new GameObject("FirebaseManager").AddComponent<FirebaseManager>();
//                DontDestroyOnLoad(instance.gameObject);
//            }
//            return instance;
//        }
//    }


//    private FirebaseApp app;
//    private FirebaseFunctions functions;

//    public event Action OnInitialized;

//    public bool IsInitialized => app != null;
//    public string UserGuid { get; set; }


//    public async void Initialize()
//    {
//        if (app != null)
//        {
//            Debug.Log("Firebase already initialized");
//            return;
//        }
//        try
//        {
//            DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();
//            if (status == DependencyStatus.Available)
//            {
//                app = FirebaseApp.DefaultInstance;
//                functions = FirebaseFunctions.GetInstance(app);
//                OnInitialized?.Invoke();
//            }
//            else
//            {
//                throw new FirebaseException(0, "Couldn't check and fix dependencies with resulting status: " + status);
//            }
//        }
//        catch (Exception e)
//        {
//            Debug.LogError("Couldn't initialize firebase: " + e);
//            Debug.Log("Retrying in 3 sec...");
//            await Task.Delay(3000);
//            Initialize();
//        }
//    }


//    public async Task<GuidData> GetId()
//    {
//        try
//        {
//            HttpsCallableResult response = await functions.GetHttpsCallable("getUniqueGuid").CallAsync();
//            GuidData data = new GuidData();
//            data.guid = (string)response.Data;
//            return data;
//        }
//        catch (Exception e)
//        {
//            Debug.LogError("Couldn't get the user GUID: " + e);
//            return null;
//        }
//    }

//    public async Task<ScoreResponse> SendScoreAndGetRanking(ScoreQuery localEntryData)
//    {
//        string json = JsonUtility.ToJson(localEntryData);
//        if (await SendScore(json) == false)
//        {
//            return null;
//        }
//        return await GetRanking(json);
//    }

//    private async Task<bool> SendScore(string json)
//    {
//        try
//        {
//            await functions.GetHttpsCallable("setScore").CallAsync(json);
//            return true;
//        }
//        catch (Exception e)
//        {
//            Debug.LogError("Couldn't send user score: " + e);
//            return false;
//        }
//    }

//    public async Task<ScoreResponse> GetRanking(string json)
//    {
//        try
//        {
//            HttpsCallableResult response = await functions.GetHttpsCallable("getRanking").CallAsync(json);
//            ScoreResponse data = JsonUtility.FromJson<ScoreResponse>((string)response.Data);
//            return data;
//        }
//        catch (Exception e)
//        {
//            Debug.LogError("Couldn't receive leaderboards: " + e);
//            return null;
//        }
//    }
//}