using UnityEngine;
using UnityEngine.EventSystems; // EventSystem�� ����ϱ� ���� ���ӽ����̽�

public class UpgradeTower : MonoBehaviour
{
	[SerializeField] private UIManager uiManager;

	[Header("User Interface")]
	[SerializeField] private RectTransform towerManagementWindow; // RectTransform���� ����

	[Header("System Variables")]
	[SerializeField] private LayerMask towerLayerMask; // Ÿ�� ���̾ ���Ե� LayerMask ����
	[SerializeField] private LayerMask uiLayerMask;
	[SerializeField] private Vector2 offset; // UI�� Ÿ�� ��ġ�κ��� �̵���Ű�� ������

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			// UI�� Ŭ���ߴ��� Ȯ��
			if (!EventSystem.current.IsPointerOverGameObject())
			{
				// UI�� �ƴ� ���� Ŭ�������� UI�� ����
				if (towerManagementWindow.gameObject.activeSelf)
				{
					towerManagementWindow.gameObject.SetActive(false);
				}
				else
				{
					SelectTower();
				}
			}
			else
			{
				return;
			}
		}
	}

	private void SelectTower()
	{
		Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		RaycastHit2D towerHit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, towerLayerMask);
		if (towerHit.collider != null && towerHit.transform.CompareTag("Tower"))
		{
			ShowTowerUI(towerHit.collider.gameObject);
			ViewerTowerInfo.Instance.RefreshInterface(towerHit.transform.gameObject);
		}
	}

	private void ShowTowerUI(GameObject tower)
	{
		// Ÿ���� ���� ��ǥ ���
		Vector3 towerWorldPosition = tower.transform.position;

		// Ÿ���� ȭ�� ��ǥ ���
		Vector3 towerScreenPosition = Camera.main.WorldToScreenPoint(towerWorldPosition);

		// ȭ�� ũ��
		float screenWidth = Screen.width;
		float screenHeight = Screen.height;

		// UI�� RectTransform�� ���
		RectTransform uiRectTransform = towerManagementWindow;

		// UI�� ũ��
		float uiWidth = uiRectTransform.rect.width;
		float uiHeight = uiRectTransform.rect.height;

		// �ʱ� UI ��ġ�� Ÿ���� ����
		Vector3 newScreenPosition = towerScreenPosition + new Vector3(0, uiHeight / 2 + offset.y, 0);

		// UI�� ��ġ ���� (ȭ���� �Ѿ�� �ʵ���)
		// ������ ��� Ȯ�� �� ����
		if (newScreenPosition.x + uiWidth / 2 > screenWidth)
		{
			newScreenPosition.x = screenWidth - uiWidth / 2 - offset.x;
		}

		// ���� ��� Ȯ�� �� ����
		if (newScreenPosition.x - uiWidth / 2 < 0)
		{
			newScreenPosition.x = uiWidth / 2 + offset.x;
		}

		// ��� ��� Ȯ�� �� ����
		if (newScreenPosition.y + uiHeight / 2 > screenHeight)
		{
			newScreenPosition.y = screenHeight - uiHeight / 2 - offset.y;
		}

		// �ϴ� ��� Ȯ�� �� ����
		if (newScreenPosition.y - uiHeight / 2 < 0)
		{
			newScreenPosition.y = uiHeight / 2 + offset.y;
		}

		// ���ο� ���� ��ǥ
		Vector3 newWorldPosition = Camera.main.ScreenToWorldPoint(newScreenPosition);
		newWorldPosition.z = towerWorldPosition.z; // z���� Ÿ���� z������ ����

		// UI�� ���ο� ��ġ ����
		uiRectTransform.position = newWorldPosition;
		towerManagementWindow.gameObject.SetActive(true);
	}
}
