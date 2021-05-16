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

	void Start()
	{

		//Build3dLevel();
		//SetBlocksIds();
	}

	public void CheckAll()
	{
		for (int i = 0; i < blockSequence.Count; i++)
		{
			blockSequence[i].transform.GetChild(0).GetComponent<Block>().Check();
			blockSequence[i].transform.GetChild(0).GetComponent<Block>().CheckNeighbors();
		}
	}

	public void CheckAllSequence()
	{

		for (int i = 0; i < blockSequence.Count; i++)
		{
			blockSequence[i].transform.GetChild(0).GetComponent<Block>().CheckNeighborsCount();
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
	}

	public void Create3dBlock(int newNodeType, Transform parent)
	{
		GameObject block3d = null;

		GameObject gameBlock = null;

		if (newNodeType == 5)//corner
		{
			block3d = Resources.Load<GameObject>("Curve3d");
			block3d.transform.localScale = new Vector3(50, 50, 50);
			gameBlock = Instantiate(block3d, parent.transform.position, parent.transform.rotation, parent);

			//ivy3dObject = transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
		}
		else if (newNodeType == 1)
		{
			block3d = Resources.Load<GameObject>("Start3d");

			block3d.transform.localScale = new Vector3(50, 50, 50);
			gameBlock = Instantiate(block3d, parent.transform.position, parent.transform.rotation, parent);

			//ivy3dObject = transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
		}
		else if (newNodeType == 2)
		{
			block3d = Resources.Load<GameObject>("Straight3d");

			block3d.transform.localScale = new Vector3(50, 50, 50);
			gameBlock = Instantiate(block3d, parent.transform.position, parent.transform.rotation, parent);

			//ivy3dObject = transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
		}
		else if (newNodeType == 3)
		{
			block3d = Resources.Load<GameObject>("T3d");

			block3d.transform.localScale = new Vector3(50, 50, 50);
			gameBlock = Instantiate(block3d, parent.transform.position, parent.transform.rotation, parent);

			//ivy3dObject = transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
		}
		else if (newNodeType == 4)
		{
			block3d = Resources.Load<GameObject>("CrossBlock3d");

			block3d.transform.localScale = new Vector3(50, 50, 50);
			gameBlock = Instantiate(block3d, parent.transform.position, parent.transform.rotation, parent);

			//ivy3dObject = transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
		}


		if (block3d != null)
		{

			blockList.Add(gameBlock);

		}
	}


	public void ResetAllVars()
    {
        /*for (int i = 0; i < blockSequence.Count; i++)
        {
			blockSequence.RemoveAt(i);
        }*/
		blockSequence.Clear();
		/*for (int i = 0; i < blockList.Count; i++)
		{
			blockList.RemoveAt(i);
		}*/

		blockList.Clear();

	}
}
