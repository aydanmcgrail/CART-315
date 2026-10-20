using UnityEngine;
using System.Collections.Generic;

public class Parallax : MonoBehaviour
{
    public float animationSpeed = 1f;
    [SerializeField] private bool mirrorAlternateTiles = true;
    [SerializeField] private float wheatfieldHeightOffset = 0.5f;
    [SerializeField] private float foregroundYOffset = -0.25f;
    [SerializeField] private int wheatfieldSortingOrder = -1;
    [SerializeField] private int foregroundWheatfieldSortingOrder = 2;
    [SerializeField] private int backgroundSortingOrder = -10;

    private static GameObject foregroundWheatfield;
    private MeshRenderer meshRenderer;
    private SpriteRenderer spriteRenderer;
    private readonly List<SpriteRenderer> tiles = new List<SpriteRenderer>();
    private readonly List<GameObject> generatedTiles = new List<GameObject>();
    private Camera mainCamera;
    private float tileWidth;
    private bool ownsForegroundWheatfield;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null) {
            bool isWheatfield = spriteRenderer.sprite != null &&
                spriteRenderer.sprite.name.Contains("wheatfield");
            bool isForegroundWheatfield = gameObject.name == "Foreground Wheatfield";

            if (isWheatfield) {
                if (isForegroundWheatfield) {
                    spriteRenderer.sortingOrder = foregroundWheatfieldSortingOrder;
                } else {
                    Vector3 raisedPosition = transform.position;
                    raisedPosition.y += wheatfieldHeightOffset;
                    transform.position = raisedPosition;
                    spriteRenderer.sortingOrder = wheatfieldSortingOrder;
                }
            } else if (spriteRenderer.sprite.name.Contains("fond")) {
                spriteRenderer.sortingOrder = backgroundSortingOrder;
            }

            mainCamera = Camera.main;
            if (mainCamera == null || spriteRenderer.sprite == null) {
                Debug.LogError("A scrolling sprite background requires a MainCamera and an assigned sprite.", this);
                enabled = false;
                return;
            }

            tileWidth = spriteRenderer.bounds.size.x;
            if (tileWidth <= 0f) {
                Debug.LogError("The scrolling sprite must have a non-zero width.", this);
                enabled = false;
                return;
            }

            Vector3 cameraPosition = mainCamera.ViewportToWorldPoint(
                new Vector3(0f, 0.5f, Mathf.Abs(transform.position.z - mainCamera.transform.position.z)));
            float cameraLeft = cameraPosition.x;
            float cameraRight = cameraLeft + mainCamera.orthographicSize * 2f * mainCamera.aspect;

            float originalLeft = spriteRenderer.bounds.min.x;
            float originalRight = spriteRenderer.bounds.max.x;
            int tilesToLeft = Mathf.CeilToInt(Mathf.Max(0f, originalLeft - cameraLeft) / tileWidth);
            int tilesToRight = Mathf.CeilToInt(Mathf.Max(0f, cameraRight - originalRight) / tileWidth);

            for (int i = tilesToLeft; i > 0; i--) {
                SpriteRenderer tile = CreateTile(-i, i % 2 != 0);
                tiles.Add(tile);
            }

            tiles.Add(spriteRenderer);

            for (int i = 1; i <= tilesToRight + 1; i++) {
                tiles.Add(CreateTile(i, i % 2 != 0));
            }

            Collider2D sceneryCollider = GetComponent<Collider2D>();
            if (sceneryCollider != null) {
                sceneryCollider.enabled = false;
            }

        } else {
            meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer == null) {
                Debug.LogError("Parallax requires a SpriteRenderer or MeshRenderer.", this);
                enabled = false;
            }
        }
    }

    private void Start()
    {
        if (spriteRenderer != null && IsHighestWheatfieldLayer()) {
            CreateForegroundWheatfield();
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < generatedTiles.Count; i++) {
            if (generatedTiles[i] != null) {
                generatedTiles[i].SetActive(false);
            }
        }
    }

    private void OnEnable()
    {
        for (int i = 0; i < generatedTiles.Count; i++) {
            if (generatedTiles[i] != null) {
                generatedTiles[i].SetActive(true);
            }
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < generatedTiles.Count; i++) {
            if (generatedTiles[i] != null) {
                Destroy(generatedTiles[i]);
            }
        }

        if (ownsForegroundWheatfield && foregroundWheatfield != null) {
            Destroy(foregroundWheatfield);
            foregroundWheatfield = null;
        }
    }

    private void Update()
    {
        if (spriteRenderer != null) {
            float movement = animationSpeed * tileWidth * Time.deltaTime;
            for (int i = 0; i < tiles.Count; i++) {
                if (tiles[i] != null) {
                    tiles[i].transform.position += Vector3.left * movement;
                }
            }

            for (int i = tiles.Count - 1; i >= 0; i--) {
                if (tiles[i] == null) {
                    tiles.RemoveAt(i);
                }
            }

            if (tiles.Count == 0) {
                return;
            }

            Vector3 cameraPosition = mainCamera.ViewportToWorldPoint(
                new Vector3(0f, 0.5f, Mathf.Abs(transform.position.z - mainCamera.transform.position.z)));
            float leftEdge = cameraPosition.x;

            while (tiles.Count > 0 && tiles[0].bounds.max.x < leftEdge) {
                SpriteRenderer recycledTile = tiles[0];
                SpriteRenderer rightmostTile = tiles[tiles.Count - 1];
                tiles.RemoveAt(0);
                PlaceLeftEdgeAt(recycledTile, rightmostTile.bounds.max.x);
                if (mirrorAlternateTiles) {
                    recycledTile.flipX = !rightmostTile.flipX;
                }
                tiles.Add(recycledTile);
            }
        } else {
            Vector2 offset = meshRenderer.material.mainTextureOffset;
            offset.x = Mathf.Repeat(offset.x + animationSpeed * Time.deltaTime, 1f);
            meshRenderer.material.mainTextureOffset = offset;
        }
    }

    private SpriteRenderer CreateTile(int offset, bool flipX)
    {
        GameObject tile = new GameObject($"{gameObject.name} Loop Tile");
        tile.layer = gameObject.layer;
        tile.transform.SetParent(transform.parent, true);
        tile.transform.position = transform.position + Vector3.right * (offset * tileWidth);
        tile.transform.rotation = transform.rotation;
        tile.transform.localScale = transform.localScale;

        SpriteRenderer tileRenderer = tile.AddComponent<SpriteRenderer>();
        tileRenderer.sprite = spriteRenderer.sprite;
        tileRenderer.color = spriteRenderer.color;
        tileRenderer.flipX = mirrorAlternateTiles ? (flipX != spriteRenderer.flipX) : spriteRenderer.flipX;
        tileRenderer.flipY = spriteRenderer.flipY;
        tileRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
        tileRenderer.sortingOrder = spriteRenderer.sortingOrder;
        tileRenderer.maskInteraction = spriteRenderer.maskInteraction;
        tileRenderer.sharedMaterial = spriteRenderer.sharedMaterial;
        generatedTiles.Add(tile);
        return tileRenderer;
    }

    private void CreateForegroundWheatfield()
    {
        if (foregroundWheatfield != null) {
            return;
        }

        ownsForegroundWheatfield = true;
        foregroundWheatfield = new GameObject("Foreground Wheatfield");
        foregroundWheatfield.layer = gameObject.layer;
        foregroundWheatfield.transform.SetParent(transform.parent, true);
        foregroundWheatfield.transform.position = transform.position + Vector3.up * foregroundYOffset;
        foregroundWheatfield.transform.rotation = transform.rotation;
        foregroundWheatfield.transform.localScale = transform.localScale;

        SpriteRenderer foregroundRenderer = foregroundWheatfield.AddComponent<SpriteRenderer>();
        foregroundRenderer.sprite = spriteRenderer.sprite;
        foregroundRenderer.color = spriteRenderer.color;
        foregroundRenderer.flipX = spriteRenderer.flipX;
        foregroundRenderer.flipY = spriteRenderer.flipY;
        foregroundRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
        foregroundRenderer.sortingOrder = foregroundWheatfieldSortingOrder;
        foregroundRenderer.maskInteraction = spriteRenderer.maskInteraction;
        foregroundRenderer.sharedMaterial = spriteRenderer.sharedMaterial;

        Parallax foregroundParallax = foregroundWheatfield.AddComponent<Parallax>();
        foregroundParallax.animationSpeed = animationSpeed;
        foregroundParallax.mirrorAlternateTiles = mirrorAlternateTiles;
        foregroundParallax.wheatfieldSortingOrder = wheatfieldSortingOrder;
        foregroundParallax.foregroundWheatfieldSortingOrder = foregroundWheatfieldSortingOrder;
        foregroundParallax.backgroundSortingOrder = backgroundSortingOrder;
    }

    private bool IsHighestWheatfieldLayer()
    {
        if (!spriteRenderer.sprite.name.Contains("wheatfield")) {
            return false;
        }

        Parallax[] parallaxLayers = FindObjectsByType<Parallax>(FindObjectsSortMode.None);
        Parallax highestLayer = this;

        for (int i = 0; i < parallaxLayers.Length; i++) {
            SpriteRenderer layerRenderer = parallaxLayers[i].GetComponent<SpriteRenderer>();
            if (layerRenderer != null && layerRenderer.sprite != null &&
                layerRenderer.sprite.name.Contains("wheatfield") &&
                layerRenderer.transform.position.y > highestLayer.transform.position.y) {
                highestLayer = parallaxLayers[i];
            }
        }

        return highestLayer == this;
    }

    private static void PlaceLeftEdgeAt(SpriteRenderer renderer, float leftEdge)
    {
        Vector3 position = renderer.transform.position;
        position.x += leftEdge - renderer.bounds.min.x;
        renderer.transform.position = position;
    }

}
