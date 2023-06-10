using UnityEngine;

namespace Generation
{
    //this scripts uses grid information to build a level with bricks, colliders, rooms, etc...
    public class GenProBuilder : MonoBehaviour
    {
        [Header("--References--")] 
        [SerializeField] private Vector3 tileSize;

        [Header("--References--")]
        [SerializeField] private Transform levelParent;

        [Header("--BuildingAssets--")] 
        [SerializeField] private GameObject proceduralObjectPrefab;
        
        [Header("---Grids---")]
        public int[,] buildingGrid;
        public int[,] heightMap;
        
        [Header("--Debug Bricks--")]
        [SerializeField] private GameObject brick;
        [SerializeField] private Material bridgeMat;
        [SerializeField] private Material centerMat;
        [SerializeField] private Material brickMat;
        public void BuildLevel()
        {
            //initiates grid size
            InitiateGrid();
            //builds a buffer for each part of the level that will be built
            CraftParts();
            VisualiseGrid();
            return;
            
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

        public void CraftParts()
        {
            
        }

        public void VisualiseGrid()
        {
            for (int i = 0; i < buildingGrid.GetLength(0); i++)
            {
                for (int j = 0; j < buildingGrid.GetLength(1); j++)
                {
                    if (buildingGrid[i, j] != 0)
                    {
                        Vector3 pos = new Vector3(i * tileSize.x, 0 * tileSize.y, j * tileSize.z);
                        GameObject newBrick = Instantiate(brick, pos, Quaternion.identity);
                        newBrick.transform.localScale = new Vector3(tileSize.x, tileSize.y, tileSize.z);
                        newBrick.transform.parent = levelParent;
                        Material mat = brickMat;
                        switch (buildingGrid[i, j])
                        {
                            case 1 : mat = brickMat; break;
                            case 2 : mat = bridgeMat; break;
                            case 3 : mat = centerMat; break;
                        }
                        newBrick.GetComponent<MeshRenderer>().material = mat;
                    }
                }
            }
        }

        public GameObject ProceduralObject(Vector2Int[] gridVertices, Material material)
        {
            // //creates object
            // GameObject newObject = Instantiate(proceduralObjectPrefab);
            // MeshFilter filter = newObject.GetComponent<MeshFilter>();
            // MeshRenderer meshRenderer = newObject.GetComponent<MeshRenderer>();
            // MeshCollider collider = newObject.GetComponent<MeshCollider>();
            //
            // Vector3[] vertices = new Vector3[gridVertices.Length];
            // //convert vertices from grid position to world position to match desired scale
            // for (int i = 0; i < vertices.Length; i++)
            // {
            //     vertices[i] = new Vector3()
            // }
            //
            // //creates mesh using vertices
            // //updates collider
            // //adds material
            return brick;
        }

        public void DestroyLevelInstance()
        {
            //destroys grid and level
            foreach (Transform child in levelParent)
            {
                DestroyImmediate(child);
            }
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
    }
}
