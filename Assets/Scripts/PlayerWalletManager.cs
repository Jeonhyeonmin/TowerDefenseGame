using UnityEngine;

public class PlayerWalletManager : SingletonManager<PlayerWalletManager>
{
    private int totalCoin;
    private int totalCrystal;

    public int TotalCoin
    {
        get => totalCoin; set => totalCoin = Mathf.Clamp(value, 0, int.MaxValue);
    }

    public int TotalCrystal
    {
        get => totalCrystal; set => totalCrystal = Mathf.Clamp(value, 0, int.MaxValue);
    }

	private void OnEnable()
	{
        totalCoin = PlayerPrefs.GetInt("PlayerWallet_Coin");
        totalCrystal = PlayerPrefs.GetInt("PlayerWallet_Crystal");
	}

	private void Start()
	{
        InvokeRepeating("SavePlayerWallet", 0.1f, 3.0f);
	}

    private void SavePlayerWallet()
    {
        PlayerPrefs.SetInt("PlayerWallet_Coin", totalCoin);
		PlayerPrefs.SetInt("PlayerWallet_Crystal", totalCrystal);
        PlayerPrefs.Save();
	}

    public int GetTotalCoin()
    {
        return totalCoin;
	}

    public int GetTotalCrystal()
    {
        return totalCrystal;
	}
}
