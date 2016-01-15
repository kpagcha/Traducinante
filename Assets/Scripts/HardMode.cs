using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HardMode : MonoBehaviour {

    private Game game;
    private Hashtable langItems = new Hashtable();
    private ArrayList langItemKeys = new ArrayList();
    private Hashtable correctAnswersHash = new Hashtable();

    private int correctAnswers;
    private int incorrectAnswers;
    private bool alreadyAnswered;

    void Start()
    {
        if (langItemKeys.Count == 0)
        {
            game = GameObject.Find("_GameController").GetComponent<Game>();

            correctAnswers = 0;
            incorrectAnswers = 0;

            InitializeLangItems();

            langItemKeys = new ArrayList(langItems.Keys);

            NextQuestion();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (langItemKeys.Count > 0)
            {
                if (alreadyAnswered)
                    NextQuestion();
            }
            else
                FinishLevel();
        }

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

    public void NextQuestion()
    {
        System.Random rnd = new System.Random();
        int pos = rnd.Next(0, langItemKeys.Count);

        LangObject langObject = langItemKeys[pos] as LangObject;

        game.setCurrentLangObject(langObject);

        GameObject gameObject = game.InitializeObject();

        game.setCurrentGameObject(gameObject);

        game.LoadAudio(gameObject, langObject, langStr);
        game.PlayAudio(gameObject);

        game.InitializeChoicesText((ArrayList)langItems[langObject]);

        GameObject.Find("CorrectMsg").GetComponent<Text>().text = "";
        GameObject.Find("IncorrectMsg").GetComponent<Text>().text = "";

        alreadyAnswered = false;

        langItemKeys.RemoveAt(pos);
    }

    private bool CheckAnswer(LangObject langObject, string answer)
    {
        ArrayList langs = langObject.getLangs();

        for (int i = 0; i < langs.Count; i++)
        {
            Hashtable lang = langs[i] as Hashtable;
            if (lang["text"].ToString() == answer)
                return true;
        }

        return false;
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
        // POR HACER: mostrar pantalla resumen y con botones para repetir nivel o ir al menú principal

        correctAnswers = 0;
        incorrectAnswers = 0;
        Application.LoadLevel("HardMode"); // TEMPORAL
    }

    private void InitializeLangItems()
    {
        ArrayList allObjects = game.getObjects().Clone() as ArrayList;

        System.Random rnd = new System.Random();
        for (int i = 0; i < game.numberOfQuestions; i++)
        {
            int pos = rnd.Next(0, allObjects.Count);

            LangObject langObject = allObjects[pos] as LangObject;

            ArrayList langs = langObject.getLangs() as ArrayList;
            string correctAnswer = langObject.getLang((string)langs[rnd.Next(0, langs.Count)])["text"] as string;

            correctAnswersHash.Add(langObject, correctAnswer);

            ArrayList choices = GenerateChoices(langObject);
            langItems.Add(langObject, choices);

            allObjects.RemoveAt(pos);
        }
    }

    private ArrayList GenerateChoices(LangObject o)
    {
        ArrayList allObjects = game.getObjects().Clone() as ArrayList;
        ArrayList choices = new ArrayList();

        System.Random rnd = new System.Random();

        ArrayList langs = o.getLangs() as ArrayList;
        string correctAnswer = o.getLang((string)langs[rnd.Next(0, langs.Count)])["text"] as string;
        choices.Add(correctAnswersHash[o]);

        for (int i = 0; i < allObjects.Count; i++)
        {
            LangObject x = allObjects[i] as LangObject;
            if (x.getName() == o.getName())
            {
                allObjects.RemoveAt(i);
                break;
            }
        }

        for (int i = 0; i < game.numberOfChoices - 1; i++)
        {
            int pos = rnd.Next(0, allObjects.Count);

            LangObject langObject = allObjects[pos] as LangObject;

            string wrongAnswer = langObject.getLang((string)langs[rnd.Next(0, langs.Count)])["text"] as string;
            choices.Add(wrongAnswer);

            allObjects.RemoveAt(pos);
        }

        return game.Shuffle(choices);
    }
}
