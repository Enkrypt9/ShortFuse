using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NodeCreator : MonoBehaviour
{

    public GameObject node;
    public GameObject floor;

    public GameObject gridObj;
    private Tilemap floorTileMap;
    private GridLayout grid;

    // Start is called before the first frame update
    void Start()
    {
        floorTileMap = floor.GetComponent<Tilemap>();
        grid = gridObj.GetComponent<GridLayout>();

        TileBase[] allTiles = floorTileMap.GetTilesBlock(floorTileMap.cellBounds);
        Debug.Log(allTiles);
        Debug.Log("BOUNDSSSSSSSS: " + floorTileMap.cellBounds);

        Debug.Log("BOUNDSX: " + floorTileMap.cellBounds.x);
        Debug.Log("BOUNDSY: " + floorTileMap.cellBounds.y);

        int boundX = floorTileMap.cellBounds.x;
        int boundY = floorTileMap.cellBounds.y;

        /*
        if (boundX < 0)
        {
            boundX = -boundX;
        }
        if (boundY < 0)
        {
            boundY = -boundY;
        }
        */
        Debug.Log(allTiles.Length);
        
        for (int i = 0; i < floorTileMap.size.x; i++)
        {
            for (int j = 0; j < floorTileMap.size.y; j++)
            {
                //TileBase tile = allTiles[i + j * floorTileMap.cellBounds.x];
                TileBase tile = floorTileMap.GetTile(new Vector3Int(boundX + i, boundY + j, (int)floorTileMap.transform.position.y));
                if (tile != null)
                {
                    Vector3Int localPlace = (new Vector3Int(boundX + i, boundY + j, (int)floorTileMap.transform.position.y));
                    Vector3 place = floorTileMap.CellToWorld(localPlace);

                    Instantiate(node, new Vector3(place.x + 0.5f, place.y + 0.5f), Quaternion.identity);
                    //Debug.Log("X: " + i + " Y: " + j + "Tile: " + tile.name);
                }
                else
                {
                    //Debug.Log("X: " + i + " Y: " + j + "Tile: " + "NULL");
                }
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
