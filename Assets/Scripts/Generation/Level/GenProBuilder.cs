using System;
using System.Collections.Generic;
using UnityEngine;

namespace Generation.Level
{
    //this scripts uses grid information to build a level with bricks, colliders, rooms, etc...
    public class GenProBuilder : MonoBehaviour
    {
        public static GenProBuilder instance;

        [Header("--Values--")] 
        [SerializeField] private float tileSize;

        [Header("--References--")] 
        [SerializeField] private Transform levelParent;
        
        [Header("--BuildingAssets--")]
        [SerializeField] private GameObject groundTile;
        [SerializeField] private GameObject wallBelowGroundTile;
        [SerializeField] private GameObject exteriorWallsTile;

        [Header("--Tiles--")] [Header("---Grids---")]
        public int[,] buildingGrid;
        public int[,] heightMap;
        
        private void Awake()
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
            BuildGroundTiles();
            BuildBelowFloor();
            BuildExteriorWalls();
            //add game components necessary for the game to work
            AddGameComponents();
        }

        void BuildGroundTiles()
        {
            //gets ground tiles
            Vector2Int[] groundTiles = GetTilesOfIndex(1);
            foreach (var tile in groundTiles)
            {
                CreateTile(groundTile, tile);
            }
        }

        void BuildBelowFloor()
        {
            Vector2Int[] groundTiles = GetTilesOfIndex(1);
            GenerateWalls(wallBelowGroundTile, groundTiles, 1);
        }

        void BuildExteriorWalls()
        {
            Vector2Int[] wallTiles = GetTilesOfIndex(3);
            GenerateWalls(exteriorWallsTile, wallTiles, 3);
        }

        void GenerateWalls(GameObject wallPrefab, Vector2Int[] wallTiles, int filter)
        {
            //for each tile, checks if the surrounding tiles are tiles of the same type
            foreach (var tile in wallTiles)
            {
                List<Vector2Int> surroundingTiles = new List<Vector2Int>{tile + Vector2Int.down, tile + Vector2Int.left};
               
                foreach (var analysedTile in surroundingTiles)
                {
                    if (buildingGrid[analysedTile.x, analysedTile.y] == filter) continue;
                    
                    //if the tile is next to a different type, spawns a wall underneath facing void
                    Vector3 tilePosition = new Vector3(tile.x, heightMap[tile.x, tile.y], tile.y);
                    Vector3 voidTilePosition = new Vector3(analysedTile.x, heightMap[analysedTile.x, analysedTile.y], analysedTile.y);
                    Vector3 direction = voidTilePosition - tilePosition;
                    
                    //creates a new wall tile
                    GameObject newWall = Instantiate(wallPrefab);
                    
                    //place it below the tile facing void
                    newWall.transform.position = tilePosition + 0.5f * tileSize * direction.normalized;
                    newWall.transform.LookAt(newWall.transform.position + direction * 1000);
                    break;
                }
            }
        }

        public void AddGameComponents()
        {
            //add room components

            //instantiates shops

            //builds stairs

            //generates enemies

            //place player
        }

        public void DestroyLevelInstance()
        {
            //destroys grid and level
            foreach (Transform child in levelParent)
            {
                DestroyImmediate(child);
            }
        }

        Vector2Int[] GetTilesOfIndex(int index)
        {
            List<Vector2Int> tiles = new List<Vector2Int>();
            //for each tile of the grid
            for (int x = 0; x < buildingGrid.GetLength(0); x++)
            {
                for (int y = 0; y < buildingGrid.GetLength(1); y++)
                {
                    //checks if the tile matches the searched index
                    if (buildingGrid[x, y] == index)
                    {
                        tiles.Add(new Vector2Int(x, y));
                    }
                }
            }
            //returns all of the tiles that match
            return tiles.ToArray();
        }

        Vector3 TileToWorldPos(Vector2Int tilePos)
        {
            return new Vector3(tilePos.x, 0, tilePos.y) * tileSize;
        }

        void CreateTile(GameObject tile, Vector2Int position, Vector3 rotation)
        {
            Vector3 pos = TileToWorldPos(position);
            GameObject newTile = Instantiate(tile, pos, Quaternion.identity);
            newTile.transform.localScale = Vector3.one * tileSize;
            newTile.transform.parent = levelParent;
            newTile.transform.rotation = Quaternion.Euler(rotation);
        }
        void CreateTile(GameObject tile, Vector2Int position)
        {
            Vector3 pos = TileToWorldPos(position);
            GameObject newTile = Instantiate(tile, pos, Quaternion.identity);
            newTile.transform.localScale = Vector3.one * tileSize;
            newTile.transform.parent = levelParent;
        }

        void GroundTileMap()
        {
            int[,] groundGrid = buildingGrid;
            
            
            int rockIndex = 3;
            int semiSandIndex = 3;
            int sandIndex = 3;
            
            
            
            
            //then checks for the tiles 
        }
    }
}