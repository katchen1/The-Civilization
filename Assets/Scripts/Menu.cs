using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public Button playButton;
    public Button aboutButton;
    public Button quitButton;
    public GameObject aboutPanel;

    // Start is called before the first frame update
    void Start()
    {
        playButton.onClick.AddListener(() => PlayOnClick());
        aboutButton.onClick.AddListener(() => AboutOnClick());
        quitButton.onClick.AddListener(() => QuitOnClick());
    }

    void PlayOnClick()
    {
        SceneManager.LoadScene("MainScene");
    }

    void AboutOnClick()
    {
        aboutPanel.SetActive(true);
    }

    void QuitOnClick()
    {
        Application.Quit();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            aboutPanel.SetActive(false);
        }  
    }
}
