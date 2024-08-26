using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Shop/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
	public Item[] items;
}
