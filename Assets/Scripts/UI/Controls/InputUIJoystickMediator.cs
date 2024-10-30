#if ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

[AddComponentMenu("Input/UIJoystick Mediator")]
public class InputUIJoystickMediator : OnScreenControl
{
	[InputControl(layout = "Vector2")]
	[SerializeField]
	private string m_ControlPath;

	protected override string controlPathInternal
	{
		get => m_ControlPath;
		set => m_ControlPath = value;
	}

	public void SetValue(Vector2 position)
	{
		SendValueToControl(position);
	}
}
#endif