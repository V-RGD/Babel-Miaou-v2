using System.Collections.Generic;
using Generation.Components;
using UnityEngine;

namespace Generation.Level
{
    //this scripts uses grid information to build a level with bricks, colliders, rooms, etc...
    public class GenProBuilder : MonoBehaviour
    {
        public static GenProBuilder instance;

        [Header("--Values--")]
        [SerializeField] float tileSize = 10;

        [SerializeField] float wallTopOffset = 20;
        [SerializeField] float exteriorWallsOffset = -5;
        [SerializeField] float groundWallOffset = 10;

        [Header("--References--")] [SerializeField]
        Transform levelParent;

        [Header("--Tiles--")] 
        [SerializeField] GameObject groundTile;
        [SerializeField] GameObject wallBelowGroundTile;
        
        [SerializeField] GameObject topWallTile;
        [SerializeField] GameObject exteriorWallsTile;
        
        [SerializeField] GameObject bridgeTile;
        [SerializeField] GameObject bridgeWallTile;

        [Header("---Grids---")] public int[,] buildingGrid;
        public int[,] heightMap;

        void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        public void BuildLevel()
        {
            //builds level tiles
            BuildTiles();
            //add game components necessary for the game to work
            ComponentsGeneration.instance.CreateRoomComponents();
        }

        void BuildTiles()
        {
            //ground
            GenerateLayer(new List<int>{1, 2, 3, 4}, groundTile, wallBelowGroundTile, 0, groundWallOffset);
            //bridges
            GenerateLayer(new List<int>{6}, bridgeTile, bridgeWallTile, 0, groundWallOffset);
            //exterior walls
            GenerateLayer(new List<int>{5}, topWallTile, exteriorWallsTile, wallTopOffset, exteriorWallsOffset);
        }

        void GenerateLayer(List<int> indices, GameObject groundTile, GameObject wallTile, float offsetFromGround, float wallOffsetFromGround)
        {
            Vector2Int[] wallTiles = GridUtilities.GetTilesOfIndices(buildingGrid, indices);
            foreach (var tile in wallTiles)
            {
                CreateTile(groundTile, tile, new Vector3(90, 0, 0), new Vector3(0, offsetFromGround, 0)); 
            }
            GenerateWalls(wallTile, wallTiles, indices, wallOffsetFromGround);
        }

        void GenerateWalls(GameObject wallPrefab, Vector2Int[] groundTiles, List<int> filters, float offsetFromLayer)
        {
            //for each tile
            foreach (var tile in groundTiles)
            {
                List<Vector2Int> surroundingTiles = new List<Vector2Int>
                    {tile + Vector2Int.down, tile + Vector2Int.left};

                //checks if the surrounding tiles are tiles are void
                foreach (var analysedTile in surroundingTiles)
                {
                    //if the surrounding tile is void
                    int tileValue;
                    if (analysedTile.x >= buildingGrid.GetLength(0) || analysedTile.y >= buildingGrid.GetLength(1) || analysedTile.x < 0 || analysedTile.y < 0)
                    {
                        tileValue = 0;
                    }
                    else
                    {
                        tileValue = buildingGrid[analysedTile.x, analysedTile.y];
                    }
                    
                    if (filters.Contains(tileValue)) continue;
                    
                    //calculates position and orientation compared to the ground tile
                    Vector3 tilePosition = TileToWorldPos(tile) + Vector3.up * heightMap[tile.x, tile.y];
                    Vector3 voidTilePosition = TileToWorldPos(analysedTile) + Vector3.up * heightMap[tile.x, tile.y];
                    Vector3 wallDirection = (voidTilePosition - tilePosition).normalized;

                    //creates a new wall tile
                    GameObject newWall = Instantiate(wallPrefab, levelParent);
                    
                    //places the wall below the ground tile at a certain offset
                    Vector3 offsetFromGround = offsetFromLayer * Vector3.down + tileSize/2 * wallDirection;
                    Vector3 wallPosition = tilePosition + offsetFromGround;
                    
                    newWall.transform.localScale = Vector3.one * tileSize / 10;
                    newWall.transform.position = wallPosition;

                    //rotates the wall towards the void
                    if(wallDirection == Vector3.left) newWall.transform.rotation = Quaternion.Euler(0, -90, 0);
                    if(wallDirection == Vector3.back) newWall.transform.rotation = Quaternion.Euler(0, 180, 0);
                }
            }
        }

        

        public void DestroyLevelInstance()
        {
            //destroys grid and level
            foreach (Transform child in levelParent)
            {
                DestroyImmediate(child);
            }
        }

        Vector3 TileToWorldPos(Vector2Int tilePos)
        {
            return new Vector3(tilePos.x, 0, tilePos.y) * tileSize;
        }

        void CreateTile(GameObject tile, Vector2Int position, Vector3 rotation, Vector3 offset)
        {
            Vector3 pos = TileToWorldPos(position) + offset;
            GameObject newTile = Instantiate(tile, pos, Quaternion.Euler(rotation));
            newTile.transform.localScale = Vector3.one * tileSize / 8;
            newTile.transform.parent = levelParent;
        }
    }
}