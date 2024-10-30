using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class UIVersionComponent : MonoBehaviour
{
	private void Start()
	{
		GetComponent<TMP_Text>().text = $"v{Application.version}";
	}
}