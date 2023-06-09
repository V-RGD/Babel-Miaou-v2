using UnityEngine;

namespace Generation
{
    //this scripts uses grid information to build a level with bricks, colliders, rooms, etc...
    public class GenProBuilder : MonoBehaviour
    {
        [Header("--References--")] 
        [SerializeField] private float brickSize;

        [Header("--References--")]
        [SerializeField] private Transform levelParent;

        [Header("--BuildingAssets--")] 
        [SerializeField] private GameObject brick;
        
        [Header("---Grids---")]
        public int[,] buildingGrid;
        public int[,] heightMap;

        public void BuildLevel()
        {
            InitiateGrid();
            //Step 1 : define textures and mesh
            DefineVisuals();
            //Step 2 : set collisions throughout the level
            SetCollisions();
            //Step 3 : add game components necessary for the game to work
            AddGameComponents();

            //uses grid to build the floor of the level, then adds a procedural collider to match the mesh
            //sets the grid size to reflect the actual max positions on the rooms
        }

        public void InitiateGrid()
        {
            //match building grid size with the max positions of the planner
        }

        public void DefineVisuals()
        {
            //builds the level with the corresponding building blocks of the scene : floor tiles, pillars, bridges, etc...
        }

        public void SetCollisions()
        {
            
        }

        public void AddGameComponents()
        {
            
        }

        public void DestroyLevelInstance()
        {
            //destroys grid and level
            foreach (Transform child in levelParent)
            {
                DestroyImmediate(child);
            }
        }
    }
}
