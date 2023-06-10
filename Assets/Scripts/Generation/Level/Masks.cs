using UnityEngine;

namespace Generation
{
    public class Masks : MonoBehaviour
    {
        public static int[,] RectangularMask(Vector2Int size, int type)
        {
            //creates mask
            int[,] mask = new int[size.x, size.y];
            //assigns value to each tile
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    //define every tile of the mask as a ground tile
                    mask[i, j] = type;
                }
            }
        
            return mask;
        }
    }
}
