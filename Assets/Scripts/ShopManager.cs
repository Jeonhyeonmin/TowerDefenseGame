using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.U2D;
using UnityEngine.UI;
using static UnityEditor.Progress;
using Random = UnityEngine.Random;

public class ShopManager : MonoBehaviour
{
	#region ���� ����

	[SerializeField] private ItemDatabase freeitemDatabase;
	[SerializeField] private ItemDatabase buyitemDatabase;
	[SerializeField] private List<Item> availablefreeItems = new List<Item>();
	[SerializeField] private List<Item> availablebuyItems = new List<Item>();
	[SerializeField] private int freeItemsToDisplay = 2;
    [SerializeField] private int buyitemsToDisplay = 4;
    [SerializeField] private GameObject itemUIPrefab;
    [SerializeField] private Transform itemFreeUIParent;
    [SerializeField] private Transform itemBuyUIParent;
    [SerializeField] private GameObject shopPanel;

	[SerializeField] private string firstTimeOpenShopKey = "FirstTimeOpenShopKey";

	[SerializeField] private Sprite coinSprite;
	[SerializeField] private Sprite crystalSprite;

	private const string FreeItemKey = "AvailableFreeItems";
	private const string BuyItemKey = "AvailableBuyItems";

	private DateTime endTime;

	#endregion

	private void Start()
	{ 
		SettingsItemDisplayCount(); // ������ �����ͺ��̽��� �������� ������ "itemsToDisplay"���� ������ �ڵ����� "itemsToDisplay"�� ���� �����մϴ�.
		NotifyItemCount();  // �����ڿ��� ������ ���̽��� ���� ����, Ȱ��ȭ �� �������� ������ �˷��ݴϴ�.	
	}

	public void LoadItemData()
	{
		string endTimeString = PlayerPrefs.GetString("ShopEndTime", null);

		if (!string.IsNullOrEmpty(endTimeString))
		{
			endTime = DateTime.Parse(endTimeString);
		}
		else
		{
			endTime = DateTime.Now;
			Debug.Log("'endTime'�� ����� �ӽ� �ʱ�ȭ �Ǿ����ϴ�.");
		}

		TimeSpan remainingTime = endTime - DateTime.Now;

		if (remainingTime.TotalSeconds <= 0)
		{
			Debug.Log($"remainingTime : {remainingTime.TotalSeconds}�̹Ƿ� ���� ���� �� ���� �������� �ʱ�ȭ �Ǿ����ϴ�.");
			return;
		}
		else
		{
			Debug.Log($"{remainingTime.TotalSeconds.ToString("F2")}�ʰ� �������Ƿ� ���� �ʱ�ȭ�� �̷������ �ʰ� ���� ���� �������� �ε��˴ϴ�");
		}

		// availablefreeItems �ҷ�����
		if (PlayerPrefs.HasKey(FreeItemKey))
		{
			string freeItemsJson = PlayerPrefs.GetString(FreeItemKey);
			ItemListWrapper freeItemsWrapper = JsonUtility.FromJson<ItemListWrapper>(freeItemsJson);
			availablefreeItems = freeItemsWrapper.items;
		}

		// availablebuyItems �ҷ�����
		if (PlayerPrefs.HasKey(BuyItemKey))
		{
			string buyItemsJson = PlayerPrefs.GetString(BuyItemKey);
			ItemListWrapper buyItemsWrapper = JsonUtility.FromJson<ItemListWrapper>(buyItemsJson);
			availablebuyItems = buyItemsWrapper.items;
		}

		if (PlayerPrefs.HasKey(FreeItemKey) || PlayerPrefs.HasKey(BuyItemKey))
		{
			if (availablefreeItems.Count != 0 || availablebuyItems.Count != 0)
			{
				Debug.Log("������ UI ���� �õ� �� �Դϴ�.");
				InstantiateItemUI();
			}
			else
			{
				Debug.LogError($"availablefreeItems : {availablefreeItems.Count}�� �����ϰ� availablebuyItems : {availablebuyItems.Count}�� �����ϹǷ� UI�� ������ �� �����ϴ�.");
			}
		}
	}	  // ���� ���� ���� �� ���� �Ǿ� �ִ� ���� �������� �ҷ��� UI �ݿ�

	private void SettingsItemDisplayCount()
    {
		#region ���� ������

		if (freeitemDatabase != null)
		{
			if (freeitemDatabase.items.Length >= freeItemsToDisplay)
			{
				Debug.Log($"���� �������� ���� ��� ������ {freeItemsToDisplay}�� �Դϴ�.");
			}
			else
			{
				freeItemsToDisplay = freeitemDatabase.items.Length;
				Debug.LogError($"���� ������ ������ ���̽��� ��� �� �������� ���� 'itemToDisplay' ������ �ڵ����� �����Ǿ����ϴ�. \n���� ��� ������ {freeItemsToDisplay}�� �Դϴ�.");
			}
		}

		#endregion

		#region ���� ������

		if (buyitemDatabase != null)
		{
			if (buyitemDatabase.items.Length >= buyitemsToDisplay)
			{
				Debug.Log($"���� �������� ���� ��� ������ {buyitemsToDisplay}�� �Դϴ�.");
			}
			else
			{
				buyitemsToDisplay = buyitemDatabase.items.Length;
				Debug.LogError($"���� ������ ������ ���̽��� ��� �� �������� ���� 'itemToDisplay' ������ �ڵ����� �����Ǿ����ϴ�. \n���� ��� ������ {buyitemsToDisplay}�� �Դϴ�.");
			}
		}
		
		#endregion
	}   // ������ ������ ���̽��� ��ϵǾ� �ִ� �������� �⺻ DisplayCount �������� ���� ��, DisplayCount ������ ������ 

	private void NotifyItemCount()
	{
#if UNITY_EDITOR
		#region ���� ������

		if (freeitemDatabase != null)
		{
			Debug.Log($"{freeitemDatabase}�� �����ϸ�, {freeitemDatabase.items.Length}���� ���� ������ �����ͺ��̽��� �����մϴ�.");
		}
		else
		{
			Debug.LogError($"'freeitemDatabase'�� �������� �ʽ��ϴ�.");
		}

		#endregion

		#region ���� ������

		if (buyitemDatabase != null)
		{
			Debug.Log($"{buyitemDatabase}�� �����ϸ�, {buyitemDatabase.items.Length}���� ���� ������ �����ͺ��̽��� �����մϴ�.");
		}
		else
		{
			Debug.LogError($"'buyitemDatabase'�� �������� �ʽ��ϴ�.");
		}

		#endregion
#endif
	}	  // ������ ������ ���̽��� ���� ������ ���� ���� �ʱ⿡ �˷���

	public void FirstTimeOpenShopUI()
	{
		if (PlayerPrefs.HasKey(firstTimeOpenShopKey))
		{
			Debug.LogError("�̹� ������ �湮�߱� ������ 'FirstTimeOpenShopUI' �ż���� �۵����� �ʽ��ϴ�.");
			return;
		}

		PlayerPrefs.SetString(firstTimeOpenShopKey, "Visit");
		PlayerPrefs.Save();

		#region ���� ������ ���� ����

		if (freeitemDatabase != null)
		{
			List<Item> _ItemsCurrentSelectIndex = new List<Item>();

			int _ItemCurrentRandomIndex = 0;
			int _ItemCurrentIndex = 0;

			bool _ItemCurrentInserted;

			for (int i = 0; i < freeItemsToDisplay; i++)
			{
				GameObject temp_FreeItemObject = Instantiate(itemUIPrefab, itemFreeUIParent);

				TMP_Text temp_FreeItemObject_Title = FindChildRecursive(temp_FreeItemObject.transform, "Item_Title").GetComponent<TMP_Text>();
				Image temp_FreeItemObject_ItemImage = FindChildRecursive(temp_FreeItemObject.transform, "Item_Image").GetComponent<Image>();

				TMP_Text temp_FreeItemObject_Price = FindChildRecursive(temp_FreeItemObject.transform, "Buy_PriceText").GetComponent<TMP_Text>();
				Image temp_FreeItemObject_CurrencyType = FindChildRecursive(temp_FreeItemObject.transform, "Buy_Coin_Image").GetComponent<Image>();

#if UNITY_EDITOR
				#region ������ ����� �ڵ�

				if (temp_FreeItemObject_Title != null)
				{
					Debug.Log("'temp_FreeItemObject_Title'�� ã�ҽ��ϴ�.");
				}
				else
				{
					Debug.Log("'temp_FreeItemObject_Title'�� ã�� �� �����ϴ�.");
				}

				if (temp_FreeItemObject_ItemImage != null)
				{
					Debug.Log("'temp_FreeItemObject_ItemImage'�� ã�ҽ��ϴ�.");
				}
				else
				{
					Debug.Log("'temp_FreeItemObject_ItemImage'�� ã�� �� �����ϴ�.");
				}

				if (temp_FreeItemObject_Price != null)
				{
					Debug.Log("'temp_FreeItemObject_Price'�� ã�ҽ��ϴ�.");
				}
				else
				{
					Debug.Log("'temp_FreeItemObject_Price'�� ã�� �� �����ϴ�.");
				}

				#endregion
#endif

				if (freeitemDatabase.items.Length == freeItemsToDisplay)    // ���� ������ ���̽��� ��ϵǾ� �ִ� �������� ������ 'freeItemsToDisplay'�� ���ٸ�
				{
					temp_FreeItemObject_Title.text = freeitemDatabase.items[_ItemCurrentIndex].itemName;
					temp_FreeItemObject_ItemImage.sprite = freeitemDatabase.items[_ItemCurrentIndex].SkillIcon;
					temp_FreeItemObject_ItemImage.SetNativeSize();

					temp_FreeItemObject_Price.text = freeitemDatabase.items[_ItemCurrentIndex].price.ToString("#,##0");

					switch (freeitemDatabase.items[_ItemCurrentIndex].currencyType)
					{
						case CurrencyType.Coin:
							temp_FreeItemObject_CurrencyType.sprite = coinSprite;
							Debug.Log("'temp_FreeItemObject_CurrencyType'�� ȭ��� �����Դϴ�.");
							break;
						case CurrencyType.Crystal:
							temp_FreeItemObject_CurrencyType.sprite = crystalSprite;
							temp_FreeItemObject_CurrencyType.transform.localScale = new Vector3(.8f, .8f, .8f);
							Debug.Log("'temp_FreeItemObject_CurrencyType'�� ȭ��� ũ����Ż�Դϴ�.");
							break;
					}

					temp_FreeItemObject_CurrencyType.SetNativeSize();

					availablefreeItems.Add(freeitemDatabase.items[_ItemCurrentIndex]);

					_ItemCurrentIndex++;
				}
				else
				{
					while (availablebuyItems.Count < buyitemsToDisplay)	// ���� �� �������� RandomIndex�� �����ϰ�, ���������� �����Ѵ�.
					{
						_ItemCurrentRandomIndex = Random.Range(0, freeitemDatabase.items.Length);
						_ItemCurrentInserted = false;
						
						if (_ItemsCurrentSelectIndex != null)
						{
							foreach (Item availablefreeItem in availablefreeItems)
							{
								if (availablefreeItem.itemName == freeitemDatabase.items[_ItemCurrentRandomIndex].itemName)
								{
									Debug.Log($"�̹� ���� �Ǿ� �ִ� ���� �������� �־� {freeitemDatabase.items[_ItemCurrentRandomIndex].itemName}��(��) ��ϵ��� �ʽ��ϴ�.");
									_ItemCurrentInserted = true;
									break;
								}
							}
						}

						if (_ItemCurrentInserted)
						{
							Debug.Log($"������ ���� ���� : {_ItemCurrentInserted}�̹Ƿ� �ݺ����� �Ѱ���ϴ�.");
							continue;
						}

						Debug.Log($"������ ���� ���� : {_ItemCurrentInserted}�̹Ƿ� �ݺ��� �ѱ��� �ʾҽ��ϴ�.");

						availablefreeItems.Add(freeitemDatabase.items[_ItemCurrentRandomIndex]);
						break;
					}

					temp_FreeItemObject_Title.text = freeitemDatabase.items[_ItemCurrentRandomIndex].itemName;
					temp_FreeItemObject_ItemImage.sprite = freeitemDatabase.items[_ItemCurrentRandomIndex].SkillIcon;
					temp_FreeItemObject_ItemImage.SetNativeSize();

					temp_FreeItemObject_Price.text = freeitemDatabase.items[_ItemCurrentRandomIndex].price.ToString("#,##0");

					switch (freeitemDatabase.items[_ItemCurrentRandomIndex].currencyType)
					{
						case CurrencyType.Coin:
							temp_FreeItemObject_CurrencyType.sprite = coinSprite;
							Debug.Log("'temp_FreeItemObject_CurrencyType'�� ȭ��� �����Դϴ�.");
							break;
						case CurrencyType.Crystal:
							temp_FreeItemObject_CurrencyType.sprite = crystalSprite;
							temp_FreeItemObject_CurrencyType.transform.localScale = new Vector3(.8f, .8f, .8f);
							Debug.Log("'temp_FreeItemObject_CurrencyType'�� ȭ��� ũ����Ż�Դϴ�.");
							break;
					}

					temp_FreeItemObject_CurrencyType.SetNativeSize();

					_ItemsCurrentSelectIndex.Add(freeitemDatabase.items[_ItemCurrentRandomIndex]);
				}
			}
		}

		#endregion

		#region ���� ������ ���� ����

		if (buyitemDatabase != null)
		{
			List<Item> _ItemsCurrentSelectIndex = new List<Item>();

			int _ItemCurrentRandomIndex = 0;
			int _ItemCurrentIndex = 0;

			bool _ItemCurrentInserted;

			for (int i = 0; i < buyitemsToDisplay; i++)
			{
				GameObject temp_BuyItemObject = Instantiate(itemUIPrefab, itemBuyUIParent);

				TMP_Text temp_BuyItemObject_Title = FindChildRecursive(temp_BuyItemObject.transform, "Item_Title").GetComponent<TMP_Text>();
				Image temp_BuyItemObject_ItemImage = FindChildRecursive(temp_BuyItemObject.transform, "Item_Image").GetComponent<Image>();

				TMP_Text temp_BuyItemObject_Price = FindChildRecursive(temp_BuyItemObject.transform, "Buy_PriceText").GetComponent<TMP_Text>();
				Image temp_BuyItemObject_CurrencyType = FindChildRecursive(temp_BuyItemObject.transform, "Buy_Coin_Image").GetComponent<Image>();

#if UNITY_EDITOR
				#region ������ ����� �ڵ�

				if (temp_BuyItemObject_Title != null)
				{
					Debug.Log("'temp_BuyItemObject_Title'�� ã�ҽ��ϴ�.");
				}
				else
				{
					Debug.Log("'temp_BuyItemObject_Title'�� ã�� �� �����ϴ�.");
				}

				if (temp_BuyItemObject_ItemImage != null)
				{
					Debug.Log("'temp_BuyItemObject_ItemImage'�� ã�ҽ��ϴ�.");
				}
				else
				{
					Debug.Log("'temp_BuyItemObject_ItemImage'�� ã�� �� �����ϴ�.");
				}

				if (temp_BuyItemObject_Price != null)
				{
					Debug.Log("'temp_BuyItemObject_Price'�� ã�ҽ��ϴ�.");
				}
				else
				{
					Debug.Log("'temp_BuyItemObject_Price'�� ã�� �� �����ϴ�.");
				}

				#endregion
#endif

				if (buyitemDatabase.items.Length == buyitemsToDisplay)    // ���� ������ ���̽��� ��ϵǾ� �ִ� �������� ������ 'buyitemsToDisplay'�� ���ٸ�
				{
					temp_BuyItemObject_Title.text = buyitemDatabase.items[_ItemCurrentIndex].itemName;
					temp_BuyItemObject_ItemImage.sprite = buyitemDatabase.items[_ItemCurrentIndex].SkillIcon;
					temp_BuyItemObject_ItemImage.SetNativeSize();

					temp_BuyItemObject_Price.text = buyitemDatabase.items[_ItemCurrentIndex].price.ToString("#,##0");

					switch (buyitemDatabase.items[_ItemCurrentIndex].currencyType)
					{
						case CurrencyType.Coin:
							temp_BuyItemObject_CurrencyType.sprite = coinSprite;
							Debug.Log("'temp_BuyItemObject_CurrencyType'�� ȭ��� �����Դϴ�.");
							break;
						case CurrencyType.Crystal:
							temp_BuyItemObject_CurrencyType.sprite = crystalSprite;
							temp_BuyItemObject_CurrencyType.transform.localScale = new Vector3(.8f, .8f, .8f);
							Debug.Log("'temp_BuyItemObject_CurrencyType'�� ȭ��� ũ����Ż�Դϴ�.");
							break;
					}

					temp_BuyItemObject_CurrencyType.SetNativeSize();

					availablebuyItems.Add(buyitemDatabase.items[_ItemCurrentIndex]);

					_ItemCurrentIndex++;
				}
				else  // ���� ������ ���̽��� ��ϵǾ� �ִ� �������� ������ 'buyItemsToDisplay'���� ũ�ٸ�
				{
					while (availablebuyItems.Count < buyitemsToDisplay)	// ���� �� �������� RandomIndex�� �����ϰ�, ���������� �����Ѵ�.
					{
						_ItemCurrentRandomIndex = Random.Range(0, buyitemDatabase.items.Length);
						_ItemCurrentInserted = false;
						
						if (_ItemsCurrentSelectIndex != null)
						{
							foreach (Item availablebuyItem in availablebuyItems)
							{
								if (availablebuyItem.itemName == buyitemDatabase.items[_ItemCurrentRandomIndex].itemName)
								{
									Debug.Log($"�̹� ���� �Ǿ� �ִ� ���� �������� �־� {buyitemDatabase.items[_ItemCurrentRandomIndex].itemName}��(��) ��ϵ��� �ʽ��ϴ�.");
									_ItemCurrentInserted = true;
									break;
								}
							}
						}

						if (_ItemCurrentInserted)
						{
							Debug.Log($"������ ���� ���� : {_ItemCurrentInserted}�̹Ƿ� �ݺ����� �Ѱ���ϴ�.");
							continue;
						}

						Debug.Log($"������ ���� ���� : {_ItemCurrentInserted}�̹Ƿ� �ݺ��� �ѱ��� �ʾҽ��ϴ�.");

						availablebuyItems.Add(buyitemDatabase.items[_ItemCurrentRandomIndex]);
						break;
					}

					temp_BuyItemObject_Title.text = buyitemDatabase.items[_ItemCurrentRandomIndex].itemName;
					Debug.Log(temp_BuyItemObject_Title.text);
					temp_BuyItemObject_ItemImage.sprite = buyitemDatabase.items[_ItemCurrentRandomIndex].SkillIcon;
					temp_BuyItemObject_ItemImage.SetNativeSize();

					temp_BuyItemObject_Price.text = buyitemDatabase.items[_ItemCurrentRandomIndex].price.ToString("#,##0");

					switch (buyitemDatabase.items[_ItemCurrentRandomIndex].currencyType)
					{
						case CurrencyType.Coin:
							temp_BuyItemObject_CurrencyType.sprite = coinSprite;
							Debug.Log("'temp_BuyItemObject_CurrencyType'�� ȭ��� �����Դϴ�.");
							break;
						case CurrencyType.Crystal:
							temp_BuyItemObject_CurrencyType.sprite = crystalSprite;
							temp_BuyItemObject_CurrencyType.transform.localScale = new Vector3(.8f, .8f, .8f);
							Debug.Log("'temp_BuyItemObject_CurrencyType'�� ȭ��� ũ����Ż�Դϴ�.");
							break;
					}

					temp_BuyItemObject_CurrencyType.SetNativeSize();

					_ItemsCurrentSelectIndex.Add(buyitemDatabase.items[_ItemCurrentRandomIndex]);
				}
			}
		}

		#endregion

	}	 // ���� ��ġ �� ���� ���� ���� �� ����Ǵ� �ڵ�

	public void InitializationItems()	// ���� �ð��� ���� ��, �ʱ�ȭ �� ���� ������ ���� ���
	{
		if (CountdownTimer.Instance.remainingTime.TotalSeconds > 0)
		{
			Debug.Log($"�ʱ�ȭ���� {CountdownTimer.Instance.remainingTime.TotalSeconds.ToString("F2")}�ʰ� �����ֽ��ϴ�.");
			return;
		}

		if (!PlayerPrefs.HasKey(firstTimeOpenShopKey))
		{
			Debug.LogError("������ ó�� �����߽��ϴ�. 'InitializationItems' �ż���� ������� �ʽ��ϴ�.");
			return;
		}

		if (shopPanel.activeInHierarchy)
		{
			Debug.LogError($"{CountdownTimer.Instance.remainingTime.TotalSeconds.ToString("F0")}�� ������ ������ ������ �ʾ� �ʱ�ȭ�� �̷����ϴ�.");
			return;
		}

		CountdownTimer.Instance.endTime = DateTime.Now.AddHours(24);
		PlayerPrefs.SetString("ShopEndTime", CountdownTimer.Instance.endTime.ToString());
		PlayerPrefs.Save();
		Debug.Log($"'CountdownTimer'�� endTime�� ���ΰ�ħ �Ǿ����ϴ�.");

		#region ���� ������

		foreach (Transform childObjectUI in itemFreeUIParent)	// Ȥ���� ������ ��� �� ������ UI�� �����Ѵٸ� ��ȸ�ϸ� �����Ѵ�.
		{
			TMP_Text itemTitle = FindChildRecursive(childObjectUI.transform, "Item_Title").GetComponent<TMP_Text>();

			if (itemTitle != null)
			{
				Item itemToRemove = availablefreeItems.FirstOrDefault(item => item.itemName == itemTitle.text);

				if (itemToRemove != null)
				{
					availablefreeItems.Remove(itemToRemove);
				}
			}

			Destroy(childObjectUI);
		}

		#region ���� ������ ���� ����

		if (freeitemDatabase != null)
		{
			List<Item> _ItemsCurrentSelectIndex = new List<Item>();

			int _ItemCurrentRandomIndex = 0;
			int _ItemCurrentIndex = 0;

			bool _ItemCurrentInserted;

			for (int i = 0; i < freeItemsToDisplay; i++)
			{
				GameObject temp_FreeItemObject = Instantiate(itemUIPrefab, itemFreeUIParent);

				TMP_Text temp_FreeItemObject_Title = FindChildRecursive(temp_FreeItemObject.transform, "Item_Title").GetComponent<TMP_Text>();
				Image temp_FreeItemObject_ItemImage = FindChildRecursive(temp_FreeItemObject.transform, "Item_Image").GetComponent<Image>();

				TMP_Text temp_FreeItemObject_Price = FindChildRecursive(temp_FreeItemObject.transform, "Buy_PriceText").GetComponent<TMP_Text>();
				Image temp_FreeItemObject_CurrencyType = FindChildRecursive(temp_FreeItemObject.transform, "Buy_Coin_Image").GetComponent<Image>();

#if UNITY_EDITOR
				#region ������ ����� �ڵ�

				if (temp_FreeItemObject_Title != null)
				{
					Debug.Log("'temp_FreeItemObject_Title'�� ã�ҽ��ϴ�.");
				}
				else
				{
					Debug.Log("'temp_FreeItemObject_Title'�� ã�� �� �����ϴ�.");
				}

				if (temp_FreeItemObject_ItemImage != null)
				{
					Debug.Log("'temp_FreeItemObject_ItemImage'�� ã�ҽ��ϴ�.");
				}
				else
				{
					Debug.Log("'temp_FreeItemObject_ItemImage'�� ã�� �� �����ϴ�.");
				}

				if (temp_FreeItemObject_Price != null)
				{
					Debug.Log("'temp_FreeItemObject_Price'�� ã�ҽ��ϴ�.");
				}
				else
				{
					Debug.Log("'temp_FreeItemObject_Price'�� ã�� �� �����ϴ�.");
				}

				#endregion
#endif

				if (freeitemDatabase.items.Length == freeItemsToDisplay)    // ���� ������ ���̽��� ��ϵǾ� �ִ� �������� ������ 'freeItemsToDisplay'�� ���ٸ�
				{
					temp_FreeItemObject_Title.text = freeitemDatabase.items[_ItemCurrentIndex].itemName;
					temp_FreeItemObject_ItemImage.sprite = freeitemDatabase.items[_ItemCurrentIndex].SkillIcon;
					temp_FreeItemObject_ItemImage.SetNativeSize();

					temp_FreeItemObject_Price.text = freeitemDatabase.items[_ItemCurrentIndex].price.ToString("#,##0");

					switch (freeitemDatabase.items[_ItemCurrentIndex].currencyType)
					{
						case CurrencyType.Coin:
							temp_FreeItemObject_CurrencyType.sprite = coinSprite;
							Debug.Log("'temp_FreeItemObject_CurrencyType'�� ȭ��� �����Դϴ�.");
							break;
						case CurrencyType.Crystal:
							temp_FreeItemObject_CurrencyType.sprite = crystalSprite;
							temp_FreeItemObject_CurrencyType.transform.localScale = new Vector3(.8f, .8f, .8f);
							Debug.Log("'temp_FreeItemObject_CurrencyType'�� ȭ��� ũ����Ż�Դϴ�.");
							break;
					}

					temp_FreeItemObject_CurrencyType.SetNativeSize();

					availablefreeItems.Add(freeitemDatabase.items[_ItemCurrentIndex]);

					_ItemCurrentIndex++;
				}
				else
				{
					while (availablefreeItems.Count < freeItemsToDisplay) // ���� �� �������� RandomIndex�� �����ϰ�, ���������� �����Ѵ�.
					{
						_ItemCurrentRandomIndex = Random.Range(0, freeitemDatabase.items.Length);
						_ItemCurrentInserted = false;

						if (_ItemsCurrentSelectIndex != null)
						{
							foreach (Item availablefreeItem in availablefreeItems)
							{
								if (availablefreeItem.itemName == freeitemDatabase.items[_ItemCurrentRandomIndex].itemName)
								{
									Debug.Log($"�̹� ���� �Ǿ� �ִ� ���� �������� �־� {freeitemDatabase.items[_ItemCurrentRandomIndex].itemName}��(��) ��ϵ��� �ʽ��ϴ�.");
									_ItemCurrentInserted = true;
									break;
								}
							}
						}

						if (_ItemCurrentInserted)
						{
							Debug.Log($"������ ���� ���� : {_ItemCurrentInserted}�̹Ƿ� �ݺ����� �Ѱ���ϴ�.");
							continue;
						}

						Debug.Log($"������ ���� ���� : {_ItemCurrentInserted}�̹Ƿ� �ݺ��� �ѱ��� �ʾҽ��ϴ�.");

						availablefreeItems.Add(freeitemDatabase.items[_ItemCurrentRandomIndex]);
						break;
					}

					temp_FreeItemObject_Title.text = freeitemDatabase.items[_ItemCurrentRandomIndex].itemName;
					temp_FreeItemObject_ItemImage.sprite = freeitemDatabase.items[_ItemCurrentRandomIndex].SkillIcon;
					temp_FreeItemObject_ItemImage.SetNativeSize();

					temp_FreeItemObject_Price.text = freeitemDatabase.items[_ItemCurrentRandomIndex].price.ToString("#,##0");

					switch (freeitemDatabase.items[_ItemCurrentRandomIndex].currencyType)
					{
						case CurrencyType.Coin:
							temp_FreeItemObject_CurrencyType.sprite = coinSprite;
							Debug.Log("'temp_FreeItemObject_CurrencyType'�� ȭ��� �����Դϴ�.");
							break;
						case CurrencyType.Crystal:
							temp_FreeItemObject_CurrencyType.sprite = crystalSprite;
							temp_FreeItemObject_CurrencyType.transform.localScale = new Vector3(.8f, .8f, .8f);
							Debug.Log("'temp_FreeItemObject_CurrencyType'�� ȭ��� ũ����Ż�Դϴ�.");
							break;
					}

					temp_FreeItemObject_CurrencyType.SetNativeSize();

					_ItemsCurrentSelectIndex.Add(freeitemDatabase.items[_ItemCurrentRandomIndex]);
				}
			}
		}

		#endregion

		#endregion

		#region ���� ������

		foreach (Transform childObjectUI in itemBuyUIParent)   // Ȥ���� ������ ��� �� ������ UI�� �����Ѵٸ� ��ȸ�ϸ� �����Ѵ�.
		{
			TMP_Text itemTitle = FindChildRecursive(childObjectUI.transform, "Item_Title").GetComponent<TMP_Text>();

			if (itemTitle != null)
			{
				Item itemToRemove = availablebuyItems.FirstOrDefault(item => item.itemName == itemTitle.text);

				if (itemToRemove != null)
				{
					availablebuyItems.Remove(itemToRemove);
				}
			}

			Destroy(childObjectUI);
		}

		#region ���� ������ ���� ����

		if (buyitemDatabase != null)
		{
			List<Item> _ItemsCurrentSelectIndex = new List<Item>();

			int _ItemCurrentRandomIndex = 0;
			int _ItemCurrentIndex = 0;

			bool _ItemCurrentInserted;

			for (int i = 0; i < buyitemsToDisplay; i++)
			{
				GameObject temp_BuyItemObject = Instantiate(itemUIPrefab, itemBuyUIParent);

				TMP_Text temp_BuyItemObject_Title = FindChildRecursive(temp_BuyItemObject.transform, "Item_Title").GetComponent<TMP_Text>();
				Image temp_BuyItemObject_ItemImage = FindChildRecursive(temp_BuyItemObject.transform, "Item_Image").GetComponent<Image>();

				TMP_Text temp_BuyItemObject_Price = FindChildRecursive(temp_BuyItemObject.transform, "Buy_PriceText").GetComponent<TMP_Text>();
				Image temp_BuyItemObject_CurrencyType = FindChildRecursive(temp_BuyItemObject.transform, "Buy_Coin_Image").GetComponent<Image>();

#if UNITY_EDITOR
				#region ������ ����� �ڵ�

				if (temp_BuyItemObject_Title != null)
				{
					Debug.Log("'temp_BuyItemObject_Title'�� ã�ҽ��ϴ�.");
				}
				else
				{
					Debug.Log("'temp_BuyItemObject_Title'�� ã�� �� �����ϴ�.");
				}

				if (temp_BuyItemObject_ItemImage != null)
				{
					Debug.Log("'temp_BuyItemObject_ItemImage'�� ã�ҽ��ϴ�.");
				}
				else
				{
					Debug.Log("'temp_BuyItemObject_ItemImage'�� ã�� �� �����ϴ�.");
				}

				if (temp_BuyItemObject_Price != null)
				{
					Debug.Log("'temp_BuyItemObject_Price'�� ã�ҽ��ϴ�.");
				}
				else
				{
					Debug.Log("'temp_BuyItemObject_Price'�� ã�� �� �����ϴ�.");
				}

				#endregion
#endif

				if (buyitemDatabase.items.Length == buyitemsToDisplay)    // ���� ������ ���̽��� ��ϵǾ� �ִ� �������� ������ 'freeItemsToDisplay'�� ���ٸ�
				{
					temp_BuyItemObject_Title.text = buyitemDatabase.items[_ItemCurrentIndex].itemName;
					temp_BuyItemObject_ItemImage.sprite = buyitemDatabase.items[_ItemCurrentIndex].SkillIcon;
					temp_BuyItemObject_ItemImage.SetNativeSize();

					temp_BuyItemObject_Price.text = buyitemDatabase.items[_ItemCurrentIndex].price.ToString("#,##0");

					switch (buyitemDatabase.items[_ItemCurrentIndex].currencyType)
					{
						case CurrencyType.Coin:
							temp_BuyItemObject_CurrencyType.sprite = coinSprite;
							Debug.Log("'temp_BuyItemObject_CurrencyType'�� ȭ��� �����Դϴ�.");
							break;
						case CurrencyType.Crystal:
							temp_BuyItemObject_CurrencyType.sprite = crystalSprite;
							temp_BuyItemObject_CurrencyType.transform.localScale = new Vector3(.8f, .8f, .8f);
							Debug.Log("'temp_BuyItemObject_CurrencyType'�� ȭ��� ũ����Ż�Դϴ�.");
							break;
					}

					temp_BuyItemObject_CurrencyType.SetNativeSize();

					availablebuyItems.Add(buyitemDatabase.items[_ItemCurrentIndex]);

					_ItemCurrentIndex++;
				}
				else  // ���� ������ ���̽��� ��ϵǾ� �ִ� �������� ������ 'buyItemsToDisplay'���� ũ�ٸ�
				{
					while (availablebuyItems.Count < buyitemsToDisplay) // ���� �� �������� RandomIndex�� �����ϰ�, ���������� �����Ѵ�.
					{
						_ItemCurrentRandomIndex = Random.Range(0, buyitemDatabase.items.Length);
						_ItemCurrentInserted = false;

						if (_ItemsCurrentSelectIndex != null)
						{
							foreach (Item availablebuyItem in availablebuyItems)
							{
								if (availablebuyItem.itemName == buyitemDatabase.items[_ItemCurrentRandomIndex].itemName)
								{
									Debug.Log($"�̹� ���� �Ǿ� �ִ� ���� �������� �־� {buyitemDatabase.items[_ItemCurrentRandomIndex].itemName}��(��) ��ϵ��� �ʽ��ϴ�.");
									_ItemCurrentInserted = true;
									break;
								}
							}
						}

						if (_ItemCurrentInserted)
						{
							Debug.Log($"������ ���� ���� : {_ItemCurrentInserted}�̹Ƿ� �ݺ����� �Ѱ���ϴ�.");
							continue;
						}

						Debug.Log($"������ ���� ���� : {_ItemCurrentInserted}�̹Ƿ� �ݺ��� �ѱ��� �ʾҽ��ϴ�.");

						availablebuyItems.Add(buyitemDatabase.items[_ItemCurrentRandomIndex]);
						break;
					}

					temp_BuyItemObject_Title.text = buyitemDatabase.items[_ItemCurrentRandomIndex].itemName;
					Debug.Log(temp_BuyItemObject_Title.text);
					temp_BuyItemObject_ItemImage.sprite = buyitemDatabase.items[_ItemCurrentRandomIndex].SkillIcon;
					temp_BuyItemObject_ItemImage.SetNativeSize();

					temp_BuyItemObject_Price.text = buyitemDatabase.items[_ItemCurrentRandomIndex].price.ToString("#,##0");

					switch (buyitemDatabase.items[_ItemCurrentRandomIndex].currencyType)
					{
						case CurrencyType.Coin:
							temp_BuyItemObject_CurrencyType.sprite = coinSprite;
							Debug.Log("'temp_BuyItemObject_CurrencyType'�� ȭ��� �����Դϴ�.");
							break;
						case CurrencyType.Crystal:
							temp_BuyItemObject_CurrencyType.sprite = crystalSprite;
							temp_BuyItemObject_CurrencyType.transform.localScale = new Vector3(.8f, .8f, .8f);
							Debug.Log("'temp_BuyItemObject_CurrencyType'�� ȭ��� ũ����Ż�Դϴ�.");
							break;
					}

					temp_BuyItemObject_CurrencyType.SetNativeSize();

					_ItemsCurrentSelectIndex.Add(buyitemDatabase.items[_ItemCurrentRandomIndex]);
				}
			}
		}

		#endregion

		#endregion
	}

	private Transform FindChildRecursive(Transform parent, string childName)
	{
		foreach (Transform childObject in parent)
		{
			if (childObject.name == childName)
			{
				return childObject;
			}

			Transform foundChildObject = FindChildRecursive(childObject, childName);

			if (foundChildObject != null)
			{
				return foundChildObject;
			}
		}

		return null;
	}	// �ڽ� ��ü ���� �ڽ� ��ü�� ã�� ���� �ż���

	private void OnApplicationQuit()
	{
		SaveItemData();
	}	// ������ ������ �� ����Ǵ� �ż���

	private void SaveItemData()
	{
		string freeItemJson = JsonUtility.ToJson(new ItemListWrapper(availablefreeItems));
		Debug.Log(freeItemJson);
		PlayerPrefs.SetString(FreeItemKey, freeItemJson);

		string buyItemJson = JsonUtility.ToJson(new ItemListWrapper(availablebuyItems));
		Debug.Log(buyItemJson);
		PlayerPrefs.SetString(BuyItemKey, buyItemJson);

		// �׽�Ʈ �ڵ�
		//PlayerPrefs.DeleteAll();
		PlayerPrefs.Save();
	}   // availableItems�� Json(String)���� ��ȯ�ϰ� ��ȯ �� String�� PlayerPrefs�� �����Ѵ�.

	[Serializable]
	private class ItemListWrapper	// Json���� ��ȯ ��, ������ ����Ʈ�� ����ȭ�ϱ� ���� ��� �� ���� Ŭ�����̴�.
	{
		public List<Item> items;

		public ItemListWrapper(List<Item> items)
		{
			this.items = items;
		}
	}

	public void InstantiateItemUI()	// ������ ����� �Ǿ��ٸ�, �� �ż��带 ������ ���� �� �� �ִ� ���� UI�� �����Ѵ�.
	{
		#region ���� ������

		foreach (Item avilableItem in availablefreeItems)	// ��� �� �������� ��ȸ�ϸ鼭,
		{
			foreach (Item dataBaseItem in freeitemDatabase.items)	// ������ ���̽��� ��� �� �������� ��ȸ�մϴ�.
			{
				if (dataBaseItem.itemName == avilableItem.itemName)	// ��� �� �������� ������ ���̽��� �����Ѵٸ�
				{
					Debug.Log("������ ���̽����� ���� ���� ���� ���� �� �������� ã�ҽ��ϴ�.");

					GameObject temp_FreeItemObject = Instantiate(itemUIPrefab, itemFreeUIParent);

					TMP_Text temp_FreeItemObject_Title = FindChildRecursive(temp_FreeItemObject.transform, "Item_Title").GetComponent<TMP_Text>();
					Image temp_FreeItemObject_ItemImage = FindChildRecursive(temp_FreeItemObject.transform, "Item_Image").GetComponent<Image>();

					TMP_Text temp_FreeItemObject_Price = FindChildRecursive(temp_FreeItemObject.transform, "Buy_PriceText").GetComponent<TMP_Text>();
					Image temp_FreeItemObject_CurrencyType = FindChildRecursive(temp_FreeItemObject.transform, "Buy_Coin_Image").GetComponent<Image>();

#if UNITY_EDITOR
					#region ������ ����� �ڵ�

					if (temp_FreeItemObject_Title != null)
					{
						Debug.Log("'temp_FreeItemObject_Title'�� ã�ҽ��ϴ�.");
					}
					else
					{
						Debug.Log("'temp_FreeItemObject_Title'�� ã�� �� �����ϴ�.");
					}

					if (temp_FreeItemObject_ItemImage != null)
					{
						Debug.Log("'temp_FreeItemObject_ItemImage'�� ã�ҽ��ϴ�.");
					}
					else
					{
						Debug.Log("'temp_FreeItemObject_ItemImage'�� ã�� �� �����ϴ�.");
					}

					if (temp_FreeItemObject_Price != null)
					{
						Debug.Log("'temp_FreeItemObject_Price'�� ã�ҽ��ϴ�.");
					}
					else
					{
						Debug.Log("'temp_FreeItemObject_Price'�� ã�� �� �����ϴ�.");
					}

					#endregion
#endif

					temp_FreeItemObject_Title.text = avilableItem.itemName;
					temp_FreeItemObject_ItemImage.sprite = dataBaseItem.SkillIcon;
					temp_FreeItemObject_ItemImage.SetNativeSize();

					temp_FreeItemObject_Price.text = dataBaseItem.price.ToString("#,##0");

					switch (avilableItem.currencyType)
					{
						case CurrencyType.Coin:
							temp_FreeItemObject_CurrencyType.sprite = coinSprite;
							Debug.Log("'temp_FreeItemObject_CurrencyType'�� ȭ��� �����Դϴ�.");
							break;
						case CurrencyType.Crystal:
							temp_FreeItemObject_CurrencyType.sprite = crystalSprite;
							temp_FreeItemObject_CurrencyType.transform.localScale = new Vector3(.8f, .8f, .8f);
							Debug.Log("'temp_FreeItemObject_CurrencyType'�� ȭ��� ũ����Ż�Դϴ�.");
							break;
					}

					temp_FreeItemObject_CurrencyType.SetNativeSize();
				}
			}

		}

		#endregion

		#region ���� ������

		foreach (Item avilableItem in availablebuyItems)   // ��� �� �������� ��ȸ�ϸ鼭,
		{
			foreach (Item dataBaseItem in buyitemDatabase.items)   // ������ ���̽��� ��� �� �������� ��ȸ�մϴ�.
			{
				if (dataBaseItem.itemName == avilableItem.itemName) // ��� �� �������� ������ ���̽��� �����Ѵٸ�
				{
					Debug.Log("������ ���̽����� ���� ���� ���� ���� �� �������� ã�ҽ��ϴ�.");

					GameObject temp_BuyItemObject = Instantiate(itemUIPrefab, itemBuyUIParent);

					TMP_Text temp_BuyItemObject_Title = FindChildRecursive(temp_BuyItemObject.transform, "Item_Title").GetComponent<TMP_Text>();
					Image temp_BuyItemObject_ItemImage = FindChildRecursive(temp_BuyItemObject.transform, "Item_Image").GetComponent<Image>();

					TMP_Text temp_BuyItemObject_Price = FindChildRecursive(temp_BuyItemObject.transform, "Buy_PriceText").GetComponent<TMP_Text>();
					Image temp_BuyItemObject_CurrencyType = FindChildRecursive(temp_BuyItemObject.transform, "Buy_Coin_Image").GetComponent<Image>();

#if UNITY_EDITOR
					#region ������ ����� �ڵ�

					if (temp_BuyItemObject_Title != null)
					{
						Debug.Log("'temp_BuyItemObject_Title'�� ã�ҽ��ϴ�.");
					}
					else
					{
						Debug.Log("'temp_BuyItemObject_Title'�� ã�� �� �����ϴ�.");
					}

					if (temp_BuyItemObject_ItemImage != null)
					{
						Debug.Log("'temp_BuyItemObject_ItemImage'�� ã�ҽ��ϴ�.");
					}
					else
					{
						Debug.Log("'temp_BuyItemObject_ItemImage'�� ã�� �� �����ϴ�.");
					}

					if (temp_BuyItemObject_Price != null)
					{
						Debug.Log("'temp_BuyItemObject_Price'�� ã�ҽ��ϴ�.");
					}
					else
					{
						Debug.Log("'temp_BuyItemObject_Price'�� ã�� �� �����ϴ�.");
					}

					#endregion
#endif

					temp_BuyItemObject_Title.text = avilableItem.itemName;
					temp_BuyItemObject_ItemImage.sprite = dataBaseItem.SkillIcon;
					temp_BuyItemObject_ItemImage.SetNativeSize();
					Debug.Log("���� ����");
					temp_BuyItemObject_Price.text = dataBaseItem.price.ToString("#,##0");

					switch (avilableItem.currencyType)
					{
						case CurrencyType.Coin:
							temp_BuyItemObject_CurrencyType.sprite = coinSprite;
							Debug.Log("'temp_BuyItemObject_CurrencyType'�� ȭ��� �����Դϴ�.");
							break;
						case CurrencyType.Crystal:
							temp_BuyItemObject_CurrencyType.sprite = crystalSprite;
							temp_BuyItemObject_CurrencyType.transform.localScale = new Vector3(.8f, .8f, .8f);
							Debug.Log("'temp_BuyItemObject_CurrencyType'�� ȭ��� ũ����Ż�Դϴ�.");
							break;
					}

					temp_BuyItemObject_CurrencyType.SetNativeSize();
				}
			}

		}

		#endregion
	}
}
