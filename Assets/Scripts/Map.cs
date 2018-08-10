using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.Scripts.ColliderBuilder;
using UnityEngine.AI;
using Assets.Scripts.Player;

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

    public GameObject player;
    public GameObject enemyChaser;
    public GameObject enemyShooter;
    public GameObject enemyBomber;
    public GameObject exitArea;

    public float tileScale = 0.5f;
    public int tileRotationOffset = 180;

    TileMap tileMap;

    ObjectPool enemyPool;

    BoxCollider floor;
    MeshCollider walls;

    public void Start()
    {
        walls = GetComponent<MeshCollider>();
        floor = GetComponent<BoxCollider>();
        CreateEnemyPool();
    }

    public void GenerateMap(System.Random random)
    {
        foreach(Transform child in gameObject.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        tileMap = new TileMap((chunksX * chunkWidth) + 1, (chunksY * chunkHeight) + 1, random);
        tileMap.GenerateMap();

        GenerateWorldGeometry();

        player.GetComponent<PlayerStats>().attributes.RecalculateStats();

        SpawnPlayerAndEnemies(random);
    }

    private void CreateEnemyPool()
    {
        enemyPool = new ObjectPool(
            new ObjectPoolData(enemyChaser, 60), 
            new ObjectPoolData(enemyShooter, 60), 
            new ObjectPoolData(enemyBomber, 30)
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
                c.GetComponent<MeshFilter>().mesh = combinedMesh;

                //We have to refresh the gameobject in order for the mesh collider to update.
                c.SetActive(false);
                c.SetActive(true);

            }
        }

        //Wall Mesh Collider
        var colliderPolygon = new MapColliderBuilder(edges);
        walls.sharedMesh = colliderPolygon.GenerateWallMesh(2);

        //Floor Box Collider
        floor.center = new Vector3(chunksX * chunkWidth / 2.0f, 0, chunksY * chunkHeight / 2.0f);
        floor.size = new Vector3(chunksX * chunkWidth, 0, chunksY * chunkHeight);

        //Navmesh
        var surface = GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();

    }

    private void SpawnPlayerAndEnemies(System.Random random)
    {
        foreach(GameObject enemy in enemyPool)
        {
            enemy.SetActive(false);
        }
        enemyPool.Shuffle(random);

        List<Rectangle> playAreas = tileMap.Rooms.Select((r) => r.Dimensions).ToList();

        //Create the spawn room
        int spawnRoomIndex = random.Next(0, playAreas.Count);
        Rectangle spawnRoom = playAreas[spawnRoomIndex];
        playAreas.RemoveAt(spawnRoomIndex);
        
        //Create the exit room
        int exitRoomIndex = random.Next(0, playAreas.Count);
        Rectangle exitRoom = playAreas[exitRoomIndex];
        playAreas.RemoveAt(exitRoomIndex);

        player.transform.SetPositionAndRotation(new Vector3(spawnRoom.MidCenter.X, 0.1f, spawnRoom.MidCenter.Y), Quaternion.identity);
        exitArea.transform.SetPositionAndRotation(new Vector3(exitRoom.MidCenter.X, 0.2f, exitRoom.MidCenter.Y), Quaternion.Euler(new Vector3(90, 0, 0)));

        foreach (Rectangle r in playAreas)
        {
            var points = r.GetEvenlySpacedPointsAuto(2);
            foreach (Point p in points)
            {
                if (random.Next(0, 4) == 1)
                {
                    //Take the first enemy from our shuffled pool of enemies
                    var enemy = enemyPool.Spawn(new Vector3(p.X, 0, p.Y), Quaternion.identity);

                    //Refresh the enemy's NavMeshAgent to make it attach to the current floors navmesh
                    var enemyNavMesh = enemy.GetComponent<NavMeshAgent>();
                    enemyNavMesh.enabled = false;
                    enemyNavMesh.enabled = true;
                }
            }
        }
    }
}
