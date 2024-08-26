using UnityEngine;

public class CameraController : MonoBehaviour
{
	[Header("ī�޶� �̵� ����")]
	[SerializeField] private float panSpeed = 0.1f;

	[Header("�� �ӵ�")]
	[SerializeField] private float zoomspeed = 10f;

	[Header("�� �ּ�, �ִ밪")]
	[SerializeField] private float minZoomValue = 2.5f;
	[SerializeField] private float maxZoomValue = 5.5f;

	[Header("ī�޶� ���")]
	[SerializeField] private Vector2 minCameraPosition;
	[SerializeField] private Vector2 maxCameraPosition;

	[Header("�ε巯�� ī�޶� �̵�")]
	private Vector3 velocity;
	[SerializeField] private float smoothTime = 0.2f;
	private Vector3 targetPosition; // ��ǥ ��ġ

	private Camera cam;
	private Vector3 lastTouchPosition;
	private Vector3 lastMousePosition;
	private bool isPanning;

	private void Awake()
	{
		cam = Camera.main;
		targetPosition = cam.transform.position;
	}

	private void Update()
	{
#if UNITY_EDITOR_WIN || UNITY_STANDALONE

		HandleCameraMovement_PC();
		HandleCameraZoom_PC();
#endif

#if UNITY_ANDROID && !UNITY_EDITOR

        HandleCameraMovement();
        HandleCameraZoom();

#endif
	}

	/// <summary>
	/// WINDOW �ü�� ����Ƽ �����Ϳ����� ������ ������ �ڵ��̴�.
	/// </summary>

#if UNITY_EDITOR_WIN || UNITY_STANDALONE

	private void HandleCameraMovement_PC()
	{
		if (Input.GetMouseButtonDown(0))  // ���콺 ��ư�� ó�� ������ ��
		{
			lastMousePosition = Input.mousePosition;
			isPanning = true;
		}
		else if (Input.GetMouseButton(0) && isPanning)  // ���콺 ��ư�� ������ ���� ��
		{
			Vector3 delta = Input.mousePosition - lastMousePosition;
			Vector3 move = delta * panSpeed * Time.deltaTime;
			targetPosition = cam.transform.position - move; // targetPosition ������Ʈ

			// ��� ���� �˻�
			targetPosition.x = Mathf.Clamp(targetPosition.x, minCameraPosition.x, maxCameraPosition.x);
			targetPosition.y = Mathf.Clamp(targetPosition.y, minCameraPosition.y, maxCameraPosition.y);

			lastMousePosition = Input.mousePosition;
		}
		else if (Input.GetMouseButtonUp(0))  // ���콺 ��ư�� ������ ��
		{
			isPanning = false;
		}

		// SmoothDamp�� ����� �ε巯�� �̵� �� ���� ó��
		cam.transform.position = Vector3.SmoothDamp(cam.transform.position, targetPosition, ref velocity, smoothTime);

		// �д��� ���� �� velocity�� ������ 0���� ���̱� ���� ���� ������ ����
		if (!isPanning && velocity.magnitude > 0.01f) // �̼��� ������ ����
		{
			velocity = Vector3.Lerp(velocity, Vector3.zero, smoothTime);
		}
	}

	private void HandleCameraZoom_PC()
	{
		float scroll = Input.GetAxis("Mouse ScrollWheel");

		if (scroll != 0)
		{
			float newZoom = cam.orthographicSize - scroll * zoomspeed * Time.deltaTime;
			cam.orthographicSize = Mathf.Clamp(newZoom, minZoomValue, maxZoomValue);
		}
	}

#endif


	/// <summary>
	/// �ȵ���̵忡���� ���� ������ �ڵ�
	/// ����Ƽ �����Ϳ��� �����ϱ� ���ؼ��� "!UNITY_EDITOR"�� �����ؾ� �Ѵ�.
	/// </summary>

#if UNITY_ANDROID && !UNITY_EDITOR
    private void HandleCameraMovement()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPosition = touch.position;
                isPanning = true;
            }
            else if (touch.phase == TouchPhase.Moved && isPanning)
            {
                Vector3 delta = new Vector3(touch.position.x, touch.position.y, 0) - new Vector3(lastTouchPosition.x, lastTouchPosition.y, 0);
                Vector3 move = delta * panSpeed * Time.deltaTime;
                targetPosition = cam.transform.position - move; // targetPosition ������Ʈ

                // ��� ���� �˻�
                targetPosition.x = Mathf.Clamp(targetPosition.x, minCameraPosition.x, maxCameraPosition.x);
                targetPosition.y = Mathf.Clamp(targetPosition.y, minCameraPosition.y, maxCameraPosition.y);

                lastTouchPosition = touch.position; // ������Ʈ�� ��ġ ����
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isPanning = false;
            }

            // �ε巯�� �̵� �� ���� ó��
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, targetPosition, ref velocity, smoothTime);

            if (!isPanning && velocity.magnitude > 0.01f)
            {
                velocity = Vector3.Lerp(velocity, Vector3.zero, smoothTime);
            }
        }
    }

    private void HandleCameraZoom()
    {
        if(Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
            Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;

            float prevTouchDeltaMag = (touch1PrevPos - touch2PrevPos).magnitude;
            float touchDeltaMag = (touch1.position - touch2.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + deltaMagnitudeDiff * zoomspeed * Time.deltaTime, minZoomValue, maxZoomValue);
        }
    }

#endif
}