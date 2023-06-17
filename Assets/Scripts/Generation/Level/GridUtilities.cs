using System.Collections.Generic;
using UnityEngine;

namespace Generation.Level
{
    public class GridUtilities : MonoBehaviour
    {
        public static void ApplyMask(int[,] mask, Vector2Int location, bool hasPriority)
        {
            int xLength = mask.GetLength(0);
            int yLength = mask.GetLength(1);

            //iterates through the effected area and applies the mask at each position
            for (int i = 0; i < xLength; i++)
            {
                for (int j = 0; j < yLength; j++)
                {
                    //if the tile isn't defined, leave the tile unchanged
                    if (mask[i, j] == 0) continue;

                    //if the tile is defined, replace the one on the grid by the one on the mask
                    Vector2Int tilePos = location + new Vector2Int(i, j);
                    //if this mask has no priority over other masks, it will not replace a tile that's already defined
                    if (!hasPriority && GenProBuilder.instance.buildingGrid[tilePos.x, tilePos.y] != 0) continue;

                    GenProBuilder.instance.buildingGrid[tilePos.x, tilePos.y] = mask[i, j];
                }
            }
        }

        public static Vector2Int[] GetTilesOfIndex(int[,] grid, int index)
        {
            List<Vector2Int> tiles = new List<Vector2Int>();
            //for each tile of the grid
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    //checks if the tile matches the searched index
                    if (grid[x, y] == index)
                    {
                        tiles.Add(new Vector2Int(x, y));
                    }
                }
            }

            //returns all of the tiles that match
            return tiles.ToArray();
        }
        
        public static Vector2Int[] GetTilesOfIndices(int[,] grid, List<int> indexes)
        {
            List<Vector2Int> tiles = new List<Vector2Int>();
            //for each tile of the grid
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    //checks if the tile matches the searched index
                    if (indexes.Contains(grid[x, y]))
                    {
                        tiles.Add(new Vector2Int(x, y));
                    }
                }
            }

            //returns all of the tiles that match
            return tiles.ToArray();
        }

        public static Vector2Int[] GetSurroundingTiles(Vector2Int tile)
        {
            List<Vector2Int> surroundingTiles = new List<Vector2Int>();
            surroundingTiles.Add(tile + Vector2Int.down);
            surroundingTiles.Add(tile + Vector2Int.left);
            surroundingTiles.Add(tile + Vector2Int.right);
            surroundingTiles.Add(tile + Vector2Int.up);
            return surroundingTiles.ToArray();
        }

        //scans every sprite on the list to see which are the ones that matches the requirements
        public static Sprite FindSpriteOfEntranceType(List<Sprite> list, GenProPlanner.RoomEntranceDir type)
        {
            List<Sprite> validSprites = new List<Sprite>();
            //for each sprite on the list
            foreach (var plan in list)
            {
                //converts sprite to grid
                int[,] grid = MaskConverter.MaskToGrid(plan.texture);
                //finds the entrance
                Vector2Int entryTile = GetTilesOfIndex(grid, 2)[0];
                //checks whether the entrance matches the requirements or not
                Vector2Int[] surroundingTiles = GetSurroundingTiles(entryTile);
                Vector2Int voidTile = new Vector2Int(9999, 9999);
                //for each of the surrounding tiles
                foreach (Vector2Int tile in surroundingTiles)
                {
                    //checks if the tile exists
                    int tileValue;
                    if (tile.x >= grid.GetLength(0) || tile.y >= grid.GetLength(1) || tile.x < 0 || tile.y < 0)
                    {
                        tileValue = 0;
                    }
                    else
                    {
                        tileValue = grid[tile.x, tile.y];
                    }

                    //checks if the tile index corresponds to void
                    if (tileValue != 0) continue;

                    //if it does, this tile is considered at the void tile
                    voidTile = tile;
                }
                
                //checks where the void is located compared to the entrance tile
                GenProPlanner.RoomEntranceDir spriteEntranceType;

                Vector2Int positionDiff = voidTile - entryTile;

                if (positionDiff == Vector2Int.left)
                {
                    spriteEntranceType = GenProPlanner.RoomEntranceDir.Left;
                }

                else if (positionDiff == Vector2Int.down)
                {
                    spriteEntranceType = GenProPlanner.RoomEntranceDir.Down;
                }

                else
                {
                    return null;
                }

                if (spriteEntranceType != type) continue;
                validSprites.Add(plan);
                break;
            }

            //then picks a random sprite from the list
            return validSprites[Random.Range(0, validSprites.Count)];
        }
        
        public static GenProPlanner.RoomExitDir CheckExitType(Sprite plan)
        {
            //converts sprite to grid
            int[,] grid = MaskConverter.MaskToGrid(plan.texture);
            //finds the exit
            Vector2Int exitTile = GetTilesOfIndex(grid, 3)[0];
            //checks whether the entrance matches the requirements or not
            Vector2Int[] surroundingTiles = GetSurroundingTiles(exitTile);
            Vector2Int voidTile = new Vector2Int(9999, 9999);
            //for each of the surrounding tiles
            foreach (Vector2Int tile in surroundingTiles)
            {
                //checks if the tile exists
                int tileValue;
                if (tile.x >= grid.GetLength(0) || tile.y >= grid.GetLength(1) || tile.x < 0 || tile.y < 0)
                {
                    tileValue = 0;
                }
                else
                {
                    tileValue = grid[tile.x, tile.y];
                }

                //checks if the tile index corresponds to void
                if (tileValue != 0) continue;

                //if it does, this tile is considered at the void tile
                voidTile = tile;
            }
                
            //checks where the void is located compared to the entrance tile
            GenProPlanner.RoomExitDir spriteExitType;

            Vector2Int positionDiff = voidTile - exitTile;

            if (positionDiff == Vector2Int.right)
            {
                spriteExitType = GenProPlanner.RoomExitDir.Right;
            }

            else if (positionDiff == Vector2Int.up)
            {
                spriteExitType = GenProPlanner.RoomExitDir.Up;
            }

            else
            {
                Debug.LogError("Didn't Find the exit type of the room");
                return GenProPlanner.RoomExitDir.Error;
            }

            return spriteExitType;
        }
        
        public static Vector3 TileToWorldPos(Vector2Int tilePos)
        {
            int[,] buildingGrid = GenProBuilder.instance.buildingGrid;
            int[,] heightMap = GenProBuilder.instance.heightMap;
            float tileSize = GenProBuilder.instance.tileSize;
            float heightValue;
            if (tilePos.x >= buildingGrid.GetLength(0) || tilePos.y >= buildingGrid.GetLength(1) || tilePos.x < 0 || tilePos.y < 0)
            {
                heightValue = 0;
            }
            else
            {
                heightValue = heightMap[tilePos.x, tilePos.y];
            }
            return new Vector3(tilePos.x, heightValue, tilePos.y) * tileSize;
        }
    }
}