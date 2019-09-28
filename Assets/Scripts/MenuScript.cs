using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {

    private CanvasGroup canvas;

    public GameObject dialog;

    public Button[] buttons;
    public TextAsset[] themes;

    public Transform miniMenu;

    private void Start()
    {
        
        dialog.SetActive(false);

        canvas = GetComponent<CanvasGroup>();
        canvas.alpha = 1;
        print("Button length: "+buttons.Length);
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[index].onClick.AddListener(()=>StartGame(index));
            buttons[index].GetComponentInChildren<Text>().text = themes[index].name.ToUpper();
            print(i+" "+themes[index].name+" "+themes[index]);
        }
    }

    public void StartGame(int index){

        WordHunt.instance.wordsSource = themes[index];
        WordHunt.instance.Setup();

        canvas.alpha = 0;
        canvas.blocksRaycasts = false;

        miniMenu.DOMoveY(0,.6f).SetEase(Ease.OutBack);
        
    }

    public void Home(){
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

}
