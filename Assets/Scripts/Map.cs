using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    TileMap tileMap;

    void Start ()
    {
        tileMap = new TileMap((chunksX * chunkWidth) + 1, (chunksY * chunkHeight) + 1);

        tileMap.GenerateMap();
        GenerateChunkMeshes();

    }

    public void GenerateChunkMeshes()
    {
        for(int cy = 0; cy < chunksX; cy++)
        {
            for(int cx = 0; cx < chunksY; cx++)
            {
                var c = Instantiate(ChunkPrefab, new Vector3(cx * chunkWidth, 0, cy * chunkHeight), Quaternion.identity, this.transform);
                chunks.Add(c);

                for(int y = 0; y < chunkHeight + 1; y++)
                {
                    for(int x = 0; x < chunkWidth + 1; x++)
                    {

                        var xPos = x + (cx * chunkWidth);
                        var yPos = y + (cy * chunkWidth);
                        var mst = tileMap.SampleTiles(xPos, yPos);
                        switch (mst.Type)
                        {
                            case MarchingSquareTileType.Empty:
                                Instantiate(tile_0, new Vector3(xPos, 0, yPos), Quaternion.Euler(0, mst.Rotation + 180, 0), c.transform);
                                break;
                            case MarchingSquareTileType.Single:
                                Instantiate(tile_1, new Vector3(xPos, 0, yPos), Quaternion.Euler(0, mst.Rotation + 180, 0), c.transform);
                                break;
                            case MarchingSquareTileType.Double:
                                Instantiate(tile_2, new Vector3(xPos, 0, yPos), Quaternion.Euler(0, mst.Rotation + 180, 0), c.transform);
                                break;
                            case MarchingSquareTileType.Triple:
                                Instantiate(tile_3, new Vector3(xPos, 0, yPos), Quaternion.Euler(0, mst.Rotation + 180, 0), c.transform);
                                break;
                            case MarchingSquareTileType.Quad:
                                Instantiate(tile_4, new Vector3(xPos, 0, yPos), Quaternion.Euler(0, mst.Rotation + 180, 0), c.transform);
                                break;
                            default:
                                break;
                        }
                    }
                }
                //TODO: Merge chunk meshes
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
	    
	}
}
