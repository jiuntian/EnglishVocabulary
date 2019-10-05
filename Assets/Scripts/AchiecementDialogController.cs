using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GooglePlayGames;

public class AchiecementDialogController : MonoBehaviour {

	[Header("References")]
	public Transform achievementDialog;
	private GameObject achievementButton;
	private Text signInButtonText;

	// Use this for initialization
	void Start () {
		achievementButton = GameObject.Find("PlayAchievementButton");
		signInButtonText =
            GameObject.Find("Loginbutton").GetComponentInChildren<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		achievementButton.SetActive(PlayGamesPlatform.Instance.localUser.authenticated);
		if(PlayGamesPlatform.Instance.localUser.authenticated){
			signInButtonText.text = "Sign out";
		}
	}

	public void ShowPopUp(){
		achievementDialog.DOMoveY (500, .6f).SetEase (Ease.OutBack);
	}

	public void HidePopUp(){
		achievementDialog.DOMoveY (-500, .6f).SetEase (Ease.OutBack);
	}
}
