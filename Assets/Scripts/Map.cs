using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.Scripts.ColliderBuilder;
using UnityEngine.AI;

public class Map : MonoBehaviour {

    public GameObject ChunkPrefab;

    // Use this for initialization

    List<GameObject> chunks = new List<GameObject>();

    public int chunksX = 5;
    public int chunksY = 5;
    public int chunkWidth = 8;
    public int chunkHeight = 8;

    public GameObject tile_0;
    public GameObject tile_1;
    public GameObject tile_2;
    public GameObject tile_3;
    public GameObject tile_4;

    public GameObject player_prefab;
    public GameObject enemy_chaser;
    public GameObject enemy_shooter;
    public GameObject enemy_bomber;


    public float tileScale = 0.5f;
    public int tileRotationOffset = 180;

    private System.Random random;
    private TileMap tileMap;

    private ObjectPool enemyPool;

    public void Start()
    {

    }

    public void GenerateMap(System.Random random)
    {
        this.random = random;
        tileMap = new TileMap((chunksX * chunkWidth) + 1, (chunksY * chunkHeight) + 1, random);
        tileMap.GenerateMap();

        GenerateWorldGeometry();

        if (enemyPool == null)
            CreateEnemyPool();

        enemyPool.Shuffle(random);

        SpawnPlayerAndEnemies();
    }

    private void CreateEnemyPool()
    {
        enemyPool = new ObjectPool(
            new ObjectPoolData(enemy_chaser, 60), 
            new ObjectPoolData(enemy_shooter, 60), 
            new ObjectPoolData(enemy_bomber, 30)
            );
    }

    private void GenerateWorldGeometry()
    {
        List<Edge> edges = new List<Edge>();
        for(int cy = 0; cy < chunksX; cy++)
        {
            for(int cx = 0; cx < chunksY; cx++)
            {
                var c = Instantiate(ChunkPrefab, new Vector3(cx * chunkWidth, 0, cy * chunkHeight), Quaternion.identity, this.transform);
                chunks.Add(c);
                CombineInstance[] meshCombines = new CombineInstance[chunkWidth * chunkHeight];
                //List<CombineInstance> colliderCombines = new List<CombineInstance>();
                for(int y = 0; y < chunkHeight; y++)
                {
                    for(int x = 0; x < chunkWidth; x++)
                    {

                        var xPos = x + (cx * chunkWidth);
                        var yPos = y + (cy * chunkWidth);
                        var mst = tileMap.SampleTiles(xPos, yPos);
                        var tile = tile_4;

                        switch (mst.Type)
                        {
                            case MarchingSquareTileType.Empty:
                                tile = tile_0;
                                break;
                            case MarchingSquareTileType.Single:
                                tile = tile_1;
                                break;
                            case MarchingSquareTileType.Double:
                                tile = tile_2;
                                break;
                            case MarchingSquareTileType.Triple:
                                tile = tile_3;
                                break;
                            case MarchingSquareTileType.Quad:
                                tile = tile_4;
                                break;
                            default:
                                break;
                        }
                        
                        var tileTransform = Matrix4x4.TRS(new Vector3(x, 0, y), Quaternion.Euler(0, mst.Rotation + tileRotationOffset, 0), new Vector3(tileScale, tileScale, tileScale));
                        meshCombines[x * chunkWidth + y].mesh = tile.GetComponent<MeshFilter>().sharedMesh;
                        meshCombines[x * chunkWidth + y].transform = tileTransform;

                        var edge = mst.GetEdge(new Vector3(xPos, 0, yPos));
                        if (edge != null)
                            edges.Add(edge.Value);
                    }
                }


                var combinedMesh = new Mesh();
                combinedMesh.CombineMeshes(meshCombines, true);
                UnityEditor.MeshUtility.Optimize(combinedMesh);
                c.GetComponent<MeshFilter>().mesh = combinedMesh;

                //We have to refresh the gameobject in order for the mesh collider to update.
                c.SetActive(false);
                c.SetActive(true);

            }
        }

        //Wall Mesh Collider
        var colliderPolygon = new MapColliderBuilder(edges);
        this.GetComponent<MeshCollider>().sharedMesh = colliderPolygon.GenerateWallMesh(2);

        //Floor Box Collider
        var floor = this.GetComponent<BoxCollider>();
        floor.center = new Vector3(chunksX * chunkWidth / 2.0f, 0, chunksY * chunkHeight / 2.0f);
        floor.size = new Vector3(chunksX * chunkWidth, 0, chunksY * chunkHeight);

        //Navmesh
        var watch3 = System.Diagnostics.Stopwatch.StartNew();
        var surface = GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();

        watch3.Stop();
        Debug.Log("Navmesh creation: " + watch3.ElapsedMilliseconds);
    }

    private void SpawnPlayerAndEnemies()
    {
        List<Rectangle> playAreas = tileMap.Rooms.Select((r) => r.Dimensions).ToList();

        int spawnRoomIndex = random.Next(0, playAreas.Count);
        Rectangle spawnRoom = playAreas[spawnRoomIndex];
        playAreas.RemoveAt(spawnRoomIndex);

        GameObject player = Instantiate(player_prefab, new Vector3(spawnRoom.MidCenter.X, 0.1f, spawnRoom.MidCenter.Y), Quaternion.identity);

        Camera.main.GetComponent<CameraController>().player = player;

        foreach (Rectangle r in playAreas)
        {
            var points = r.GetEvenlySpacedPointsAuto(2);
            foreach (Point p in points)
            {
                if (random.Next(0, 4) == 1)
                {
                    var enemy = enemyPool.Spawn(new Vector3(p.X, 0, p.Y), Quaternion.identity);
                    var enemyNavMesh = enemy.GetComponent<NavMeshAgent>();
                    enemyNavMesh.enabled = false;
                    enemyNavMesh.enabled = true;
                }
            }
        }
    }
}
