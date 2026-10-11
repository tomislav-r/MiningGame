using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MineGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField] private int width = 30;
    [SerializeField] private int height = 20;

    [Header("Generation Settings")]
    [Range(0, 100)]
    [SerializeField] private int rockChance = 55;

    [SerializeField] private int smoothIterations = 5;

    [Header("Cave Size Settings")]
    [Min(1)]
    [SerializeField] private int passageRadius = 2;

    [Min(1)]
    [SerializeField] private int spawnClearRadius = 3;

    [Min(0)]
    [SerializeField] private int caveExpansionPasses = 1;


    [Header("Region Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float stone1RegionEnd = 0.35f;

    [Range(0f, 1f)]
    [SerializeField] private float stone2RegionEnd = 0.65f;

    [SerializeField] private float regionNoiseScale = 0.12f;

    [Range(0f, 0.5f)]
    [SerializeField] private float regionNoiseStrength = 0.15f;


    [Header("Natural Obsidian Border")]
    [Min(1)]
    [SerializeField] private int solidObsidianThickness = 2;

    [Min(1)]
    [SerializeField] private int transitionBorderThickness = 4;

    [SerializeField] private float borderNoiseScale = 0.15f;
    [SerializeField] private float borderNoiseStrength = 1.2f;

    [Range(0f, 1f)]
    [SerializeField] private float nearEdgeObsidianChance = 0.85f;

    [Range(0f, 1f)]
    [SerializeField] private float innerBorderObsidianChance = 0.20f;

    [Range(0f, 1f)]
    [SerializeField] private float nearEdgeFillChance = 0.80f;

    [Range(0f, 1f)]
    [SerializeField] private float innerBorderFillChance = 0.25f;


    [Header("Ore Generation")]
    [SerializeField] private int coalVeins = 8;
    [SerializeField] private int coalVeinSize = 6;

    [SerializeField] private int copperVeins = 6;
    [SerializeField] private int copperVeinSize = 5;

    [SerializeField] private int ironVeins = 6;
    [SerializeField] private int ironVeinSize = 5;

    [SerializeField] private int goldVeins = 4;
    [SerializeField] private int goldVeinSize = 4;

    [Header("Tilemap")]
    [SerializeField] private Tilemap mineTilemap;

    [Header("Floor Sprites")]
    [SerializeField] private Sprite floorCenterSprite;
    [SerializeField] private Sprite floorTopSprite;
    [SerializeField] private Sprite floorBottomSprite;
    [SerializeField] private Sprite floorLeftSprite;
    [SerializeField] private Sprite floorRightSprite;

    [SerializeField] private Sprite floorTopLeftSprite;
    [SerializeField] private Sprite floorTopRightSprite;
    [SerializeField] private Sprite floorBottomLeftSprite;
    [SerializeField] private Sprite floorBottomRightSprite;

    [Header("Stone Sprites")]
    [SerializeField] private Sprite stone1Sprite;
    [SerializeField] private Sprite stone2Sprite;
    [SerializeField] private Sprite stone3Sprite;

    [Header("Ore Sprites")]
    [SerializeField] private Sprite coalSprite;
    [SerializeField] private Sprite copperSprite;
    [SerializeField] private Sprite ironSprite;
    [SerializeField] private Sprite goldSprite;
    [SerializeField] private Sprite obsidianSprite;

    [Header("Extraction Zone")]
    [SerializeField] private Transform extractionZone;

    [Header("Rock Health")]

    [SerializeField] private int stone1Health = 2;
    [SerializeField] private int coalHealth = 2;
    [SerializeField] private int copperHealth = 3;

    [SerializeField] private int stone2Health = 4;
    [SerializeField] private int ironHealth = 5;

    [SerializeField] private int stone3Health = 6;
    [SerializeField] private int goldHealth = 8;

    [Header("Floor")]
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Color floorColor = new Color(0.25f, 0.25f, 0.25f, 1f);

    private Tile floorTile;


    [Header("Player")]
    [SerializeField] private Transform player;

    [SerializeField] private PlayerInventory playerInventory;

    [Header("Enemy")]
    [SerializeField] private EnemySpawner enemySpawner;


    [Header("Placeholder Colors")]
    [SerializeField]
    private Color stone1Color =
        new Color(0.65f, 0.65f, 0.65f);

    [SerializeField]
    private Color stone2Color =
        new Color(0.45f, 0.45f, 0.45f);

    [SerializeField]
    private Color stone3Color =
        new Color(0.28f, 0.28f, 0.28f);

    [SerializeField]
    private Color coalColor =
        Color.black;

    [SerializeField]
    private Color copperColor =
        new Color(0.72f, 0.35f, 0.15f);

    [SerializeField]
    private Color ironColor =
        new Color(0.55f, 0.6f, 0.65f);

    [SerializeField]
    private Color goldColor =
        new Color(1f, 0.75f, 0.1f);

    [SerializeField]
    private Color obsidianColor =
        new Color(0.08f, 0.05f, 0.12f);


    private Tile stone1Tile;
    private Tile stone2Tile;
    private Tile stone3Tile;

    private Tile coalTile;
    private Tile copperTile;
    private Tile ironTile;
    private Tile goldTile;
    private Tile obsidianTile;


    //Ground tiles variables
    private Tile floorCenterTile;

    private Tile floorTopTile;
    private Tile floorBottomTile;
    private Tile floorLeftTile;
    private Tile floorRightTile;

    private Tile floorTopLeftTile;
    private Tile floorTopRightTile;
    private Tile floorBottomLeftTile;
    private Tile floorBottomRightTile;

    private TileType[,] map;
    private int[,] regionMap;
    private Dictionary<Vector2Int, int> damagedRockHealth = new Dictionary<Vector2Int, int>();
    private List<Vector2Int> mainCaveRegion;
    private Vector2Int playerSpawnCell;


    private void Start()
    {
        CreateTiles();
        CreateFloorTiles(); 
        GenerateMap();
    }


    // TILE CREATION

    private void CreateTiles()
    {
        Texture2D texture = new Texture2D(1, 1);

        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, 1, 1),
            new Vector2(0.5f, 0.5f),
            1
        );

        stone1Tile = CreateColoredTile(stone1Sprite, Color.white);

        stone2Tile = CreateColoredTile(stone2Sprite, Color.white);

        stone3Tile = CreateColoredTile(stone3Sprite, Color.white);

        coalTile = CreateColoredTile(coalSprite, Color.white);

        copperTile = CreateColoredTile(copperSprite, Color.white);

        ironTile = CreateColoredTile(ironSprite, Color.white);

        goldTile = CreateColoredTile(goldSprite, Color.white);

        obsidianTile = CreateColoredTile(obsidianSprite, new Color(1.15f, 1.15f, 1.15f, 1f));
    }


    private Tile CreateColoredTile(Sprite sprite, Color color)
    {
        Tile tile = ScriptableObject.CreateInstance<Tile>();

        tile.sprite = sprite;
        tile.color = color;
        tile.colliderType = Tile.ColliderType.Grid;
        tile.flags = TileFlags.None;

        return tile;
    }


    // MAP GENERATION

    private void GenerateMap()
    {
        damagedRockHealth.Clear();

        CreateRandomMap();

        for (int i = 0; i < smoothIterations; i++)
        {
            SmoothMap();
        }

        ConnectCaves();

        ExpandCavesForPlayer();

        PreparePlayerSpawn();

        AssignStoneRegions();

        GenerateOreVeins();

        ApplyNaturalObsidianBorder();

        FillFloorTilemap();

        DrawMap();

        SpawnPlayer();

        if (enemySpawner != null)
        {
            enemySpawner.SpawnEnemies();
        }
    }


    private void CreateRandomMap()
    {
        map = new TileType[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (IsBorder(x, y))
                {
                    map[x, y] =
                        TileType.Obsidian;

                    continue;
                }

                int randomValue =
                    Random.Range(0, 100);

                if (randomValue < rockChance)
                {
                    map[x, y] =
                        TileType.Stone1;
                }
                else
                {
                    map[x, y] =
                        TileType.Empty;
                }
            }
        }
    }


    private void SmoothMap()
    {
        TileType[,] newMap =
            new TileType[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (IsBorder(x, y))
                {
                    newMap[x, y] =
                        TileType.Obsidian;

                    continue;
                }

                int neighbourRockCount =
                    GetNeighbourRockCount(x, y);

                if (neighbourRockCount > 4)
                {
                    newMap[x, y] =
                        TileType.Stone1;
                }
                else if (neighbourRockCount < 4)
                {
                    newMap[x, y] =
                        TileType.Empty;
                }
                else
                {
                    newMap[x, y] =
                        map[x, y];
                }
            }
        }

        map = newMap;
    }


    private int GetNeighbourRockCount(
        int gridX,
        int gridY)
    {
        int rockCount = 0;

        for (
            int x = gridX - 1;
            x <= gridX + 1;
            x++
        )
        {
            for (
                int y = gridY - 1;
                y <= gridY + 1;
                y++
            )
            {
                if (
                    x < 0 ||
                    x >= width ||
                    y < 0 ||
                    y >= height
                )
                {
                    rockCount++;
                    continue;
                }

                if (
                    x == gridX &&
                    y == gridY
                )
                {
                    continue;
                }

                if (IsSolid(map[x, y]))
                {
                    rockCount++;
                }
            }
        }

        return rockCount;
    }

    public void RegenerateMap()
    {
        GenerateMap();
    }

    // CAVE CONNECTION

    private void ConnectCaves()
    {
        List<List<Vector2Int>> caveRegions =
            GetRegions(TileType.Empty);

        if (caveRegions.Count == 0)
        {
            Vector2Int center =
                new Vector2Int(
                    width / 2,
                    height / 2
                );

            DrawCircle(center, 3);

            mainCaveRegion =
                GetRegionTiles(
                    center.x,
                    center.y,
                    new bool[width, height],
                    TileType.Empty
                );

            return;
        }

        caveRegions.Sort(
            (a, b) =>
                b.Count.CompareTo(a.Count)
        );

        mainCaveRegion =
            caveRegions[0];

        List<Vector2Int> connectedCaveTiles =
            new List<Vector2Int>(
                mainCaveRegion
            );

        for (
            int i = 1;
            i < caveRegions.Count;
            i++
        )
        {
            List<Vector2Int> currentRegion =
                caveRegions[i];

            List<Vector2Int> currentEdges =
                GetEdgeTiles(currentRegion);

            List<Vector2Int> connectedEdges =
                GetEdgeTiles(
                    connectedCaveTiles
                );

            Vector2Int bestTileA =
                Vector2Int.zero;

            Vector2Int bestTileB =
                Vector2Int.zero;

            float bestDistance =
                float.MaxValue;

            foreach (
                Vector2Int tileA
                in currentEdges
            )
            {
                foreach (
                    Vector2Int tileB
                    in connectedEdges
                )
                {
                    float distance =
                        (tileA - tileB)
                        .sqrMagnitude;

                    if (
                        distance <
                        bestDistance
                    )
                    {
                        bestDistance =
                            distance;

                        bestTileA =
                            tileA;

                        bestTileB =
                            tileB;
                    }
                }
            }

            CreatePassage(
                bestTileA,
                bestTileB
            );

            connectedCaveTiles.AddRange(
                currentRegion
            );
        }
    }


    private List<List<Vector2Int>> GetRegions(
        TileType tileType)
    {
        List<List<Vector2Int>> regions =
            new List<List<Vector2Int>>();

        bool[,] checkedTiles =
            new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (checkedTiles[x, y])
                {
                    continue;
                }

                if (map[x, y] != tileType)
                {
                    continue;
                }

                List<Vector2Int> region =
                    GetRegionTiles(
                        x,
                        y,
                        checkedTiles,
                        tileType
                    );

                regions.Add(region);
            }
        }

        return regions;
    }


    private List<Vector2Int> GetRegionTiles(
        int startX,
        int startY,
        bool[,] checkedTiles,
        TileType tileType)
    {
        List<Vector2Int> tiles =
            new List<Vector2Int>();

        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        Vector2Int startTile = new Vector2Int(startX, startY);

        queue.Enqueue(startTile);

        checkedTiles[startX, startY] = true;

        while (queue.Count > 0)
        {
            Vector2Int tile = queue.Dequeue();
            tiles.Add(tile);

            CheckNeighbour(tile.x + 1, tile.y, queue, checkedTiles, tileType
            );

            CheckNeighbour(tile.x - 1, tile.y, queue, checkedTiles, tileType
            );

            CheckNeighbour(tile.x, tile.y + 1, queue, checkedTiles, tileType
            );

            CheckNeighbour(tile.x, tile.y - 1, queue, checkedTiles, tileType
            );
        }

        return tiles;
    }


    private void CheckNeighbour(
        int x,
        int y,
        Queue<Vector2Int> queue,
        bool[,] checkedTiles,
        TileType tileType)
    {
        if (
            x < 0 ||
            x >= width ||
            y < 0 ||
            y >= height
        )
        {
            return;
        }

        if (checkedTiles[x, y])
        {
            return;
        }

        if (map[x, y] != tileType)
        {
            return;
        }

        checkedTiles[x, y] = true;

        queue.Enqueue(
            new Vector2Int(x, y)
        );
    }


    private List<Vector2Int> GetEdgeTiles(
        List<Vector2Int> region)
    {
        List<Vector2Int> edgeTiles =
            new List<Vector2Int>();

        foreach (Vector2Int tile in region)
        {
            bool isEdge =
                IsRock(tile.x + 1, tile.y) ||
                IsRock(tile.x - 1, tile.y) ||
                IsRock(tile.x, tile.y + 1) ||
                IsRock(tile.x, tile.y - 1);

            if (isEdge)
            {
                edgeTiles.Add(tile);
            }
        }

        return edgeTiles;
    }


    private bool IsRock(int x, int y)
    {
        if (
            x < 0 ||
            x >= width ||
            y < 0 ||
            y >= height
        )
        {
            return true;
        }

        return IsSolid(map[x, y]);
    }


    private void CreatePassage(
        Vector2Int start,
        Vector2Int end)
    {
        List<Vector2Int> line =
            GetLine(start, end);

        foreach (Vector2Int tile in line)
        {
            DrawCircle(
                tile,
                passageRadius
            );
        }
    }


    private List<Vector2Int> GetLine(
        Vector2Int start,
        Vector2Int end)
    {
        List<Vector2Int> line =
            new List<Vector2Int>();

        int deltaX =
            end.x - start.x;

        int deltaY =
            end.y - start.y;

        int steps =
            Mathf.Max(
                Mathf.Abs(deltaX),
                Mathf.Abs(deltaY)
            );

        if (steps == 0)
        {
            line.Add(start);
            return line;
        }

        for (
            int i = 0;
            i <= steps;
            i++
        )
        {
            float t =
                i / (float)steps;

            int x =
                Mathf.RoundToInt(
                    Mathf.Lerp(
                        start.x,
                        end.x,
                        t
                    )
                );

            int y =
                Mathf.RoundToInt(
                    Mathf.Lerp(
                        start.y,
                        end.y,
                        t
                    )
                );

            line.Add(
                new Vector2Int(x, y)
            );
        }

        return line;
    }


    private void DrawCircle(
        Vector2Int position,
        int radius)
    {
        for (
            int x = -radius;
            x <= radius;
            x++
        )
        {
            for (
                int y = -radius;
                y <= radius;
                y++
            )
            {
                if (
                    x * x + y * y >
                    radius * radius
                )
                {
                    continue;
                }

                int drawX =
                    position.x + x;

                int drawY =
                    position.y + y;

                if (
                    drawX <= 0 ||
                    drawX >= width - 1 ||
                    drawY <= 0 ||
                    drawY >= height - 1
                )
                {
                    continue;
                }

                map[drawX, drawY] =
                    TileType.Empty;
            }
        }
    }


    private void ExpandCavesForPlayer()
    {
        for (
            int pass = 0;
            pass < caveExpansionPasses;
            pass++
        )
        {
            TileType[,] originalMap =
                (TileType[,])map.Clone();

            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    if (
                        originalMap[x, y] !=
                        TileType.Empty
                    )
                    {
                        continue;
                    }

                    ClearCellIfValid(x + 1, y);
                    ClearCellIfValid(x - 1, y);
                    ClearCellIfValid(x, y + 1);
                    ClearCellIfValid(x, y - 1);
                }
            }
        }
    }


    private void ClearCellIfValid(
        int x,
        int y)
    {
        if (
            x <= 0 ||
            x >= width - 1 ||
            y <= 0 ||
            y >= height - 1
        )
        {
            return;
        }

        map[x, y] =
            TileType.Empty;
    }


    // PLAYER SPAWN

    private void PreparePlayerSpawn()
    {
        if (
            mainCaveRegion == null ||
            mainCaveRegion.Count == 0
        )
        {
            Debug.LogWarning(
                "Nije pronađena glavna pećina."
            );

            return;
        }

        Vector2 mapCenter =
            new Vector2(
                width / 2f,
                height / 2f
            );

        float bestDistance =
            float.MaxValue;

        foreach (
            Vector2Int cell
            in mainCaveRegion
        )
        {
            float distance = Vector2.SqrMagnitude((Vector2)cell - mapCenter);

            if (distance < bestDistance)
            {
                bestDistance = distance;

                playerSpawnCell = cell;
            }
        }

        for (
            int x = -spawnClearRadius;
            x <= spawnClearRadius;
            x++
        )
        {
            for (
                int y = -spawnClearRadius;
                y <= spawnClearRadius;
                y++
            )
            {
                int clearX =
                    playerSpawnCell.x + x;

                int clearY =
                    playerSpawnCell.y + y;

                if (
                    clearX <= 0 ||
                    clearX >= width - 1 ||
                    clearY <= 0 ||
                    clearY >= height - 1
                )
                {
                    continue;
                }

                map[clearX, clearY] =
                    TileType.Empty;
            }
        }
    }


    // STONE REGIONS

    private void AssignStoneRegions()
    {
        float maxDistance =
            GetMaximumRegionDistance();

        float noiseOffsetX =
            Random.Range(0f, 10000f);

        float noiseOffsetY =
            Random.Range(0f, 10000f);

        regionMap =
            new int[width, height];

        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                Vector2Int cell =
                    new Vector2Int(x, y);

                float distance =
                    Vector2.Distance(
                        playerSpawnCell,
                        cell
                    );

                float normalizedDistance =
                    distance / maxDistance;

                float noise = Mathf.PerlinNoise((x + noiseOffsetX) * regionNoiseScale, (y + noiseOffsetY) * regionNoiseScale);

                float centeredNoise = (noise - 0.5f) * 2f;

                float distortedDistance = normalizedDistance + centeredNoise * regionNoiseStrength;

                distortedDistance =
                    Mathf.Clamp01(
                        distortedDistance
                    );

                int region;

                if (distortedDistance < stone1RegionEnd)
                {
                    region = 1;
                }
                else if (distortedDistance < stone2RegionEnd)
                {
                    region = 2;
                }
                else
                {
                    region = 3;
                }

                regionMap[x, y] = region;

                if (!IsSolid(map[x, y]))
                {
                    continue;
                }

                switch (region)
                {
                    case 1:
                        map[x, y] = TileType.Stone1;
                        break;

                    case 2:
                        map[x, y] = TileType.Stone2;
                        break;

                    case 3:
                        map[x, y] = TileType.Stone3;
                        break;
                }
            }
        }
    }


    private float GetMaximumRegionDistance()
    {
        Vector2Int bottomLeft =
            new Vector2Int(1, 1);

        Vector2Int bottomRight =
            new Vector2Int(
                width - 2,
                1
            );

        Vector2Int topLeft =
            new Vector2Int(
                1,
                height - 2
            );

        Vector2Int topRight =
            new Vector2Int(
                width - 2,
                height - 2
            );

        float maxDistance =
            Vector2.Distance(
                playerSpawnCell,
                bottomLeft
            );

        maxDistance =
            Mathf.Max(
                maxDistance,
                Vector2.Distance(
                    playerSpawnCell,
                    bottomRight
                )
            );

        maxDistance =
            Mathf.Max(
                maxDistance,
                Vector2.Distance(
                    playerSpawnCell,
                    topLeft
                )
            );

        maxDistance =
            Mathf.Max(
                maxDistance,
                Vector2.Distance(
                    playerSpawnCell,
                    topRight
                )
            );

        return maxDistance;
    }


    private int GetDistanceToNearestEdge(
        int x,
        int y)
    {
        int left = x;
        int right = width - 1 - x;
        int bottom = y;
        int top = height - 1 - y;

        return Mathf.Min(
            left,
            right,
            bottom,
            top
        );
    }


    private TileType GetRegionStoneTile(
        int x,
        int y)
    {
        int region = regionMap[x, y];

        switch (region)
        {
            case 1:
                return TileType.Stone1;

            case 2:
                return TileType.Stone2;

            case 3:
                return TileType.Stone3;

            default:
                return TileType.Stone3;
        }
    }


    private void ApplyNaturalObsidianBorder()
    {
        int safeSolidThickness =
            Mathf.Max(1, solidObsidianThickness);

        int safeTransitionThickness =
            Mathf.Max(1, transitionBorderThickness);

        int totalBorderThickness =
            safeSolidThickness +
            safeTransitionThickness;

        float noiseOffsetX =
            Random.Range(0f, 10000f);

        float noiseOffsetY =
            Random.Range(0f, 10000f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int distanceToEdge =
                    GetDistanceToNearestEdge(
                        x,
                        y
                    );

                // Always keep several completely solid layers
                // so the player can never leave the generated map.
                if (distanceToEdge < safeSolidThickness)
                {
                    map[x, y] =
                        TileType.Obsidian;

                    continue;
                }

                // Cells deeper than the transition zone are untouched.
                if (distanceToEdge >= totalBorderThickness)
                {
                    continue;
                }

                float noise =
                    Mathf.PerlinNoise(
                        (x + noiseOffsetX) *
                        borderNoiseScale,

                        (y + noiseOffsetY) *
                        borderNoiseScale
                    );

                float centeredNoise =
                    (noise - 0.5f) * 2f;

                float adjustedDistance =
                    distanceToEdge +
                    centeredNoise *
                    borderNoiseStrength;

                float transitionStart =
                    safeSolidThickness;

                float transitionEnd =
                    totalBorderThickness;

                float edgeInfluence =
                    1f - Mathf.InverseLerp(
                        transitionStart,
                        transitionEnd,
                        adjustedDistance
                    );

                edgeInfluence =
                    Mathf.Clamp01(edgeInfluence);

                float obsidianChance =
                    Mathf.Lerp(
                        innerBorderObsidianChance,
                        nearEdgeObsidianChance,
                        edgeInfluence
                    );

                float fillChance =
                    Mathf.Lerp(
                        innerBorderFillChance,
                        nearEdgeFillChance,
                        edgeInfluence
                    );

                TileType currentType =
                    map[x, y];

                // Existing rocks and ores stay mixed into the transition,
                // but some are replaced with obsidian more often near the edge.
                if (currentType != TileType.Empty)
                {
                    if (Random.value < obsidianChance)
                    {
                        map[x, y] =
                            TileType.Obsidian;
                    }

                    continue;
                }

                // Some empty cave cells near the border are filled back in.
                // This gives the edge a thicker, irregular bedrock-like shape.
                if (Random.value >= fillChance)
                {
                    continue;
                }

                if (Random.value < obsidianChance)
                {
                    map[x, y] =
                        TileType.Obsidian;
                }
                else
                {
                    map[x, y] =
                        GetRegionStoneTile(
                            x,
                            y
                        );
                }
            }
        }
    }


    // ORE GENERATION

    private void GenerateOreVeins()
    {
        GenerateVeins(TileType.Stone1, TileType.Coal, coalVeins, coalVeinSize);

        GenerateVeins(TileType.Stone1, TileType.Copper, copperVeins, copperVeinSize);

        GenerateVeins(TileType.Stone2, TileType.Iron, ironVeins, ironVeinSize);

        GenerateVeins(TileType.Stone3, TileType.Gold, goldVeins, goldVeinSize);
    }


    private void GenerateVeins(
        TileType regionType,
        TileType oreType,
        int veinCount,
        int veinSize)
    {
        List<Vector2Int> possibleCells =
            new List<Vector2Int>();

        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (
                    map[x, y] ==
                    regionType
                )
                {
                    possibleCells.Add(
                        new Vector2Int(x, y)
                    );
                }
            }
        }

        if (possibleCells.Count == 0)
        {
            return;
        }

        for (int i = 0; i < veinCount; i++)
        {
            Vector2Int start =
                possibleCells[
                    Random.Range(
                        0,
                        possibleCells.Count
                    )
                ];

            CreateOreVein(
                start,
                regionType,
                oreType,
                veinSize
            );
        }
    }


    private void CreateOreVein(
        Vector2Int start,
        TileType regionType,
        TileType oreType,
        int veinSize)
    {
        Vector2Int current = start;

        HashSet<Vector2Int> veinTiles =
            new HashSet<Vector2Int>();

        int attempts = 0;

        int maxAttempts =
            veinSize * 10;

        while (
            veinTiles.Count < veinSize &&
            attempts < maxAttempts
        )
        {
            attempts++;

            if (
                current.x <= 0 ||
                current.x >= width - 1 ||
                current.y <= 0 ||
                current.y >= height - 1
            )
            {
                break;
            }

            if (
                map[current.x, current.y] ==
                regionType
            )
            {
                map[current.x, current.y] =
                    oreType;

                veinTiles.Add(current);
            }

            List<Vector2Int> neighbours =
                GetValidOreNeighbours(
                    current,
                    regionType,
                    oreType
                );

            if (neighbours.Count == 0)
            {
                break;
            }

            current =
                neighbours[
                    Random.Range(
                        0,
                        neighbours.Count
                    )
                ];
        }
    }


    private List<Vector2Int> GetValidOreNeighbours(
        Vector2Int position,
        TileType regionType,
        TileType oreType)
    {
        List<Vector2Int> neighbours =
            new List<Vector2Int>();

        Vector2Int[] directions =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighbour =
                position + direction;

            if (
                neighbour.x <= 0 ||
                neighbour.x >= width - 1 ||
                neighbour.y <= 0 ||
                neighbour.y >= height - 1
            )
            {
                continue;
            }

            TileType type =
                map[
                    neighbour.x,
                    neighbour.y
                ];

            if (
                type == regionType ||
                type == oreType
            )
            {
                neighbours.Add(neighbour);
            }
        }

        return neighbours;
    }


    // DRAW MAP

    private void DrawMap()
    {
        mineTilemap.ClearAllTiles();

        int startX =
            -width / 2;

        int startY =
            -height / 2;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TileBase tile =
                    GetTileForType(
                        map[x, y]
                    );

                if (tile == null)
                {
                    continue;
                }

                Vector3Int tilePosition =
                    new Vector3Int(
                        startX + x,
                        startY + y,
                        0
                    );

                mineTilemap.SetTile(
                    tilePosition,
                    tile
                );

                ApplyRandomTileRotation(
                    tilePosition,
                    map[x, y]
                );
            }
        }
    }


    private TileBase GetTileForType(
        TileType type)
    {
        switch (type)
        {
            case TileType.Stone1:
                return stone1Tile;

            case TileType.Stone2:
                return stone2Tile;

            case TileType.Stone3:
                return stone3Tile;

            case TileType.Coal:
                return coalTile;

            case TileType.Copper:
                return copperTile;

            case TileType.Iron:
                return ironTile;

            case TileType.Gold:
                return goldTile;

            case TileType.Obsidian:
                return obsidianTile;

            default:
                return null;
        }
    }


    // PLAYER SPAWN

    private void SpawnPlayer()
    {
        if (player == null)
        {
            Debug.LogWarning(
                "Player nije povezan s MineGeneratorom."
            );

            return;
        }

        Vector3Int tilemapCell =
            MapToTilemapCell(
                playerSpawnCell.x,
                playerSpawnCell.y
            );

        Vector3 spawnPosition =
            mineTilemap.GetCellCenterWorld(
                tilemapCell
            );

        if (extractionZone != null)
        {
            extractionZone.position =
                spawnPosition;
        }

        Rigidbody2D rb =
            player.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity =
                Vector2.zero;

            rb.angularVelocity =
                0f;

            rb.position =
                (Vector2)spawnPosition;
        }
        else
        {
            player.position =
                spawnPosition;
        }
    }


    // MINING

    public void MineTile(Vector3 worldPosition, int miningPower)
    {
        Vector3Int tileCell =
            mineTilemap.WorldToCell(
                worldPosition
            );

        if (!TryGetMapCoordinates(
                tileCell,
                out int mapX,
                out int mapY))
        {
            return;
        }

        TileType tileType =
            map[mapX, mapY];

        if (
            tileType == TileType.Empty ||
            tileType == TileType.Obsidian
        )
        {
            return;
        }

        Vector2Int mapPosition =
            new Vector2Int(
                mapX,
                mapY
            );

        int currentHealth;

        if (damagedRockHealth.ContainsKey(mapPosition))
        {
            currentHealth =
                damagedRockHealth[mapPosition];
        }
        else
        {
            currentHealth =
                GetMaxHealth(tileType);
        }

        currentHealth -= miningPower;

        Debug.Log(
            tileType +
            " | HP: " +
            currentHealth +
            "/" +
            GetMaxHealth(tileType)
        );

        if (currentHealth > 0)
        {
            damagedRockHealth[mapPosition] =
                currentHealth;

            UpdateRockDamageVisual(
                tileCell,
                tileType,
                currentHealth
            );

            return;
        }

        damagedRockHealth.Remove(mapPosition);

        DropResource(tileType);

        map[mapX, mapY] = TileType.Empty;

        mineTilemap.SetTile(tileCell, null);

        RefreshFloorAround(mapX, mapY);

    }


    public bool HasRockAtWorldPosition(
        Vector3 worldPosition)
    {
        Vector3Int cell =
            mineTilemap.WorldToCell(
                worldPosition
            );

        return mineTilemap.HasTile(cell);
    }


    public Vector3 GetCellCenter(
        Vector3 worldPosition)
    {
        Vector3Int cell =
            mineTilemap.WorldToCell(
                worldPosition
            );

        return mineTilemap
            .GetCellCenterWorld(cell);
    }


    public Vector2 GetCellWorldSize()
    {
        Vector3 origin =
            mineTilemap.CellToWorld(
                Vector3Int.zero
            );

        Vector3 right =
            mineTilemap.CellToWorld(
                Vector3Int.right
            );

        Vector3 up =
            mineTilemap.CellToWorld(
                Vector3Int.up
            );

        return new Vector2(
            Vector3.Distance(origin, right),
            Vector3.Distance(origin, up)
        );
    }


    public bool IsRockExposedToPlayer(
        Vector3 playerWorldPosition,
        Vector3 targetWorldPosition)
    {
        Vector3Int playerCell =
            mineTilemap.WorldToCell(
                playerWorldPosition
            );

        Vector3Int targetCell =
            mineTilemap.WorldToCell(
                targetWorldPosition
            );

        if (!IsMineableTile(targetCell))
        {
            return false;
        }

        int x0 = playerCell.x;
        int y0 = playerCell.y;

        int x1 = targetCell.x;
        int y1 = targetCell.y;

        int dx =
            Mathf.Abs(x1 - x0);

        int dy =
            Mathf.Abs(y1 - y0);

        int stepX =
            x0 < x1 ? 1 : -1;

        int stepY =
            y0 < y1 ? 1 : -1;

        int error =
            dx - dy;

        while (true)
        {
            if (
                x0 != playerCell.x ||
                y0 != playerCell.y
            )
            {
                Vector3Int currentCell =
                    new Vector3Int(
                        x0,
                        y0,
                        0
                    );

                if (
                    mineTilemap.HasTile(
                        currentCell
                    )
                )
                {
                    return
                        currentCell ==
                        targetCell;
                }
            }

            if (
                x0 == x1 &&
                y0 == y1
            )
            {
                break;
            }

            int doubleError =
                2 * error;

            if (doubleError > -dy)
            {
                error -= dy;
                x0 += stepX;
            }

            if (doubleError < dx)
            {
                error += dx;
                y0 += stepY;
            }
        }

        return false;
    }


    private bool IsMineableTile(
        Vector3Int tileCell)
    {
        if (!TryGetMapCoordinates(
                tileCell,
                out int mapX,
                out int mapY))
        {
            return false;
        }

        TileType type =
            map[mapX, mapY];

        return
            type != TileType.Empty &&
            type != TileType.Obsidian;
    }


    // HELPERS

    private bool IsSolid(
        TileType type)
    {
        return type != TileType.Empty;
    }


    private bool IsBorder(
        int x,
        int y)
    {
        return
            x == 0 ||
            x == width - 1 ||
            y == 0 ||
            y == height - 1;
    }


    private Vector3Int MapToTilemapCell(
        int mapX,
        int mapY)
    {
        int startX =
            -width / 2;

        int startY =
            -height / 2;

        return new Vector3Int(
            startX + mapX,
            startY + mapY,
            0
        );
    }


    private bool TryGetMapCoordinates(
        Vector3Int tileCell,
        out int mapX,
        out int mapY)
    {
        int startX =
            -width / 2;

        int startY =
            -height / 2;

        mapX =
            tileCell.x - startX;

        mapY =
            tileCell.y - startY;

        return
            mapX >= 0 &&
            mapX < width &&
            mapY >= 0 &&
            mapY < height;
    }

    private int GetMaxHealth(TileType type)
    {
        switch (type)
        {
            case TileType.Stone1:
                return stone1Health;

            case TileType.Coal:
                return coalHealth;

            case TileType.Copper:
                return copperHealth;

            case TileType.Stone2:
                return stone2Health;

            case TileType.Iron:
                return ironHealth;

            case TileType.Stone3:
                return stone3Health;

            case TileType.Gold:
                return goldHealth;

            default:
                return 0;
        }
    }

    private void UpdateRockDamageVisual(
    Vector3Int tileCell,
    TileType tileType,
    int currentHealth)
    {
        int maxHealth =
            GetMaxHealth(tileType);

        if (maxHealth <= 0)
        {
            return;
        }

        float healthPercentage =
            currentHealth / (float)maxHealth;

        float brightness =
            Mathf.Lerp(
                0.35f,
                1f,
                healthPercentage
            );

        Color damageColor =
            new Color(
                brightness,
                brightness,
                brightness,
                1f
            );

        mineTilemap.SetColor(
            tileCell,
            damageColor
        );
    }

    private void DropResource(TileType type)
    {
        if (playerInventory == null)
        {
            Debug.LogWarning(
                "PlayerInventory nije povezan s MineGeneratorom."
            );

            return;
        }

        switch (type)
        {
            case TileType.Coal:
            case TileType.Copper:
            case TileType.Iron:
            case TileType.Gold:
                playerInventory.AddResource(type);
                break;
        }
    }

    public List<Vector2Int> GetEmptyCellsInRegion(int region)
    {
        List<Vector2Int> cells =
            new List<Vector2Int>();

        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (
                    map[x, y] == TileType.Empty &&
                    regionMap[x, y] == region
                )
                {
                    cells.Add(
                        new Vector2Int(x, y)
                    );
                }
            }
        }

        return cells;
    }

    public Vector3 GetWorldPositionFromMapCell(
    Vector2Int mapCell)
    {
        Vector3Int tilemapCell =
            MapToTilemapCell(
                mapCell.x,
                mapCell.y
            );

        return mineTilemap
            .GetCellCenterWorld(tilemapCell);
    }

    public bool TryGetMapCellFromWorldPosition(
    Vector3 worldPosition,
    out Vector2Int mapCell)
    {
        Vector3Int tileCell =
            mineTilemap.WorldToCell(worldPosition);

        if (TryGetMapCoordinates(
            tileCell,
            out int mapX,
            out int mapY))
        {
            mapCell =
                new Vector2Int(mapX, mapY);

            return true;
        }

        mapCell = Vector2Int.zero;

        return false;
    }


    public bool IsWalkableCell(Vector2Int cell)
    {
        if (
            cell.x < 0 ||
            cell.x >= width ||
            cell.y < 0 ||
            cell.y >= height
        )
        {
            return false;
        }

        return map[cell.x, cell.y] == TileType.Empty;
    }

    private void CreateFloorTiles()
    {
        floorCenterTile =
            CreateFloorTile(floorCenterSprite);

        floorTopTile =
            CreateFloorTile(floorTopSprite);

        floorBottomTile =
            CreateFloorTile(floorBottomSprite);

        floorLeftTile =
            CreateFloorTile(floorLeftSprite);

        floorRightTile =
            CreateFloorTile(floorRightSprite);

        floorTopLeftTile =
            CreateFloorTile(floorTopLeftSprite);

        floorTopRightTile =
            CreateFloorTile(floorTopRightSprite);

        floorBottomLeftTile =
            CreateFloorTile(floorBottomLeftSprite);

        floorBottomRightTile =
            CreateFloorTile(floorBottomRightSprite);
    }


    private Tile CreateFloorTile(Sprite sprite)
    {
        Tile tile =
            ScriptableObject.CreateInstance<Tile>();

        tile.sprite = sprite;
        tile.color = Color.white;
        tile.colliderType = Tile.ColliderType.None;
        tile.flags = TileFlags.None;

        return tile;
    }

    private void FillFloorTilemap()
    {
        if (floorTilemap == null)
        {
            return;
        }

        floorTilemap.ClearAllTiles();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int tilemapCell =
                    MapToTilemapCell(
                        x,
                        y
                    );

                Tile floorTile =
                    GetFloorTileForCell(
                        x,
                        y
                    );

                floorTilemap.SetTile(
                    tilemapCell,
                    floorTile
                );

                if (floorTile == floorCenterTile)
                {
                    ApplyRandomFloorRotation(
                        tilemapCell
                    );
                }
            }
        }
    }

    private Tile GetFloorTileForCell(
    int x,
    int y)
    {
        if (map[x, y] != TileType.Empty)
        {
            return floorCenterTile;
        }

        bool rockUp =
            IsSolidCell(x, y + 1);

        bool rockDown =
            IsSolidCell(x, y - 1);

        bool rockLeft =
            IsSolidCell(x - 1, y);

        bool rockRight =
            IsSolidCell(x + 1, y);


        if (rockUp && rockLeft)
        {
            return floorTopLeftTile;
        }

        if (rockUp && rockRight)
        {
            return floorTopRightTile;
        }

        if (rockDown && rockLeft)
        {
            return floorBottomLeftTile;
        }

        if (rockDown && rockRight)
        {
            return floorBottomRightTile;
        }


        // Ravni rubovi.
        if (rockUp)
        {
            return floorTopTile;
        }

        if (rockDown)
        {
            return floorBottomTile;
        }

        if (rockLeft)
        {
            return floorLeftTile;
        }

        if (rockRight)
        {
            return floorRightTile;
        }

        return floorCenterTile;
    }

    private void ApplyRandomTileRotation(
    Vector3Int tilePosition,
    TileType tileType)
    {
        if (tileType == TileType.Empty)
        {
            return;
        }

        int randomRotation =
            Random.Range(0, 4) * 90;

        Matrix4x4 transformMatrix =
            Matrix4x4.TRS(
                Vector3.zero,
                Quaternion.Euler(
                    0f,
                    0f,
                    randomRotation
                ),
                Vector3.one
            );

        mineTilemap.SetTransformMatrix(
            tilePosition,
            transformMatrix
        );
    }

    private bool IsSolidCell(
    int x,
    int y)
    {
        if (
            x < 0 ||
            x >= width ||
            y < 0 ||
            y >= height
        )
        {
            return true;
        }

        return IsSolid(
            map[x, y]
        );
    }

    private void RefreshFloorAround(
    int mapX,
    int mapY)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                int checkX =
                    mapX + x;

                int checkY =
                    mapY + y;


                if (
                    checkX < 0 ||
                    checkX >= width ||
                    checkY < 0 ||
                    checkY >= height
                )
                {
                    continue;
                }


                Vector3Int tilemapCell =
                    MapToTilemapCell(
                        checkX,
                        checkY
                    );


                Tile floorTile =
                    GetFloorTileForCell(
                        checkX,
                        checkY
                    );


                floorTilemap.SetTile(
                    tilemapCell,
                    floorTile
                );
            }
        }
    }

    private void ApplyRandomFloorRotation(
    Vector3Int tilePosition)
    {
        int randomRotation =
            Random.Range(0, 4) * 90;

        Matrix4x4 transformMatrix =
            Matrix4x4.TRS(
                Vector3.zero,
                Quaternion.Euler(
                    0f,
                    0f,
                    randomRotation
                ),
                Vector3.one
            );

        floorTilemap.SetTransformMatrix(
            tilePosition,
            transformMatrix
        );
    }
}