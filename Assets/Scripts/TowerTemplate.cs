using UnityEngine;

[CreateAssetMenu(fileName = "Tower", menuName = "Tower Template/Create Tower Template", order = int.MaxValue)]
public class TowerTemplate : ScriptableObject
{
	public enum TowerType
	{
		����Ÿ��,
		���Ÿ��,
		����Ÿ��,
	}
	[SerializeField] private WeaponProperties[] weaponProperties;
	public WeaponProperties[] WeaponPropertie => weaponProperties;

	[System.Serializable]
	public struct WeaponProperties
	{
		public Sprite sprite;
		public string name;
		public TowerType towerType;
		public int upgradeCost;
		public float damage;
		public float support;
		public float range;
		public float rate;
		public int availableSoldiers;
	}
}
