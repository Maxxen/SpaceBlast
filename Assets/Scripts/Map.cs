using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    void Start ()
    {
        random = new System.Random(1337);
        Debug.Log(chunksX * chunkWidth);
        tileMap = new TileMap((chunksX * chunkWidth) + 1, (chunksY * chunkHeight) + 1, random);

        tileMap.GenerateMap();
        GenerateChunkMeshes();
        SpawnPlayerAndEnemies();
    }

    public void GenerateChunkMeshes()
    {
        for(int cy = 0; cy < chunksX; cy++)
        {
            for(int cx = 0; cx < chunksY; cx++)
            {
                var c = Instantiate(ChunkPrefab, new Vector3(cx * chunkWidth, 0, cy * chunkHeight), Quaternion.identity, this.transform);
                chunks.Add(c);
                CombineInstance[] meshCombines = new CombineInstance[chunkWidth * chunkHeight];
                List<CombineInstance> colliderCombines = new List<CombineInstance>();

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

                        if(tile != tile_4)
                        {
                            //The "filled" tile_4 doesnt have a collider, so we cant just create a parallell array to the geometry combines.
                            //We could further optimize this list by merging adjacent identical faces, (greedy meshing), but thats for another time.
                            colliderCombines.Add(new CombineInstance() { mesh = tile.GetComponent<MeshCollider>().sharedMesh, transform = tileTransform });
                        }
                    }
                }


                var combinedMesh = new Mesh();
                combinedMesh.CombineMeshes(meshCombines, true);
                UnityEditor.MeshUtility.Optimize(combinedMesh);
                c.GetComponent<MeshFilter>().mesh = combinedMesh;
                
                c.GetComponent<MeshCollider>().sharedMesh = new Mesh();
                c.GetComponent<MeshCollider>().sharedMesh.CombineMeshes(colliderCombines.ToArray());
                //We have to refresh the gameobject in order for the mesh collider to update.
                c.SetActive(false);
                c.SetActive(true);

            }
        }
    }

    public void SampleColliders(int cx, int cy)
    {
        CombineInstance[] meshCombines = new CombineInstance[chunkWidth * chunkHeight];
        MarchingSquareTile[][] tiles = new MarchingSquareTile[chunkHeight][];
        //Initialize jagged array
        for(int i = 0; i < chunkHeight; i++)
        {
            tiles[i] = new MarchingSquareTile[chunkWidth];
        }

        for (int y = 0; y < chunkHeight; y++)
        {
            for (int x = 0; x < chunkWidth; x++)
            {
                var xPos = x + (cx * chunkWidth);
                var yPos = y + (cy * chunkWidth);
                var mst = tileMap.SampleTiles(xPos, yPos);
                tiles[y][x] = mst;
                var tileMesh = tile_4;

                switch (mst.Type)
                {
                    case MarchingSquareTileType.Empty:
                        tileMesh = tile_0;
                        break;
                    case MarchingSquareTileType.Single:
                        tileMesh = tile_1;
                        break;
                    case MarchingSquareTileType.Double:
                        tileMesh = tile_2;
                        break;
                    case MarchingSquareTileType.Triple:
                        tileMesh = tile_3;
                        break;
                    case MarchingSquareTileType.Quad:
                        tileMesh = tile_4;
                        break;
                    default:
                        break;
                }

                var tileTransform = Matrix4x4.TRS(new Vector3(x, 0, y), Quaternion.Euler(0, mst.Rotation + tileRotationOffset, 0), new Vector3(tileScale, tileScale, tileScale));
                meshCombines[x * chunkWidth + y].mesh = tileMesh.GetComponent<MeshFilter>().sharedMesh;
                meshCombines[x * chunkWidth + y].transform = tileTransform;
            }
        }

        bool[][] traversed = new bool[chunkHeight][];
        for(int i = 0; i < chunkHeight; i++)
        {
            traversed[i] = new bool[chunkWidth];
        }

        for (int y = 0; y < chunkHeight; y++)
        {
            for (int x = 0; x < chunkWidth; x++)
            {
                if (!traversed[y][x])
                {
                    var mst = tiles[y][x];
                    switch (mst.Type)
                    {
                        case MarchingSquareTileType.Empty:
                            //Try to widen collider as much as possible

                            int height = 0;
                            //This query selects an entire "row" of adjacent, matching and not-traversed tiles.
                            var row = tiles[y + height].Skip(x).TakeWhile((MarchingSquareTile t, int i) => (t.Type == mst.Type && !traversed[y + height][x + i]));
                            int width = row.Count(); 

                            while (row.Count() == width && height < chunkHeight)
                            {
                                //If the current row is just as wide as the inital row's width, mark it as traversed and increase height
                                for(int t = 0; t < x + width; t++)
                                {
                                    traversed[y + height][x + t] = true;
                                }
                                height++;
                            }

                            Rectangle collider = new Rectangle(new Point(x, y), new Point(x + width, y + height));
                            //TODO Reactangle.ToMesh()


                            break;
                        case MarchingSquareTileType.Single:
                            break;
                        case MarchingSquareTileType.Double:
                            break;
                        case MarchingSquareTileType.Triple:
                            break;
                        case MarchingSquareTileType.Quad:
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
	
    public void SpawnPlayerAndEnemies()
    {
        List<Rectangle> playAreas = tileMap.Rooms.Select((r) => r.Dimensions).ToList();

        int spawnRoomIndex = random.Next(0, playAreas.Count);
        Rectangle spawnRoom = playAreas[spawnRoomIndex];
        playAreas.RemoveAt(spawnRoomIndex);

        var player = Instantiate(player_prefab, new Vector3(spawnRoom.MidCenter.X, 0.1f, spawnRoom.MidCenter.Y), Quaternion.identity);
        Camera.main.GetComponent<CameraController>().player = player;

        foreach (Rectangle r in playAreas)
        {
            var points = r.GetEvenlySpacedPointsAuto(2);
            foreach (Point p in points)
            {
                if (random.Next(0, 4) == 1)
                {
                    var e = Instantiate(enemy_chaser, new Vector3(p.X, 0.1f, p.Y), Quaternion.identity);
                    e.GetComponent<Enemy_Chaser_Controller>().Player = player;
                }
            }
        }
    }
}
