using System;
using System.Collections.Generic;
using UnityEngine;

namespace Generation.Level
{
    public class GridInterpreter : MonoBehaviour
    {
        public static GridInterpreter instance;

        public List<PixelType> pixelTypes;

        [Serializable]
        public class PixelType
        {
            public int index;
            public Color color;
        }

        void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }
        
        
    }
}