using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NodeBlock
{
  public NodeBlock()
  {
    pos = new Vector2(-1.0f, -1.0f);
    value = -1;
  }

  public Vector2 pos;
  public int value;
}

public class LevelManager : MonoBehaviour {

  public GameObject startBlockObj, endBlockObj;
  public NodeBlock[] startBlocks;
  public NodeBlock[] endBlocks;
  private List<GameObject> startBlockObjs, endBlockObjs;

  // Use this for initialization
  void Start () {

  }
	
	// Update is called once per frame
	void Update () {
		
	}

  void ResetLevel()
  {
    if (startBlockObjs != null)
    {
      foreach (GameObject go in startBlockObjs) Destroy(go);
    }

    if (endBlockObjs != null)
    {
      foreach (GameObject go in endBlockObjs) Destroy(go);
    }
  }

  public void GenerateLevel(PuzzleData puzzle)
  {
    ResetLevel();

    startBlockObjs = new List<GameObject>();
    endBlockObjs = new List<GameObject>();

    //GenerateBlocks(startBlocks, startBlockObj, startBlockObjs);
    //GenerateBlocks(endBlocks, endBlockObj, endBlockObjs);

    //int currentLevel = GameManager.Instance.currentLevel;

    startBlocks = puzzle.startBlocks.ToArray();
    endBlocks = puzzle.endBlocks.ToArray();
    GenerateBlocks(startBlocks, startBlockObj, startBlockObjs);
    GenerateBlocks(endBlocks, endBlockObj, endBlockObjs);

    GameManager.Instance.SetEndBlocks(endBlockObjs);
  }

  void GenerateBlocks(NodeBlock[] blocks, GameObject obj, List<GameObject> objs)
  {
    GridManager gridMan = GetComponent<GridManager>();
    Vector2 gridBlockSize = gridMan.gridBlockSize;
    float spacing = gridMan.spacing;
    int halfGridX = gridMan.gridSizeX / 2;
    int halfGridY = gridMan.gridSizeY / 2;
    int xOffset = gridMan.gridSizeX % 2;
    int yOffset = gridMan.gridSizeY % 2;
    float posOffsetX = xOffset == 0 ? gridBlockSize.x : 0;
    float posOffsetY = yOffset == 0 ? gridBlockSize.y : 0;

    for (int i = 0; i < blocks.Length; ++i)
    {
      Vector3 instPos = new Vector3((-halfGridX + blocks[i].pos.x) * (gridBlockSize.x + spacing) + posOffsetX,
        (-halfGridY + blocks[i].pos.y) * (gridBlockSize.y + spacing) + posOffsetY, 0.0f);
      instPos += transform.position;  // Add position of obj to offset entire grid

      GameObject curGridBlock = Instantiate(obj, instPos, Quaternion.identity);
      curGridBlock.transform.localScale = new Vector3(gridBlockSize.x, gridBlockSize.y, 1.0f);

      // Add grid coordinates
      curGridBlock.GetComponent<GridBehaviour>().coordinates = new int[2] { (int)blocks[i].pos.x, (int)blocks[i].pos.y };

      // Update GameManager
      if ((int)blocks[i].pos.x >= 0 && (int)blocks[i].pos.x < gridMan.gridSizeX &&
        (int)blocks[i].pos.y >= 0 && (int)blocks[i].pos.y < gridMan.gridSizeY)
      GameManager.Instance.grid[(int)blocks[i].pos.x][(int)blocks[i].pos.y] = curGridBlock;

      GridBehaviour gridScript = curGridBlock.GetComponent<GridBehaviour>();
      if (gridScript)
      {
        gridScript.SetValue(blocks[i].value, true);
      }

      // Initialize list of blocks
      objs.Add(curGridBlock);
    }
  }
}
