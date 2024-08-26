using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrack : Tower
{
	[SerializeField] private GameObject soldierPrefab;
	[SerializeField] private Transform soldierFirePos;

	private List<GameObject> selectedTargets;

	[SerializeField] private int soldierPoolSize = 30;
	private Queue<GameObject> soldierObjectPool;

	private bool currentLockTarget;

	private Coroutine attackCoroutine;

	public LayerMask enemyLayerMask;

	public override void SearchTarget()
	{
		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, Range, enemyLayerMask);

		if (colliders.Length > 0)
		{
			if (!currentLockTarget)
			{
				selectedTargets = new List<GameObject>();

				foreach (Collider2D collider in colliders)
				{
					selectedTargets.Add(collider.gameObject);
				}

				currentLockTarget = true;

				if (attackCoroutine == null)
				{
					attackCoroutine = StartCoroutine(AttackRoutine());
				}
			}
			else
			{
				UpdateTargets(colliders);

				if (!ShouldLockTarget(colliders))
				{
					selectedTargets = null;
					currentLockTarget = false;

					if (attackCoroutine != null)
					{
						StopCoroutine(attackCoroutine);
						attackCoroutine = null;
					}
				}

				if (attackCoroutine == null)
				{
					attackCoroutine = StartCoroutine(AttackRoutine());
				}
			}
		}
		else
		{
			if (currentLockTarget)
			{
				selectedTargets = null;
				currentLockTarget = false;

				if (attackCoroutine != null)
				{
					StopCoroutine(attackCoroutine);
					attackCoroutine = null;
				}
			}
		}
	}

	private void UpdateTargets(Collider2D[] enemyColliders)
	{
		// �ӽ� ����Ʈ�� ����Ͽ� ������ Ÿ���� �����մϴ�.
		List<GameObject> targetsToRemove = new List<GameObject>();

		// ���� Ÿ�� ����Ʈ�� ��ȸ�ϸ� ���� �ۿ� �ִ� Ÿ���� ã���ϴ�.
		foreach (GameObject target in selectedTargets)
		{
			bool isStillInRange = false;

			foreach (Collider2D collider in enemyColliders)
			{
				if (collider.gameObject == target)
				{
					isStillInRange = true;
					break;
				}
			}

			if (!isStillInRange)
			{
				targetsToRemove.Add(target);
			}
		}

		// ������ ��� Ÿ���� �����մϴ�.
		foreach (GameObject targetToRemove in targetsToRemove)
		{
			selectedTargets.Remove(targetToRemove);
		}

		// ���Ӱ� ���� �ȿ� ���� ���� �߰��մϴ�.
		foreach (Collider2D collider in enemyColliders)
		{
			if (!selectedTargets.Contains(collider.gameObject))
			{
				selectedTargets.Add(collider.gameObject);
			}
		}
	}


	private void Awake()
	{
		Setup();
	}

	public override void Setup()
	{
		base.Setup();

		soldierObjectPool = new Queue<GameObject>();

		for (int i = 0; i < soldierPoolSize; i++)
		{
			GameObject clone = Instantiate(soldierPrefab);
			clone.transform.parent = transform;
			clone.SetActive(false);
			soldierObjectPool.Enqueue(clone);
		}

		CurrentLevel = 0;
	}

	private GameObject GetSoldierObject()
	{
		if (soldierObjectPool.Count > 0)
		{
			GameObject clone = soldierObjectPool.Dequeue();
			clone.SetActive(true);
			return clone;
		}
		else if (soldierObjectPool.Count <= 0 && transform.childCount < AvailableSoldiers)
		{
			GameObject clone = Instantiate(soldierPrefab);
			clone.transform.parent = transform;
			return clone;
		}

		return null;
	}

	public void ReturnObject(GameObject obj)
	{
		obj.SetActive(false);
		soldierObjectPool.Enqueue(obj);
	}

	private void Update()
	{
		SearchTarget();
	}

	public bool ShouldLockTarget(Collider2D[] colliders)
	{
		foreach (Collider2D collider in colliders)
		{
			if (selectedTargets.Contains(collider.gameObject))
			{
				return true;
			}
		}
		return false;
	}

	private IEnumerator AttackRoutine()
	{
		bool _RegisteringAnim = false;

		while (currentLockTarget && selectedTargets != null && selectedTargets.Count > 0)
		{
			if (selectedTargets == null || selectedTargets.Count == 0)
			{
				currentLockTarget = false;
				attackCoroutine = null;
				yield break;
			}

			if (!_RegisteringAnim)
			{
				yield return new WaitForSeconds(TowerResponseTime);
			}

			_RegisteringAnim = true;

			// Ȱ��ȭ�� �ڽ� ������Ʈ�� ������ ���
			int activeChildrenCount = 0;
			foreach (Transform child in transform)
			{
				if (child.gameObject.activeSelf)
				{
					activeChildrenCount++;
				}
			}

			// Ȱ��ȭ�� �ڽ� ������Ʈ�� ������ AvailableSoldiers�� �ʰ��ϸ� ����
			if (activeChildrenCount >= AvailableSoldiers)
			{
				StopCoroutine(attackCoroutine);
				attackCoroutine = null;
				yield break;
			}

			// �������� Ÿ�� ����
			int randomIndex = Random.Range(0, selectedTargets.Count);
			GameObject randomTarget = selectedTargets[randomIndex];

			if (randomTarget != null)
			{
				SpawnAttackBarrackSoldier(randomTarget.transform);
				yield return new WaitForSeconds(Rate);
			}

			if (selectedTargets == null || selectedTargets.Count == 0)
			{
				currentLockTarget = false;
				attackCoroutine = null;
				yield break;
			}

			// ���� ���� �غ�
			yield return new WaitForSeconds(0.1f); // �ణ�� ���� �߰�
		}
	}

	public void SpawnAttackBarrackSoldier(Transform target)
	{
		if (target == null)
		{
			Debug.Log("Ÿ���� ã�� �� �����ϴ�.");
			return;
		}

		GameObject solider = GetSoldierObject();

		if (solider == null)
		{
			Debug.Log("��� ������ ���簡 �����ϴ�.");
			return;
		}

		solider.transform.position = soldierFirePos.position;
		Soldier soldierScript = solider.GetComponent<Soldier>();
		soldierScript.FollowSetup(target, Damage);
	}


	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, Range);
	}
}
