using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PhotoEditor : MonoBehaviour
{
	[SerializeField] private GameObject EditorTool;      // ���� ������ ������ UI ��ü�� �����մϴ�.
	[SerializeField] private RawImage photoDisplay;       // ���õ� ������ ǥ���� UI RawImage�� �����մϴ�.
	[SerializeField] private RectTransform cropArea;      // �ڸ��� ������ �����ϴ� UI RectTransform�� �����մϴ�.
	[SerializeField] private Button confirmButton;        // Ȯ�� ��ư�� �����մϴ�.
	[SerializeField] private Button cancelButton;

	private Texture2D selectedPhoto;                      // ���õ� ������ ������ Texture2D ��ü�Դϴ�.

	[SerializeField] private Image previewProfileImage;   // ������ ������ �̸����⸦ ǥ���� Image�� �����մϴ�.
	[SerializeField] private Image profileImage;          // ���� ������ ������ ������ Image�� �����մϴ�.

	private Vector2 lastMousePosition;                    // ������ ���콺 ��ġ�� �����ϴ� �����Դϴ�.
	[SerializeField] private float minSize = 100f;        // �ڸ��� ������ �ּ� ũ���Դϴ�.
	[SerializeField] private float maxSize = 500f;        // �ڸ��� ������ �ִ� ũ���Դϴ�.

	[SerializeField] private TMP_Text unityWebRequestText;
	[SerializeField] private Sprite networkError;
	[SerializeField] private Sprite protocol404Error;
	[SerializeField] private Sprite protocol500Error;
	[SerializeField] private Sprite protocol403Error;
	[SerializeField] private Sprite protocolDefaultError;

	private void Start()
	{
		// Ȯ�� ��ư�� CropPhoto �޼��带 �����ʷ� ����մϴ�.
		confirmButton.onClick.AddListener(CropPhoto);
		cancelButton.onClick.AddListener(CancelCropPhoto);
	}

	public void OpenEditor(string imagePath)
	{
		// ���� ���� UI�� Ȱ��ȭ�մϴ�.
		EditorTool.SetActive(true);

		// �̹��� ��θ� ����Ͽ� �̹����� �񵿱������� �ε��մϴ�.
		StartCoroutine(LoadImage(imagePath));
	}

	private IEnumerator LoadImage(string imagePath)
	{
		// �÷����� ���� �̹��� ��θ� �����մϴ�.
		string filePath = "";


		if (imagePath.StartsWith("https://", System.StringComparison.OrdinalIgnoreCase))
		{
			filePath = imagePath;
		}
		else
		{
#if UNITY_ANDROID

			// �ȵ���̵忡���� 'file://' ���������� ����մϴ�.
			filePath = "file://" + imagePath;
#else

                        // �ٸ� �÷��������� 'file:///' ���������� ����մϴ�.
                        filePath = "file:///" + imagePath;
#endif
		}

		// �̹��� �ε带 ���� UnityWebRequest�� �����մϴ�.
		UnityWebRequest www = UnityWebRequestTexture.GetTexture(filePath);
		// �� ��û�� �񵿱������� �����մϴ�.
		yield return www.SendWebRequest();

		if (www.result == UnityWebRequest.Result.Success)
		{
			// ��û�� �����ϸ�, ���信�� Texture2D ��ü�� �����ɴϴ�.
			selectedPhoto = DownloadHandlerTexture.GetContent(www);
			// �ε�� ������ UI RawImage�� ǥ���մϴ�.
			photoDisplay.texture = selectedPhoto;
			unityWebRequestText.text = "The image was successfully loaded";
		}
		else if (www.result == UnityWebRequest.Result.InProgress)
		{
			unityWebRequestText.text = "Trying to load an image";
		}
		else if (www.result == UnityWebRequest.Result.ConnectionError)
		{
			unityWebRequestText.text = "Network connection error";
			photoDisplay.texture = textureFromSprite(networkError);
		}
		else if (www.result == UnityWebRequest.Result.ProtocolError)
		{
			long responseCode = www.responseCode;

			switch (responseCode)
			{
				case 404:
					unityWebRequestText.text = "���� 404\n��û�� ���ҽ��� ã�� �� �����ϴ�.";
					photoDisplay.texture = textureFromSprite(protocol404Error);
					break;
				case 500:
					unityWebRequestText.text = "���� 500\n���� ���� ����.";
					photoDisplay.texture = textureFromSprite(protocol500Error);
					break;
				case 403:
					unityWebRequestText.text = "���� 403\n������ �����Ǿ����ϴ�.";
					photoDisplay.texture = textureFromSprite(protocol403Error);
					break;
				default:
					unityWebRequestText.text = $"�������� ����\nHTTP {responseCode}";
					photoDisplay.texture = textureFromSprite(protocolDefaultError);
					break;
			}
		}
		else
		{
			unityWebRequestText.text = $"Unknown Error\n" + www.error;
			photoDisplay.texture = textureFromSprite(protocolDefaultError);
		}
	}

	private Texture2D textureFromSprite(Sprite sprite)
	{
		if (sprite == null)
		{
			Debug.LogError("������ �´� ��������Ʈ �̹����� �����ϴ�.");
			EditorTool.SetActive(false);
			selectedPhoto = null;
			return null;
		}

		return sprite.texture;
	}

	private void Update()
	{
		// �ڸ��� ������ �����ϸ��� ó���մϴ�.
		HandleCropAreaScaling();
	}

	private void HandleCropAreaScaling()
	{
		// �� �հ������� ��ġ�� �� ��ġ ���� ó���մϴ�.
		if (Input.touchCount == 2)
		{
			Touch touchZero = Input.GetTouch(0);   // ù ��° ��ġ
			Touch touchOne = Input.GetTouch(1);    // �� ��° ��ġ

			// ���� �������� ��ġ ��ġ�� ����մϴ�.
			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			//Debug.Log("touchZeroPrevPos : " + touchZeroPrevPos);
			//Debug.Log("touchZeroDelta : " + touchZero.deltaPosition);
			//Debug.Log("TouchZeroPosition : " + touchZero.position);
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
			//Debug.Log("touchOnePrevPos : " + touchOnePrevPos);
			//Debug.Log("touchOneDelta : " + touchOne.deltaPosition);
			//Debug.Log("TouchOnePosition : " + touchOne.position);

			// ��ġ ���� ���� �� ���� �Ÿ� ���̸� ����մϴ�.
			float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			//Debug.Log("prevMagnitude : " + prevMagnitude);
			float currentMagnitude = (touchZero.position - touchOne.position).magnitude;
			//Debug.Log("Magnitude : " + currentMagnitude);

			// �Ÿ� ���̸� ����Ͽ� �ڸ��� ������ ũ�⸦ �����մϴ�.
			float difference = currentMagnitude - prevMagnitude;
			ResizeCropArea(difference * 0.1f);  // �Ÿ� ������ 10%�� ũ�� �������� �����մϴ�.
		}

		// ���콺 �� ��ũ���� ����Ͽ� �ڸ��� ������ ũ�⸦ �����մϴ�.
		float scroll = Input.GetAxis("Mouse ScrollWheel");
		//Debug.Log(scroll);

		if (scroll != 0)
		{
			ResizeCropArea(scroll * 100f);  // ��ũ�� ���� 100�� ���Ͽ� ũ�� �������� �����մϴ�.
		}
	}

	private void ResizeCropArea(float increment)
	{
		// �ڸ��� ������ ���ο� ũ�⸦ ����մϴ�.
		Vector2 newSize = cropArea.sizeDelta + new Vector2(increment, increment);

		// �ڸ��� ������ ũ�⸦ �ּ� �� �ִ� ũ��� �����մϴ�.
		newSize.x = Mathf.Clamp(newSize.x, minSize, maxSize);
		newSize.y = Mathf.Clamp(newSize.y, minSize, maxSize);

		// ���ο� ũ�⸦ RectTransform�� �����մϴ�.
		cropArea.sizeDelta = newSize;
	}

	private void CropPhoto()
	{
		if (selectedPhoto == null)
		{
			// ���õ� ������ ������ ��� �޽����� ����մϴ�.
			Debug.LogWarning("No photo selected!");
			EditorTool.SetActive(false);
			selectedPhoto = null;
			return;
		}

		// photoDisplay�� ũ�⸦ �����ɴϴ�.
		RectTransform photoDisplayRectTransform = photoDisplay.GetComponent<RectTransform>();
		Vector2 photoDisplaySize = photoDisplayRectTransform.sizeDelta;

		// �ڸ��� ������ ũ��� ��ġ�� ����մϴ�.
		Rect cropRect = GetCropRect();

		// �ڸ��� ������ ���� �̹��� ũ�⿡ �°� �����մϴ�.
		float x = cropRect.x / photoDisplaySize.x * selectedPhoto.width;
		// -60 / 780 * 1255 = -96.54
		float y = cropRect.y / photoDisplaySize.y * selectedPhoto.height;
		// -60 / 780 * 1255 = -96.54
		float width = cropRect.width / photoDisplaySize.x * selectedPhoto.width;
		// 900 / 780 * 1255 = 1,448.08
		float height = cropRect.height / photoDisplaySize.y * selectedPhoto.height;
		// 900 / 780 * 1255 = 1,448.08

		// ������ ũ��� ��ġ�� ���ο� Rect�� �����մϴ�.
		cropRect = new Rect(x, y, width, height);
		// -96.54, -96.54, 1,448.08, 1,448.08

		// �ڸ��� �۾��� �����մϴ�.
		Texture2D croppedPhoto = CropToCircle(selectedPhoto, cropRect);

		// ���� ������ ������ �����մϴ�.
		SetProfilePicture(croppedPhoto);
	}

	private Rect GetCropRect()
	{
		// cropArea�� ũ��� ��ġ�� �����ɴϴ�.
		Vector2 size = cropArea.rect.size;
		Vector2 position = cropArea.anchoredPosition;

		// photoDisplay�� ũ�⸦ �����ɴϴ�.
		RectTransform photoDisplayRectTransform = photoDisplay.GetComponent<RectTransform>();
		Vector2 photoDisplaySize = photoDisplayRectTransform.sizeDelta;

		// cropArea�� ��ġ�� photoDisplay�� ��ǥ�迡 ���߾� ��ȯ�մϴ�.
		float x = position.x + photoDisplaySize.x / 2;
		float y = position.y + photoDisplaySize.y / 2;
		return new Rect(x - size.x / 2, y - size.y / 2, size.x, size.y);
		// -60, - 60, 900, 900
	}

	private Texture2D CropToCircle(Texture2D originalTexture, Rect cropRect)
	{
		// cropRect�� ���� �̹����� ��� ���� �����մϴ�.
		cropRect.x = Mathf.Clamp(cropRect.x, 0, originalTexture.width);
		cropRect.y = Mathf.Clamp(cropRect.y, 0, originalTexture.height);
		cropRect.width = Mathf.Clamp(cropRect.width, 0, originalTexture.width - cropRect.x);
		cropRect.height = Mathf.Clamp(cropRect.height, 0, originalTexture.height - cropRect.y);

		// �ڸ� ������ ũ��� ���ο� Texture2D�� �����մϴ�.
		int width = Mathf.RoundToInt(cropRect.width);
		int height = Mathf.RoundToInt(cropRect.height);
		Texture2D croppedTexture = new Texture2D(width, height);

		// �ڸ� ������ �ȼ� �����͸� �����ɴϴ�.
		Color[] pixels = originalTexture.GetPixels(Mathf.RoundToInt(cropRect.x), Mathf.RoundToInt(cropRect.y), width, height);

		// ���� ������ �������� �߽����� ����մϴ�.
		float radius = width / 2f;
		Vector2 center = new Vector2(width / 2f, height / 2f);

		// �ڸ��� ������ �� �ȼ��� ���� ������ �����մϴ�.
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				// ���� �ȼ��� ��ġ�� ����մϴ�.
				Vector2 pixelPos = new Vector2(x, y);

				// ���� �ȼ��� ���� ���� �ȿ� �ִ��� Ȯ���մϴ�.
				if (Vector2.Distance(pixelPos, center) <= radius)
				{
					// ���� ���� �ȿ� �ִ� ��� �ش� �ȼ��� ������ �����մϴ�.
					croppedTexture.SetPixel(x, y, pixels[y * width + x]);
				}
				else
				{
					// ���� ���� �ۿ� �ִ� ��� �ش� �ȼ��� �����ϰ� �����մϴ�.
					croppedTexture.SetPixel(x, y, Color.clear);
				}
			}
		}

		// ����� �ؽ�ó �����͸� �����մϴ�.
		croppedTexture.Apply();

		// �ڸ� �ؽ�ó�� ��ȯ�մϴ�.
		return croppedTexture;
	}

	public void SetProfilePicture(Texture2D profilePhoto)
	{
		// �̸����� �̹����� ������ �̹����� �����մϴ�.
		previewProfileImage.sprite = SpriteFromTexture2D(profilePhoto);
		profileImage.sprite = previewProfileImage.sprite;

		// ���� ���� UI�� ��Ȱ��ȭ�մϴ�.
		EditorTool.SetActive(false);
	}

	private Sprite SpriteFromTexture2D(Texture2D texture)
	{
		// Texture2D���� Sprite�� �����մϴ�.
		Rect rect = new Rect(0, 0, texture.width, texture.height);
		return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
	}

	private void CancelCropPhoto()
	{
		StopAllCoroutines();
		selectedPhoto = null;
		EditorTool.SetActive(false);
	}

	private void OnDisable()
	{
		confirmButton.onClick.RemoveListener(CropPhoto);
		cancelButton.onClick.RemoveListener(CancelCropPhoto);
	}
}
