using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GPSignIn : MonoBehaviour {

	private Text signInButtonText;
    private Text authStatus;
	private GameObject achievementButton;

	// Use this for initialization
	void Start () {
		signInButtonText =
            GameObject.Find("Loginbutton").GetComponentInChildren<Text>();
        authStatus = GameObject.Find("authStatus").GetComponentInChildren<Text>();
		PlayGamesPlatform.Instance.Authenticate(SignInCallback, true);
		achievementButton = GameObject.Find("AchievementButton");
	}
	
	// Update is called once per frame
	void Update () {
		achievementButton.SetActive(Social.localUser.authenticated);
	}

	public void SignIn () {
		if (!PlayGamesPlatform.Instance.localUser.authenticated) {
            // Sign in with Play Game Services, showing the consent dialog
            // by setting the second parameter to isSilent=false.
            PlayGamesPlatform.Instance.Authenticate(SignInCallback, false);
        } else {
            // Sign out of play games
            PlayGamesPlatform.Instance.SignOut();
            
            // Reset UI
            signInButtonText.text = "Sign In";
            authStatus.text = "";
        }
	}

	public void SignInCallback(bool success) {
        if (success) {
            Debug.Log("(Elingo) Signed in!");
            
            // Change sign-in button text
            signInButtonText.text = "Sign out";
            
            // Show the user's name
            authStatus.text = "Signed in as: " + Social.localUser.userName;
        } else {
            Debug.Log("(Lollygagger) Sign-in failed...");
            
            // Show failure message
            signInButtonText.text = "Sign in";
            authStatus.text = "Sign-in failed";
        }
    }

	public void ShowAchievements() {
        if (PlayGamesPlatform.Instance.localUser.authenticated) {
            PlayGamesPlatform.Instance.ShowAchievementsUI();
        }
        else {
          Debug.Log("Cannot show Achievements, not logged in");
        }
    }
	
}
