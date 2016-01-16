using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Vuforia;

public class VirtualButtonEventHandlerEasyMode : MonoBehaviour,
                                         IVirtualButtonEventHandler
{
    private Game game;
    private EasyMode easyMode;

    private string answer;

    void Start()
    {
        VirtualButtonBehaviour[] vbs = GetComponentsInChildren<VirtualButtonBehaviour>();
        for (int i = 0; i < vbs.Length; ++i)
        {
            vbs[i].RegisterEventHandler(this);
        }

        game = GameObject.Find("_GameController").GetComponent<Game>();
        easyMode = GameObject.Find("_GameController").GetComponent<EasyMode>();
    }

    public void OnButtonPressed(VirtualButtonAbstractBehaviour vb)
    {
        switch (vb.VirtualButtonName)
        {
            case "Choice1":
                answer = GameObject.Find("TextChoice1").GetComponent<Text>().text.ToString();
                break;
            case "Choice2":
                answer = GameObject.Find("TextChoice2").GetComponent<Text>().text.ToString();
                break;

            case "Choice3":
                answer = GameObject.Find("TextChoice3").GetComponent<Text>().text.ToString();
                break;

            case "Choice4":
                answer = GameObject.Find("TextChoice4").GetComponent<Text>().text.ToString();
                break;
        }
    }

    public void OnButtonReleased(VirtualButtonAbstractBehaviour vb)
    {
        LangObject langObject = game.getCurrentLangObject();
        string lang = easyMode.GetSelectedLang();

        easyMode.UpdateScore(easyMode.CheckAnswer(langObject, answer));

        if (easyMode.AreThereMoreQuestions())
        {
            easyMode.NextQuestion();
        }
        else
        {
            easyMode.FinishLevel();
        }
    }
}