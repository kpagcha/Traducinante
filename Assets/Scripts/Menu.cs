using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	public void LoadMainMenu()
	{
		Application.LoadLevel ("MainMenu");
	}

	public void LoadEasyModeMenu()
	{
		Application.LoadLevel ("EasyModeMenu");
	}

	public void LoadEasyMode()
	{
		Application.LoadLevel ("EasyMode");
	}

	public void LoadHardMode()
	{
		Application.LoadLevel ("HardMode");
	}
}
