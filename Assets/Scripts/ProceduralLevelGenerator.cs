using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralLevelGenerator : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private Tilemap decorationTilemap;
    [SerializeField] private Tilemap hazardTilemap;
    [SerializeField] private Tilemap backgroundTilemap;

    [Header("Tiles")]
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase platformTile;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private TileBase decorationTile;
    [SerializeField] private TileBase backgroundWallTile;
    [SerializeField] private TileBase hazardTile;

    [Header("Player")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerSpawnPoint;

    [Header("Infinite Room Settings")]
    [SerializeField] private int roomWidth = 20;
    [SerializeField] private int roomHeight = 14;
    [SerializeField] private int initialRooms = 3;
    [SerializeField] private int roomsAhead = 2;

    [Header("Platform Settings")]
    [SerializeField] private int platformCountPerRoom = 7;
    [SerializeField] private int minPlatformWidth = 4;
    [SerializeField] private int maxPlatformWidth = 6;
    [SerializeField] private int minPlatformHeight = 4;
    [SerializeField] private int maxPlatformHeight = 10;
    [SerializeField] private int maxPlatformStep = 2;

    [Header("Hazard Settings")]
    [SerializeField] private int hazardCountPerRoom = 2;

    [Header("Decoration Settings")]
    [SerializeField] private int decorationCountPerRoom = 2;

    [Header("Puzzle Settings")]
    [SerializeField] private PuzzleController puzzleController;
    [SerializeField] private GameObject potPrefab;
    [SerializeField] private GameObject switchPrefab;
    [SerializeField] private int requiredSwitches = 4;
    [SerializeField] private int objectiveRoomCount = 8;

    [Header("Switch Placement")]
    [Range(0f, 1f)]
    [SerializeField] private float switchInPotChance = 0.5f;

    [Header("Additional Pot Settings")]
    [SerializeField] private int additionalPotsPerRoom = 3;
    [SerializeField] private int potHazardSafeDistance = 2;

    [Header("Portal Settings")]
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private int portalRoomIndex = 9;
    [SerializeField] private float portalOffsetX = 1.5f;

    [Header("Other Objects")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject airEnemyPrefab;
    [SerializeField] private GameObject flowersPrefab;
    [SerializeField] private int enemiesPerRoom = 1;
    [SerializeField] private int airEnemiesPerRoom = 1;
    [SerializeField] private int flowersPerRoom = 1;

    [Header("Hazard Spacing")]
    [SerializeField] private int flowerHazardSafeDistance = 4;
    [SerializeField] private int enemyHazardSafeDistance = 4;

    [Header("Spawn Safety")]
    [SerializeField] private float spawnSafeRadius = 5f;

    [Header("Random Settings")]
    [SerializeField] private int randomSeed = 0;

    private int nextRoomIndex = 0;
    private bool levelGenerated = false;
    private bool portalSpawned = false;

    private Dictionary<int, Vector3Int> puzzlePositions =
        new Dictionary<int, Vector3Int>();

    private HashSet<Vector3Int> puzzleCells =
        new HashSet<Vector3Int>();

    private HashSet<Vector3Int> occupiedSpawnCells =
        new HashSet<Vector3Int>();

    private void Start()
    {
        GenerateLevel();
    }

    private void Update()
    {
        if (!levelGenerated || player == null)
            return;

        float generationPoint =
            (nextRoomIndex - roomsAhead) * roomWidth;

        if (player.position.x >= generationPoint)
        {
            GenerateNextRoom();
        }
    }

    public void GenerateLevel()
    {
        if (groundTilemap == null ||
            wallTilemap == null ||
            floorTile == null ||
            platformTile == null ||
            wallTile == null)
        {
            Debug.LogError(
                "Assign the required Tilemaps, Ground Tile, Platform Tile, and Wall Tile!"
            );
            return;
        }

        if (puzzleController == null)
        {
            Debug.LogError("PuzzleController reference is missing!");
            return;
        }

        if (potPrefab == null || switchPrefab == null)
        {
            Debug.LogError("Assign both the Pot and Switch prefabs!");
            return;
        }

        if (portalPrefab == null)
        {
            Debug.LogError("Portal prefab is missing!");
            return;
        }

        if (randomSeed == 0)
            Random.InitState(System.Environment.TickCount);
        else
            Random.InitState(randomSeed);

        groundTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();

        if (decorationTilemap != null)
            decorationTilemap.ClearAllTiles();

        if (hazardTilemap != null)
            hazardTilemap.ClearAllTiles();

        if (backgroundTilemap != null)
            backgroundTilemap.ClearAllTiles();

        nextRoomIndex = 0;
        portalSpawned = false;
        levelGenerated = false;

        puzzlePositions.Clear();
        puzzleCells.Clear();
        occupiedSpawnCells.Clear();

        puzzleController.SetRequiredSwitches(requiredSwitches);
        puzzleController.ResetPuzzle();

        GeneratePuzzlePositions();

        for (int i = 0; i < initialRooms; i++)
        {
            GenerateNextRoom();
        }

        SpawnPlayer();

        levelGenerated = true;

        Debug.Log("Initial dungeon rooms generated!");
    }

    private void GeneratePuzzlePositions()
    {
        if (objectiveRoomCount < requiredSwitches)
        {
            Debug.LogError(
                "Objective Room Count must be at least " +
                requiredSwitches + "!"
            );
            return;
        }

        List<int> availableRooms = new List<int>();

        for (int i = 1; i <= objectiveRoomCount; i++)
            availableRooms.Add(i);

        // Shuffle rooms so each switch gets a different room.
        for (int i = 0; i < availableRooms.Count; i++)
        {
            int randomIndex = Random.Range(i, availableRooms.Count);

            int temp = availableRooms[i];
            availableRooms[i] = availableRooms[randomIndex];
            availableRooms[randomIndex] = temp;
        }

        for (int switchId = 1; switchId <= requiredSwitches; switchId++)
        {
            int roomIndex = availableRooms[switchId - 1];

            int startX = roomIndex * roomWidth;
            int endX = startX + roomWidth;

            int x = Random.Range(startX + 3, endX - 3);

            Vector3Int cell;

            bool spawnOnPlatform = Random.value < 0.5f;

            if (spawnOnPlatform)
            {
                int minY = Mathf.Max(4, minPlatformHeight + 1);
                int maxY = Mathf.Min(
                    roomHeight - 3,
                    maxPlatformHeight + 1
                );

                int platformSurfaceY =
                    Random.Range(minY, maxY + 1);

                cell = new Vector3Int(x, platformSurfaceY, 0);
            }
            else
            {
                cell = new Vector3Int(x, 2, 0);
            }

            puzzlePositions.Add(switchId, cell);
            puzzleCells.Add(cell);
        }

        Debug.Log(
            "Puzzle locations selected on ground and floating platforms!"
        );
    }

    private void GenerateNextRoom()
    {
        GenerateRoom(nextRoomIndex);

        Debug.Log("Generated Room " + (nextRoomIndex + 1));

        nextRoomIndex++;
    }

    private void GenerateRoom(int roomIndex)
    {
        int startX = roomIndex * roomWidth;
        int endX = startX + roomWidth;

        occupiedSpawnCells.Clear();

        // Background.
        if (backgroundTilemap != null &&
            backgroundWallTile != null)
        {
            for (int x = startX; x < endX; x++)
            {
                for (int y = 0; y <= roomHeight; y++)
                {
                    backgroundTilemap.SetTile(
                        new Vector3Int(x, y, 0),
                        backgroundWallTile
                    );
                }
            }
        }

        // Ground floor.
        for (int x = startX; x < endX; x++)
        {
            groundTilemap.SetTile(
                new Vector3Int(x, 0, 0),
                floorTile
            );

            groundTilemap.SetTile(
                new Vector3Int(x, 1, 0),
                floorTile
            );
        }

        // Ceiling.
        for (int x = startX; x < endX; x++)
        {
            wallTilemap.SetTile(
                new Vector3Int(x, roomHeight, 0),
                wallTile
            );
        }

        // Close the left side of the starting room.
        if (roomIndex == 0)
        {
            for (int y = 2; y < roomHeight; y++)
            {
                wallTilemap.SetTile(
                    new Vector3Int(startX, y, 0),
                    wallTile
                );
            }
        }

        GeneratePlatforms(startX, roomIndex);
        GeneratePuzzlePlatforms(roomIndex);
        GenerateHazards(startX, roomIndex);
        GenerateDecorations(startX);

        SpawnPuzzleObjects(roomIndex);
        SpawnAdditionalPots(startX, endX, roomIndex);
        SpawnEnemies(startX, endX, roomIndex);
        SpawnAirEnemies(startX, endX, roomIndex);
        SpawnLanterns(startX, endX, roomIndex);

        if (roomIndex == portalRoomIndex)
            SpawnPortal(roomIndex);
    }

    // Every room gets at least three guaranteed floating platforms.
    // Remaining platforms are placed randomly for variety.
    private void GeneratePlatforms(int startX, int roomIndex)
    {
        int safeMinimumX = roomIndex == 0
            ? Mathf.CeilToInt(spawnSafeRadius) + 1
            : 2;

        // Three separated lanes ensure that every room has midair platforms.
        int[] platformXPositions;

        if (roomIndex == 0)
        {
            platformXPositions = new int[]
            {
                startX + Mathf.Max(6, safeMinimumX),
                startX + 11,
                startX + 16
            };
        }
        else
        {
            platformXPositions = new int[]
            {
                startX + 2,
                startX + 8,
                startX + 14
            };
        }

        int[] platformHeights =
            {
                Random.Range(7, 9),
                Random.Range(9, 11),
                Random.Range(11, 15)
            };

        for (int i = 0; i < platformXPositions.Length; i++)
        {
            int platformX = platformXPositions[i];
            int platformY = Mathf.Clamp(
                platformHeights[i],
                3,
                roomHeight - 2
            );

            // Use four tiles for guaranteed platforms so all three fit
            // inside a 20-tile room without overlapping each other.
            CreatePlatform(platformX, platformY, 4);
        }

        // Add extra randomized platforms beyond the guaranteed three.
        int extraPlatformCount = Mathf.Max(0, platformCountPerRoom - 3);

        for (int i = 0; i < extraPlatformCount; i++)
        {
            int width = Random.Range(
                Mathf.Max(3, minPlatformWidth),
                Mathf.Max(minPlatformWidth + 1, maxPlatformWidth + 1)
            );

            int minX = startX + (roomIndex == 0
                ? Mathf.Max(6, safeMinimumX)
                : 1);
            int maxX = startX + roomWidth - width - 1;

            if (maxX < minX)
                continue;

            int x = Random.Range(minX, maxX + 1);
            int y = Random.Range(
                Mathf.Max(3, minPlatformHeight),
                Mathf.Min(roomHeight - 1, maxPlatformHeight + 1)
            );

            CreatePlatform(x, y, width);
        }

        Debug.Log(
            "Generated at least 3 floating platforms in Room " +
            (roomIndex + 1)
        );
    }

    private void GeneratePuzzlePlatforms(int roomIndex)
    {
        foreach (KeyValuePair<int, Vector3Int> entry in puzzlePositions)
        {
            Vector3Int spawnCell = entry.Value;

            if (spawnCell.x / roomWidth != roomIndex)
                continue;

            if (spawnCell.y <= 2)
                continue;

            int platformY = spawnCell.y - 1;
            int platformStartX = spawnCell.x - 2;
            int platformWidth = 5;

            CreatePlatform(
                platformStartX,
                platformY,
                platformWidth
            );

            // Prevent other objects from spawning inside this platform.
            for (int x = 0; x < platformWidth; x++)
            {
                occupiedSpawnCells.Add(
                    new Vector3Int(platformStartX + x, platformY, 0)
                );
            }
        }
    }

    private void CreatePlatform(int startX, int y, int width)
    {
        for (int x = 0; x < width; x++)
        {
            Vector3Int position =
                new Vector3Int(startX + x, y, 0);

            groundTilemap.SetTile(position, platformTile);
        }
    }

    private void GenerateHazards(int startX, int roomIndex)
    {
        if (hazardTilemap == null || hazardTile == null)
            return;

        int endX = startX + roomWidth;
        int placedHazards = 0;
        int attempts = 0;

        while (placedHazards < hazardCountPerRoom &&
               attempts < hazardCountPerRoom * 10)
        {
            attempts++;

            int hazardX = Random.Range(startX + 2, endX - 2);

            if (roomIndex == 0 &&
                hazardX < startX + spawnSafeRadius)
            {
                continue;
            }

            Vector3Int hazardPosition =
                new Vector3Int(hazardX, 2, 0);

            if (IsNearPuzzlePosition(hazardPosition, 3))
                continue;

            if (hazardTilemap.GetTile(hazardPosition) != null)
                continue;

            hazardTilemap.SetTile(hazardPosition, hazardTile);
            placedHazards++;
        }
    }

    private bool IsNearPuzzlePosition(
        Vector3Int position,
        int horizontalDistance)
    {
        foreach (Vector3Int puzzleCell in puzzleCells)
        {
            bool closeHorizontally =
                Mathf.Abs(position.x - puzzleCell.x)
                <= horizontalDistance;

            bool closeVertically =
                Mathf.Abs(position.y - puzzleCell.y) <= 1;

            if (closeHorizontally && closeVertically)
                return true;
        }

        return false;
    }

    private void GenerateDecorations(int startX)
    {
        if (decorationTilemap == null || decorationTile == null)
            return;

        int endX = startX + roomWidth;

        for (int i = 0; i < decorationCountPerRoom; i++)
        {
            int x = Random.Range(startX + 1, endX - 1);
            int y = Random.Range(3, roomHeight - 1);

            Vector3Int position = new Vector3Int(x, y, 0);

            if (decorationTilemap.GetTile(position) == null)
            {
                decorationTilemap.SetTile(
                    position,
                    decorationTile
                );
            }
        }
    }

    private void SpawnPuzzleObjects(int roomIndex)
    {
        foreach (KeyValuePair<int, Vector3Int> entry in puzzlePositions)
        {
            Vector3Int cell = entry.Value;

            if (roomIndex != cell.x / roomWidth)
                continue;

            int switchId = entry.Key;

            Vector3 spawnPosition =
                groundTilemap.GetCellCenterWorld(cell);

            // Create the switch.
            GameObject switchObject = Instantiate(
                switchPrefab,
                spawnPosition,
                Quaternion.identity
            );

            Switch spawnedSwitch =
                switchObject.GetComponentInChildren<Switch>();

            if (spawnedSwitch == null)
            {
                Debug.LogError(
                    "Switch prefab needs a Switch component!"
                );
            }
            else
            {
                spawnedSwitch.Initialize(
                    switchId,
                    puzzleController
                );
            }

            bool hideSwitchInPot =
                Random.value < switchInPotChance;

            if (hideSwitchInPot)
            {
                // Create the pot.
                GameObject potObject = Instantiate(
                    potPrefab,
                    spawnPosition,
                    Quaternion.identity
                );

                // Give the pot its hidden switch.
                Pot spawnedPot =
                    potObject.GetComponentInChildren<Pot>(true);

                if (spawnedPot != null)
                {
                    spawnedPot.SetHiddenSwitch(switchObject);

                    Debug.Log(
                        "Switch " + switchId +
                        " hidden inside a pot."
                    );
                }
                else
                {
                    Debug.LogError(
                        "Pot prefab is missing the Pot component!"
                    );
                }
            }
            else
            {
                Debug.Log(
                    "Switch " + switchId +
                    " spawned without a pot."
                );
            }

            occupiedSpawnCells.Add(cell);

            string surface = cell.y <= 2
                ? "Ground"
                : "Floating Platform";

            Debug.Log(
                "Spawned Switch " + switchId +
                " on " + surface +
                " in Room " + (roomIndex + 1)
            );
        }
    }

    private void SpawnAdditionalPots(
        int startX,
        int endX,
        int roomIndex)
    {
        if (potPrefab == null || additionalPotsPerRoom <= 0)
            return;

        int spawnedCount = 0;

        for (int i = 0; i < additionalPotsPerRoom; i++)
        {
            Vector3Int cell;

            if (!TryGetRandomPotSpawnCell(
                startX,
                endX,
                roomIndex,
                out cell))
            {
                continue;
            }

            Vector3 position =
                groundTilemap.GetCellCenterWorld(cell);

            Instantiate(
                potPrefab,
                position,
                Quaternion.identity
            );

            occupiedSpawnCells.Add(cell);
            spawnedCount++;
        }

        Debug.Log(
            "Spawned " + spawnedCount +
            " additional pots in Room " + (roomIndex + 1)
        );
    }

    private bool TryGetRandomPotSpawnCell(
        int startX,
        int endX,
        int roomIndex,
        out Vector3Int result)
    {
        result = Vector3Int.zero;

        for (int attempt = 0; attempt < 50; attempt++)
        {
            bool spawnOnPlatform = Random.value < 0.5f;

            Vector3Int cell;

            if (spawnOnPlatform)
            {
                int x = Random.Range(startX + 2, endX - 2);
                int y = Random.Range(3, roomHeight - 1);

                Vector3Int platformCell =
                    new Vector3Int(x, y, 0);

                if (groundTilemap.GetTile(platformCell) != platformTile)
                    continue;

                // Object sits above the platform tile.
                cell = platformCell + Vector3Int.up;
            }
            else
            {
                int x = Random.Range(startX + 2, endX - 2);

                // Ground object position.
                cell = new Vector3Int(x, 2, 0);
            }

            if (roomIndex == 0 &&
                cell.x < startX + spawnSafeRadius)
            {
                continue;
            }

            if (puzzleCells.Contains(cell))
                continue;

            if (occupiedSpawnCells.Contains(cell))
                continue;

            if (hazardTilemap != null &&
                hazardTilemap.GetTile(cell) != null)
            {
                continue;
            }

            if (IsTooCloseToHazard(cell, potHazardSafeDistance))
                continue;

            result = cell;
            return true;
        }

        return false;
    }

    private void SpawnEnemies(
        int startX,
        int endX,
        int roomIndex)
    {
        if (enemyPrefab == null)
            return;

        for (int i = 0; i < enemiesPerRoom; i++)
        {
            Vector3Int cell;

            if (!TryGetRandomSpawnCell(
                startX,
                endX,
                roomIndex,
                enemyHazardSafeDistance,
                out cell))
            {
                continue;
            }

            float groundTopY =
                groundTilemap.CellToWorld(
                    new Vector3Int(cell.x, 2, 0)
                ).y;

            Vector3 position =
                groundTilemap.GetCellCenterWorld(cell);

            position.y = groundTopY;

            GameObject spawnedEnemy = Instantiate(
                enemyPrefab,
                position,
                Quaternion.identity
            );

            AdjustEnemyToSurface(spawnedEnemy, groundTopY);

            occupiedSpawnCells.Add(cell);
        }
    }

    private void SpawnAirEnemies(
        int startX,
        int endX,
        int roomIndex)
    {
        if (airEnemyPrefab == null || airEnemiesPerRoom <= 0)
            return;

        List<Vector3Int> platformCells =
            new List<Vector3Int>();

        for (int x = startX + 2; x < endX - 2; x++)
        {
            for (int y = 3; y < roomHeight - 1; y++)
            {
                Vector3Int platformCell =
                    new Vector3Int(x, y, 0);

                if (groundTilemap.GetTile(platformCell) != platformTile)
                    continue;

                Vector3Int spawnCell =
                    platformCell + Vector3Int.up;

                if (puzzleCells.Contains(spawnCell))
                    continue;

                if (occupiedSpawnCells.Contains(spawnCell))
                    continue;

                if (roomIndex == 0 &&
                    x < startX + spawnSafeRadius)
                {
                    continue;
                }

                if (IsTooCloseToHazard(
                    spawnCell,
                    enemyHazardSafeDistance))
                {
                    continue;
                }

                platformCells.Add(platformCell);
            }
        }

        for (int i = 0; i < airEnemiesPerRoom; i++)
        {
            if (platformCells.Count == 0)
                break;

            int randomIndex =
                Random.Range(0, platformCells.Count);

            Vector3Int platformCell = platformCells[randomIndex];

            platformCells.RemoveAt(randomIndex);

            Vector3 position =
                groundTilemap.GetCellCenterWorld(platformCell);

            float platformTopY =
                groundTilemap.CellToWorld(platformCell).y +
                groundTilemap.cellSize.y;

            GameObject spawnedEnemy = Instantiate(
                airEnemyPrefab,
                position,
                Quaternion.identity
            );

            AdjustEnemyToSurface(spawnedEnemy, platformTopY);

            occupiedSpawnCells.Add(
                platformCell + Vector3Int.up
            );

            Debug.Log(
                "Spawned air enemy on floating platform in Room " +
                (roomIndex + 1)
            );
        }
    }

    private void AdjustEnemyToSurface(
        GameObject spawnedEnemy,
        float surfaceY)
    {
        Physics2D.SyncTransforms();

        Collider2D enemyCollider =
            spawnedEnemy.GetComponentInChildren<Collider2D>();

        if (enemyCollider != null)
        {
            float adjustment =
                surfaceY - enemyCollider.bounds.min.y;

            spawnedEnemy.transform.position +=
                Vector3.up * adjustment;

            Physics2D.SyncTransforms();
        }
    }

    private void SpawnLanterns(
        int startX,
        int endX,
        int roomIndex)
    {
        if (flowersPrefab == null)
            return;

        for (int i = 0; i < flowersPerRoom; i++)
        {
            Vector3Int cell;

            if (!TryGetRandomSpawnCell(
                startX,
                endX,
                roomIndex,
                Mathf.Max(flowerHazardSafeDistance, 5),
                out cell))
            {
                continue;
            }

            Vector3 position =
                groundTilemap.GetCellCenterWorld(cell);

            Instantiate(
                flowersPrefab,
                position,
                Quaternion.identity
            );

            occupiedSpawnCells.Add(cell);
        }
    }

    private bool TryGetRandomSpawnCell(
        int startX,
        int endX,
        int roomIndex,
        int minHazardDistance,
        out Vector3Int result)
    {
        result = Vector3Int.zero;

        for (int attempt = 0; attempt < 50; attempt++)
        {
            int x = Random.Range(startX + 3, endX - 3);

            Vector3Int cell = new Vector3Int(x, 2, 0);

            if (roomIndex == 0 &&
                x < startX + spawnSafeRadius)
            {
                continue;
            }

            if (puzzleCells.Contains(cell))
                continue;

            if (occupiedSpawnCells.Contains(cell))
                continue;

            if (hazardTilemap != null &&
                hazardTilemap.GetTile(cell) != null)
            {
                continue;
            }

            if (IsTooCloseToHazard(cell, minHazardDistance))
                continue;

            result = cell;
            return true;
        }

        Debug.LogWarning(
            "Could not find a free spawn position in Room " +
            (roomIndex + 1)
        );

        return false;
    }

    private bool IsTooCloseToHazard(
        Vector3Int spawnCell,
        int minimumDistance)
    {
        if (hazardTilemap == null || minimumDistance <= 0)
            return false;

        for (int x = -minimumDistance; x <= minimumDistance; x++)
        {
            for (int y = -minimumDistance; y <= minimumDistance; y++)
            {
                Vector3Int checkCell =
                    spawnCell + new Vector3Int(x, y, 0);

                if (hazardTilemap.GetTile(checkCell) != null)
                    return true;
            }
        }

        return false;
    }

    private void SpawnPortal(int roomIndex)
    {
        if (portalSpawned || portalPrefab == null)
            return;

        int startX = roomIndex * roomWidth;

        Vector3Int portalCell =
            new Vector3Int(startX + roomWidth - 4, 2, 0);

        Vector3 portalPosition =
            groundTilemap.GetCellCenterWorld(portalCell) +
            Vector3.right * portalOffsetX;

        GameObject portalObject = Instantiate(
            portalPrefab,
            portalPosition,
            Quaternion.identity
        );

        portalObject.SetActive(false);

        puzzleController.RegisterPortal(portalObject);

        portalSpawned = true;

        Debug.Log(
            "Portal generated in Room " + (roomIndex + 1)
        );
    }

    private void SpawnPlayer()
    {
        if (player == null)
        {
            Debug.LogWarning("Player reference is missing!");
            return;
        }

        Vector3 spawnPosition = playerSpawnPoint != null
            ? playerSpawnPoint.position
            : new Vector3(2f, 2.5f, 0f);

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.position = new Vector2(
                spawnPosition.x,
                spawnPosition.y
            );

            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        else
        {
            player.position = spawnPosition;
        }

        Physics2D.SyncTransforms();
    }
}