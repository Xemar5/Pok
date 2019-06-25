using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if UNITY_ANDROID
using System.Threading.Tasks;
#endif
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
    [SerializeField]
    private Button leaderboardsButton = null;
    [SerializeField]
    private RankingManager rankingManager = null;

    [Header("Help")]
    [SerializeField]
    private Button helpButton = null;
    [SerializeField]
    private CanvasGroup helpCanvasGroup = null;



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
        helpButton.onClick.AddListener(ShowHelpFromMainMenu);

        ChangeVisibility(mobileInformPanel, false, 0);
        ChangeVisibility(webInformPanel, false, 0);
        ChangeVisibility(startMenuCanvasGroup, false, 0);

        ChangeVisibility(resultsCanvasGroup, false, 0);
        ChangeVisibility(durationCanvasGroup, false, 0);
        ChangeVisibility(heightCanvasGroup, false, 0);
        ChangeVisibility(helpCanvasGroup, false, 0);

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
            PlayerPrefs.DeleteKey("Scores");
            FirebaseManager.Instance.UserGuid = null;
        }
    }
#endif

    private void Player_OnKill(PlayerController playerController)
    {
        if (playerController == player)
        {
            bool newHighScore = PlayerPrefs.GetInt("PlayerScore", 0) < score.CurrentScore;
            if (newHighScore == true)
            {
                PlayerPrefs.SetInt("PlayerScore", score.CurrentScore);
                StartCoroutine(ShowResults());
            }
            else
            {
                Sequence sequence = DOTween.Sequence()
                    .SetDelay(1)
                    .OnComplete(Hide);
            }
        }
    }

    private void OnNameEditEnd(string name)
    {

        //if (eventSystem.currentSelectedGameObject == nameInput)
        try
        {
            eventSystem.SetSelectedGameObject(null);
        }
        catch (Exception) { }

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
        ChangeVisibility(webInformPanel, true);
#elif UNITY_ANDROID
        ChangeVisibility(mobileInformPanel, true);
#endif
    }
    private void StopInform()
    {
        player.CanMove = false;
#if UNITY_WEBGL
        ChangeVisibility(webInformPanel, false);
#elif UNITY_ANDROID
        ChangeVisibility(mobileInformPanel, false);
#endif
    }


    private void Show()
    {
        ChangeVisibility(startMenuCanvasGroup, true);
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
        ChangeVisibility(startMenuCanvasGroup, false);
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


        ChangeVisibility(resultsCanvasGroup, true);
        durationText.text = "-" + score.HighestScoreTime.ToString("F1");
        heightText.text = score.HighestScore.ToString("F1");

        yield return new WaitForSeconds(fadeDuration);
        ChangeVisibility(durationCanvasGroup, true);

        yield return new WaitForSeconds(fadeDuration * 2);
        ChangeVisibility(heightCanvasGroup, true);

        yield return new WaitForSeconds(fadeDuration * 6);

        ChangeVisibility(resultsCanvasGroup, false);
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

        ChangeVisibility(startMenuCanvasGroup, false);

        player.CanMove = false;
        StopInform();
        ShowRankings();
    }

    private void ShowHelpFromMainMenu()
    {
        StartCoroutine(ShowHelp());
    }
    private IEnumerator ShowHelp()
    {
        ChangeVisibility(startMenuCanvasGroup, false);
        ChangeVisibility(helpCanvasGroup, true);
        score.Hide(fadeDuration);
        StartInform();
        player.CanMove = false;

        bool exitHelp = false;
        Action<PlayerController> tryJump = x =>
        {
            exitHelp = true;
        };
        player.OnTryJump += tryJump;

        yield return new WaitWhile(() => exitHelp == false);

        player.OnTryJump -= tryJump;
        Hide();
    }


    private void ShowRankings()
    {
        score.Hide(fadeDuration);
        StartCoroutine(rankingManager.Show(fadeDuration));
    }

    private void ChangeVisibility(CanvasGroup canvasGroup, bool visible)
    {
        ChangeVisibility(canvasGroup, visible, fadeDuration);
    }
    private void ChangeVisibility(CanvasGroup canvasGroup, bool visible, float fadeDuration)
    {
        if (visible == true)
        {
            canvasGroup.DOFade(1, fadeDuration);
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            canvasGroup.DOFade(0, fadeDuration);
            canvasGroup.blocksRaycasts = false;
        }
    }

}