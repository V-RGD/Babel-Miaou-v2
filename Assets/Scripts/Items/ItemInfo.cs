using System;
using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    [Serializable] public class ItemInfo : MonoBehaviour
    {
        public string name;
        public string description;
        public Sprite icon;
    }

    public static class ItemInfos
    {
        public static List<ItemInfo> infos;
    }
}

