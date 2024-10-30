#if ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

[AddComponentMenu("Input/UIButton Mediator")]
public class InputUIButtonMediator : OnScreenControl
{
	[InputControl(layout = "Button")]
	[SerializeField]
	private string m_ControlPath;

	protected override string controlPathInternal
	{
		get => m_ControlPath;
		set => m_ControlPath = value;
	}

	public void SetValue()
	{
		SendValueToControl(1.0f);
	}
	public void SetCancel()
	{
		SendValueToControl(0.0f);
	}
}
#endif