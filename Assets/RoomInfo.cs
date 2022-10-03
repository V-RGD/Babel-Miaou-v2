using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    public bool isThereARoomRight;
    public bool isThereARoomUp;
    public bool isThereARoomDown;
    public bool isThereARoomLeft;

    [HideInInspector]public int roomType; // 0(normal) 1(Start) 2(End) 3(Bonus) 4(Shop) 5(boss final)
}
