using UnityEngine;
using System.Collections;
using System.IO;
using LitJson;

public class Game : MonoBehaviour
{
	private ArrayList objects = new ArrayList();
	public int numberOfQuestions = 5;

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

				if (lang != "model") {
					string text = itemJson[lang]["text"].ToString();
					string audio = itemJson[lang]["audio"].ToString();
					langObject.addLang(lang, text, audio);
				}
			}

			objects.Add(langObject);
		}

		ShuffleObjects ();
		LogLangObjects ();
	}

	private void ShuffleObjects()
	{
		ArrayList tmp = objects;
		ArrayList shuffled = new ArrayList();
			
		System.Random rnd = new System.Random ();

		while (tmp.Count != 0) {
			int i = rnd.Next(tmp.Count);
			shuffled.Add(tmp[i]);
			tmp.RemoveAt(i);
		}

		objects = shuffled;
	}

	private void LogLangObjects()
	{
		foreach (LangObject o in objects) {
			Debug.Log(o.getName() + ", " + o.getModel());
			Hashtable ht1 = o.getLang("English");
			Hashtable ht2 = o.getLang("Spanish");
			foreach (string s in ht1.Values) {
				Debug.Log(s);
			}
			foreach (string s in ht2.Values) {
				Debug.Log(s);
			}
			Debug.Log("---------------");
		}
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