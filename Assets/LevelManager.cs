using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class LevelManager : MonoBehaviour
{
	[Header("Objects Settings")]
	[SerializeField] private GameObject canvas;
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

	float cycleDuration = 2f;
	int keysCount = 10; 
	float startVal = 1f;
	float randMin = -1f, randMax = 1f; 
	float flickrIntensity = 4f; 
	private AnimationCurve curve;

	private bool hasMainNode;

	// Use this for initialization
	private void Awake()
	{
		// Set Visual 
		SetDefaultNodestColor(defaultNodeColor);
		canvas.SetActive(false);

		// Check if We area Building Levels
		if (BuildRandomLevel)
		{
			if (levelSettings.width == 0 || levelSettings.height == 0)
			{
				Debug.LogError("Need to define a width and height for Auto Level Build!");
				Debug.Break();
			}

			GenerateLevel();
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

		foreach (var item in levelSettings.nodes)
		{
			Debug.Log(item.gameObject.name);
		}

		levelSettings.totalLinks = GetLinksRequired();

		RotateNodes();

		levelSettings.curLinkCount = CheckNodes();
	}

	private void SetAllColor(Color color)
	{
		foreach (Node node in levelSettings.nodes)
		{
			node.GetComponent<SpriteRenderer>().color = color;
			Debug.Log($" Change Color: {node.gameObject.name}");
		}
	}

	private void SetDefaultNodestColor(Color color)
    {
		foreach(GameObject node in nodesPrefabs)
        {
			node.GetComponent<SpriteRenderer>().color = color;
			Debug.Log($" Change Color: {node.gameObject.name}");
		}
    }

	private void GenerateLevel()
	{
		levelSettings.nodes = new Node[levelSettings.width, levelSettings.height];

		int[] auxSides = { 0, 0, 0, 0 };

		for (int h = 0; h < levelSettings.height; h++)
		{
			for (int w = 0; w < levelSettings.width; w++)
			{
				// Set Limits
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
					newNode.GetComponent<Node>().MainNode(true);
					hasMainNode = true;
				}

				levelSettings.nodes[w, h] = newNode.GetComponent<Node>();
			}
		}

		Camera.main.transform.position = new Vector3(levelSettings.width / 2, levelSettings.height / 2, -10);
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

	public Level Level()
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
						value++;
				}

				{
					if (w != levelSettings.width - 1)
						if (levelSettings.nodes[w, h].ActiveSides()[1] == 1 && levelSettings.nodes[w + 1, h].ActiveSides()[3] == 1)
							value++;
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
				value++;
		}

		if (w != levelSettings.width - 1)
		{
			if (levelSettings.nodes[w, h].ActiveSides()[1] == 1 && levelSettings.nodes[w + 1, h].ActiveSides()[3] == 1)
				value++;
		}

		if (w != 0)
		{
			if (levelSettings.nodes[w, h].ActiveSides()[3] == 1 && levelSettings.nodes[w - 1, h].ActiveSides()[1] == 1)
				value++;
		}

		if (h != 0)
		{
			if (levelSettings.nodes[w, h].ActiveSides()[2] == 1 && levelSettings.nodes[w, h - 1].ActiveSides()[0] == 1)
				value++;
		}

		return value;
	}

	public IEnumerator TurnLight(bool value)
    {
		float minLuminosity = 1; 
		float maxLuminosity = 3;
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
		}

		float currentIntensity = mainlight.intensity;

		while (counter < duration)
		{
			counter += Time.deltaTime;

			mainlight.intensity = Mathf.Lerp(a, b, counter / duration);

			yield return null;
		}
	}

	public IEnumerator FinishUI()
	{		

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

		canvas.SetActive(true);
	}

	[HideInInspector] public bool playAgain = false;

	public void PlayAgain()
	{
		StartCoroutine(TurnLight(false));

		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Node"))
        {
			Destroy(obj);
        }

		levelSettings.nodes = new Node[levelSettings.width, levelSettings.height];
		playAgain = true;

		SetNewLevelSize();
		GenerateLevel();

		levelSettings.totalLinks = GetLinksRequired();
		RotateNodes();
		levelSettings.curLinkCount = CheckNodes();

		canvas.SetActive(false);
	}

	public void SetNewLevelSize()
    {
		levelSettings.width = UnityEngine.Random.Range(3, 8);
		levelSettings.height = UnityEngine.Random.Range(3, 8);
	}

	private void PauseGame()
	{
		Time.timeScale = 0;
	}
	private void ContinueGame()
	{
		Time.timeScale = 1;
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

