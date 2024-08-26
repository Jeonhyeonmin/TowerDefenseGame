using UnityEngine;

public class SaveScoreManager : SingletonManager<SaveScoreManager>
{
    public Sprite oneStar;
	public Sprite twoStar;
	public Sprite threeStar;

    private const string StageScoreKey = "StageScore_";
    private const string StageStarKey = "StageStar_";

    public void SetScore(string stageName, int score)
    {
        if (score > int.MaxValue)
        {
            Debug.LogError("�ý��� ���� : ���� ���� �ִ�ġ�� �Ѱ���ϴ�.");
            return;
        }

        if (PlayerPrefs.GetInt(StageScoreKey + score) > score)
		{
			Debug.LogError("���� �������� ���� ������ �� �����ϴ�.");
			return;
		}

		PlayerPrefs.SetInt(StageScoreKey + stageName, score);
        PlayerPrefs.Save();
    }

    public void SetStars(string stageName, int starCount)
    {
        if (starCount < 0 || starCount > 3)
        {
            Debug.LogError("�� ������ 0�� 3 ���̿��� �մϴ�.");
            return;
        }

        if (PlayerPrefs.GetInt(StageStarKey + stageName) > starCount)
        {
            Debug.LogError("���� �� �������� ���� �� ������ �� �����ϴ�.");
            return;
        }

        PlayerPrefs.SetInt(StageStarKey + stageName, starCount);
        PlayerPrefs.Save();
    }

    public int GetScore(string stageName)
    {
        return PlayerPrefs.GetInt(StageScoreKey + stageName, 0);
    }

    public int GetStars(string stageName)
    {
        return PlayerPrefs.GetInt(StageStarKey + stageName, 0);
    }
}
