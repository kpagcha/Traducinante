using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class EasyMode : MonoBehaviour {

	private Game game;
	private Hashtable langItems = new Hashtable();
    private ArrayList langItemKeys = new ArrayList();

    private string selectedLang;
	private int correctAnswers;
	private int incorrectAnswers;

    private bool gameFinished = false;
	private string selectedAnswer;
	private bool finalAnswerConfirmed = false;
	private bool firstQuestion = true;
	private bool waitingForNextQuestion = false;

	private GameObject confirmAnswerButton;
	private GameObject nextQuestionButton;

	void Start()
	{
        game = GameObject.Find("_GameController").GetComponent<Game>();

        selectedLang = "Russian";
        correctAnswers = 0;
        incorrectAnswers = 0;

        InitializeLangItems();

        langItemKeys = new ArrayList(langItems.Keys);

		confirmAnswerButton = GameObject.Find ("ConfirmAnswerButton");
		nextQuestionButton = GameObject.Find ("NextQuestionButton");
		confirmAnswerButton.SetActive (false);
		nextQuestionButton.SetActive (false);
    }

    void Update()
    {
        if (gameFinished) {
			FinishLevel ();
			gameFinished = false;
		} 
		else if (firstQuestion) 
		{
			if (game.GetImageTargetFound())
			{
				NextQuestion();
				firstQuestion = false;
			}
		}
		else if (waitingForNextQuestion && game.GetImageTargetFound())
		{
			NextQuestion();
				waitingForNextQuestion = false;
		}
        else {
            if (finalAnswerConfirmed)
			{
				if (langItemKeys.Count == 0)
					gameFinished = true;
				else
					nextQuestionButton.SetActive(true);
			}
			else
			{
				string answer = game.GetSelectedAnswer();
				
				if (answer != null && answer != "")
				{
					selectedAnswer = answer;
					GameObject.Find ("SelectedAnswer").GetComponent<Text>().text = selectedAnswer;
					confirmAnswerButton.SetActive(true);
				}
			}
        }
    }

	public void ConfirmAnswer()
	{
		if (!finalAnswerConfirmed)
		{
			bool isCorrect = CheckAnswer(game.getCurrentLangObject(), selectedAnswer);
			UpdateScore(isCorrect);
			
			if (isCorrect)
				GameObject.Find("CorrectMsg").GetComponent<Text>().text = "¡CORRECTO!";
			else
				GameObject.Find("IncorrectMsg").GetComponent<Text>().text = "INCORRECTO...";
			
			finalAnswerConfirmed = true;
		}
	}

    public void NextQuestion()
    {
		ResetSceneElements ();

        System.Random rnd = new System.Random();
        int pos = rnd.Next(0, langItemKeys.Count);
        
        LangObject langObject = langItemKeys[pos] as LangObject;

        game.setCurrentLangObject(langObject);
        
        GameObject gameObject = game.InitializeObject();

        game.setCurrentGameObject(gameObject);

        game.LoadAudio(gameObject, langObject, selectedLang);
        game.PlayAudio(gameObject);

        game.InitializeChoicesText((ArrayList)langItems[langObject]);

        langItemKeys.RemoveAt(pos);
    }

	public void WaitingForNextQuestion()
	{
		waitingForNextQuestion = true;
		ResetSceneElements ();
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
		ResetSceneElements ();

        game.GameFinishedScreen(correctAnswers, correctAnswers + incorrectAnswers);

        correctAnswers = 0;
        incorrectAnswers = 0;

		game.playAudioClipButton.SetActive (false);
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

	private void ResetSceneElements()
	{
		GameObject.Find ("CorrectMsg").GetComponent<Text> ().text = "";
		GameObject.Find ("IncorrectMsg").GetComponent<Text> ().text = "";
		GameObject.Find ("SelectedAnswer").GetComponent<Text> ().text = "";
		confirmAnswerButton.SetActive (false);
		nextQuestionButton.SetActive (false);
		EventSystem.current.SetSelectedGameObject (null);
		game.playAudioClipButton.SetActive (false);
		GameObject.Find ("SelectedAnswer").GetComponent<Text>().text = "";
		confirmAnswerButton.SetActive(false);
		for (int i = 0; i < 4; i++)
			GameObject.Find("TextChoice" + (i + 1)).GetComponent<Text>().text = "";

		finalAnswerConfirmed = false;
		selectedAnswer = "";
		game.SetSelectedAnswer ("");
	}
}
