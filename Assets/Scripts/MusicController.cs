using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour
{
    public GameObject cloudPrefab;
    public GameObject cam;
    public Sprite pinkCloudSprite;
    public Sprite whiteCloudSprite;

    private int NUM_CLOUDS = 15;
    private ChuckIntSyncer m_ckCurrentStep;
    private GameObject[] m_clouds;
    private int m_previousStep = -1;
    private float m_scale = 1.1f;
    
    void Start()
    {   
        InitializeGraphics();
        InitializeAudio();
    }

    void InitializeGraphics()
    {
        // Instantiate a row of clouds that will react to the background music
        m_clouds = new GameObject[NUM_CLOUDS];
        Vector3 startPos = new Vector3(cam.transform.position.x - 6.5f, cam.transform.position.y + 4.5f, -1f);
        float xOffset = 5.4f;
        float yOffset = 1.4f;
        for (int i = 0; i < NUM_CLOUDS; i++)
        {
            m_clouds[i] = Instantiate(cloudPrefab, new Vector3(
                startPos.x + xOffset * i, 
                startPos.y - yOffset * Random.Range(0f, 2f), 
                startPos.z
            ), Quaternion.identity); 
            m_clouds[i].transform.localScale = new Vector3(1.5f, 0.5f, 1f);
        }
    }

    void InitializeAudio()
    {
        // Run the sequencer
        GetComponent<ChuckSubInstance>().RunFile("music.ck", true);

        // Add the int sync
        m_ckCurrentStep = gameObject.AddComponent<ChuckIntSyncer>();
        m_ckCurrentStep.SyncInt(GetComponent<ChuckSubInstance>(), "currentStep");
    }

    void Update()
    {
        // Get current step number
        int currentStep = m_ckCurrentStep.GetCurrentValue();
        if (currentStep != m_previousStep)
        {
            StartCoroutine(EnlargeClouds());
            m_previousStep = currentStep;
        }
    }

    // Clouds expand then shrink according to the beat of the music
    // The degree of expansion (m_scale) depends on the current volume (gain)
    IEnumerator EnlargeClouds()
    {
        for (int i = 0; i < NUM_CLOUDS; i++)
        {
            Vector3 currScale = m_clouds[i].transform.localScale;
            m_clouds[i].transform.localScale = new Vector3(currScale.x * m_scale, currScale.y * m_scale, currScale.z);
            yield return new WaitForSeconds(0.1f);
            m_clouds[i].transform.localScale = new Vector3(currScale.x, currScale.y, currScale.z);
        }
    }

    // Called whenever there is a change in the sounds being played
    public void EditMusic(string attribute, int on)
    {
        // Reflect graphically
        for (int i = 0; i < NUM_CLOUDS; i++)
        {
            Vector3 currScale = m_clouds[i].transform.localScale;
            if (on == 1)
            {
                if (attribute == "frequency1") m_clouds[i].transform.localScale = new Vector3(currScale.x * 2f, currScale.y, currScale.z);
                else if (attribute == "dynamics") m_scale = 1.3f;
                else if (attribute == "frequency2") m_clouds[i].transform.localScale = new Vector3(currScale.x / 2f, currScale.y, currScale.z);
                else if (attribute == "harmony") m_clouds[i].GetComponent<SpriteRenderer>().sprite = pinkCloudSprite;
            }
            else if (on == 0)
            {
                if (attribute == "frequency1") m_clouds[i].transform.localScale = new Vector3(currScale.x / 2f, currScale.y, currScale.z);
                else if (attribute == "dynamics") m_scale = 1.1f;
                else if (attribute == "frequency2") m_clouds[i].transform.localScale = new Vector3(currScale.x * 2f, currScale.y, currScale.z);
                else if (attribute == "harmony") m_clouds[i].GetComponent<SpriteRenderer>().sprite = whiteCloudSprite;
            }
        }

        // Communicate with chuck
        GetComponent<ChuckSubInstance>().SetString("editAttribute", attribute);
        GetComponent<ChuckSubInstance>().SetInt("editOn", on);
        GetComponent<ChuckSubInstance>().BroadcastEvent("editHappened");
    }
}
