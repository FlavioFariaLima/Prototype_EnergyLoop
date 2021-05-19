﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int id = 0;

	[SerializeField] private LevelManager levelManager;

	[SerializeField] public BlockManager blockManager;

    public GameObject ivyLeafsObject;
	public GameObject ivyStickObject;

	public List<GameObject> neighborsBlocks = new List<GameObject>();

	Material m_Material;

	public void SetId(int _id)
    {
        id = _id;
    }
    public int GetId()
    {
        return id;
    }

	public void SetColor(float value)
	{
		m_Material = transform.GetChild(0).GetComponent<Renderer>().material;

		m_Material.color = new Color(value, value, value, 1);
	}

	void Start()
	{
		levelManager = GameObject.Find("Global").GetComponent<LevelManager>();

		blockManager = GameObject.Find("Global").GetComponent<BlockManager>();

		//ivy3dObject = transform.GetChild(0).GetChild(0).gameObject;

		CheckNode((int)transform.parent.transform.position.x, (int)transform.parent.transform.position.y);


	}

	public void StartIvyIncrease()
    {
		ivyLeafsObject.GetComponent<IvyController>().PlayIvyAnim(true);
		ivyStickObject.GetComponent<IvyController>().PlayIvyAnim(true);
	}

	public void Check()
    {

		DestroyIvy();
		
		CheckNode((int)transform.parent.transform.position.x, (int)transform.parent.transform.position.y);

		CheckExNode((int)transform.parent.transform.position.x, (int)transform.parent.transform.position.y);


		PlayIvysSequence();


	}


	public void CheckNeighbors()
	{
		for (int i = 0; i < neighborsBlocks.Count; i++)
		{
			neighborsBlocks[i].transform.GetChild(0).GetComponent<Block>().Check();
		
		}
	}

	public void CheckExNode(int w, int h)
	{

		if (h != levelManager.LevelSettings.height - 1)
		{
			if (levelManager.LevelSettings.nodes[w, h].ActiveSides()[0] == 0 && levelManager.LevelSettings.nodes[w, h + 1].ActiveSides()[2] == 1)
			{

				neighborsBlocks.Remove(levelManager.LevelSettings.nodes[w, h + 1].gameObject);

				levelManager.LevelSettings.nodes[w, h + 1].transform.GetChild(0).GetComponent<Block>().neighborsBlocks.Remove(levelManager.LevelSettings.nodes[w, h].gameObject);
			}
		}

		if (w != levelManager.LevelSettings.width - 1)
		{
			if (levelManager.LevelSettings.nodes[w, h].ActiveSides()[1] == 0 && levelManager.LevelSettings.nodes[w + 1, h].ActiveSides()[3] == 1)
			{

				neighborsBlocks.Remove(levelManager.LevelSettings.nodes[w + 1, h].gameObject);

				levelManager.LevelSettings.nodes[w + 1, h].transform.GetChild(0).GetComponent<Block>().neighborsBlocks.Remove(levelManager.LevelSettings.nodes[w, h].gameObject);

			}
		}

		if (w != 0)
		{
			if (levelManager.LevelSettings.nodes[w, h].ActiveSides()[3] == 0 && levelManager.LevelSettings.nodes[w - 1, h].ActiveSides()[1] == 1)
			{

				neighborsBlocks.Remove(levelManager.LevelSettings.nodes[w - 1, h].gameObject);

				levelManager.LevelSettings.nodes[w - 1, h].transform.GetChild(0).GetComponent<Block>().neighborsBlocks.Remove(levelManager.LevelSettings.nodes[w, h].gameObject);

			}
		}

		if (h != 0)
		{
			if (levelManager.LevelSettings.nodes[w, h].ActiveSides()[2] == 0 && levelManager.LevelSettings.nodes[w, h - 1].ActiveSides()[0] == 1)
			{

				neighborsBlocks.Remove(levelManager.LevelSettings.nodes[w, h - 1].gameObject);

				levelManager.LevelSettings.nodes[w, h - 1].transform.GetChild(0).GetComponent<Block>().neighborsBlocks.Remove(levelManager.LevelSettings.nodes[w, h].gameObject);

			}
		}

	}


	void PlayIvysSequence()
    {

		blockManager.PlaySqIvyAnim();
		/*
		for (int i = 0; i < neighborsBlocks.Count; i++)
        {
			neighborsBlocks[i].transform.GetChild(0).GetComponent<Block>().StartIvyIncrease();
		
		}*/
	}

	void PlayIvyAnim()
	{
		ivyLeafsObject.GetComponent<IvyController>().PlayIvyAnim(true);
	}

	public void CheckNode(int w, int h)
	{
		// Check Sides Clock Wise
		if (h != levelManager.LevelSettings.height - 1)
		{
			if (levelManager.LevelSettings.nodes[w, h].ActiveSides()[0] == 1 && levelManager.LevelSettings.nodes[w, h + 1].ActiveSides()[2] == 1)
			{
	
					if (blockManager.blockSequence.Contains(levelManager.LevelSettings.nodes[w, h].gameObject) == false)
					{

						blockManager.blockSequence.Add(levelManager.LevelSettings.nodes[w, h].gameObject);

					}
					if (blockManager.blockSequence.Contains(levelManager.LevelSettings.nodes[w, h + 1].gameObject) == false)
					{

						blockManager.blockSequence.Add(levelManager.LevelSettings.nodes[w, h + 1].gameObject);

					}


				if (neighborsBlocks.Contains((levelManager.LevelSettings.nodes[w, h + 1].gameObject)) == false)
					neighborsBlocks.Add(levelManager.LevelSettings.nodes[w, h + 1].gameObject);

				if (levelManager.LevelSettings.nodes[w, h + 1].transform.GetChild(0).GetComponent<Block>().neighborsBlocks.Contains(levelManager.LevelSettings.nodes[w, h].gameObject) == false)
					levelManager.LevelSettings.nodes[w, h + 1].transform.GetChild(0).GetComponent<Block>().neighborsBlocks.Add(levelManager.LevelSettings.nodes[w, h].gameObject);

			}

		}

		if (w != levelManager.LevelSettings.width - 1)
		{
			if (levelManager.LevelSettings.nodes[w, h].ActiveSides()[1] == 1 && levelManager.LevelSettings.nodes[w + 1, h].ActiveSides()[3] == 1)
			{

				if (blockManager.blockSequence.Contains(levelManager.LevelSettings.nodes[w, h].gameObject) == false)
				{
					blockManager.blockSequence.Add(levelManager.LevelSettings.nodes[w, h].gameObject);
				}
				if (blockManager.blockSequence.Contains(levelManager.LevelSettings.nodes[w + 1, h].gameObject) == false)
				{
					blockManager.blockSequence.Add(levelManager.LevelSettings.nodes[w + 1, h].gameObject);
				}

				if (neighborsBlocks.Contains((levelManager.LevelSettings.nodes[w + 1, h].gameObject)) == false)
					neighborsBlocks.Add(levelManager.LevelSettings.nodes[w + 1, h].gameObject);

				if (levelManager.LevelSettings.nodes[w + 1, h].transform.GetChild(0).GetComponent<Block>().neighborsBlocks.Contains(levelManager.LevelSettings.nodes[w, h].gameObject) == false)
					levelManager.LevelSettings.nodes[w + 1, h].transform.GetChild(0).GetComponent<Block>().neighborsBlocks.Add(levelManager.LevelSettings.nodes[w, h].gameObject);

			}

		}

		if (w != 0)
		{
			if (levelManager.LevelSettings.nodes[w, h].ActiveSides()[3] == 1 && levelManager.LevelSettings.nodes[w - 1, h].ActiveSides()[1] == 1)
			{

				if (blockManager.blockSequence.Contains(levelManager.LevelSettings.nodes[w, h].gameObject) == false)
				{
					blockManager.blockSequence.Add(levelManager.LevelSettings.nodes[w, h].gameObject);

				}
				if (blockManager.blockSequence.Contains(levelManager.LevelSettings.nodes[w - 1, h].gameObject) == false)
				{
					blockManager.blockSequence.Add(levelManager.LevelSettings.nodes[w - 1, h].gameObject);

				}


				if (neighborsBlocks.Contains((levelManager.LevelSettings.nodes[w - 1, h].gameObject)) == false)
					neighborsBlocks.Add(levelManager.LevelSettings.nodes[w - 1, h].gameObject);

				if (levelManager.LevelSettings.nodes[w - 1, h].transform.GetChild(0).GetComponent<Block>().neighborsBlocks.Contains(levelManager.LevelSettings.nodes[w, h].gameObject) == false)
					levelManager.LevelSettings.nodes[w - 1, h].transform.GetChild(0).GetComponent<Block>().neighborsBlocks.Add(levelManager.LevelSettings.nodes[w, h].gameObject);

			}

		}

		if (h != 0)
		{
			if (levelManager.LevelSettings.nodes[w, h].ActiveSides()[2] == 1 && levelManager.LevelSettings.nodes[w, h - 1].ActiveSides()[0] == 1)
			{
				if (blockManager.blockSequence.Contains(levelManager.LevelSettings.nodes[w, h].gameObject) == false)
				{
					blockManager.blockSequence.Add(levelManager.LevelSettings.nodes[w, h].gameObject);
				}
				if (blockManager.blockSequence.Contains(levelManager.LevelSettings.nodes[w, h - 1].gameObject) == false)
				{
					blockManager.blockSequence.Add(levelManager.LevelSettings.nodes[w, h - 1].gameObject);

				}


				if (neighborsBlocks.Contains((levelManager.LevelSettings.nodes[w, h - 1].gameObject)) == false)
				neighborsBlocks.Add(levelManager.LevelSettings.nodes[w, h - 1].gameObject);

				if (levelManager.LevelSettings.nodes[w, h - 1].transform.GetChild(0).GetComponent<Block>().neighborsBlocks.Contains(levelManager.LevelSettings.nodes[w, h].gameObject) == false)
					levelManager.LevelSettings.nodes[w, h - 1].transform.GetChild(0).GetComponent<Block>().neighborsBlocks.Add(levelManager.LevelSettings.nodes[w, h].gameObject);

			}


		}

	}

	private void DestroyIvy()
	{
		ivyLeafsObject.GetComponent<IvyController>().PlayParticles();

		ivyLeafsObject.GetComponent<IvyController>().ResetVars();

		//ivyStickObject.GetComponent<IvyController>().PlayParticles();
		ivyStickObject.GetComponent<IvyController>().ResetVars();

		blockManager.blockSequence.Remove(transform.parent.gameObject);

	}

	public void ResetVars()
    {
        for (int i = 0; i < neighborsBlocks.Count; i++)
        {
			neighborsBlocks.RemoveAt(i);

		}
    }
}
