using GameName.Core;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class UICore : MonoBehaviour
{
	[SerializeField]
	private UIScreenController m_uiController;

	[Space]
	[SerializeField]
	private Image m_bgImage;

	private UIMainMenuScreen _mainMenuScreen;

	private Profile _profile;
	private CoreFlow _coreFlow;

	private IObjectResolver _resolver;

	[Inject]
	public void Construct(
		IObjectResolver resolver,
		Profile profile,
		CoreFlow coreFlow)
	{
		_resolver = resolver;

		_profile = profile;
		_coreFlow = coreFlow;
	}

	private void OnEnable()
	{
		_coreFlow.OnStateChanged += CoreFlowOnStateChange;

		m_uiController.OnShow.AddListener<UIMainMenuScreen>(UIMainMenuScreenOnShow);
		m_uiController.OnHide.AddListener<UIMainMenuScreen>(UIMainMenuScreenOnHide);
	}
	private void OnDisable()
	{
		_coreFlow.OnStateChanged -= CoreFlowOnStateChange;

		m_uiController.OnShow.RemoveListener<UIMainMenuScreen>();
		m_uiController.OnHide.RemoveListener<UIMainMenuScreen>();
	}

	private void CoreFlowOnStateChange(CoreFlow.CoreState state)
	{
		switch (state)
		{
			case CoreFlow.CoreState.Meta:
				_mainMenuScreen?.ApplyState();
				m_bgImage.gameObject.SetActive(true);

				break;
			case CoreFlow.CoreState.Game:
				_mainMenuScreen?.ApplyState();
				m_bgImage.gameObject.SetActive(false);

				break;
			default:
				break;
		}
	}

	private void UIMainMenuScreenOnShow(UIScreen screen)
	{
		UIMainMenuScreen mainMenuScreen = screen as UIMainMenuScreen;

		_resolver.Inject(mainMenuScreen);

		_mainMenuScreen = mainMenuScreen;
	}
	private void UIMainMenuScreenOnHide(UIScreen screen)
	{
		UIMainMenuScreen mainMenuScreen = screen as UIMainMenuScreen;

		_mainMenuScreen = null;
	}
}