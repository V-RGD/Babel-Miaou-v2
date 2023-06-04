using System.Collections.Generic;
using Items;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HUD : MonoBehaviour
    {
        public static HUD instance;
        [Header("Health")] [SerializeField] private TMP_Text healthTxt;
        [Header("Items")] [SerializeField] private List<Image> itemImages;
        [Header("Money")] [SerializeField] private TMP_Text moneyTxt;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        public void UpdateHealth() => healthTxt.text = HealthSystem.instance.health.ToString();
        public void UpdateMoney() => moneyTxt.text = Inventory.instance.money.ToString();

        public void UpdateItemVisuals()
        {
            itemImages.Clear();
            for (int i = 0; i < Inventory.instance.itemsEquipped.Length; i++)
            {
                itemImages[i].sprite = ItemInfos.infos[Inventory.instance.itemsEquipped[i]].icon;
            }
        }
    }
}