using GameName.Data;
using UnityEngine;

public class BuildingIcon : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer m_spriteIconBorder;
	[SerializeField]
	private SpriteRenderer m_spriteIcon;

	[Space]
	[SerializeField]
	private Vector2 m_iconSize;

	public void SetData(RecipeData.RecipeItem data, SharedViewData viewData)
	{
		m_spriteIcon.sprite = viewData.GetItemViewData(data.type).icon;
		Progress(0.0f);
	}

	public void Progress(float value)
	{
		m_spriteIcon.size = new Vector2(m_iconSize.x, m_iconSize.y * value);
	}
}