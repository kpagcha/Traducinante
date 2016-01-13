using UnityEngine;
using System.Collections;

public class EasyMode : MonoBehaviour {

	private Game game;
	private Hashtable langItems = new Hashtable();

	private string selectedLang;
	private int correctAnswers;
	private int incorrectAnswers;

	void Start()
	{
		game = GameObject.Find ("_GameController").GetComponent<Game>();
		selectedLang = "Spanish";
		correctAnswers = 0;
		incorrectAnswers = 0;

		InitializeLangItems ();
	}

	private void InitializeLangItems()
	{
		ArrayList allObjects = game.getObjects ();

		Debug.Log (allObjects.Count);

		int numberOfQuestions = game.numberOfQuestions;

		System.Random rnd = new System.Random ();
		for (int i = 0; i < numberOfQuestions; i++) {
			int pos = rnd.Next(allObjects.Count);

			LangObject langObject = allObjects[i] as LangObject;
			allObjects.RemoveAt(i);

			Debug.Log(pos + " " + langObject.getName());
		}
	}
}
