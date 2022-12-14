using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Doublsb.Dialog;
using UnityEngine.SceneManagement;
using TMPro;

public class MainScript : MonoBehaviour
{
    public DialogManager dm;
    public GameObject player;
    public ChestBox chest1, chest2, chest3, chest4, chest5, chest6;
    public GameObject stone;
    public GameObject scorePanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI messageText;
    public GameObject musicController;

    private bool allSolved;

    // Timer system
    private float timeRemaining = 60 * 10;
    private bool timerIsRunning = false;
    public TextMeshProUGUI timeText;

    void Start()
    {
        allSolved = false;
        ShowInitialDialog();
    }

    void Update()
    {
        // Camera follows player
        transform.position = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);

        // Check if all puzzles are solved
        if (!allSolved && chest6.IsSolved())
        {
            allSolved = true;
            ShowEndDialogSuccess();
        }

        // Timer system
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                ShowEndDialogFail();
            }
        }
    }

    // Minus 30 seconds
    public void MinusTime()
    {
        timeRemaining -= 30;
        DisplayTime(timeRemaining);
        StartCoroutine(ChangeTimerTextColor());
    }

    // Flashes the timer text red black red black
    IEnumerator ChangeTimerTextColor()
    {
        timeText.color = new Color(255, 0, 0, 255);
        yield return new WaitForSeconds(0.5f);
        timeText.color = new Color(0, 0, 0, 255);
        yield return new WaitForSeconds(0.5f);
        timeText.color = new Color(255, 0, 0, 255);
        yield return new WaitForSeconds(0.5f);
        timeText.color = new Color(0, 0, 0, 255);
        yield return new WaitForSeconds(0.5f);
    }

    // Updates the timer text based on the current remaining time
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60); 
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Introduction to the game
    void ShowInitialDialog()
    {
        List<DialogData> dd = new List<DialogData>();
        dd.Add(new DialogData("I can't believe it, after all these years of searching, I've finally found the fabled Λ Civilization, which holds the key to eternal life!", "Professor Wang"));
        DialogData tmp = new DialogData("I just hope that no one else has discovered it before me...", "Professor Wang");
        tmp.Callback = () => PlayChickenSoundCallback(true);
        dd.Add(tmp);
        dd.Add(new DialogData("Ah, Professor Wang! I see you've discovered the Λ Civilization." , "Chickencer King"));
        dd.Add(new DialogData("Too bad you won't be able to keep it for yourself...", "Chickencer King"));
        tmp = new DialogData("/emote:Anxious/W-what do you want, Chickencer King?", "Professor Wang");
        tmp.Callback = () => PlayChickenSoundCallback(true);
        dd.Add(tmp);
        dd.Add(new DialogData("I want what any archaeologist wants - the treasures of the Λ Civilization, especially the key to eternal life!", "Chickencer King"));
        dd.Add(new DialogData("And I'm not afraid to use force to get it.", "Chickencer King"));
        tmp = new DialogData("/emote:Anxious/But that's not what archaeology is about! It's about discovering and preserving history, not plundering it for personal gain!", "Professor Wang");
        tmp.Callback = () => PlayChickenSoundCallback(true);
        dd.Add(tmp);
        dd.Add(new DialogData("Such noble ideals, Professor Wang, but they won't stop me from taking what I want.", "Chickencer King"));
        tmp = new DialogData("Chickencer Guards, seize him!", "Chickencer King");
        tmp.Callback = () => PlayChickenSoundCallback(false);
        dd.Add(tmp);
        dd.Add(new DialogData("Attack!!! Bok bok bok bok bok bok", "Chickencer Guards"));
        dd.Add(new DialogData("/color:red/The Chickencer Guards move to surround Professor Wang.", "None"));
        dd.Add(new DialogData("/color:red/But before they can lay a hand on him, a group of Heroic Archaeologists appear out of nowhere, armed with trowels and brushes.", "None"));
        dd.Add(new DialogData("Not so fast, Chickencer King! We won't let you desecrate this sacred site!", "Heroic Archaeologist"));
        dd.Add(new DialogData("Don't worry, Professor Wang, we'll protect you and the treasures of the Λ Civilization!", "Heroic Archaeologist"));
        tmp = new DialogData("/color:red/Chickencer King and his guards are outnumbered, and after a brief struggle, they are forced to retreat.", "None");
        tmp.Callback = () => PlayChickenSoundCallback(true);
        dd.Add(tmp);
        dd.Add(new DialogData("Aghhh! We are outnumbered. Chickencer Guards, retreat!", "Chickencer King"));
        dd.Add(new DialogData("Professor Wang, I will not let you slide.", "Chickencer King"));
        dd.Add(new DialogData("I will be back with more Chickencer Guards in 10 minutes.", "Chickencer King"));
        DialogData last = new DialogData("/emote:Anxious/Quick, Heroic Archeologist! We need to explore the Λ Civilization and find the key to eternal life before the Chickencers come back...", "Professor Wang");
        last.Callback = () => InitialDialogCallback();
        dd.Add(last);
        dm.Show(dd);
    }

    // Plays 1 low sounds for the Chickencer King, otherwise play 6 high sounds for the Chickencer Army 
    void PlayChickenSoundCallback(bool king)
    {
        if (king) musicController.GetComponent<MusicController>().EditMusic("", 2); // 1 low
        else musicController.GetComponent<MusicController>().EditMusic("", 3); // 3 high
    }

    // Activate the player, first puzzle, and timer
    void InitialDialogCallback()
    {
        player.SetActive(true);
        chest1.Unlock();
        timerIsRunning = true;
    }

    // Conclusion (success case)
    public void ShowEndDialogSuccess()
    {
        stone.SetActive(true);
        List<DialogData> dd = new List<DialogData>();
        dd.Add(new DialogData("We've done it! We've discovered the Stone of the Sublime!", "Professor Wang"));
        dd.Add(new DialogData("It's the key to eternal life, just as the Λ Civilization had promised!", "Professor Wang"));
        dd.Add(new DialogData("Congratulations, Professor Wang! It's a huge achievement!", "Heroic Archaeologist"));
        dd.Add(new DialogData("And I couldn't have done it without your help, Heroic Archaeologist.", "Professor Wang"));
        dd.Add(new DialogData("You were brave and resourceful throughout our journey. Thank you!", "Professor Wang"));
        dd.Add(new DialogData("It was my pleasure, Professor Wang.", "Heroic Archaeologist"));
        dd.Add(new DialogData("I'm just glad we were able to stop the Chickencer King from getting his hands on the Stone of the Sublime.", "Heroic Archaeologist"));
        DialogData tmp = new DialogData("Yes, he was a dangerous and unscrupulous rival. But thanks to you, we were able to outsmart him and keep the Stone of the Sublime safe.", "Professor Wang");
        tmp.Callback = () => PlayChickenSoundCallback(true);
        dd.Add(tmp);
        dd.Add(new DialogData("NOOOOOO! I can't believe I've been defeated by you! The Stone of the Sublime was supposed to be mine!", "Chickencer King"));
        dd.Add(new DialogData("/emote:Smirking/Sorry, Chickencer King, but you won't be getting your hands on the Stone of the Sublime.", "Professor Wang"));
        dd.Add(new DialogData("/emote:Smirking/It belongs to the Λ Civilization, and we're taking it back to our lab for further study.", "Professor Wang"));
        dd.Add(new DialogData("Come on, Professor Wang. Let's go home and celebrate our victory.", "Heroic Archaeologist"));
        dd.Add(new DialogData("Yes, let's go!", "Professor Wang"));
        DialogData last = new DialogData("/emote:Smirking/And as for you, Chickencer King, you can cry all you want, but you'll NEVER win against us!", "Professor Wang");
        last.Callback = () => EndDialogSuccessCallback();
        dd.Add(last);
        dm.Show(dd); 
    }

    // Show win message and go back to menu
    void EndDialogSuccessCallback()
    {
        scorePanel.SetActive(true);
        titleText.text = "SUCCESS";
        messageText.text = "As the Heroic Archaeologist, you have successfully stopped the Chickencer King and saved the treasures of the Λ Civilization in time!";
        StartCoroutine(BackToMenu());
    }

    // Conclusion (fail case)
    void ShowEndDialogFail()
    {
        List<DialogData> dd = new List<DialogData>();
        PlayChickenSoundCallback(true);
        dd.Add(new DialogData("Ha ha ha! I'm back, and I've brought more Chickencer Guards with me!", "Chickencer King"));
        DialogData tmp = new DialogData("/emote:Anxious/The Chickencer King! We thought you were gone for good!", "Professor Wang");
        tmp.Callback = () => PlayChickenSoundCallback(true);
        dd.Add(tmp);
        dd.Add(new DialogData("(chuckles) Foolish professor. I may have been gone 10 minutes, but I have returned to claim what is rightfully mine - the treasures of the Λ civilization!", "Chickencer King"));
        tmp = new DialogData("Not so fast, Chickencer King! We won't let you get away with this!", "Heroic Archaeologist");
        tmp.Callback = () => PlayChickenSoundCallback(true);
        dd.Add(tmp);
        dd.Add(new DialogData("(scoffs) You're no match for my Chickencer Army! Prepare to meet your doom!", "Chickencer King"));
        DialogData last = new DialogData("/emote:Anxious/Oh dear, we're vastly outnumbered. What are we going to do?", "Professor Wang");
        last.Callback = () => EndDialogFailCallback();
        dd.Add(last);
        dm.Show(dd); 
    }

    // Show lose message and go back to menu
    void EndDialogFailCallback()
    {
        PlayChickenSoundCallback(false);
        scorePanel.SetActive(true);
        titleText.text = "FAILED";
        messageText.text = "Time's up! The Chickencer King attacks Professor Wang and the Heroic Archaeologists with his large Chickencer Army. They steal the treasures of the Λ civilization. Try again.";
        StartCoroutine(BackToMenu());
    }

    // Goes back to the menu scene
    IEnumerator BackToMenu()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("MenuScene");
    }
}
