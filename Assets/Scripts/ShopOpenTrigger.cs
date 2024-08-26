using UnityEngine;

public class ShopOpenTrigger : MonoBehaviour
{
	private bool alreayOpen;
	[SerializeField] private ShopManager shopManager;

	private void OnEnable()
	{
		InvokeRepeating("InvokeInitializationItems", 0.0f, 1.0f);

		if (!alreayOpen)
		{
			shopManager.LoadItemData(); // ���� ���� ���� ���� �� �������� �ִ� �� �˻� �� �ҷ��ɴϴ�.
			shopManager.FirstTimeOpenShopUI(); // ���ʷ� ������ ���� ���� �� ������ ���̽��� �ִ� ������ �������� ����մϴ�.
			alreayOpen = true;
		}
	}

	private void OnDisable()
	{
		CancelInvoke("InvokeInitializationItems");
	}

	private void InvokeInitializationItems()
	{
		shopManager.InitializationItems();
	}
}
