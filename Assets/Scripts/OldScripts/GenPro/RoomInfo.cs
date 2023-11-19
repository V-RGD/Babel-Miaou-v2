using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    public bool isThereARoomRight;
    public bool isThereARoomUp;
    public bool isThereARoomDown;
    public bool isThereARoomLeft;

    [HideInInspector]public int roomType; // 0(normal) 1(Start) 2(End) 3(Bonus) 4(Shop) 5(boss final)

    public int[] doors = new int[4];

    private void Awake()
    {
        if (isThereARoomLeft)
        {
            doors[0] = 1;
        }
        if (isThereARoomUp)
        {
            doors[1] = 1;
        }
        if (isThereARoomRight)
        {
            doors[2] = 1;
        }
        if (isThereARoomDown)
        {
            doors[3] = 1;
        }
    }
}
