using UnityEngine;

namespace TopDownShooter
{
    [System.Serializable]
    public class LootItem
    {
        public GameObject itemPrefab;
        [Range(0, 1)] public float dropChance;
    }

    public class LootDrop_D : MonoBehaviour
    {
        [SerializeField] private LootItem[] lootTable;

        public void SpawnLoot()
        {
            foreach (var item in lootTable)
            {
                if (Random.value <= item.dropChance)
                {
                    Instantiate(item.itemPrefab, transform.position, Quaternion.identity);
                }
            }
        }
    }
}