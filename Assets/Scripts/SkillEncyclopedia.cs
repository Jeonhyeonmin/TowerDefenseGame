using UnityEngine;

public class SkillEncyclopedia : SingletonManager<SkillEncyclopedia>
{
	#region ����

	private ItemDatabase[] itemDatabases;

	#endregion

	private void Start()
	{
		FindItemDatabase();
	}

	private void FindItemDatabase()
	{
		itemDatabases = Resources.LoadAll<ItemDatabase>("Item/ItemDataBase");
		Debug.Log($"������ ������ ���̽� {itemDatabases.Length}���� �ν� �Ǿ����ϴ�.");
	}	// ������ ������ ���̽��� �ڵ����� �˻����ִ� �ż���
}
