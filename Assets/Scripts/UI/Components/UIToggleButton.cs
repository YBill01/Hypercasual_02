using System;
using UnityEngine;
using UnityEngine.UI;

public class UIToggleButton : MonoBehaviour
{
	public Action<bool> onValueChanged;

	[SerializeField]
	private Button m_onButton;
	[SerializeField]
	private Button m_offButton;

	private bool _value;
	public bool Value 
	{
		get => _value;
		set
		{
			_value = value;

			SetValue(_value);
		}
	}

	private void Awake()
	{
		SetValue(false);
	}

	private void OnEnable()
	{
		m_onButton.onClick.AddListener(OnButtonOnClick);
		m_offButton.onClick.AddListener(OffButtonOnClick);
	}
	private void OnDisable()
	{
		m_onButton.onClick.RemoveListener(OnButtonOnClick);
		m_offButton.onClick.RemoveListener(OffButtonOnClick);
	}

	private void OnButtonOnClick()
	{
		Value = false;
		onValueChanged?.Invoke(false);
	}
	private void OffButtonOnClick()
	{
		Value = true;
		onValueChanged?.Invoke(true);
	}

	private void SetValue(bool value)
	{
		m_onButton.gameObject.SetActive(value);
		m_offButton.gameObject.SetActive(!value);
	}
}