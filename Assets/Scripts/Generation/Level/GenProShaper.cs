using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Generation
{
    public class GenProShaper : MonoBehaviour
    {
        //this script uses the info given by the planner to shape rooms and bridges
        [Header("---Variables---")] [SerializeField]
        private Shape roomShape;

        [SerializeField] private int bridgeWidth;

        private enum Shape
        {
            Round,
            Square
        }

        private GenProBuilder _builder;
        private GenProPlanner _planner;
        private void Awake()
        {
            _builder = GetComponent<GenProBuilder>();
            _planner = GetComponent<GenProPlanner>();
        }

        public void ShapeLevel()
        {
            //Step 5 : define room shapes
            CreateRooms();
            //Step 6 : links each room with a bridge
            AddBridges();
            //Step 7 : send info to the builder to create the level
            _builder.BuildLevel();
            Debug.Log("Created level grid");
        }

        void CreateRooms()
        {
            //builds every room
            for (int i = 0; i < _planner.roomBuffer.Count; i++) CreateRoom(i);
        }

        void CreateRoom(int index)
        {
            //uses center and size to build a room with a round or more squared shape
            GenProPlanner.RoomGenerationInfo info = _planner.roomBuffer[index];
            Vector2Int centerPos = info.centerPosition;
            int size = Random.Range(info.generationValues.size.x, info.generationValues.size.y);
            //room size has to be an uneven number
            if (size % 2 == 0) size++;

            switch (roomShape)
            {
                case Shape.Round:
                    Debug.LogError("Round rooms are currently unsupported");
                    return;
                case Shape.Square:
                    ShapeSquareRoom(size, centerPos);
                    break;
            }
        }

        void ShapeSquareRoom(int size, Vector2Int centerPos)
        {
            //create a new mask to shape the room
            int[,] mask = Masks.RectangularMask(new Vector2Int(size, size), 1);
            int offset = -(size - 1) / 2;
            Vector2Int pos = centerPos + new Vector2Int(offset, offset);
            //then applies it
            ApplyMask(mask, pos, true);
        }

        void AddBridges()
        {
            //for each connection between rooms, builds a bridge between centers
            
            //for each room (except the last one)
            for (int i = 0; i < _planner.roomBuffer.Count - 1; i++)
            {
                Vector2Int roomCenter = _planner.roomBuffer[i].centerPosition;
                Vector2Int nextRoomCenter = _planner.roomBuffer[i + 1].centerPosition;

                //calculates distance between both rooms
                Vector2Int distance = nextRoomCenter - roomCenter;
                Vector2Int maskPlacementOffset = Vector2Int.zero;

                //if the next room is on the right, bridge length is on the X axis (and the width is on the Y)
                //the mask will be applied below the starting point
                Vector2Int size = Vector2Int.zero;
                if (distance.x > distance.y)
                {
                    size = new Vector2Int(distance.x, bridgeWidth);
                    maskPlacementOffset = new Vector2Int(1, -((bridgeWidth - 1) / 2));
                }

                //if the next room is on the top, bridge length : Y axis (and width : X)
                //the mask will be applied on the left of the starting point
                if (distance.y > distance.x)
                {
                    size = new Vector2Int(bridgeWidth, distance.y);
                    maskPlacementOffset = new Vector2Int(-((bridgeWidth - 1) / 2), 1);
                }

                if (distance.y == distance.x) Debug.LogError("Failed to compare room distances");

                int[,] bridgeMask = Masks.RectangularMask(size, 2);
                ApplyMask(bridgeMask, roomCenter + maskPlacementOffset, false);
                //add noise to destroy some parts, but make sure that the bridge hasn't got any holes
            }
        }

        void ApplyMask(int[,] mask, Vector2Int location, bool hasPriority)
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
                    if (!hasPriority && _builder.buildingGrid[tilePos.x, tilePos.y] != 0) continue;
                    
                    _builder.buildingGrid[tilePos.x, tilePos.y] = mask[i, j];
                }
            }
        }
    }
}