using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class EnemySpawner : MonoBehaviour
{
	public StageTemplate stageTemplate;

	public int currentWaveNumber = 0;
	public int maximumWave = 0;

	public int currentEnemyCount;
	public int currentWaveEnemyCount;
	public int maximumEnemyCount;

	public bool isWave;

	[SerializeField] private UIManager uiManager;


	private void Awake()
	{
		AutomaticScriptable();
		NextSetup();
		uiManager = FindAnyObjectByType<UIManager>();

		switch (GameManager.Instance.modeType)
		{
			case GameManager.ModeType.isEasyMode:
				maximumWave = stageTemplate.easyWaves.Count;
				break;
			case GameManager.ModeType.isNormalMode:
				maximumWave = stageTemplate.easyWaves.Count;
				break;
			case GameManager.ModeType.isHardMode:
				maximumWave = stageTemplate.easyWaves.Count;
				break;
		}

		Debug.Log("Spawner Awake ����");
	}

	private void AutomaticScriptable()
	{
		string currentSceneName = SceneManager.GetActiveScene().name;
		StageTemplate[] stageTemplates = Resources.LoadAll<StageTemplate>("Stage Template");

		foreach (StageTemplate stageTemplate in stageTemplates)
		{
			if (stageTemplate.StageDeveloperName == currentSceneName)
			{
				this.stageTemplate = stageTemplate;
			}
		}
	}

	public virtual void NextSetup()
	{
		switch (GameManager.Instance.modeType)
		{
			case GameManager.ModeType.isEasyMode:
				maximumWave = stageTemplate.easyWaves.Count;
				currentWaveEnemyCount = stageTemplate.easyWaves[currentWaveNumber].enemyCount;
				break;

			case GameManager.ModeType.isNormalMode:
				maximumWave = stageTemplate.normalWaves.Count;
				currentWaveEnemyCount = stageTemplate.normalWaves[currentWaveNumber].enemyCount;
				break;

			case GameManager.ModeType.isHardMode:
				maximumWave = stageTemplate.hardWaves.Count;
				currentWaveEnemyCount = stageTemplate.hardWaves[currentWaveNumber].enemyCount;
				break;
		}

		Debug.Log("Spawner NextSetup ����");
		maximumEnemyCount += currentEnemyCount;
	}


	public IEnumerator StartWave()
	{
		currentEnemyCount = 0;

		Debug.Log(currentWaveEnemyCount);

		while (currentEnemyCount < currentWaveEnemyCount)
		{
			++currentEnemyCount;
			Debug.Log("���� �Ŵ��� �θ�");
			// �� ������Ʈ�� Ǯ�� �Ŵ������� �����ɴϴ�.
			GameObject temp_EnemyObject = PoolingManager.Instance.GetEnemyQueue();
			if (temp_EnemyObject != null)
			{
				GameManager.Instance.currentWaveLivingEnemy++;
				isWave = true;

				// ���� ��忡 ���� �� �������� �����մϴ�.
				GameObject enemyPrefab = null;
				switch (GameManager.Instance.modeType)
				{
					case GameManager.ModeType.isEasyMode:
						enemyPrefab = stageTemplate.easyWaves[currentWaveNumber].enemyPrefab[Random.Range(0, stageTemplate.easyWaves[currentWaveNumber].enemyPrefab.Length)];
						break;
					case GameManager.ModeType.isNormalMode:
						enemyPrefab = stageTemplate.normalWaves[currentWaveNumber].enemyPrefab[Random.Range(0, stageTemplate.normalWaves[currentWaveNumber].enemyPrefab.Length)];
						break;
					case GameManager.ModeType.isHardMode:
						enemyPrefab = stageTemplate.hardWaves[currentWaveNumber].enemyPrefab[Random.Range(0, stageTemplate.hardWaves[currentWaveNumber].enemyPrefab.Length)];
						break;
				}

				if (enemyPrefab != null)
				{
					// ���� ������Ʈ�� �޽��� �ؽ�ó�� �� ���������� ��ü�մϴ�.
					var enemyRenderer = temp_EnemyObject.GetComponent<SpriteRenderer>();
					var prefabRenderer = enemyPrefab.GetComponent<SpriteRenderer>();
					if (enemyRenderer != null && prefabRenderer != null)
					{
						enemyRenderer.sprite = prefabRenderer.sprite;

					}

					// �� ���ø��� �ε��մϴ�.
					string objectName = enemyPrefab.transform.name;
					temp_EnemyObject.name = objectName;
					EnemyTemplate enemyTemplate = Resources.Load<EnemyTemplate>($"Enemy Template/{objectName}");

					// ���� ü���� �����մϴ�.
					var enemyHealth = temp_EnemyObject.GetComponent<EnemyHealth>();
					if (enemyHealth != null)
					{
						enemyHealth.MaxHealth = enemyTemplate.health;
						enemyHealth.Health = enemyTemplate.health;
					}

					temp_EnemyObject.GetComponent<Movement2D>().MoveSpeed = enemyTemplate.speed;
					// ���� ������Ʈ�� ��ġ�� �ʱ�ȭ�մϴ�.
					temp_EnemyObject.transform.position = Vector2.zero;
					temp_EnemyObject.SetActive(true);
				}

				// �� ���� �ӵ��� ���� ����մϴ�.
				float temp_timer = 0;
				switch (GameManager.Instance.modeType)
				{
					case GameManager.ModeType.isEasyMode:
						temp_timer = Random.Range(stageTemplate.easyWaves[currentWaveNumber].enemySpawnRate / 1.5f, stageTemplate.easyWaves[currentWaveNumber].enemySpawnRate);
						break;
					case GameManager.ModeType.isNormalMode:
						temp_timer = Random.Range(stageTemplate.normalWaves[currentWaveNumber].enemySpawnRate / 1.5f, stageTemplate.normalWaves[currentWaveNumber].enemySpawnRate);
						break;
					case GameManager.ModeType.isHardMode:
						temp_timer = Random.Range(stageTemplate.hardWaves[currentWaveNumber].enemySpawnRate / 1.5f, stageTemplate.hardWaves[currentWaveNumber].enemySpawnRate);
						break;
				}
				yield return new WaitForSeconds(temp_timer);
			}
		}

		// ������ ��� óġ�� ������ ��ٸ��ϴ�.
		while (GameManager.Instance.currentWaveLivingEnemy > 0)
		{
			yield return null;
		}

		GameManager.Instance.currentScore += 185;
		PlayerStatus.Instance.Coin += 1000;
		isWave = false;
		currentWaveNumber++;
		NextWaveSetting();
	}



	public virtual void NextWaveSetting()
	{
		if (currentWaveNumber >= maximumWave)
		{
			uiManager.OnVictory();
			Debug.Log("���̺� ����");
			return;
		}

		GameManager.Instance.currentScore += 300;
		uiManager.gameReadyGroup.SetActive(true);
		Debug.Log($"���� ���̺� : {currentWaveNumber} �ִ� ���̺� : {maximumWave}");
		NextSetup();
	}
}
