using System;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public abstract class UIScreen : MonoBehaviour
{
	public Action<UIScreen> onPreShow;
	public Action<UIScreen> onPreHide;
	public Action<UIScreen> onShow;
	public Action<UIScreen> onHide;

	public int layerIndex = -1;

	public bool single = true;

	public bool IsShowing => State == ScreenState.Show || State == ScreenState.ShowProcess;

	public int InstanceIndex { get; set; }

	public ScreenState State { get; private set; }
	public enum ScreenState
	{
		None,
		ShowProcess,
		Show,
		HideProcess,
		Hide
	}

	protected UIScreenController _controller;

	internal void Init(UIScreenController controller)
	{
		State = ScreenState.Hide;

		_controller = controller;

		onPreShow = null;
		onPreHide = null;
		onShow = null;
		onHide = null;

		gameObject.SetActive(false);

		InstanceIndex = 0;

		OnInit();
	}

	internal void Activate(Action action)
	{
		OnActivate(action);
	}
	internal void DeActivate(Action action)
	{
		OnDeActivate(action);
	}

	internal UIScreen SelfShowInternal()
	{
		if (_controller != null)
		{
			return _controller.Show(this);
		}

		return this;
	}
	internal UIScreen SelfHideInternal()
	{
		if (_controller != null)
		{
			return _controller.Hide(this);
		}

		return this;
	}

	protected void OnPreActivate(Action action = null)
	{
		State = ScreenState.ShowProcess;

		gameObject.SetActive(true);

		action?.Invoke();

		onPreShow?.Invoke(this);
		onPreShow = null;
		
		OnPreShow();
	}
	protected void OnPreDeActivate(Action action = null)
	{
		State = ScreenState.HideProcess;

		action?.Invoke();

		onPreHide?.Invoke(this);
		onPreHide = null;
		
		OnPreHide();
	}

	protected void OnPostActivate(Action action = null)
	{
		State = ScreenState.Show;

		action?.Invoke();

		onShow?.Invoke(this);
		onShow = null;

		OnShow();
	}
	protected void OnPostDeActivate(Action action = null)
	{
		State = ScreenState.Hide;

		gameObject.SetActive(false);

		action?.Invoke();

		onHide?.Invoke(this);
		onHide = null;

		OnHide();

		_controller = null;
	}

	protected virtual void OnActivate(Action action)
	{
		OnPreActivate(action);
		OnPostActivate();
	}
	protected virtual void OnDeActivate(Action action)
	{
		OnPreDeActivate();
		OnPostDeActivate(action);
	}
	
	protected virtual void OnInit()
	{
		//Do Nothing...
	}

	protected virtual void OnPreShow()
	{
		//Do Nothing...
	}
	protected virtual void OnPreHide()
	{
		//Do Nothing...
	}

	protected virtual void OnShow()
	{
		//Do Nothing...
	}
	protected virtual void OnHide()
	{
		//Do Nothing...
	}
}

public static class UIScreenExtensions
{
	public static T SelfShow<T>(this T t) where T : UIScreen
	{
		return (T)t.SelfShowInternal();
	}
	public static T SelfHide<T>(this T t) where T : UIScreen
	{
		return (T)t.SelfHideInternal();
	}

	public static T OnPreShow<T>(this T t, Action<UIScreen> action) where T : UIScreen
	{
		t.onPreShow = action;

		return t;
	}
	public static T OnPreHide<T>(this T t, Action<UIScreen> action) where T : UIScreen
	{
		t.onPreHide = action;

		return t;
	}

	public static T OnShow<T>(this T t, Action<UIScreen> action) where T : UIScreen
	{
		t.onShow = action;

		return t;
	}
	public static T OnHide<T>(this T t, Action<UIScreen> action) where T : UIScreen
	{
		t.onHide = action;

		return t;
	}
}