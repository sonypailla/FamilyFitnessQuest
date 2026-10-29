using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoopGameManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text teamScoreText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Button player1Button;
    [SerializeField] private Button player2Button;
    [SerializeField] private Button startGameButton;
    [SerializeField] private TMP_Text startButtonText;

    [Header("Game Settings")]
    [SerializeField] private int goalScore = 20;
    [SerializeField] private float roundDuration = 30f;

    private int player1Score;
    private int player2Score;
    private float timeRemaining;
    private bool gameActive;

    private void Start()
    {
        player1Button.onClick.AddListener(() => AddPoint(1));
        player2Button.onClick.AddListener(() => AddPoint(2));
        startGameButton.onClick.AddListener(StartGame);

        player1Button.interactable = false;
        player2Button.interactable = false;

        timeRemaining = roundDuration;
        UpdateScore();
        UpdateTimer();
        startButtonText.text = "Start Game";
    }

    private void Update()
    {
        if (!gameActive) return;

        timeRemaining -= Time.unscaledDeltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            EndGame(false);
        }

        UpdateTimer();
    }

    public void StartGame()
    {
        player1Score = 0;
        player2Score = 0;
        timeRemaining = roundDuration;
        gameActive = true;

        player1Button.interactable = true;
        player2Button.interactable = true;
        startButtonText.text = "Restart Game";

        UpdateScore();
        UpdateTimer();
    }

    private void AddPoint(int playerNumber)
    {
        if (!gameActive) return;

        if (playerNumber == 1)
            player1Score++;
        else
            player2Score++;

        UpdateScore();

        if (player1Score + player2Score >= goalScore)
            EndGame(true);
    }

    private void UpdateScore()
    {
        int total = player1Score + player2Score;
        teamScoreText.text =
            $"Team: {total} / {goalScore} | P1: {player1Score} | P2: {player2Score}";
    }

    private void UpdateTimer()
    {
        timerText.text =
            $"Time: {Mathf.CeilToInt(timeRemaining)} seconds";
    }

    private void EndGame(bool won)
    {
        gameActive = false;
        player1Button.interactable = false;
        player2Button.interactable = false;

        timerText.text = won
            ? "Team Win! Great cooperation!"
            : "Time Up! Try Again";

        startButtonText.text = "Play Again";
    }
}