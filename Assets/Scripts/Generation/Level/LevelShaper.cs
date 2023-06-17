using UnityEngine;

namespace Generation.Level
{
    public class LevelShaper : MonoBehaviour
    {
        public static LevelShaper instance;

        //this script uses the info given by the planner to shape rooms and bridges

        void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        public void ShapeLevel()
        {
            //Step 5 : define room shapes
            CreateRooms();
            //Step 6 : links each room with a bridge
            AddBridges();
            //Step 7 : send info to the builder to create the level
            LevelBuilder.instance.BuildLevel();
            Debug.Log("Created level grid");

            //create a room using a mask available of this type
        }

        void CreateRooms()
        {
            //builds every room
            foreach (LevelPlanner.RoomGenerationInfo info in LevelPlanner.instance.roomBuffer)
            {
                //converts sprite plan to mask
                int[,] mask = MaskConverter.MaskToGrid(info.plan.texture);
                //match mask offset with generation position
                Vector2Int pos = info.instantiationTile;
                //apply mask
                GridUtilities.ApplyMask(mask, pos, true);
            }
        }

        void AddBridges()
        {
            //for each connection between rooms, builds a bridge between exits and entrances
            for (int i = 0; i < LevelPlanner.instance.roomBuffer.Count - 1; i++)
            {
                //calculates distance between both rooms
                Vector2Int exit = LevelPlanner.instance.roomBuffer[i].exitTile;
                Vector2Int entrance = LevelPlanner.instance.roomBuffer[i+1].entryTile;
                Vector2Int distance = entrance - exit;
                int bridgeWidth = LevelPlanner.instance.generationSettings.bridgeWidth;

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

                int[,] bridgeMask = Masks.RectangularMask(size, 6);
                GridUtilities.ApplyMask(bridgeMask, exit + maskPlacementOffset, false);
            }
        }
    }
}