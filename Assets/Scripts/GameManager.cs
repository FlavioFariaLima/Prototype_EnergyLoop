using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class GameManager : MonoBehaviour
{
    [Header("Menus Settings")]
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject menuBtn;
    [SerializeField] private GameObject advertisingPanel;
    [SerializeField] private GameObject mainBtn;
    [SerializeField] public GameObject score;
    [SerializeField] private LevelsBag levelsBag;
    [SerializeField] private GameObject secretMenu;
    [SerializeField] private bool secretMenuOn;

    public LevelsBag SavedLevels
    {
        get { return levelsBag; }
        set { levelsBag = value; }
    }

    [Header("Audio Settings")]
    [SerializeField] public AudioClip audioMenuClick;
    [SerializeField] public AudioClip audioNodeClick;
    [SerializeField] public AudioClip audioVictory;
    [SerializeField] public AudioClip audioScore;
    [SerializeField] public Sprite muteSprite;
    [SerializeField] public Sprite unmuteSprite;
    private AudioSource mainAudioSource;

    [Header("Sub-Menus Panels")]
    [SerializeField] private GameObject selectLevelPanel;
    [SerializeField] public ParticleSystem particles;

    [HideInInspector] public LevelManager levelManager;
    private Vector2 originalMenuPosition;
    private Vector2 endMenuPosition;
    private bool isOpen = false;

    [Header("Misc")]
    [SerializeField] private Material bkgMaterial;
    private int sessionScore = 0;
    public int SessionScore
    {
        get { return sessionScore; }
        set { sessionScore = value; }
    }
    private int playingLevel = 0;
    public int PlayingLevel
    {
        get { return playingLevel; }
        set { playingLevel = value; }
    }

    public GameObject GetMainBtn()
    {
        return mainBtn;
    }

    private void Awake()
    {
        levelManager = GetComponent<LevelManager>();
        mainAudioSource = GetComponent<AudioSource>();
        //PlayerPrefs.DeleteAll();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set Panels
        selectLevelPanel.SetActive(false);
        menu.SetActive(true);

        if (secretMenuOn)
            secretMenu.SetActive(true);
        else
            secretMenu.SetActive(false);

        // Setup Positions
        originalMenuPosition = menu.GetComponent<RectTransform>().anchoredPosition;
        endMenuPosition = new Vector2(0, (originalMenuPosition.y + menu.GetComponent<RectTransform>().rect.height) + 30);

        SetupSavedLevels();

        // Force Portrait Orientation
        Screen.orientation = ScreenOrientation.Portrait;
    }

    public void MuteAudio()
    {
        StartCoroutine(PlaySound(audioMenuClick, 0.1f)); ;
        mainAudioSource.mute = !mainAudioSource.mute;

        if (mainAudioSource.mute)
            menu.transform.Find("Sound").GetComponent<Image>().sprite = muteSprite;
        else
            menu.transform.Find("Sound").GetComponent<Image>().sprite = unmuteSprite;
    }

    public IEnumerator PlaySound(AudioClip clip, float volume)
    {
        if (mainAudioSource)
        {
            mainAudioSource.volume = volume;
            mainAudioSource.clip = clip;
            mainAudioSource.Play();
            yield return new WaitForSeconds(clip.length);
        }
    }

    public Material GetBkgMaterial()
    {
        return bkgMaterial;
    }

    // Open Main Menu
    private IEnumerator LerpMenu()
    {
        float timeOfTravel = 0.15f;
        float currentTime = 0;
        float normalizedValue;

        while (currentTime <= timeOfTravel)
        {
            currentTime += Time.deltaTime;
            normalizedValue = currentTime / timeOfTravel;

            if (!isOpen)
            {
                menu.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(originalMenuPosition, endMenuPosition, normalizedValue);
                Camera.main.orthographicSize = Mathf.Lerp(8.5f, 12, 8.5f);
            }
            else
            {
                menu.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(endMenuPosition, originalMenuPosition, normalizedValue);
                Camera.main.orthographicSize = Mathf.Lerp(12, 8.5f, 12);
            }

            yield return null;
        }

        isOpen = !isOpen;

        if (isOpen)
        {
            StartCoroutine(PlaySound(audioMenuClick, 0.1f));
            //menu.transform.Find("ClickOut").gameObject.SetActive(true);
        }
        else
        {
            //menu.transform.Find("ClickOut").gameObject.SetActive(false);
        }

        levelManager.CameraFocus();
    }
        
    public void ShowMenu()
    {
        StartCoroutine(LerpMenu());
    }

    public void ShowAdvertisingPanel()
    {
        StartCoroutine(PlaySound(audioMenuClick, 0.1f));
        advertisingPanel.SetActive(!advertisingPanel.activeSelf);
    }

    // Select Level to Play
    public void ShowSelectLevelPanel()
    {
        StartCoroutine(PlaySound(audioMenuClick, 0.1f));
        selectLevelPanel.SetActive(!selectLevelPanel.activeSelf);
    }

    public void SetupSavedLevels()
    {
        int bestLevel = 0;

        if (PlayerPrefs.HasKey("BestLevel"))
        {
            bestLevel = PlayerPrefs.GetInt("BestLevel");
        }
        else
            PlayerPrefs.SetInt("BestLevel", bestLevel);

        foreach (Transform child in selectLevelPanel.transform.Find("LevelsParent"))
        {
            // Setup Button
            child.GetComponent<Button>().onClick.RemoveAllListeners();
            child.GetComponent<Button>().onClick.AddListener(delegate { LoadSelectedLevel(child.GetSiblingIndex()); });

            if (child.GetSiblingIndex() > bestLevel)
            {
                child.GetComponent<Button>().interactable = false;
                child.GetComponentInChildren<Text>().text = $"";
            }
            else
            {
                child.GetComponent<Button>().interactable = true;
                child.GetComponentInChildren<Text>().text = $"{child.GetSiblingIndex() + 1}";
            }
        }
    }

    public void MainButtonClick()
    {
        mainBtn.GetComponent<Button>().onClick.RemoveAllListeners();
        mainBtn.GetComponent<Button>().onClick.AddListener(delegate { levelManager.LoadThisLevel(playingLevel); });
    }

    public void LoadSelectedLevel(int index)
    {
        StartCoroutine(PlaySound(audioMenuClick, 0.1f));
        levelManager.LoadThisLevel(index);
        ShowSelectLevelPanel();
    }

    public void CheckScore()
    {
        // Check Level Progress
        if (PlayerPrefs.HasKey("BestLevel"))
        {
            if (PlayerPrefs.GetInt("BestLevel") < playingLevel)
                PlayerPrefs.SetInt("BestLevel", playingLevel);
        }
        else
        {
            PlayerPrefs.SetInt("BestLevel", playingLevel);
        }

        // Check Score
        int tempScore = PlayerPrefs.GetInt("PlayerScore") + sessionScore;
        PlayerPrefs.SetInt("PlayerScore", tempScore);

        score.GetComponent<Text>().text = $"{PlayerPrefs.GetInt("PlayerScore")}";
    }

    public void CleanCanvas()
    {
        if (isOpen)
            ShowMenu();
    }

    // Score Scale
    public IEnumerator ScaleOverSeconds(GameObject objectToScale, Vector3 scaleTo, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingScale = objectToScale.transform.localScale;
        Vector3 startPost = objectToScale.transform.position;

        while (elapsedTime < seconds)
        {
            objectToScale.transform.localScale = Vector3.Lerp(startingScale, scaleTo, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectToScale.transform.position = scaleTo;

        StartCoroutine(PlaySound(audioScore, 0.1f));
        elapsedTime = 0;

        while (elapsedTime < seconds)
        {
            objectToScale.transform.localScale = Vector3.Lerp(scaleTo, startingScale, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectToScale.transform.position = startPost;

    }
}
