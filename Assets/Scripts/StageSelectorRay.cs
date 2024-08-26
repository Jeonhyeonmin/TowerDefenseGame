using UnityEngine;
using UnityEngine.UI;

public class StageSelectorRay : MonoBehaviour
{
	[SerializeField] private LayerMask layerMask;

	private static StageSelectorRay instance;

	public static StageSelectorRay Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindAnyObjectByType<StageSelectorRay>();

				if (instance == null)
				{
					Debug.LogError("StageSelectorRay가 없습니다.");
				}
			}

			return instance;
		}
	}

	public string seletedStageName = string.Empty;

	[SerializeField] private Button gameStartButton;

	private void Update()
	{
		RaycastSelection();
		GameStartButtonAnimation();
	}

	private void RaycastSelection()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector2 rayPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			RaycastHit2D hitInfo = Physics2D.Raycast(rayPosition, Vector3.zero, Mathf.Infinity, layerMask);

			if (hitInfo.collider != null)
			{
				seletedStageName = hitInfo.transform.GetComponent<StageJoinFlag>().stageName;
				GameManager.Instance.seletedJoinFlag = hitInfo.transform.GetComponent<StageJoinFlag>();
				GameManager.Instance.seletedJoinFlagName = hitInfo.transform.GetComponent<StageJoinFlag>().stageName;
			}
		}
	}

	private void GameStartButtonAnimation()
	{
		if (!string.IsNullOrEmpty(seletedStageName))
		{
			gameStartButton.GetComponent<Animator>().SetBool("Seleted", true);
		}
		else
		{
			gameStartButton.GetComponent<Animator>().SetBool("Seleted", false);
		}
	}
}
