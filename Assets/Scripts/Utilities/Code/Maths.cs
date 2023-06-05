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

        public static Vector3 IgnoreY(Vector3 targetPos, Vector3 keepY)
        {
            return new Vector3(targetPos.x, keepY.y, targetPos.z);
        }
    }
}
