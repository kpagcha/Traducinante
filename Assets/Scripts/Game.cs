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

			foreach (DictionaryEntry langItem in itemJson)
            {
				string lang = langItem.Key as string;

				if (lang != "model")
                {
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
        gameObject.transform.position = new Vector3(0f, 0f, 0f);
        gameObject.transform.localScale = new Vector3(0.06447458f, 0.06447458f, 0.06447458f);

        AudioSource audioSource = gameObject.AddComponent<AudioSource>();

        return gameObject;
    }

    public void InitializeChoicesText(ArrayList choices)
    {
        for (int i = 0; i < choices.Count; i++)
            GameObject.Find("TextChoice" + (i + 1)).GetComponent<Text>().text = choices[i].ToString();
    }

    public ArrayList getObjects()
	{
		return objects;
	}

    public void setCurrentGameObject(GameObject gameObject)
    {
        Destroy(currentGameObject);
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
			
		System.Random rnd = new System.Random ();

        for (int i = 0; i < 100000; i++)
        {
            var i1 = rnd.Next(tmp.Count);
            var i2 = rnd.Next(tmp.Count);

            var eTemp = tmp[i1];
            tmp[i1] = tmp[i2];
            tmp[i2] = eTemp;
        }
        return tmp;
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

    public ArrayList getLangs()
    {
        return hash.Values as ArrayList;
    }
}