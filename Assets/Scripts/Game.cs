using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using LitJson;

public class Game : MonoBehaviour
{
	private ArrayList objects = new ArrayList();
	private GameObject currentGameObject;
    private LangObject currentLangObject;
	public int numberOfQuestions = 5;
    public int numberOfChoices = 4;
	
	void Start ()
	{
		string jsonString = File.ReadAllText (Application.dataPath + "/Resources/items.json");
		JsonData jsonData = JsonMapper.ToObject (jsonString);

		foreach (DictionaryEntry item in jsonData) {
			string key = item.Key as string;
			JsonData itemJson = item.Value as JsonData;

			string model = itemJson["model"].ToString() as string;

			LangObject langObject = new LangObject(key, model);

			foreach (DictionaryEntry langItem in itemJson) {
				string lang = langItem.Key as string;

				if (lang != "model")  {
					string text = itemJson[lang]["text"].ToString();
					string audio = itemJson[lang]["audio"].ToString();
					langObject.addLang(lang, text, audio);
				}
			}

			objects.Add(langObject);
		}

		objects = Shuffle (objects);
	}


    public GameObject InitializeObject()
    {
        string path = "Models/" + currentLangObject.getModel();

        GameObject gameObject = Instantiate(Resources.Load(path)) as GameObject;
        gameObject.transform.parent = GameObject.Find("ImageTarget").transform;

        AudioSource audioSource = gameObject.AddComponent<AudioSource>();

        return gameObject;
    }

    public void DestroyObject()
    {
        Destroy(currentGameObject);
    }

    public void InitializeChoices(ArrayList choices)
    {
        for (int i = 0; i < choices.Count; i++)
            GameObject.Find("TextChoice" + (i + 1)).GetComponent<Text>().text = choices[i].ToString();
    }

    public bool CheckAnswer(LangObject langObject, string answer, string language)
    {
        string correctAnswer = langObject.getLang(language)["text"].ToString();

        return answer == correctAnswer;
    }

    public ArrayList getObjects()
	{
		return objects;
	}

    public void setCurrentGameObject(GameObject gameObject)
    {
        currentGameObject = gameObject;
    }

	public GameObject getCurrentGameObject()
	{
		return currentGameObject;
	}

    public void setCurrentLangObject(LangObject langObject)
    {
        currentLangObject = langObject;
    }

	public LangObject getCurrentLangObject()
	{
		return currentLangObject;
	}

	public ArrayList Shuffle(ArrayList list)
	{
		ArrayList tmp = list.Clone() as ArrayList;
		ArrayList shuffled = new ArrayList();
			
		System.Random rnd = new System.Random ();

		while (tmp.Count != 0) {
            int i = rnd.Next(0, tmp.Count - 1);
			shuffled.Add(tmp[i]);
			tmp.RemoveAt(i);
		}

		return shuffled;
    }

    public void LoadAudio(GameObject gameObject, LangObject langObject, string lang)
    {
        string audioPath = "Audios/" + langObject.getLang(lang)["audio"].ToString();
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = Resources.Load(audioPath) as AudioClip;
    }

    public void PlayAudio(GameObject gameObject)
    {
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.Play();
    }
}

public class LangObject
{
	private string name;
	private string model;
	private Hashtable hash = new Hashtable ();
	
	public LangObject(string name, string model)
	{
		this.name = name;
		this.model = model;
	}
	
	public void addLang(string lang, string text, string audio)
	{
		Hashtable attrs = new Hashtable ();
		attrs.Add ("text", text);
		attrs.Add ("audio", audio);
		
		hash.Add (lang, attrs);
	}

	public string getName() {
		return name;
	}

	public string getModel() {
		return model;
	}
	
	public Hashtable getLang(string lang)
	{
		return hash [lang] as Hashtable;
	}
}