using System.Collections;
using UnityEngine;

public class UtensilSpawner : MonoBehaviour
{
    [SerializeField] private Sprite forkSprite;
    [SerializeField] private Sprite knifeSprite;
    [SerializeField] private float minimumSpawnInterval = 2.5f;
    [SerializeField] private float maximumSpawnInterval = 4f;
    [SerializeField] private float spawnOffset = 1f;
    [SerializeField] private int minimumRowSize = 8;
    [SerializeField] private int maximumRowSize = 12;
    [SerializeField] private float spacing = 0.75f;
    [SerializeField] private float baseHeight = -2.6f;
    [SerializeField] private float spriteScale = 0.28f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpChance = 0.4f;
    [SerializeField] private float passedBirdJumpChance = 0.85f;
    [SerializeField] private float minimumJumpDelay = 0.4f;
    [SerializeField] private float maximumJumpDelay = 1.5f;
    [SerializeField] private float minimumJumpHeight = 1.5f;
    [SerializeField] private float maximumJumpHeight = 3.5f;
    [SerializeField] private float jumpDuration = 0.8f;

    private Camera mainCamera;
    private Player player;
    private Coroutine spawnRoutine;

    private void OnEnable()
    {
        if (forkSprite == null || knifeSprite == null) {
            Debug.LogError("Assign both utensil sprites to the UtensilSpawner component.", this);
            enabled = false;
            return;
        }

        if (minimumSpawnInterval > maximumSpawnInterval) {
            Debug.LogWarning(
                "UtensilSpawner spawn interval bounds were reversed; swapping them.",
                this);
            float previousMinimum = minimumSpawnInterval;
            minimumSpawnInterval = maximumSpawnInterval;
            maximumSpawnInterval = previousMinimum;
        }

        if (minimumSpawnInterval <= 0f || maximumSpawnInterval < minimumSpawnInterval ||
            minimumJumpDelay < 0f || maximumJumpDelay < minimumJumpDelay ||
            minimumJumpHeight < 0f || maximumJumpHeight < minimumJumpHeight ||
            minimumRowSize < 1 || maximumRowSize < minimumRowSize ||
            jumpChance < 0f || jumpChance > 1f ||
            passedBirdJumpChance < 0f || passedBirdJumpChance > 1f || spacing <= 0f ||
            spriteScale <= 0f || speed < 0f || jumpDuration <= 0f) {
            Debug.LogError(
                $"UtensilSpawner has invalid settings. Spawn interval: {minimumSpawnInterval}-{maximumSpawnInterval}; " +
                $"jump delay: {minimumJumpDelay}-{maximumJumpDelay}; " +
                $"jump height: {minimumJumpHeight}-{maximumJumpHeight}; " +
                $"row size: {minimumRowSize}-{maximumRowSize}; " +
                $"jump chances: {jumpChance}, {passedBirdJumpChance}; spacing: {spacing}; " +
                $"sprite scale: {spriteScale}; speed: {speed}; jump duration: {jumpDuration}.",
                this);
            enabled = false;
            return;
        }

        mainCamera = Camera.main;
        if (mainCamera == null) {
            Debug.LogError("UtensilSpawner requires a camera tagged MainCamera.", this);
            enabled = false;
            return;
        }

        player = FindFirstObjectByType<Player>();
        if (player == null) {
            Debug.LogError("UtensilSpawner requires a Player in the scene.", this);
            enabled = false;
            return;
        }

        spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    private void OnDisable()
    {
        if (spawnRoutine != null) {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (true) {
            yield return new WaitForSeconds(Random.Range(minimumSpawnInterval, maximumSpawnInterval));
            SpawnRow();
        }
    }

    private void SpawnRow()
    {
        Vector3 rightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1f, 0.5f, 0f));
        int rowSize = Random.Range(minimumRowSize, maximumRowSize + 1);

        for (int i = 0; i < rowSize; i++) {
            SpawnUtensil(rightEdge.x + spawnOffset + i * spacing);
        }
    }

    private void SpawnUtensil(float xPosition)
    {
        Sprite sprite = Random.value < 0.5f ? forkSprite : knifeSprite;
        GameObject hazard = new GameObject(sprite.name);
        hazard.tag = "Obstacle";

        hazard.transform.position = new Vector3(xPosition, baseHeight, -2f);
        hazard.transform.localScale = Vector3.one * spriteScale;

        SpriteRenderer spriteRenderer = hazard.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 1;

        BoxCollider2D boxCollider = hazard.AddComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;
        boxCollider.size = sprite.bounds.size;

        Utensil utensil = hazard.AddComponent<Utensil>();
        utensil.Initialize(
            mainCamera,
            player.transform,
            speed,
            baseHeight,
            Random.value < jumpChance,
            passedBirdJumpChance,
            Random.Range(minimumJumpDelay, maximumJumpDelay),
            Random.Range(minimumJumpHeight, maximumJumpHeight),
            jumpDuration);
    }
}
