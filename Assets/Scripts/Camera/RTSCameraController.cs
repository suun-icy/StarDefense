using UnityEngine;

/// <summary>
/// RTS 摄像机控制（移动 + 缩放 + 旋转 + 升降）
/// </summary>
public class RTSCameraController : MonoBehaviour
{
	[Header("移动")]
	public float moveSpeed = 20f;

	[Header("缩放")]
	public float zoomSpeed = 200f;

	[Header("旋转")]
	public float rotateSpeed = 5f;

	[Header("高度控制")]
	public float heightSpeed = 20f;

	[Header("拖拽")]
	public float dragSpeed = 0.2f;// 降低速度
	public float dragSmooth = 10f;// 平滑

	private Vector3 lastMousePos;
	private Vector3 dragVelocity;

	public float minHeight = 10f;
	public float maxHeight = 100f;

	void Update()
	{
		HandleKeyboardMove();
		HandleMouseDrag();
		HandleZoom();
		HandleRotate();
		HandleHeight();
	}

	/// <summary>
	/// WASD 平移
	/// </summary>
	void HandleKeyboardMove()
	{
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");

		// 获取相机方向（忽略Y）
		Vector3 forward = transform.forward;
		Vector3 right = transform.right;

		forward.y = 0;
		right.y = 0;

		forward.Normalize();
		right.Normalize();

		// 按视角方向移动
		Vector3 dir = forward * v + right * h;

		transform.Translate(dir * moveSpeed * Time.deltaTime, Space.World);
	}

	/// <summary>
	/// 右键拖拽平移
	/// </summary>
	void HandleMouseDrag()
	{
		if (Input.GetMouseButtonDown(1))
		{
			lastMousePos = Input.mousePosition;
		}

		if (Input.GetMouseButton(1))
		{
			Vector3 delta = Input.mousePosition - lastMousePos;

			// 右键拖拽 = 地图反向移动（更符合手感）
			Vector3 targetMove = new Vector3(-delta.x, 0, -delta.y) * dragSpeed * Time.deltaTime;

			dragVelocity = Vector3.Lerp(dragVelocity, targetMove, dragSmooth * Time.deltaTime);

			transform.Translate(dragVelocity, Space.World);

			lastMousePos = Input.mousePosition;

			Vector3 angles = transform.eulerAngles;
			angles.x = Mathf.Clamp(angles.x, 20f, 80f);
			transform.eulerAngles = angles;
		}
	}

	/// <summary>
	/// 滚轮缩放
	/// </summary>
	void HandleZoom()
	{
		float scroll = Input.GetAxis("Mouse ScrollWheel");
		transform.position += transform.forward * scroll * zoomSpeed * Time.deltaTime;
	}

	/// <summary>
	/// 中键 旋转视角
	/// </summary>
	void HandleRotate()
	{
		//if (Input.GetMouseButton(2) && Input.GetKey(KeyCode.LeftAlt))
		if (Input.GetMouseButton(2))
		{
			float mouseX = Input.GetAxis("Mouse X");
			float mouseY = Input.GetAxis("Mouse Y");

			// 左右旋转（绕Y轴）
			transform.Rotate(Vector3.up, mouseX * rotateSpeed, Space.World);

			// 上下俯仰（绕自身X轴）
			transform.Rotate(Vector3.right, -mouseY * rotateSpeed, Space.Self);
			LimitAngle();
		}
	}
	/// <summary>
	/// 限制俯仰角
	/// </summary>
	void LimitAngle()
	{
		Vector3 angles = transform.eulerAngles;

		float x = angles.x;
		if (x > 180) x -= 360;

		x = Mathf.Clamp(x, 20f, 80f);

		angles.x = x;
		transform.eulerAngles = angles;
	}

	/// <summary>
	/// 空格上升，Ctrl下降
	/// </summary>
	void HandleHeight()
	{
		if (Input.GetKey(KeyCode.Space))
		{
			transform.Translate(Vector3.up * heightSpeed * Time.deltaTime, Space.World);
		}

		if (Input.GetKey(KeyCode.LeftControl))
		{
			transform.Translate(Vector3.down * heightSpeed * Time.deltaTime, Space.World);
		}
		Vector3 pos = transform.position;
		pos.y = Mathf.Clamp(pos.y, minHeight, maxHeight);
		transform.position = pos;
	}
}