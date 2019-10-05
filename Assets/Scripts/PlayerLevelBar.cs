using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelBar : MonoBehaviour {

	[Header("References")]
	public Text level;
	public Text exp;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		int expPoint = PlayerPrefs.GetInt("exp");
		exp.text = ""+ expPoint;
		level.text = ""+ (expPoint/1000 +1);
	}
}
