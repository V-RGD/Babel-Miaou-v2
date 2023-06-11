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
            for (int i = 0; i < list.Count; i++)
            {
                //converts sprite to grid
                int[,] grid = MaskConverter.MaskToGrid(list[i].texture);
                //finds the entrance
                Vector2Int entryTile = GetTilesOfIndex(grid, 2)[0];
                //checks whether the entrance matches the requirements or not
                Vector2Int[] surroundingTiles = GetSurroundingTiles(entryTile);
                //for each of the surrounding tiles
                foreach (Vector2Int tile in surroundingTiles)
                {
                    //checks if the tile exists
                    int tileValue;
                    if (tile.x >= grid.GetLength(0) || tile.y >= grid.GetLength(1) || tile.x <= 0 || tile.y <= 0)
                    {
                        tileValue = 0;
                    }
                    else
                    {
                        tileValue = grid[tile.x, tile.y];
                    }

                    //checks if the tile index corresponds to void
                    if (tileValue != 0) continue;

                    //checks where the void is located compared to the entrance tile
                    GenProPlanner.RoomEntranceDir spriteEntranceType;

                    Vector2Int positionDiff = surroundingTiles[i] - entryTile;

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
                        Debug.Log("Matching tile value : " + tileValue);
                        Debug.Log("Grid X : " + grid.GetLength(0));
                        Debug.Log("Grid Y : " + grid.GetLength(1));
                        Debug.Log("Tile match : " + surroundingTiles[i]);
                        Debug.Log("Entry Tile :" + entryTile);
                        Debug.Log("Diff : " + positionDiff);
                        Debug.Log(list[i]);
                        Debug.LogError("Couldn't compare room entrance");
                        return null;
                    }

                    if (spriteEntranceType != type) continue;
                    validSprites.Add(list[i]);
                    break;
                }
            }

            //then picks a random sprite from the list
            return validSprites[Random.Range(0, validSprites.Count)];
        }
    }
}