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
    [SerializeField] private GameObject secretMenu;
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
    private int playingLevel = 0;

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

    private IEnumerator LerpObject()
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
        StartCoroutine(LerpObject());
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

    public void LoadSelectedLevel(int index)
    {
        levelManager.LoadThisLevel(index);
        ShowSelectLevelPanel();
    }

    public void CheckScore()
    {
        if (PlayerPrefs.HasKey("PlayerScore"))
        {
            int tempScore = PlayerPrefs.GetInt("PlayerScore") + sessionScore;
            PlayerPrefs.SetInt("PlayerScore", tempScore);
        }
        else
            PlayerPrefs.SetInt("PlayerScore", sessionScore);

        if (PlayerPrefs.HasKey("BestLevel"))
        {
            if ( PlayerPrefs.GetInt("BestLevel") < playingLevel);
                PlayerPrefs.SetInt("BestLevel", playingLevel);
        }
        else
        {
            PlayerPrefs.SetInt("BestLevel", playingLevel);
        }
    }

}
