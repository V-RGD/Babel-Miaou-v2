using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isLocked;

    private MeshCollider col;
    private ProceduralGeneration proGen;
    private Room room;
    public int itemToSpawn;
    void Start()
    {
        room = GetComponentInParent<Room>();
        proGen = GameObject.Find("LevelManager").GetComponent<ProceduralGeneration>();
        itemToSpawn = Random.Range(0, proGen.itemLoots.Length);
        col = GetComponent<MeshCollider>();
        //display the item you'll win
    }

    // Update is called once per frame
    void Update()
    {
        if (room.canEnterDoor)
        {
            Unlock();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isLocked && other.CompareTag("Player") && room.canEnterDoor) 
        {
            //generates next room
            MoveSpawnPoint();
        }
    }

    void Unlock()
    {
        col.enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
    }

    void MoveSpawnPoint()
    {
        room.doorTaken = true;
        if (CompareTag("LeftDoor"))
        {
            proGen.lastDoorPos = 1;
        }
        
        if (CompareTag("UpDoor"))
        {
            proGen.lastDoorPos = 2;
        }
        
        if (CompareTag("RightDoor"))
        {
            proGen.lastDoorPos = 3;
        }
    }
}
