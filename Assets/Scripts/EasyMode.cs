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
        ArrayList allObjects = game.getObjects().Clone() as ArrayList;

        System.Random rnd = new System.Random ();
		for (int i = 0; i < game.numberOfQuestions; i++) {
            int pos = rnd.Next(0, allObjects.Count - 1);

            LangObject langObject = allObjects[pos] as LangObject;

            ArrayList choices = GenerateChoices(langObject);
            langItems.Add(langObject, choices);

            allObjects.RemoveAt(pos);
        }

        LogChoices();
    }

    private ArrayList GenerateChoices(LangObject o)
    {
        ArrayList allObjects = game.getObjects().Clone() as ArrayList;
        ArrayList choices = new ArrayList();
       
        string correctAnswer = o.getLang(selectedLang)["text"] as string;
        choices.Add(correctAnswer);

        for (int i = 0; i < allObjects.Count; i++) {
            LangObject x = allObjects[i] as LangObject;
            if (x.getName() == o.getName()) {
                allObjects.RemoveAt(i);
                break;
            }
        }

        System.Random rnd = new System.Random();
        for (int i = 0; i < game.numberOfChoices - 1; i++) {
            int pos = rnd.Next(0, allObjects.Count - 1);

            LangObject langObject = allObjects[pos] as LangObject;

            string wrongAnswer = langObject.getLang(selectedLang)["text"] as string;
            choices.Add(wrongAnswer);

            allObjects.RemoveAt(pos);
        }

        return game.Shuffle(choices);
    }

    private void LogChoices()
    {
        foreach (DictionaryEntry item in langItems)
        {
            string s = "";
            ArrayList choices = item.Value as ArrayList;
            foreach (string choice in choices)
            {
                s += choice + " ";
            }
            LangObject it = item.Key as LangObject;
            Debug.Log(it.getName() + " -> " + s);
        }
    }
}
