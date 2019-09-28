using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class WordHunt : MonoBehaviour {

    public static WordHunt instance;

    public delegate void VisualEvents(RectTransform original, RectTransform final);
    public static event VisualEvents FoundWord;

    public delegate void Events();
    public static event Events Finish;

    private string[,] lettersGrid;
    private Transform[,] lettersTransforms;
    private string alphabet = "abcdefghijklmnopqrstuvwxyz";
    
    private Dictionary<string, string[] > dataDict = new Dictionary<string, string[]>();

    [Header("Settings")]
    public bool invertedWordsAreValid;

    [Header("Text Asset")]
    public TextAsset wordsSource;
    public bool filterBadWords;
    public TextAsset badWordsSource;
    [Space]

    [Header("List of Words")]
    public List<string> words = new List<string>();
    public List<string> insertedWords = new List<string>();
    [Header("Grid Settings")]
    public Vector2 gridSize;
    [Space]

    [Header("Cell Settings")]
    public Vector2 cellSize;
    public Vector2 cellSpacing;
    [Space]

    [Header("Public References")]
    public GameObject letterPrefab;
    public Transform gridTransform;
    [Space]

    [Header("Game Detection")]
    public string word;
    public Vector2 orig;
    public Vector2 dir;
    public bool activated;
    [Space]

    [Header("Game Dialog")]
    public GameObject dialog;
    public Transform dialog_transform;
    public Transform congratulation;
    public Text vocab;
    public Text malay;
    public Text chinese;
    public Text arabic;
    public Text sentences;
    // TODO: image
    [Space]

    [HideInInspector]
    public List<Transform> highlightedObjects = new List<Transform>();

    private void Awake()
    {
        instance = this;
    }

    public void Setup(){

        PrepareDialog();
        
        PrepareWords();

        InitializeGrid();

        InsertWordsOnGrid();

        RandomizeEmptyCells();

        DisplaySelectedWords();

    }

    private void PrepareDialog()
    {
        dataDict.Add("ruler", new string[] {
"a straight strip, typically marked at regular intervals, to draw straight lines or measure distances.",
"penguasa",
"尺",
"مسطرة",
"Draw a line with a pencil and a ruler",
"",
});dataDict.Add("desk", new string[] {
"a piece of furniture with a flat surface and typically with drawers, at which one can read, write, or do other work.",
"meja",
"桌子",
"مكتب",
"He sat at his desk, reading reports",
"",
});dataDict.Add("ugly", new string[] {
"unpleasant appearance, behaviour or sound.",
"hodoh",
"丑陋",
"قبيح",
"The donky sound is ugly",
"",
});dataDict.Add("smart", new string[] {
"having or showing intelligence.",
"pintar",
"聪明",
"ذكي",
"Sara is smart girl in her class",
"",
});dataDict.Add("race", new string[] {
"compete with another or others to see who is fastest at doing things",
"bangsa",
"种族",
"سباق",
"That horse won the race and golden prize",
"",
});dataDict.Add("listen", new string[] {
"give attention to hear someone",
"mendengar",
"听",
"استمع",
"If you listen to your mother, you will not cry again",
"",
});dataDict.Add("look", new string[] {
"an act of directing your sight in order to see someone or something.",
"lihat",
"看",
"انظر",
"Nancy likes to look at the flowers everyday",
"",
});dataDict.Add("close", new string[] {
"to bring or bind together the parts or edges of something",
"tutup",
"关",
"اغلق",
"Close your book while taking an exam",
"",
});dataDict.Add("catch", new string[] {
"hold something that has been thrown or dropped",
"tangkapan",
"抓住",
"قبض على",
"He was unable to catch to catch the ball ",
"",
});dataDict.Add("teacher", new string[] {
"a person who teaches, especially in a school.",
"guru",
"老师",
"معلم",
"Nora loves her art teacher",
"",
});dataDict.Add("number", new string[] {
"an arithmetical value, expressed by a word, symbol, or figure, representing a particular quantity ",
"nombor",
"数",
"رقم",
"Nouns are singular or plural in number",
"",
});dataDict.Add("play", new string[] {
"engage in activity for enjoyment and recreation",
"bermain",
"玩",
"يلعب",
"I love to play football",
"",
});dataDict.Add("guess", new string[] {
"estimate or suppose (something) without sufficient information ",
"meneka",
"猜测",
"خمن",
"can you guess what is the name of this cartoon charachter?",
"",
});dataDict.Add("turn", new string[] {
"an act of moving something in a circular direction around an axis or point.",
"berpaling",
"转",
"منعطف او دوران",
"You have to turn right to reach your school",
"",
});dataDict.Add("around", new string[] {
"standing or being on every side of something",
"sekitar",
"周围",
"حول",
"The family sit around the table to eat their meals",
"",
});dataDict.Add("stretch", new string[] {
"an act of stretching one's limbs or body.",
"regangan",
"伸展",
"تمدد",
"Stretching exercises makes your body flexible",
"",
});dataDict.Add("tick", new string[] {
"mark (an item) with a check mark, typically to show that it has been chosen",
"semak",
"蜱",
"علامة",
"I always tick the tasks that I have finished",
"",
});dataDict.Add("cross", new string[] {
"a mark, object, or figure formed by two short intersecting lines or pieces (+ or ×).",
"menyeberang",
"交叉",
"يعبر",
"you can cross the street when the traffic light is green in color",
"",
});
    }

    private void PrepareWords()
    {
        //get words
        words = wordsSource.text.Split(',').ToList();

        //filter bad word
        if (filterBadWords)
        {
            List<string> badWords = badWordsSource.text.Split(',').ToList();
            for (int i = 0; i < badWords.Count(); i++)
            {
                if(words.Contains(badWords[i])){
                    words.Remove(badWords[i]);
                    print("Offensive badWord <b>" + badWords[i] + "</b> <color=red> removed</color>");
                }
            }
        }

        //randomize the words
        for (int i = 0; i < words.Count; i++)
        {
            string temp = words[i];

            System.Random rn = new System.Random();

            int randomIndex = rn.Next(words.Count());
            words[i] = words[randomIndex];
            words[randomIndex] = temp;
        }

        //max grid size
        int maxGridDimension = Mathf.Max((int)gridSize.x, (int)gridSize.y);

        //make sure no big than grid
        words = words.Where(x => x.Length <= maxGridDimension).ToList();
    }

    private void InitializeGrid()
    {

        //initialize letter grid
        lettersGrid = new string[(int)gridSize.x, (int)gridSize.y];
        lettersTransforms = new Transform[(int)gridSize.x, (int)gridSize.y];

        //
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {

                lettersGrid[x, y] = "";

                GameObject letter = Instantiate(letterPrefab, transform.GetChild(0));

                letter.name = x.ToString() + "-" + y.ToString();

                lettersTransforms[x, y] = letter.transform;

            }
        }

        ApplyGridSettings();
    }

    void ApplyGridSettings()
    {
        GridLayoutGroup gridLayout = gridTransform.GetComponent<GridLayoutGroup>();

        gridLayout.cellSize = cellSize;
        gridLayout.spacing = cellSpacing;

        int cellSizeX = (int)gridLayout.cellSize.x + (int)gridLayout.spacing.x;

        transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(cellSizeX * gridSize.x, 0);


    }

    void InsertWordsOnGrid()
    {
        foreach (string word in words)
        {

            System.Random rn = new System.Random();

            bool inserted = false;
            int tryAmount = 0;

            do
            {
                int row = rn.Next((int)gridSize.x);
                int column = rn.Next((int)gridSize.y);

                int dirX = 0; int dirY = 0;

                while (dirX == 0 && dirY == 0)
                {
                    if (invertedWordsAreValid)
                    {
                        dirX = rn.Next(3) - 1;
                        dirY = rn.Next(3) - 1;
                    }else{
                        dirX = rn.Next(2);
                        dirY = rn.Next(2);
                    }
                }

                inserted = InsertWord(word, row, column, dirX, dirY);
                tryAmount++;

            } while (!inserted && tryAmount < 100);

            if (inserted)
                insertedWords.Add(word);
        }
    }

    private bool InsertWord(string word, int row, int column, int dirX, int dirY)
    {

        if (!CanInsertWordOnGrid(word, row, column, dirX, dirY))
            return false;

        for (int i = 0; i < word.Length; i++)
        {
            lettersGrid[(i * dirX) + row, (i * dirY) + column] = word[i].ToString();
            Transform t = lettersTransforms[(i * dirX) + row, (i * dirY) + column];
            t.GetComponentInChildren<Text>().text = word[i].ToString().ToUpper();
            //t.GetComponent<Image>().color = Color.grey;
        }

        return true;
    }

    private bool CanInsertWordOnGrid(string word, int row, int column, int dirX, int dirY)
    {
        if (dirX > 0)
        {
            if (row + word.Length > gridSize.x)
            {
                return false;
            }
        }
        if (dirX < 0)
        {
            if (row - word.Length < 0)
            {
                return false;
            }
        }
        if (dirY > 0)
        {
            if (column + word.Length > gridSize.y)
            {
                return false;
            }
        }
        if (dirY < 0)
        {
            if (column - word.Length < 0)
            {
                return false;
            }
        }

        for (int i = 0; i < word.Length; i++)
        {
            string currentCharOnGrid = (lettersGrid[(i * dirX) + row, (i * dirY) + column]);
            string currentCharOnWord = (word[i].ToString());

            if (currentCharOnGrid != String.Empty && currentCharOnWord != currentCharOnGrid)
            {
                return false;
            }
        }

        return true;
    }

    private void RandomizeEmptyCells()
    {

        System.Random rn = new System.Random();

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (lettersGrid[x, y] == string.Empty)
                {
                    lettersGrid[x, y] = alphabet[rn.Next(alphabet.Length)].ToString();
                    lettersTransforms[x, y].GetComponentInChildren<Text>().text = lettersGrid[x, y].ToUpper();
                }
            }
        }
    }

    public void LetterClick(int x, int y, bool state)
    {
        activated = state;
        orig = state ? new Vector2(x, y) : orig;
        dir = state ? dir : new Vector2(-1, -1);

        if (!state)
        {
            ValidateWord();
        }

    }

    private void ValidateWord()
    {
        word = string.Empty;

        foreach (Transform t in highlightedObjects)
        {
            word += t.GetComponentInChildren<Text>().text.ToLower();
        }

        if(insertedWords.Contains(word) || insertedWords.Contains(Reverse(word)))
        {
            foreach (Transform h in highlightedObjects)
            {
                h.GetComponent<Image>().color = Color.white;
                h.transform.DOPunchScale(-Vector3.one, 0.2f, 10, 1);
            }

            //Visual Event
            RectTransform r1 = highlightedObjects[0].GetComponent<RectTransform>();
            RectTransform r2 = highlightedObjects[highlightedObjects.Count() - 1].GetComponent<RectTransform>();
            FoundWord(r1, r2);

            print("<b>" + word.ToUpper() + "</b> was found!");

            GenerateDialog(word);

            ScrollViewWords.instance.CheckWord(word);

            insertedWords.Remove(word);
            insertedWords.Remove(Reverse(word));

            //TODO add dialog here

            dialog.SetActive(true);
            dialog_transform.DOMoveY(500,.6f).SetEase(Ease.OutBack);

            if(insertedWords.Count <= 0)
            {
                Finish();
                StartCoroutine(ShowCongratulationDialog());
            }
        }
        else {
            ClearWordSelection();
        }
    }

    IEnumerator ShowCongratulationDialog()
    {
        print(Time.time);
        congratulation.DOMoveY(500,.6f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(2);
        print(Time.time);
        congratulation.DOMoveY(-500,.6f).SetEase(Ease.OutBack);
    }

    public void GenerateDialog(string word)
    {
        vocab.text = word.ToUpper();
        malay.text = dataDict[word][1];
        chinese.text = dataDict[word][2];
        arabic.text = dataDict[word][3];
        sentences.text = dataDict[word][4];
    }

    public void LetterHover(int x, int y)
    {
        if (activated)
        {
            dir = new Vector2(x, y);
            if (IsLetterAligned(x, y))
            {
                HighlightSelectedLetters(x,y);
            }
        }
    }

    private void HighlightSelectedLetters(int x, int y)
    {

        ClearWordSelection();

        Color selectColor = HighlightBehaviour.instance.colors[HighlightBehaviour.instance.colorCounter];

        if (x == orig.x)
        {
            int min = (int)Math.Min(y, orig.y);
            int max = (int)Math.Max(y, orig.y);

            for (int i = min; i <= max; i++)
            {
                lettersTransforms[x, i].GetComponent<Image>().color = selectColor;
                highlightedObjects.Add(lettersTransforms[x, i]);
            }
        }
        else if (y == orig.y)
        {
            int min = (int)Math.Min(x, orig.x);
            int max = (int)Math.Max(x, orig.x);

            for (int i = min; i <= max; i++)
            {
                lettersTransforms[i, y].GetComponent<Image>().color = selectColor;
                highlightedObjects.Add(lettersTransforms[i, y]);
            }
        }
        else
        {

            // Increment according to direction (left and up decrement)
            int incX = (orig.x > x) ? -1 : 1;
            int incY = (orig.y > y) ? -1 : 1;
            int steps = (int)Math.Abs(orig.x - x);

            // Paints from (orig.x, orig.y) to (x, y)
            for (int i = 0, curX = (int)orig.x, curY = (int)orig.y; i <= steps; i++, curX += incX, curY += incY)
            {
                lettersTransforms[curX, curY].GetComponent<Image>().color = selectColor;
                highlightedObjects.Add(lettersTransforms[curX, curY]);
            }
        }

    }

    private void ClearWordSelection()
    {
        foreach (Transform h in highlightedObjects)
        {
            h.GetComponent<Image>().color = Color.white;
        }

        highlightedObjects.Clear();
    }

    public bool IsLetterAligned(int x, int y)
    {
        return (orig.x == x || orig.y == y || Math.Abs(orig.x - x) == Math.Abs(orig.y - y));
    }

    private void DisplaySelectedWords()
    {
        float delay = 0;

        for (int i = 0; i < insertedWords.Count; i++)
        {
            ScrollViewWords.instance.SpawnWordCell(insertedWords[i], delay);
            delay += .05f;
        }
    }

    public static string Reverse(string s)
    {
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

    public void HideDialog()
    {
        dialog_transform.DOMoveY(-500,.6f).SetEase(Ease.OutBack);
        print("Clicked hide dialog");
        if(insertedWords.Count <= 0){
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }
    }

}
