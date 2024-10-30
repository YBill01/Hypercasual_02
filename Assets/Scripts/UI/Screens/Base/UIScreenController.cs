using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIScreenController : MonoBehaviour
{
	public UIScreenEvent OnShow { get; } = new UIScreenEvent();
	public UIScreenEvent OnHide { get; } = new UIScreenEvent();

	[SerializeField]
	protected RectTransform instantiatedContainer;
	[SerializeField]
	protected RectTransform hideContainer;
	[SerializeField]
	protected RectTransform showContainer;

	[Space(10)]
	[SerializeField]
	protected UIScreen defaultScreen;
	[SerializeField]
	protected List<UIScreen> screens;

	private List<UIScreen> _showingScreens;

	private Dictionary<Type, UIScreenFactory> _screenFactoryDic;
	
	private void Awake()
	{
		_showingScreens = new List<UIScreen>();

		_screenFactoryDic = new Dictionary<Type, UIScreenFactory>();
		
		foreach (UIScreen screen in screens)
		{
			_screenFactoryDic.Add(screen.GetType(), new UIScreenFactory(screen, hideContainer));

			if (screen.gameObject.scene.IsValid())
			{
				OnDeActivateScreen(screen);
			}
		}

		if (instantiatedContainer != null)
		{
			instantiatedContainer.gameObject.SetActive(false);
		}

		if (hideContainer != null)
		{
			hideContainer.gameObject.SetActive(false);
		}

		if (showContainer != null)
		{
			showContainer.gameObject.SetActive(true);
		}
	}
	private void Start()
	{
		ShowDefault();
	}

	private async UniTaskVoid ShowProcess(UIScreen screen)
	{
		await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

		screen.Activate(() => 
		{
			OnActivateScreen(screen);
		});
	}
	private async UniTaskVoid HideProcess(UIScreen screen)
	{
		await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

		screen.DeActivate(() => 
		{
			OnDeActivateScreen(screen);
		});
	}

	private void OnActivateScreen(UIScreen screen)
	{
		_showingScreens.Add(screen);
		screen.transform.SetParent(showContainer);

		IndexingScreen(screen);
		SortScreens();

		OnShow.Invok(screen);
	}
	private void OnDeActivateScreen(UIScreen screen)
	{
		_showingScreens.Remove(screen);
		screen.transform.SetParent(hideContainer);

		_screenFactoryDic[screen.GetType()].Dispose(screen);

		OnHide.Invok(screen);
	}

	public void ShowDefault()
	{
		if (defaultScreen != null)
		{
			Show(defaultScreen);
		}
	}

	public UIScreen Show(Type type)
	{
		if (_screenFactoryDic.TryGetValue(type, out UIScreenFactory screenFactory))
		{
			UIScreen currentTypedScreen = GetCurrents(type).FirstOrDefault();

			if (currentTypedScreen == null || !currentTypedScreen.single)
			{
				UIScreen screen = screenFactory.Instantiate();
				screen.Init(this);

				if (screen.single)
				{
					UIScreen currentScreen = GetCurrents(screen.layerIndex)
						.LastOrDefault();

					if (currentScreen != null)
					{
						currentScreen.DeActivate(() =>
						{
							OnDeActivateScreen(currentScreen);
							ShowProcess(screen).Forget();
						});
					}
					else
					{
						ShowProcess(screen).Forget();
					}
				}
				else
				{
					ShowProcess(screen).Forget();
				}

				return screen;
			}

			return currentTypedScreen;
		}

		return null;
	}
	public UIScreen Show(UIScreen screen)
	{
		if (screen.IsShowing)
		{
			IndexingScreen(screen);
			SortScreens();

			return screen;
		}

		return Show(screen.GetType());
	}

	public UIScreen Hide(Type type)
	{
		UIScreen screen = GetCurrents(type).LastOrDefault();

		if (screen != null)
		{
			return Hide(screen);
		}

		return null;
	}
	public UIScreen Hide(UIScreen screen)
	{
		if (screen.IsShowing)
		{
			HideProcess(screen).Forget();
		}

		return screen;
	}

	public T Show<T>() where T : UIScreen
	{
		return (T)Show(typeof(T));
	}
	public T Hide<T>() where T : UIScreen
	{
		return (T)Hide(typeof(T));
	}

	public void HideAll()
	{
		List<UIScreen> list = GetCurrents();
		foreach (UIScreen screen in list)
		{
			Hide(screen);
		}
	}
	public void HideAll(Type type)
	{
		List<UIScreen> list = GetCurrents(type);
		foreach (UIScreen screen in list)
		{
			Hide(screen);
		}
	}
	public void HideAll<T>() where T : UIScreen
	{
		HideAll(typeof(T));
	}

	public bool Has(Type type)
	{
		return _screenFactoryDic.ContainsKey(type);
	}
	public bool Has<T>() where T : UIScreen
	{
		return Has(typeof(T));
	}
	
	public UIScreen GetCurrent()
	{
		return _showingScreens.LastOrDefault();
	}

	public List<UIScreen> GetCurrents()
	{
		return _showingScreens.ToList();
	}
	public List<UIScreen> GetCurrents(Type type)
	{
		return _showingScreens.Where(x => x.GetType() == type)
			.ToList();
	}
	public List<T> GetCurrents<T>() where T : UIScreen
	{
		return GetCurrents(typeof(T))
			.Select(x => x as T)
			.ToList();
	}
	public List<UIScreen> GetCurrents(int layerIndex)
	{
		return _showingScreens.Where(x => x.layerIndex == layerIndex).ToList();
	}

	private void SortScreens()
	{
		List<UIScreen> list = GetCurrents();

		list.Sort((a, b) => 
		{
			int result = a.layerIndex.CompareTo(b.layerIndex);

			if (result != 0)
			{
				return result;
			}

			return a.InstanceIndex.CompareTo(b.InstanceIndex);
		});

		for (int i = 0; i < list.Count; i++)
		{
			list[i].transform.SetSiblingIndex(i);
		}
	}

	private void IndexingScreen(UIScreen focusScreen)
	{
		List<UIScreen> list = GetCurrents(focusScreen.GetType());
		list.Sort((a, b) => a.InstanceIndex.CompareTo(b.InstanceIndex));

		int lastIndex = 0;

		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] != focusScreen)
			{
				lastIndex++;
				list[i].InstanceIndex = lastIndex;
			}
		}

		lastIndex++;

		focusScreen.InstanceIndex = lastIndex;
	}
}