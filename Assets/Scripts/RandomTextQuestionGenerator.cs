using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;


public class RandomTextQuestionGenerator
{
    private List<TextQuestion> listTextQuestions;
	private ArrayList index;
    public RandomTextQuestionGenerator()
    {
        listTextQuestions = new List<TextQuestion>();
		index = new ArrayList ();
        InitQuestions();
    }

    public TextQuestion GetRandomTextQuestion()
    {
        int randomIndex = Random.Range(0, listTextQuestions.Count);
		Debug.Log (listTextQuestions.Count);
        TextQuestion textQuestion = listTextQuestions[randomIndex];
        listTextQuestions.RemoveAt(randomIndex);
        return textQuestion;
    }

    public void AddTextQuestion(TextQuestion textQuestion)
    {
        listTextQuestions.Add(textQuestion);
    }

    public void InitQuestions()
    {
        Dictionary<string, string[]> dataDict = new Dictionary<string, string[]> ();
        WordData.UpdateData(dataDict);
        foreach(KeyValuePair<string, string[]> data in dataDict){
            AddTextQuestion(new TextQuestion(data.Value[0],data.Key));
        }
    }
}

