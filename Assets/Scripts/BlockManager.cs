using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
	public LevelManager levelManager;

    public List<GameObject> blockList = new List<GameObject>();

	public List<GameObject> blockSequence = new List<GameObject>();

	void SetBlocksIds()
    {
        for (int i = 0; i < blockList.Count; i++)
        {
            blockList[i].GetComponent<Block>().SetId(i);
        }
    }

	public void PlaySqIvyAnim()
    {
        for (int i = 0; i < blockSequence.Count; i++)
        {
			blockSequence[i].transform.GetChild(0).GetComponent<Block>().StartIvyIncrease();
		}
	}


	public void Build3dLevel()
    {
		ResetAllVars();

		foreach (Node n in levelManager.LevelSettings.nodes)
		{
			Create3dBlock(n.newNodeType, n.transform);
		}

		SetBlocksIds();

		BlocksColor();
	}

	private void BlocksColor()
    {
		float randGreyValue;

        for (int i = 0; i < blockList.Count; i++)
        {
			randGreyValue = Random.RandomRange(0.4f, 0.55f);
			if (blockList[i].transform.GetChild(0) != null)
			blockList[i].GetComponent<Block>().SetColor(randGreyValue);

		}
    }

	public void Create3dBlock(int newNodeType, Transform parent)
	{
		GameObject block3d = null;

		GameObject gameBlock = null;

		if (newNodeType == 5)//corner
		{
			block3d = Resources.Load<GameObject>("BlockCurve");
			block3d.transform.localScale = new Vector3(50, 50, 50);
			gameBlock = Instantiate(block3d, parent.transform.position, parent.transform.rotation, parent);

		}
		else if (newNodeType == 1)
		{
			block3d = Resources.Load<GameObject>("BlockStart");

			block3d.transform.localScale = new Vector3(50, 50, 50);
			gameBlock = Instantiate(block3d, parent.transform.position, parent.transform.rotation, parent);
		}
		else if (newNodeType == 2)
		{
			block3d = Resources.Load<GameObject>("BlockStraight");

			block3d.transform.localScale = new Vector3(50, 50, 50);
			gameBlock = Instantiate(block3d, parent.transform.position, parent.transform.rotation, parent);

		}
		else if (newNodeType == 3)
		{
			block3d = Resources.Load<GameObject>("BlockT");

			block3d.transform.localScale = new Vector3(50, 50, 50);
			gameBlock = Instantiate(block3d, parent.transform.position, parent.transform.rotation, parent);
		}
		else if (newNodeType == 4)
		{
			block3d = Resources.Load<GameObject>("BlockCross");

			block3d.transform.localScale = new Vector3(50, 50, 50);
			gameBlock = Instantiate(block3d, parent.transform.position, parent.transform.rotation, parent);
		}


		if (block3d != null)
		{

			blockList.Add(gameBlock);

		}
	}


	public void ResetAllVars()
    {
		blockSequence.Clear();

		blockList.Clear();
	}
}
