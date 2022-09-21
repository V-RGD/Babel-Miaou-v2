using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isLocked;

    private MeshCollider col;
    private ProceduralGeneration proGen;
    private Room room;
    private bool canOpen;

    public int itemToSpawn;
    void Start()
    {
        room = GetComponentInParent<Room>();
        proGen = GameObject.Find("LevelManager").GetComponent<ProceduralGeneration>();
        itemToSpawn = Random.Range(0, proGen.itemLoots.Length);
        col = GetComponent<MeshCollider>();
        canOpen = true;
        //display the item you'll win
    }

    // Update is called once per frame
    void Update()
    {
        if (canOpen)
        {
            //Unlock();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isLocked && other.CompareTag("Player")) 
        {
            //generates next room
            MoveSpawnPoint();
            proGen.canGenerate = true;
        }
    }

    void Unlock()
    {
        //opens if all enemies are defeated
        if (room.enemiesRemaining == 0)
        {
            canOpen = false;
            //open access
            col.enabled = false;
            GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            col.enabled = true;
        }
    }

    void MoveSpawnPoint()
    {
        if (CompareTag("LeftDoor"))
        {
            proGen.generatorPos += new Vector3(-proGen.offset, 0, 0);
        }
        
        if (CompareTag("RightDoor"))
        {
            proGen.generatorPos += new Vector3(proGen.offset, 0, 0);
        }
        
        if (CompareTag("UpDoor"))
        {
            proGen.generatorPos += new Vector3(0, 0, proGen.offset);
        }
    }
}
