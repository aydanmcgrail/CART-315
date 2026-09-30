using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Ball ball;
    public TMP_Text playerScoreText;
    public TMP_Text opponentScoreText;
    public Paddle playerPaddle;
    public Paddle opponentPaddle;

    private int playerScore;
    private int opponentScore;

    private void Awake()
    {
        if (ball == null)
        {
            ball = FindFirstObjectByType<Ball>();
        }

        if (playerPaddle == null)
        {
            playerPaddle = FindFirstObjectByType<PlayerPaddle>();
        }

        if (opponentPaddle == null)
        {
            opponentPaddle = FindFirstObjectByType<ComputerPaddle>();
        }

        UpdateScoreDisplay();
    }

    public void PlayerScores()
    {
        playerScore++;
        UpdateScoreDisplay();
        ResetPoint();
    }

    public void OpponentScores()
    {
        opponentScore++;
        UpdateScoreDisplay();
        ResetPoint();
    }

    private void ResetPoint()
    {
        if (ball != null)
        {
            ball.ResetBall();
        }

        if (playerPaddle != null)
        {
            playerPaddle.ResetPosition();
        }

        if (opponentPaddle != null)
        {
            opponentPaddle.ResetPosition();
        }
    }

    private void UpdateScoreDisplay()
    {
        if (playerScoreText != null)
        {
            playerScoreText.text = playerScore.ToString();
        }

        if (opponentScoreText != null)
        {
            opponentScoreText.text = opponentScore.ToString();
        }
    }
}
