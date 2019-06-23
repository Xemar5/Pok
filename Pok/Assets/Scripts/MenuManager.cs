using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private PlayerController player = null;
    [SerializeField]
    private float fadeDuration = 0.3f;
    [SerializeField]
    private EventSystem eventSystem = null;
    [SerializeField]
    private RankingManager rankingManager = null;
    [SerializeField]
    private Button leaderboardsButton = null;

    [Header("Background")]
    [SerializeField]
    private Image blackScreen = null;

    [Header("Start Menu")]
    [SerializeField]
    private CanvasGroup startMenuCanvasGroup = null;
    [SerializeField]
    private TMP_InputField nameInput = null;

    [Header("Inform Menu")]
    [SerializeField]
    private CanvasGroup mobileInformPanel = null;
    [SerializeField]
    private CanvasGroup webInformPanel = null;

    [Header("Results Menu")]
    [SerializeField]
    private CanvasGroup resultsCanvasGroup = null;
    [SerializeField]
    private CanvasGroup durationCanvasGroup = null;
    [SerializeField]
    private TMP_Text durationText = null;
    [SerializeField]
    private CanvasGroup heightCanvasGroup = null;
    [SerializeField]
    private TMP_Text heightText = null;

    [Header("Score")]
    [SerializeField]
    private Score score = null;

    private Coroutine informCoroutine = null;

    public bool GameStarted { get; private set; }

    private IEnumerator Start()
    {
        nameInput.text = PlayerPrefs.GetString("PlayerName", string.Empty);

        player.CanMove = false;
        player.OnKill += Player_OnKill;
        player.OnJump += FirstJump;
        nameInput.onEndEdit.AddListener(OnNameEditEnd);
        nameInput.onSelect.AddListener(x => StopInform());
        leaderboardsButton.onClick.AddListener(ShowLeaderboardsFromMainMenu);

        mobileInformPanel.DOFade(0, 0);
        webInformPanel.DOFade(0, 0);
        startMenuCanvasGroup.DOFade(0, 0);

        resultsCanvasGroup.DOFade(0, 0);
        durationCanvasGroup.DOFade(0, 0);
        heightCanvasGroup.DOFade(0, 0);

        blackScreen.DOFade(1, 0);

        Show();

        yield return new WaitForSeconds(fadeDuration);
        if (string.IsNullOrEmpty(nameInput.text) == true)
        {
            nameInput.Select();
            nameInput.ActivateInputField();
        }
        else
        {
            StartInform();
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) == true)
        {
            PlayerPrefs.DeleteKey("PlayerScore");
            PlayerPrefs.DeleteKey("PlayerName");
            PlayerPrefs.DeleteKey("PlayerGuid");
            FirebaseManager.Instance.UserGuid = null;
        }
    }
#endif

    private void Player_OnKill(PlayerController playerController)
    {
        if (playerController == player)
        {
            playerController.CanMove = false;
            bool newHighScore = PlayerPrefs.GetInt("PlayerScore", 0) < score.CurrentScore;
            if (newHighScore == true)
            {
                PlayerPrefs.SetInt("PlayerScore", score.CurrentScore);
                StartCoroutine(ShowResults());
            }
            else
            {
                Hide();
            }
        }
    }

    private void OnNameEditEnd(string name)
    {
        if (eventSystem.currentSelectedGameObject == nameInput)
        {
            eventSystem.SetSelectedGameObject(null);
        }
        if (name.Length > 0)
        {
            PlayerPrefs.SetString("PlayerName", name.ToLower());
            StartInform();
        }
    }



    private void FirstJump(PlayerController playerController)
    {
        player.OnJump -= FirstJump;
        StopInform();
        player.CanMove = true;
        StartGame();
    }

    private void StartInform()
    {
        player.CanMove = true;
#if UNITY_WEBGL
        webInformPanel.DOFade(1, fadeDuration);
#elif UNITY_ANDROID
        mobileInformPanel.DOFade(1, fadeDuration);
#endif
    }
    private void StopInform()
    {
        player.CanMove = false;
#if UNITY_WEBGL
        webInformPanel.DOFade(0, fadeDuration);
#elif UNITY_ANDROID
        mobileInformPanel.DOFade(0, fadeDuration);
#endif
    }


    private void Show()
    {
        startMenuCanvasGroup.DOFade(1, fadeDuration);
        blackScreen.DOFade(0, fadeDuration * 2);
    }
    public void Hide()
    {
        blackScreen.DOFade(1, fadeDuration * 2)
            .OnComplete(RestartGame);
    }


    private void StartGame()
    {
        GameStarted = true;
        startMenuCanvasGroup.DOFade(0, fadeDuration);
    }
    private void RestartGame()
    {
        SceneManager.LoadScene(0);
    }



    private IEnumerator ShowResults()
    {
        ScoreQuery query = new ScoreQuery();
        query.username = nameInput.text;
        query.score = score.CurrentScore;
        query.invscore = -score.CurrentScore;
        query.guid = FirebaseManager.Instance.UserGuid;
        rankingManager.SendScoreAndGetRanking(query);


        resultsCanvasGroup.DOFade(1, fadeDuration);
        durationText.text = "-" + score.HighestScoreTime.ToString("F1");
        heightText.text = score.HighestScore.ToString("F1");

        yield return new WaitForSeconds(fadeDuration);
        durationCanvasGroup.DOFade(1, fadeDuration);

        yield return new WaitForSeconds(fadeDuration * 2);
        heightCanvasGroup.DOFade(1, fadeDuration);

        yield return new WaitForSeconds(fadeDuration * 6);

        resultsCanvasGroup.DOFade(0, fadeDuration);
        ShowRankings();
    }

    private void ShowLeaderboardsFromMainMenu()
    {
        ScoreQuery query = new ScoreQuery();
        query.username = PlayerPrefs.GetString("PlayerName", "");
        query.score = PlayerPrefs.GetInt("PlayerScore", 0);
        query.invscore = -query.score;
        query.guid = string.IsNullOrWhiteSpace(query.username) == true ? "" : FirebaseManager.Instance.UserGuid;
        rankingManager.GetRanking(query);
        startMenuCanvasGroup.DOFade(0, fadeDuration);
        player.CanMove = false;
        StopInform();
        ShowRankings();
    }

    private void ShowRankings()
    {
        score.Hide(fadeDuration);
        StartCoroutine(rankingManager.Show(fadeDuration));
    }

}