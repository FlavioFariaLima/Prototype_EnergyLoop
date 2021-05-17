using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using UnityEditor;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
	[Header("Objects Settings")]
	[SerializeField] private GameObject endLevel;
	[SerializeField] private Light mainlight;
	[SerializeField] private Transform nodesParent;
	[SerializeField] private Sprite defaultNodeSprite;
	[SerializeField] private Color defaultNodeColor;
	[SerializeField] private Sprite mainNodeSprite;
	[SerializeField] private Color mainNodeColor;
	[SerializeField] private float rotationSpeed;
	[SerializeField] private GameObject[] nodesPrefabs;

	[Header("Setup Level")]
	[SerializeField] private bool BuildRandomLevel;
	[SerializeField] private Level levelSettings;
	public Level LevelSettings
	{
		get { return levelSettings; }
		set { levelSettings = value; }
	}
	[SerializeField] private int maxEndLineNodes;
	[HideInInspector] public bool playAgain = false;

	public GameManager Manager;
	private bool hasMainNode;
	private bool playingRandom = false;


	[SerializeField] BlockManager blockManager;


	// Use this for initialization
	private void Awake()
	{
		Manager = GetComponent<GameManager>();
		blockManager = GetComponent<BlockManager>();

		print(blockManager);

		// Set Visual 
		SetDefaultNodestColor(defaultNodeColor);
		endLevel.SetActive(false);

		// Check if We area Building Levels
		if (BuildRandomLevel)
		{
			if (levelSettings.width == 0 || levelSettings.height == 0)
			{
				Debug.Log(" 0,0 Mean we want to start the app in the sequence levels");
				//LoadThisLevel(PlayerPrefs.GetInt("BestLevel"));
				LoadThisLevel(5);

			}
			else
			{
				Debug.Log(" 0,0 Mean we want to start the app with a randon level");
				BuildLevel();
			}
		}
		else
		{
			Vector2 dimensions = CheckDimensions();
			levelSettings.width = (int)dimensions.x;
			levelSettings.height = (int)dimensions.y;

			levelSettings.nodes = new Node[levelSettings.width, levelSettings.height];

			foreach (var piece in GameObject.FindGameObjectsWithTag("Node"))
			{
				levelSettings.nodes[(int)piece.transform.position.x, (int)piece.transform.position.y] = piece.GetComponent<Node>();
			}
		}

		if (levelSettings.width != 0 && levelSettings.height != 0)
		{
			levelSettings.totalLinks = GetLinksRequired();

			RotateNodes();

			levelSettings.curLinkCount = CheckNodes();
			CameraFocus();
		}


	}

	public void CameraFocus()
	{
		Camera.main.transform.position = new Vector3((levelSettings.width / 2) - 0.2f, (levelSettings.height / 2) - 0.5f, -3);
	}


	private void SetAllColor(Color color)
	{
		foreach (Node node in levelSettings.nodes)
		{
			node.GetComponent<SpriteRenderer>().color = color;
		}
	}

	private void SetDefaultNodestColor(Color color)
	{
		foreach (GameObject node in nodesPrefabs)
		{
			node.GetComponent<SpriteRenderer>().color = color;
			//Debug.Log($" Change Color: {node.gameObject.name}");
		}
	}

	private void BuildLevel()
	{
		levelSettings.nodes = new Node[levelSettings.width, levelSettings.height];

		int[] auxSides = { 0, 0, 0, 0 };
		hasMainNode = false;

		for (int h = 0; h < levelSettings.height; h++)
		{
			for (int w = 0; w < levelSettings.width; w++)
			{
				// Set Limits
				auxSides = new int[4];

				if (w == 0)
					auxSides[3] = 0;
				else
					auxSides[3] = levelSettings.nodes[w - 1, h].ActiveSides()[1];

				if (w == levelSettings.width - 1)
					auxSides[1] = 0;
				else
					auxSides[1] = UnityEngine.Random.Range(0, 2);

				if (h == 0)
					auxSides[2] = 0;
				else
					auxSides[2] = levelSettings.nodes[w, h - 1].ActiveSides()[0];

				if (h == levelSettings.height - 1)
					auxSides[0] = 0;
				else
					auxSides[0] = UnityEngine.Random.Range(0, 2);

				// Return the Node Type
				int nodeType = auxSides[0] + auxSides[1] + auxSides[2] + auxSides[3];
				if (nodeType == 2 && auxSides[0] != auxSides[2])
					nodeType = 5;


				// Instantiate Node and set rotation
				GameObject newNode = Instantiate(nodesPrefabs[nodeType], new Vector3(w, h, 0), Quaternion.identity);
				newNode.GetComponent<Node>().GetNodeType = nodeType;

				newNode.GetComponent<Node>().newNodeType = nodeType;

				while (newNode.GetComponent<Node>().ActiveSides()[0] != auxSides[0] || newNode.GetComponent<Node>().ActiveSides()[1] != auxSides[1]
					|| newNode.GetComponent<Node>().ActiveSides()[2] != auxSides[2] || newNode.GetComponent<Node>().ActiveSides()[3] != auxSides[3])
				{
					newNode.GetComponent<Node>().RotateNode();
				}

				// Set Main Node
				int newNodeType = newNode.GetComponent<Node>().ActiveSides()[0]
						+ newNode.GetComponent<Node>().ActiveSides()[1]
						+ newNode.GetComponent<Node>().ActiveSides()[2]
						+ newNode.GetComponent<Node>().ActiveSides()[3];

				if (!hasMainNode && newNodeType == 1)
				{
					newNode.GetComponent<Node>().SetMainNode(true);
					hasMainNode = true;
				}

				levelSettings.nodes[w, h] = newNode.GetComponent<Node>();
			}
		}

		CameraFocus();


		blockManager.Build3dLevel();

	}


	private int GetLinksRequired()
	{
		int linkCount = 0;
		foreach (var piece in levelSettings.nodes)
		{
			foreach (var j in piece.ActiveSides())
			{
				linkCount += j;
			}
		}

		linkCount /= 2;

		return linkCount;
	}

	private void RotateNodes()
	{
		foreach (var piece in levelSettings.nodes)
		{
			int s = UnityEngine.Random.Range(0, 4);

			for (int i = 0; i < s; i++)
			{
				piece.RotateNode();
			}
		}
	}

	private Vector2 CheckDimensions()
	{
		Vector2 aux = Vector2.zero;

		GameObject[] nodes = GameObject.FindGameObjectsWithTag("Node");

		foreach (var p in nodes)
		{
			if (p.transform.position.x > aux.x)
				aux.x = p.transform.position.x;

			if (p.transform.position.y > aux.y)
				aux.y = p.transform.position.y;
		}

		aux.x++;
		aux.y++;

		return aux;
	}

	public float RotationSpeed()
	{
		return rotationSpeed;
	}

	public Level GetCurrentLevel()
	{
		return levelSettings;
	}

	public Sprite GetDefaultNodeSprite()
	{
		return default;
	}

	public Sprite GetMainNodeSprite()
	{
		return mainNodeSprite;
	}

	public int CheckNodes()
	{
		int value = 0;

		for (int h = 0; h < levelSettings.height; h++)
		{
			for (int w = 0; w < levelSettings.width; w++)
			{
				if (h != levelSettings.height - 1)
				{
					if (levelSettings.nodes[w, h].ActiveSides()[0] == 1 && levelSettings.nodes[w, h + 1].ActiveSides()[2] == 1)
					{
						value++;
					}
				}

				{
					if (w != levelSettings.width - 1)
						if (levelSettings.nodes[w, h].ActiveSides()[1] == 1 && levelSettings.nodes[w + 1, h].ActiveSides()[3] == 1)
						{
							value++;
						}
				}
			}
		}

		return value;
	}

	public int CheckNode(int w, int h)
	{
		int value = 0;

		// Check Sides Clock Wise
		if (h != levelSettings.height - 1)
		{
			if (levelSettings.nodes[w, h].ActiveSides()[0] == 1 && levelSettings.nodes[w, h + 1].ActiveSides()[2] == 1)
			{
				value++;
			}
		}

		if (w != levelSettings.width - 1)
		{
			if (levelSettings.nodes[w, h].ActiveSides()[1] == 1 && levelSettings.nodes[w + 1, h].ActiveSides()[3] == 1)
			{
				value++;
			}
		}

		if (w != 0)
		{
			if (levelSettings.nodes[w, h].ActiveSides()[3] == 1 && levelSettings.nodes[w - 1, h].ActiveSides()[1] == 1)
			{
				value++;
			}
		}

		if (h != 0)
		{
			if (levelSettings.nodes[w, h].ActiveSides()[2] == 1 && levelSettings.nodes[w, h - 1].ActiveSides()[0] == 1)
			{
				value++;
			}
		}

		return value;
	}

	// Deal with Light
	public IEnumerator TurnLight(bool value)
	{
		float minLuminosity = .5f;
		float maxLuminosity = 1.5f;
		float duration = 1;

		float counter = 0f;

		float a, b;

		if (value)
		{
			a = minLuminosity;
			b = maxLuminosity;
		}
		else
		{
			a = maxLuminosity;
			b = minLuminosity;
			endLevel.SetActive(false);
			Manager.GetBkgMaterial().color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

			var col = Manager.particles.colorOverLifetime;
			col.enabled = true;

			Gradient grad = new Gradient();
			grad.SetKeys(new GradientColorKey[] { new GradientColorKey(UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.1f, 1f), 0.0f),
												  new GradientColorKey(UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.1f, 1f), 1.0f) },
												  new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 1.0f), new GradientAlphaKey(1.0f, 1.0f) });

			col.color = grad;
		}

		while (counter < duration)
		{
			counter += Time.deltaTime;

			mainlight.intensity = Mathf.Lerp(a, b, counter / duration);

			yield return null;
		}

		Manager.GetMainBtn().SetActive(false);
	}

	// Call Victory UI
	public IEnumerator FinishUI()
	{
		//
		Handheld.Vibrate();

		float time = 0f;
		float speed = 0.5f;
		Color modelColor = defaultNodeColor;
		StartCoroutine(TurnLight(true));

		while (modelColor != mainNodeColor)
		{
			playAgain = false;
			time += Time.deltaTime * speed;
			modelColor = Color.Lerp(defaultNodeColor, mainNodeColor, time);
			SetAllColor(modelColor);

			yield return null;
		}

		StartCoroutine(Manager.ScaleOverSeconds(Manager.score, new Vector3(3, 3, 3), .5f));

		Manager.PlayingLevel++;
		Manager.SessionScore += (levelSettings.width * levelSettings.height) * 100;

		Manager.CheckScore();
		Manager.SetupSavedLevels();
		Manager.MainButtonClick();

		// Show UI
		if (!playingRandom)
			Manager.GetMainBtn().SetActive(true);

		endLevel.SetActive(true);
	}

	// Save and Load Level Methods
	public void BuildTotalRandomLevel()
	{
		playingRandom = true;
		StartCoroutine(Manager.PlaySound(Manager.audioMenuClick, 0.1f));

		StartCoroutine(TurnLight(false));

		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Node"))
		{
			Destroy(obj);
		}

		levelSettings.nodes = new Node[levelSettings.width, levelSettings.height];
		playAgain = true;

		SetNewLevelSize();
		BuildLevel();

		levelSettings.totalLinks = GetLinksRequired();
		RotateNodes();
		levelSettings.curLinkCount = CheckNodes();

		CameraFocus();
		endLevel.SetActive(false);
	}

	public void SetNewLevelSize()
	{
		levelSettings.width = UnityEngine.Random.Range(2, 7);
		levelSettings.height = UnityEngine.Random.Range(3, 13);
	}

	public void SaveLevelToObject()
	{

#if UNITY_EDITOR_WIN
		List<PseudoNode> nodes = new List<PseudoNode>();

		foreach (Node n in levelSettings.nodes)
		{
			PseudoNode p = new PseudoNode();
			p.w = (int)n.transform.position.x;
			p.h = (int)n.transform.position.y;
			p.nodeType = n.GetNodeType;
			p.mainNode = n.MainNode;
			p.activeSides = n.ActiveSides();
			p.top = n.top;
			p.right = n.right;
			p.bottom = n.bottom;
			p.left = n.left;
			p.activeSides = n.activeSides;
			p.rotationDiff = n.rotationDiff;

			nodes.Add(p);
		}

		Manager.SavedLevels.savedLevels.Add(CreateNewLevel(levelSettings.totalLinks, levelSettings.width, levelSettings.height, nodes));
		EditorUtility.SetDirty(Manager.SavedLevels);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
#endif
	}

	public void LoadThisLevel(int index)
	{

		try
		{
			StartCoroutine(Manager.PlaySound(Manager.audioMenuClick, 0.1f));
		}
		catch (Exception ex)
		{

		}


		Manager.PlayingLevel = index;
		playingRandom = false;
		StartCoroutine(TurnLight(false));

		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Node"))
		{
			Destroy(obj);
		}

		Level newLevel = new Level();
		newLevel.totalLinks = Manager.SavedLevels.savedLevels[index].totalLinks;
		newLevel.curLinkCount = Manager.SavedLevels.savedLevels[index].curLinkCount;
		newLevel.width = Manager.SavedLevels.savedLevels[index].width;
		newLevel.height = Manager.SavedLevels.savedLevels[index].height;
		newLevel.nodes = new Node[newLevel.width, newLevel.height];


		foreach (PseudoNode p in Manager.SavedLevels.savedLevels[index].nodes)
		{
			GameObject newNode = Instantiate(nodesPrefabs[p.nodeType], new Vector3(p.w, p.h, 0), Quaternion.identity);
			Node n = newNode.GetComponent<Node>();

			n.SetMainNode(p.mainNode);
			n.top = p.top;
			n.right = p.right;
			n.bottom = p.bottom;
			n.left = p.left;

			// Return the Node Type
			int nodeType = n.ActiveSides()[0] + n.ActiveSides()[1] + n.ActiveSides()[2] + n.ActiveSides()[3];
			if (nodeType == 2 && n.ActiveSides()[0] != n.ActiveSides()[2])
				nodeType = 5;

			n.newNodeType = nodeType;

			newNode.GetComponent<SpriteRenderer>().sprite = nodesPrefabs[nodeType].GetComponent<SpriteRenderer>().sprite;
			newLevel.nodes[p.w, p.h] = n;

		}


		playAgain = true;
		levelSettings = newLevel;

		levelSettings.totalLinks = GetLinksRequired();
		RotateNodes();
		CameraFocus();
		levelSettings.curLinkCount = CheckNodes();


		blockManager.Build3dLevel();

	}

	public void RemoveLastSavedLevel()
	{

#if UNITY_EDITOR_WIN
		Manager.SavedLevels.savedLevels.RemoveAt(Manager.SavedLevels.savedLevels.Count - 1);

		EditorUtility.SetDirty(Manager.SavedLevels);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
#endif
	}

	public void CleanSavedLevelList()
	{
#if UNITY_EDITOR_WIN
		Manager.SavedLevels.savedLevels.Clear();
		EditorUtility.SetDirty(Manager.SavedLevels);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
#endif
	}

	public PseudoLevel CreateNewLevel(int t, int w, int h, List<PseudoNode> n)
	{
		PseudoLevel newLvl = new PseudoLevel();

		newLvl.totalLinks = t;
		newLvl.curLinkCount = 0;
		newLvl.width = w;
		newLvl.height = h;
		newLvl.nodes = n;

		return newLvl;
	}
}

[Serializable]
public class Level
{
	public int totalLinks;
	public int curLinkCount;

	public int width;
	public int height;
	public Node[,] nodes;
}
