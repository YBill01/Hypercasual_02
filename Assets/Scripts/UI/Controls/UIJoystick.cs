using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
	[SerializeField]
	private UIJoystickTypes type = UIJoystickTypes.Fixed;

	[Header("Rect References")]
	[SerializeField]
	private RectTransform pointerRect;
	[SerializeField]
	private RectTransform restrictRect;
	[SerializeField]
	private RectTransform bodyRect;
	[SerializeField]
	private RectTransform handleRect;

	[Header("Settings")]
	[SerializeField]
	private uint movementRange = 100;
	[SerializeField]
	private float multiplierValue = 1.0f;
	[SerializeField]
	private bool invertXValue = false;
	[SerializeField]
	private bool invertYValue = false;

	[Header("Joystick events")]
	public UnityEvent<bool> OnStatePressed;
	public UnityEvent<Vector2> OnValueChange;

	private Vector2 _bodyRectPosition = Vector2.zero;

	void Start()
	{
		UpdateBodyRectPosition(Vector2.zero);
		UpdateHandleRectPosition(Vector2.zero);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		SendStatePressed(true);

		switch (type)
		{
			case UIJoystickTypes.Fixed:
				OnDrag(eventData);

				break;
			case UIJoystickTypes.Dynamic:
			case UIJoystickTypes.DynamicFloating:
				RectTransformUtility.ScreenPointToLocalPointInRectangle(pointerRect, eventData.position, eventData.pressEventCamera, out Vector2 position);
				UpdateBodyRectPosition(position + pointerRect.anchoredPosition);

				OnDrag(eventData);

				break;
			default:
				break;
		}
	}
	public void OnPointerUp(PointerEventData eventData)
	{
		UpdateBodyRectPosition(Vector2.zero);
		UpdateHandleRectPosition(Vector2.zero);

		SendStatePressed(false);
		SendJoystickValue(Vector2.zero);
	}
	public void OnDrag(PointerEventData eventData)
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(pointerRect, eventData.position, eventData.pressEventCamera, out Vector2 position);
		Vector2 clampedPosition = Vector2.ClampMagnitude(position + pointerRect.anchoredPosition - _bodyRectPosition, movementRange / multiplierValue) * multiplierValue;

		UpdateHandleRectPosition(clampedPosition);

		SendJoystickValue(ApplyInversion(clampedPosition / movementRange));

		if (type == UIJoystickTypes.DynamicFloating && clampedPosition.magnitude >= movementRange)
		{
			UpdateBodyRectPosition(_bodyRectPosition + ((position + pointerRect.anchoredPosition - _bodyRectPosition) * multiplierValue) - clampedPosition);
		}
	}

	// UI
	private void UpdateBodyRectPosition(Vector2 newPosition)
	{
		if (bodyRect != null)
		{
			float xPosition = Mathf.Clamp(newPosition.x, restrictRect.anchoredPosition.x + ((bodyRect.rect.width / 2) - (restrictRect.rect.width / 2)), restrictRect.anchoredPosition.x + ((restrictRect.rect.width / 2) - (bodyRect.rect.width / 2)));
			float yPosition = Mathf.Clamp(newPosition.y, restrictRect.anchoredPosition.y + ((bodyRect.rect.height / 2) - (restrictRect.rect.height / 2)), restrictRect.anchoredPosition.y + ((restrictRect.rect.height / 2) - (bodyRect.rect.height / 2)));

			_bodyRectPosition = new Vector2(xPosition, yPosition);

			bodyRect.anchoredPosition = _bodyRectPosition;
		}
	}
	private void UpdateHandleRectPosition(Vector2 newPosition)
	{
		if (handleRect != null)
		{
			handleRect.anchoredPosition = newPosition;
		}
	}

	// UTILS
	private Vector2 ApplyInversion(Vector2 position)
	{
		if (invertXValue)
		{
			position.x = -position.x;
		}

		if (invertYValue)
		{
			position.y = -position.y;
		}

		return position;
	}

	// HANDLERS
	private void SendStatePressed(bool value)
	{
		OnStatePressed?.Invoke(value);
	}
	private void SendJoystickValue(Vector2 position)
	{
		OnValueChange?.Invoke(position);
	}
}

public enum UIJoystickTypes
{
	Fixed,
	Dynamic,
	DynamicFloating
}