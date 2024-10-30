using GameName.Core;
using UnityEngine;
using VContainer;

public class UIControls : MonoBehaviour
{
	[SerializeField]
	private UIScreenController m_uiController;

	private Profile _profile;
	private VCamera _vCamera;

	[Inject]
	public void Construct(
		Profile profile,
		VCamera vCamera)
	{
		_profile = profile;
		_vCamera = vCamera;
	}

	private void OnEnable()
	{
		m_uiController.OnShow.AddListener<UIControlsScreen>(UIControlsScreenOnShow);
		m_uiController.OnHide.AddListener<UIControlsScreen>(UIControlsScreenOnHide);
	}
	private void OnDisable()
	{
		m_uiController.OnShow.RemoveListener<UIControlsScreen>();
		m_uiController.OnHide.RemoveListener<UIControlsScreen>();
	}

	private void UIControlsScreenOnShow(UIScreen screen)
	{
		UIControlsScreen mainMenuScreen = screen as UIControlsScreen;

#if UNITY_ANDROID || UNITY_IOS
		mainMenuScreen.SetActiveUIControls(true);
		mainMenuScreen.SetZoomSlider(1.0f - _vCamera.distanceValue);

		mainMenuScreen.Zoom += OnMainMenuScreenZoom;
#else
		mainMenuScreen.SetActiveUIControls(false);
#endif
	}
	private void UIControlsScreenOnHide(UIScreen screen)
	{
		UIControlsScreen mainMenuScreen = screen as UIControlsScreen;

#if UNITY_ANDROID || UNITY_IOS
		mainMenuScreen.Zoom -= OnMainMenuScreenZoom;
#endif
	}

	private void OnMainMenuScreenZoom(float value)
	{
		_vCamera.distanceValue = 1.0f - value;
	}
}