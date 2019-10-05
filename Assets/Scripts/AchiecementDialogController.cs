using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AchiecementDialogController : MonoBehaviour {

	[Header("References")]
	public Transform achievementDialog;
	public Button hideButton;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ShowPopUp(){
		achievementDialog.DOMoveY (500, .6f).SetEase (Ease.OutBack);
	}

	public void HidePopUp(){
		achievementDialog.DOMoveY (-500, .6f).SetEase (Ease.OutBack);
	}
}
