using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public void OnMouseDown()
    {
        Application.LoadLevel("GamePlay");
        print("clicked play");
    }
}
