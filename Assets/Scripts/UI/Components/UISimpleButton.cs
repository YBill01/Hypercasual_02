using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

[AddComponentMenu("UI/UISimpleButton", 31)]
public class UISimpleButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	[Serializable]
	public class ButtonDownEvent : UnityEvent { }
	
	[Serializable]
	public class ButtonUpEvent : UnityEvent { }

	[FormerlySerializedAs("onDown")]
	[SerializeField]
	private ButtonDownEvent m_OnDown = new ButtonDownEvent();
	
	[FormerlySerializedAs("onUp")]
	[SerializeField]
	private ButtonUpEvent m_OnUp = new ButtonUpEvent();

	public ButtonDownEvent onDown
	{
		get { return m_OnDown; }
		set { m_OnDown = value; }
	}
	public ButtonUpEvent onUp
	{
		get { return m_OnUp; }
		set { m_OnUp = value; }
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		m_OnDown.Invoke();
	}
	public void OnPointerUp(PointerEventData eventData)
	{
		m_OnUp.Invoke();
	}
}