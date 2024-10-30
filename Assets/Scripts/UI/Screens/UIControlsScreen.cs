using System;
using UnityEngine;
using UnityEngine.UI;

public class UIControlsScreen : UIScreen
{
	public event Action<float> Zoom;

	[Space]
	[SerializeField]
	private UIJoystick m_uiController;

	[SerializeField]
	private UISimpleButton m_jumpButton;
	
	[SerializeField]
	private Slider m_zoomSlider;

	private void OnEnable()
	{
		m_zoomSlider.onValueChanged.AddListener(ZoomSliderOnValueChanged);
	}
	private void OnDisable()
	{
		m_zoomSlider.onValueChanged.RemoveListener(ZoomSliderOnValueChanged);
	}

	public void SetActiveUIControls(bool value)
	{
		m_uiController.gameObject.SetActive(value);
		m_jumpButton.gameObject.SetActive(value);
	}

	public void SetZoomSlider(float value)
	{
		m_zoomSlider.SetValueWithoutNotify(value);
	}

	private void ZoomSliderOnValueChanged(float value)
	{
		Zoom?.Invoke(value);
	}
}