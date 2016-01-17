using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EasyMode : MonoBehaviour {

	private Game game;
	private Hashtable langItems = new Hashtable();
    private ArrayList langItemKeys = new ArrayList();

    private string selectedLang;
	private int correctAnswers;
	private int incorrectAnswers;
    private bool alreadyAnswered;
    private bool gameFinished = false;

	void Start()
	{
        game = GameObject.Find("_GameController").GetComponent<Game>();

        selectedLang = "Russian";
        correctAnswers = 0;
        incorrectAnswers = 0;

        InitializeLangItems();

        langItemKeys = new ArrayList(langItems.Keys);

        NextQuestion();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (gameFinished)
                Application.LoadLevel("EasyMode");
            else
            {
                if (langItemKeys.Count > 0)
                {
                    if (alreadyAnswered)
                        NextQuestion();
                }
                else
                    FinishLevel();
            }
        }

        if (!alreadyAnswered)
        {
            bool one = Input.GetKeyDown("1");
            bool two = Input.GetKeyDown("2");
            bool three = Input.GetKeyDown("3");
            bool four = Input.GetKeyDown("4");

            if (one || two || three || four)
            {
                string answer = "";

                if (one)
                    answer = GameObject.Find("TextChoice1").GetComponent<Text>().text.ToString();

                if (two)
                    answer = GameObject.Find("TextChoice2").GetComponent<Text>().text.ToString();

                if (three)
                    answer = GameObject.Find("TextChoice3").GetComponent<Text>().text.ToString();

                if (four)
                    answer = GameObject.Find("TextChoice4").GetComponent<Text>().text.ToString();

                bool isCorrect = CheckAnswer(game.getCurrentLangObject(), answer);
                UpdateScore(isCorrect);

                if (isCorrect)
                    GameObject.Find("CorrectMsg").GetComponent<Text>().text = "¡CORRECTO!";
                else
                    GameObject.Find("IncorrectMsg").GetComponent<Text>().text = "INCORRECTO...";

                alreadyAnswered = true;
            }
        }
    }

    public void NextQuestion()
    {
        System.Random rnd = new System.Random();
        int pos = rnd.Next(0, langItemKeys.Count);
        
        LangObject langObject = langItemKeys[pos] as LangObject;

        game.setCurrentLangObject(langObject);
        
        GameObject gameObject = game.InitializeObject();

        game.setCurrentGameObject(gameObject);

        game.LoadAudio(gameObject, langObject, selectedLang);
        game.PlayAudio(gameObject);

        game.InitializeChoicesText((ArrayList)langItems[langObject]);

        GameObject.Find("CorrectMsg").GetComponent<Text>().text = "";
        GameObject.Find("IncorrectMsg").GetComponent<Text>().text = "";

        alreadyAnswered = false;

        langItemKeys.RemoveAt(pos);
    }

    public bool CheckAnswer(LangObject langObject, string answer)
    {
        string correctAnswer = langObject.getLang(selectedLang)["text"].ToString();

        return answer == correctAnswer;
    }

    public string GetSelectedLang()
    {
        return selectedLang;
    }

    public void UpdateScore(bool isAnswerCorrect)
    {
        if (isAnswerCorrect)
            correctAnswers++;
        else
            incorrectAnswers++;

        Debug.Log("CORRECT/INCORRECT " + correctAnswers + "/" + incorrectAnswers);

        GameObject.Find("Correct").GetComponent<Text>().text = "ACIERTOS: " + correctAnswers;
        GameObject.Find("Incorrect").GetComponent<Text>().text = "FALLOS: " + incorrectAnswers;
    }

    public bool AreThereMoreQuestions()
    {
        return langItemKeys.Count > 0;
    }

    public int GetCorrectAnswers()
    {
        return correctAnswers;
    }

    public int GetIncorrectAnswers()
    {
        return incorrectAnswers;
    }

    public void FinishLevel()
    {
        game.GameFinishedScreen(correctAnswers, correctAnswers + incorrectAnswers);

        gameFinished = true;
        correctAnswers = 0;
        incorrectAnswers = 0;
    }

    public void PlayAgain()
    {
        Application.LoadLevel("EasyMode");
    }

    private void InitializeLangItems()
	{
        ArrayList allObjects = game.getObjects().Clone() as ArrayList;

        System.Random rnd = new System.Random ();
		for (int i = 0; i < game.numberOfQuestions; i++)
        {
            int pos = rnd.Next(0, allObjects.Count);

            LangObject langObject = allObjects[pos] as LangObject;

            ArrayList choices = GenerateChoices(langObject);
            langItems.Add(langObject, choices);

            allObjects.RemoveAt(pos);
        }
    }

    private ArrayList GenerateChoices(LangObject o)
    {
        ArrayList allObjects = game.getObjects().Clone() as ArrayList;
        ArrayList choices = new ArrayList();
       
        string correctAnswer = o.getLang(selectedLang)["text"] as string;
        choices.Add(correctAnswer);

        for (int i = 0; i < allObjects.Count; i++)
        {
            LangObject x = allObjects[i] as LangObject;
            if (x.getName() == o.getName()) {
                allObjects.RemoveAt(i);
                break;
            }
        }

        System.Random rnd = new System.Random();
        for (int i = 0; i < game.numberOfChoices - 1; i++)
        {
            int pos = rnd.Next(0, allObjects.Count);

            LangObject langObject = allObjects[pos] as LangObject;

            string wrongAnswer = langObject.getLang(selectedLang)["text"] as string;
            choices.Add(wrongAnswer);

            allObjects.RemoveAt(pos);
        }

        return game.Shuffle(choices);
    }
}
