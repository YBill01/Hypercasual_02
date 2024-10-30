using System;
using System.Collections.Generic;

public class UIScreenEvent
{
	private Dictionary<Type, Action<UIScreen>> _handlers;
	
	public UIScreenEvent()
	{
		_handlers = new Dictionary<Type, Action<UIScreen>>();
	}

	public void AddListener<T>(Action<UIScreen> handler) where T : UIScreen
	{
		_handlers.Add(typeof(T), handler);
	}
	public void RemoveListener<T>() where T : UIScreen
	{
		_handlers.Remove(typeof(T));
	}
	
	internal void Invok(UIScreen screen)
	{
		if (_handlers.TryGetValue(screen.GetType(), out Action<UIScreen> handler))
		{
			handler.Invoke(screen);
		}
	}
}