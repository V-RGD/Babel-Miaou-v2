using UnityEngine;

namespace Utilities.Code
{
    public class Maths : MonoBehaviour
    {
        public static int NextOfList(int value, int max)
        {
            if (value +  1 > max) return 0;
            else return value + 1;
        }
    }
}
