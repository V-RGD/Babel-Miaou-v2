using Generation.Level;
using UnityEngine;
public class PlanVisualiser : MonoBehaviour
{
   public static PlanVisualiser instance;
   
   public SpriteRenderer result;
   void Awake()
   {
      if (instance != null)
      {
         Destroy(gameObject);
         return;
      }

      instance = this;
   }

   public void ConvertSpriteToOutput(Sprite plan)
   {
      int[,] grid = MaskConverter.MaskToGrid(plan.texture);
      Texture2D output = new Texture2D(plan.texture.width, plan.texture.height);
      for (int i = 0; i < plan.texture.width; i++)
      {
         for (int j = 0; j < plan.texture.height; j++)
         {
            Color color = Color.gray;
            foreach (var type in GridInterpreter.instance.pixelTypes)
            {
               if (grid[i, j] == type.index)
               {
                  color = type.color;
                  break;
               }
            }
            output.SetPixel(i, j, color);
         }
      }
      Sprite sprite = Sprite.Create(output, new Rect(0, 0, output.width, output.height), Vector2.zero);
      output.filterMode = FilterMode.Point;
      result.sprite = sprite;
      output.Apply();
   }
}
