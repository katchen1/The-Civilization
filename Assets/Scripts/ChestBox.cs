using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Doublsb.Dialog;

public class ChestBox : MonoBehaviour
{
    public GameObject player;
    public GameObject warning;
    public DialogManager dm;
    public GameObject puzzle;
    public int chestNumber;
    public Button[] puzzleButtons;
    public Button submitButton;
    public ChestBox nextChest;
    public GameObject image;
    public GameObject cam;
    public GameObject musicController;

    private bool locked;
    private bool solved;
    private Animator animator;
    private string[] choices;
    private int[] choiceIndices;

    void Start()
    {
        locked = true;
        solved = false;
        animator = GetComponent<Animator>();
        animator.enabled = false;
        SetUpPuzzle();
    }

    void Update()
    {
        // If player gets close enough, indicate that it is interactable
        float d = Mathf.Abs(player.transform.position.x - transform.position.x);
        if (d < 3f)
        {
            warning.SetActive(true);
            // If player interacts with the chest
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (locked)
                {
                    // Display message if it's locked
                    dm.Show(new DialogData("/color:red/This treasure chest is locked.", "None"));
                }
                else{
                    if (!solved)
                    {
                        // Show puzzle if it's unlocked and unsolved
                        ShowPuzzle();
                    }
                    else
                    {
                        // Animate or unanimate the puzzle if it's unlocked and solved
                        if (animator.enabled) Unanimate();
                        else Animate();
                    }
                }
            }  
        }
        else{
           warning.SetActive(false); 
        }

        // If ESC is pressed, hide puzzle
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            puzzle.SetActive(false);
        }
    }

    // Set up the corresponding puzzle for this chest box
    void SetUpPuzzle()
    {
        // Answer choices to select from
        if (chestNumber == 1) choices = new string[]{"art", "pencil", "function", "bag", "design", "human", "sublime", "form"};
        else if (chestNumber == 2) choices = new string[]{"constraints", "aesthetics", "interactions"};
        else if (chestNumber == 3) choices = new string[]{"substance", "interaction", "personality", "illusion", "sound", "constraints", "graphics"};
        else if (chestNumber == 4) choices = new string[]{"good", "bad", "neutral", "good and bad", "good, bad, and neutral"};
        else if (chestNumber == 5) choices = new string[]{"texture", "frequency", "timbre", "harmony", "dynamics", "spatialization", "rhythm"};
        else if (chestNumber == 6) choices = new string[]{"design", "art", "music", "human", "sublime"};

        // Initialize selections
        choiceIndices = new int[puzzleButtons.Length];
        for (int i = 0; i < puzzleButtons.Length; i++)
        {
            choiceIndices[i] = i;
            puzzleButtons[i].GetComponentInChildren<Text>().text = choices[choiceIndices[i]];
        }
        
        // Set up button listeners
        if (puzzleButtons.Length >= 1) puzzleButtons[0].onClick.AddListener(() => PuzzleButtonOnClick(0));
        if (puzzleButtons.Length >= 2) puzzleButtons[1].onClick.AddListener(() => PuzzleButtonOnClick(1));
        if (puzzleButtons.Length >= 3) puzzleButtons[2].onClick.AddListener(() => PuzzleButtonOnClick(2));
        if (puzzleButtons.Length >= 4) puzzleButtons[3].onClick.AddListener(() => PuzzleButtonOnClick(3));
        if (puzzleButtons.Length >= 5) puzzleButtons[4].onClick.AddListener(() => PuzzleButtonOnClick(4));
        submitButton.onClick.AddListener(() => SubmitButtonOnClick());
    }

    // When a puzzle button is clicked, show the next answer choice
    void PuzzleButtonOnClick(int i)
    {
        choiceIndices[i] += 1;
        if (choiceIndices[i] >= choices.Length) choiceIndices[i] = 0;
        puzzleButtons[i].GetComponentInChildren<Text>().text = choices[choiceIndices[i]];
    }

    // When the submit button is clicked, check if the answers are correct
    // If correct, unlock the next chest, otherwise, lose 30 seconds
    void SubmitButtonOnClick()
    {
        if (chestNumber == 1)
        {
            string text1 = puzzleButtons[0].GetComponentInChildren<Text>().text;
            string text2 = puzzleButtons[1].GetComponentInChildren<Text>().text;
            if ((text1 == "function" && text2 == "form") || (text1 == "form" && text2 == "function")) UnlockNextChest();
            else HandleWrongAnswer();
        }
        else if (chestNumber == 2)
        {
            string text1 = puzzleButtons[0].GetComponentInChildren<Text>().text;
            string text2 = puzzleButtons[1].GetComponentInChildren<Text>().text;
            string text3 = puzzleButtons[2].GetComponentInChildren<Text>().text;
            if (text1 == "aesthetics" && text2 == "interactions" && text3 == "constraints") UnlockNextChest();
            else HandleWrongAnswer();
        }
        else if (chestNumber == 3)
        {
            string text1 = puzzleButtons[0].GetComponentInChildren<Text>().text;
            string text2 = puzzleButtons[1].GetComponentInChildren<Text>().text;
            string text3 = puzzleButtons[2].GetComponentInChildren<Text>().text;
            string[] toCheck = {"sound", "graphics", "interaction"};
            int a = Array.IndexOf(toCheck, text1);
            int b = Array.IndexOf(toCheck, text2);
            int c = Array.IndexOf(toCheck, text3);
            if (text1 != text2 && text1 != text3 && text2 != text3 && a != -1 && b != -1 && c != -1) UnlockNextChest();
            else HandleWrongAnswer();
        }
        else if (chestNumber == 4)
        {
            string text1 = puzzleButtons[0].GetComponentInChildren<Text>().text;
            if (text1 == "good, bad, and neutral") UnlockNextChest();
            else HandleWrongAnswer();
        }
        else if (chestNumber == 5)
        {
            string text1 = puzzleButtons[0].GetComponentInChildren<Text>().text;
            string text2 = puzzleButtons[1].GetComponentInChildren<Text>().text;
            string text3 = puzzleButtons[2].GetComponentInChildren<Text>().text;
            string text4 = puzzleButtons[3].GetComponentInChildren<Text>().text;
            if (text1 == "frequency" && text2 == "dynamics" && text3 == "rhythm" && text4 == "frequency") UnlockNextChest();
            else HandleWrongAnswer();
        }
        else if (chestNumber == 6)
        {
            string text1 = puzzleButtons[0].GetComponentInChildren<Text>().text;
            if (text1 == "sublime") UnlockNextChest();
            else HandleWrongAnswer(); 
        }
    }

    // Unlocks the next chest
    void UnlockNextChest()
    {
        puzzle.SetActive(false);
        solved = true;
        Animate();
        if (nextChest != null) nextChest.Unlock();
    }

    // Lose 30 seconds
    void HandleWrongAnswer()
    {
        cam.GetComponent<MainScript>().MinusTime();
    }

    // Make the puzzle visible
    public void ShowPuzzle()
    {
        puzzle.SetActive(true);
    }

    // Make the puzzle locked
    public void Lock()
    {
        locked = true;
    }

    // Unlock the puzzle
    public void Unlock()
    {
        locked = false;
    }

    // Animate the chest box to open and close
    public void Animate()
    {
        animator.enabled = true;
        if (image != null) image.SetActive(true);
        
        // Edit music
        string attribute = "";
        if (chestNumber == 1) attribute = "frequency1";
        else if (chestNumber == 2) attribute = "dynamics";
        else if (chestNumber == 3) attribute = "rhythm1";
        else if (chestNumber == 4) attribute = "frequency2";
        else if (chestNumber == 5) attribute = "rhythm2";
        else if (chestNumber == 6) attribute = "harmony";
        musicController.GetComponent<MusicController>().EditMusic(attribute, 1);
    }

    // Un-animate the chest box so that it's not moving
    public void Unanimate()
    {
        animator.enabled = false;
        if (image != null) image.SetActive(false);

        // Edit music
        string attribute = "";
        if (chestNumber == 1) attribute = "frequency1";
        else if (chestNumber == 2) attribute = "dynamics";
        else if (chestNumber == 3) attribute = "rhythm1";
        else if (chestNumber == 4) attribute = "frequency2";
        else if (chestNumber == 5) attribute = "rhythm2";
        else if (chestNumber == 6) attribute = "harmony";
        musicController.GetComponent<MusicController>().EditMusic(attribute, 0);
    }

    public bool IsSolved()
    {
        return solved;
    }
}
