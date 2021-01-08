using System;
using UnityEngine;

public class Rotator : MonoBehaviour
{
	public enum RotateAxis
	{
		X,
		Y,
		Z
	}

	public RotateAxis Axis = RotateAxis.Z;

	private bool isSelected = false;
	private Camera cameraCache;
	private Vector3 beginPoint;
	private Vector3 lastMousePoint;
	private Vector3 currentMousePoint;
	private Vector3 rotateAxis;
	private Plane rotatePlane;

	private Vector3 GetAxisDir(RotateAxis axis)
	{
		switch (axis)
		{
			case RotateAxis.X:
				return transform.right;

			case RotateAxis.Y:
				return transform.up;

			case RotateAxis.Z:
				return transform.forward;

			default:
				throw new ArgumentException($"Axis {axis} is not available.");
		}
	}

	private Vector3 GetRotateEulers(RotateAxis axis, float angle)
	{
		Vector3 eulers = Vector3.zero;

		switch (axis)
		{
			case RotateAxis.X:
				eulers.x = angle;
				break;

			case RotateAxis.Y:
				eulers.y = angle;
				break;

			case RotateAxis.Z:
				eulers.z = angle;
				break;

			default:
				throw new ArgumentException($"Axis {axis} is not available.");
		}

		return eulers;
	}

	private void OnMouseDown()
	{
		Vector3 rotateAxis = GetAxisDir(Axis);

		InitializeRotation(rotateAxis);

		isSelected = true;
	}

	private void OnMouseUp()
	{
		isSelected = false;
	}

	private void InitializeRotation(Vector3 rotateAxis)
	{
		this.rotateAxis = rotateAxis;

		rotatePlane = new Plane(rotateAxis, transform.position);
		cameraCache = Camera.main;

		Ray ray = cameraCache.ScreenPointToRay(Input.mousePosition);

		if (rotatePlane.Raycast(ray, out float enter))
		{
			beginPoint = ray.GetPoint(enter);
			lastMousePoint = beginPoint;
		}
	}

	private bool IsForward(Vector3 from, Vector3 to, Vector3 axisDir)
	{
		Vector3 crossDir = Vector3.Cross(from, to);
		float crossDirDotView = Vector3.Dot(crossDir, axisDir);

		return crossDirDotView > 0.0f;
	}

	private void UpdateRotation()
	{
		Ray ray = cameraCache.ScreenPointToRay(Input.mousePosition);

		if (rotatePlane.Raycast(ray, out float enter))
		{
			currentMousePoint = ray.GetPoint(enter);

			float rotateAngle = Vector3.Angle(lastMousePoint - transform.position, currentMousePoint - transform.position);

			var isForward = IsForward(lastMousePoint - transform.position, currentMousePoint - transform.position, rotateAxis);
			rotateAngle = isForward ? rotateAngle : -rotateAngle;

			Vector3 eulers = GetRotateEulers(Axis, rotateAngle);

			//Debug.Log($"isForward: {isForward}, rotateAxis: {rotateAxis}, rotateAngle: {rotateAngle}, eulers: {eulers}");

			transform.Rotate(eulers, Space.Self);

			lastMousePoint = currentMousePoint;
		}
	}

	void Update()
	{
		if (!isSelected)
		{
			return;
		}

		UpdateRotation();
	}

	private void OnDrawGizmos()
	{
		if (isSelected)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position, beginPoint);

			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position, currentMousePoint);

			Gizmos.color = Color.magenta;
			Gizmos.DrawLine(transform.position, transform.position + rotatePlane.normal);
		}
	}
}