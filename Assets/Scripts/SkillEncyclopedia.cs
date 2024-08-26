using UnityEngine;

public class SkillEncyclopedia : SingletonManager<SkillEncyclopedia>
{
	#region 변수

	private ItemDatabase[] itemDatabases;

	#endregion

	private void Start()
	{
		FindItemDatabase();
	}

	private void FindItemDatabase()
	{
		itemDatabases = Resources.LoadAll<ItemDatabase>("Item/ItemDataBase");
		Debug.Log($"아이템 데이터 베이스 {itemDatabases.Length}개가 인식 되었습니다.");
	}	// 아이템 데이터 베이스를 자동으로 검색해주는 매서드
}
