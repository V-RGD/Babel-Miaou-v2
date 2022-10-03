using UnityEngine;

public class RoomGeneration : MonoBehaviour
{
    public GameObject shop;
    public GameObject[] props;
    public GameObject[] specialProps;
    public GameObject[] doors;

    private RoomInfo _roomInfo;

    private void Start()
    {
        _roomInfo = GetComponent<RoomInfo>();

        switch (_roomInfo.roomType)
        {
            case 1 : //spawns start stairs
                break;
            case 2 : //spawns end stairs, disabled
                //Spawns the mini boss
                break;
            case 3 : //spawns stela, disabled
                break;
            case 4 : //spawns shop
                break;
        }
    }
}
