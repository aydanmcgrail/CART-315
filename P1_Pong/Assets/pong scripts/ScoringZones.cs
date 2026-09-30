using UnityEngine;

public class ScoringZones : MonoBehaviour
{
	[Tooltip("Enable for the zone beyond the opponent's paddle; disable for the zone beyond the player's paddle.")]
	public bool playerScores = true;

	public GameManager gameManager;

	private void Awake()
	{
		if (gameManager == null)
		{
			gameManager = FindFirstObjectByType<GameManager>();
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.GetComponent<Ball>() == null || gameManager == null)
		{
			return;
		}

		if (playerScores)
		{
			gameManager.PlayerScores();
		}
		else
		{
			gameManager.OpponentScores();
		}
	}
}
