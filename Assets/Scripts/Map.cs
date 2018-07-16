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

    public float tileScale = 0.5f;
    public int tileRotationOffset = 180;

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
                            //The "filled" tile doesnt have a collider, so we cant just create a parallell array to the geometry combines.
                            //We could further optimize this list by merging adjacent identical faces, (greedy meshing), but thats for another time.
                            colliderCombines.Add(new CombineInstance() { mesh = tile.GetComponent<MeshCollider>().sharedMesh, transform = tileTransform });
                        }
                    }
                }

                c.GetComponent<MeshFilter>().mesh = new Mesh();
                c.GetComponent<MeshFilter>().mesh.CombineMeshes(meshCombines, true);
                c.GetComponent<MeshCollider>().sharedMesh = new Mesh();
                c.GetComponent<MeshCollider>().sharedMesh.CombineMeshes(colliderCombines.ToArray());
                //We have to refresh the gameobject in order for the mesh collider to update.
                c.SetActive(false);
                c.SetActive(true);

            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
	    
	}
}
