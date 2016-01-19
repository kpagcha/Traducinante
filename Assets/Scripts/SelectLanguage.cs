using UnityEngine;
using System.Collections;

public class SelectLanguage : MonoBehaviour {

	private static string language;

	public void SetLanguage(string lang)
	{
		language = lang;
	}

	public string GetLanguage()
	{
		return language;
	}
}
