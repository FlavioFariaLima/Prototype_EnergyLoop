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
    [SerializeField] private GameObject secretMenu;
    [SerializeField] private GameObject mainBtn;
    [SerializeField] private GameObject score;
    [SerializeField] private bool secretMenuOn;
    [SerializeField] private LevelsBag levelsBag;
    public LevelsBag SavedLevels
    {
        get { return levelsBag; }
        set { levelsBag = value; }
    }

    [Header("Sub-Menus Panels")]
    [SerializeField] private GameObject selectLevelPanel;

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
        endMenuPosition = new Vector2(0, originalMenuPosition.y + menu.GetComponent<RectTransform>().rect.height);

        SetupSavedLevels();
    }

    public Material GetBkgMaterial()
    {
        return bkgMaterial;
    }

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
            }
            else
            {
                menu.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(endMenuPosition, originalMenuPosition, normalizedValue);
            }


            yield return null;
        }

        isOpen = !isOpen;
    }

    public void ShowMenu()
    {
        StartCoroutine(LerpMenu());
    }

    public void ShowSelectLevelPanel()
    {
        selectLevelPanel.SetActive(!selectLevelPanel.activeSelf);
    }

    public void SetupSavedLevels()
    {
        int bestLevel = 0;

        if (PlayerPrefs.HasKey("BestLevel"))
            bestLevel = PlayerPrefs.GetInt("BestLevel");
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
        mainBtn.GetComponent<Button>().onClick.AddListener(delegate { levelManager.LoadThisLevel(PlayerPrefs.GetInt("BestLevel")); });
    }

    public void LoadSelectedLevel(int index)
    {
        levelManager.LoadThisLevel(index);
        ShowSelectLevelPanel();
    }

    public void CheckScore()
    {
        // Check Level Progress
        if (PlayerPrefs.HasKey("BestLevel"))
        {
            if (PlayerPrefs.GetInt("BestLevel") < playingLevel);
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
}
