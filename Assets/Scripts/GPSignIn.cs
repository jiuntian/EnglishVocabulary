using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GPSignIn : MonoBehaviour {

	private static Text signInButtonText;
    private static Text authStatus;
	
	public GameObject levelBar;

	void Start () {
		signInButtonText =
            GameObject.Find("Loginbutton").GetComponentInChildren<Text>();
        authStatus = GameObject.Find("authStatus").GetComponentInChildren<Text>();
		PlayGamesPlatform.Instance.Authenticate(SignInCallback, true);
	}

	public static void PreparePlayService () {
        PlayGamesClientConfiguration config = 
        new PlayGamesClientConfiguration.Builder().Build ();
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.InitializeInstance (config);
        PlayGamesPlatform.Activate ();
		PlayGamesPlatform.Instance.Authenticate(SignInCallback, true);
    }

	public void SignIn () {
		if (!PlayGamesPlatform.Instance.localUser.authenticated) {
            PlayGamesPlatform.Instance.Authenticate(SignInCallback, false);
        } else {
            PlayGamesPlatform.Instance.SignOut();
            
            signInButtonText.text = "Sign In";
            authStatus.text = "";
        }
	}

	public static void SignInCallback(bool success) {
        if (success) {
            // Change sign-in button text
            signInButtonText.text = "Sign out";
            // Show the user's name
            authStatus.text = "Signed in as: " + Social.localUser.userName;
        } else {
            // Show failure message
            signInButtonText.text = "Sign In";
            authStatus.text = "Sign-in failed";
        }
    }

	public void ShowAchievements() {
        if (PlayGamesPlatform.Instance.localUser.authenticated) {
            PlayGamesPlatform.Instance.ShowAchievementsUI();
        }
    }
	
	public void ShowLeaderboards() {
        if (PlayGamesPlatform.Instance.localUser.authenticated) {
            PlayGamesPlatform.Instance.ShowLeaderboardUI();
        }
    }
}
