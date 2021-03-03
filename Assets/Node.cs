using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Node : MonoBehaviour
{
	private LevelManager levelManager;
	private float rotationSpeed;
	private bool mainNode = false;

	[Header("Node Default Active Settings")]
	[SerializeField] private bool top = false;
	[SerializeField] private bool right = false;
	[SerializeField] private bool bottom = false;
	[SerializeField] private bool left = false;

	private int[] activeSides;
	private float rotationDiff;

	// Public Methods
	public bool MainNode(bool value)
    {
		bool r = false;

		mainNode = value;

		if (mainNode)
        {
			GetComponent<SpriteRenderer>().sprite = levelManager.GetMainNodeSprite();
        }
		else
        {
			GetComponent<SpriteRenderer>().sprite = levelManager.GetDefaultNodeSprite();
		}

		return r;
    }

	public int[] ActiveSides()
    {
		return activeSides;
    }	

	// Use this for initialization
	void Awake()
	{
		// Get Globals
		levelManager = GameObject.Find("Global").GetComponent<LevelManager>();

		// Settings
		rotationSpeed = levelManager.RotationSpeed();

		// Active Sides
		activeSides = new int[4];
		activeSides[0] = top ? 1 : 0;
		activeSides[1] = right ? 1 : 0;
		activeSides[2] = bottom ? 1 : 0;
		activeSides[3] = left ? 1 : 0;
	}

	// Update is called once per frame
	void Update()
	{
		if (transform.root.eulerAngles.z != rotationDiff)
		{
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, rotationDiff), rotationSpeed);
		}
	}

	void OnMouseDown()
	{
		int difference = -levelManager.CheckNode((int)transform.position.x, (int)transform.position.y);

		RotateNode();

		difference += levelManager.CheckNode((int)transform.position.x, (int)transform.position.y);

		levelManager.Level().curLinkCount += difference;

		if (levelManager.Level().curLinkCount == levelManager.Level().totalLinks)
			StartCoroutine(levelManager.FinishUI());
	}

	public void RotateNode()
	{
		rotationDiff += 90;

		if (rotationDiff == 360)
			rotationDiff = 0;

		RotateValues();
	}

	public void RotateValues()
	{
		int aux = activeSides[0];

		for (int i = 0; i < activeSides.Length - 1; i++)
		{
			activeSides[i] = activeSides[i + 1];
		}
		activeSides[3] = aux;
	}
}
