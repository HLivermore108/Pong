using TMPro;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;
using System.Collections;

public class GameManager : NetworkBehaviour
{
    [Header("Win Condition")]
    [SerializeField] private int pointsToWin = 5;

    [Header("Scene References")]
    [SerializeField] private BallMovement ball; // If spawned, set via SetBallReference()

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI leftScoreText;
    [SerializeField] private TextMeshProUGUI rightScoreText;
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private GameObject startButton;

    [Header("Ball Reset Settings")]
    [SerializeField] private float launchDelay = 2f;

    private Coroutine resetRoutine;

    // Networked state
    public NetworkVariable<int> leftScore = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<int> rightScore = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<bool> gameOver = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<FixedString64Bytes> winMessage = new NetworkVariable<FixedString64Bytes>(
        "", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        leftScore.OnValueChanged += (_, __) => UpdateScoreUI();
        rightScore.OnValueChanged += (_, __) => UpdateScoreUI();
        gameOver.OnValueChanged += (_, __) => UpdateGameOverUI();
        winMessage.OnValueChanged += (_, __) => UpdateWinText();

        UpdateScoreUI();
        UpdateWinText();
        UpdateGameOverUI();
    }

    private void UpdateScoreUI()
    {
        if (leftScoreText != null) leftScoreText.text = leftScore.Value.ToString();
        if (rightScoreText != null) rightScoreText.text = rightScore.Value.ToString();
    }

    private void UpdateWinText()
    {
        if (winText == null) return;

        string msg = winMessage.Value.ToString();
        winText.text = msg;
        winText.gameObject.SetActive(!string.IsNullOrEmpty(msg));
    }

    private void UpdateGameOverUI()
    {
        if (startButton != null)
        {
            bool showForHost = IsServer && (gameOver.Value || (leftScore.Value == 0 && rightScore.Value == 0));
            startButton.SetActive(showForHost);
        }
    }

    // Called by UI Button (host only)
    public void StartGame()
    {
        if (!IsServer) return;

        // Cancel any pending delayed launch from a previous point
        CancelResetRoutine();

        leftScore.Value = 0;
        rightScore.Value = 0;
        gameOver.Value = false;
        winMessage.Value = "";

        if (startButton != null) startButton.SetActive(false);

        // Optional: ensure ball is centered/stopped before launching
        if (ball != null)
            ball.StopBallServer();

        LaunchBallTowardsLeftPlayer();

        Debug.Log($"StartGame called. IsServer={IsServer} ballNull={(ball == null)}");
    }

    // Called by ScoreZones on the server
    public void ScoreLeft()
    {
        if (!IsServer || gameOver.Value) return;

        leftScore.Value++;
        CheckWinOrContinue(scoredByLeft: true);
    }

    public void ScoreRight()
    {
        if (!IsServer || gameOver.Value) return;

        rightScore.Value++;
        CheckWinOrContinue(scoredByLeft: false);
    }

    private void CheckWinOrContinue(bool scoredByLeft)
    {
        if (leftScore.Value >= pointsToWin)
        {
            SetGameOver("LEFT PLAYER WINS!");
            return;
        }

        if (rightScore.Value >= pointsToWin)
        {
            SetGameOver("RIGHT PLAYER WINS!");
            return;
        }

        // Reset & delay launch toward the player who conceded the point
        StartResetAndDelayLaunch(scoredByLeft);
    }

    private void SetGameOver(string message)
    {
        if (!IsServer) return;

        // Stop any pending delayed launch
        CancelResetRoutine();

        gameOver.Value = true;
        winMessage.Value = message;

        if (ball != null)
            ball.StopBallServer();

        if (startButton != null)
            startButton.SetActive(true);
    }

    private void LaunchBallTowardsLeftPlayer()
    {
        if (ball == null) return;
        ball.ResetAndLaunchServer(new Vector2(-1f, Random.Range(-0.5f, 0.5f)));
    }

    private void LaunchBallTowardsRightPlayer()
    {
        if (ball == null) return;
        ball.ResetAndLaunchServer(new Vector2(1f, Random.Range(-0.5f, 0.5f)));
    }

    // If ball is spawned dynamically, call this after spawning to assign it.
    public void SetBallReference(BallMovement newBall)
    {
        if (!IsServer) return;
        ball = newBall;
    }

    private void StartResetAndDelayLaunch(bool leftScored)
    {
        if (!IsServer) return;

        // Cancel any previous pending launch (prevents snap/late launches)
        CancelResetRoutine();

        resetRoutine = StartCoroutine(ResetAndDelayLaunchRoutine(leftScored));
    }

    private IEnumerator ResetAndDelayLaunchRoutine(bool leftScored)
    {
        if (!IsServer) yield break;

        if (ball != null)
            ball.StopBallServer();

        yield return new WaitForSeconds(launchDelay);

        if (gameOver.Value) yield break;

        // If left scored, launch toward right player (who conceded)
        if (leftScored)
            LaunchBallTowardsRightPlayer();
        else
            LaunchBallTowardsLeftPlayer();

        resetRoutine = null;
    }

    private void CancelResetRoutine()
    {
        if (resetRoutine != null)
        {
            StopCoroutine(resetRoutine);
            resetRoutine = null;
        }
    }
}