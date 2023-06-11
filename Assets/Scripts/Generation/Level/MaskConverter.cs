using UnityEngine;

namespace Generation.Level
{
    public class MaskConverter : MonoBehaviour
    {
        
        //checks colors and indexes the pixel grid based on it
        public static int[,] MaskToGrid(Texture2D mask)
        {
            int[,] grid = new int[mask.width, mask.height];

            //loops through every pixel on the sprite
            for (int i = 0; i < mask.width; i++)
            {
                for (int j = 0; j < mask.height; j++)
                {
                    Color pixelColor = mask.GetPixel(i, j);
                    //checks pixel color, indexes pixel with corresponding id
                    foreach (var type in GridInterpreter.instance.pixelTypes)
                    {
                        if (pixelColor == type.color) grid[i, j] = type.index;
                    }
                }
            }

            return grid;
        }
    }
}