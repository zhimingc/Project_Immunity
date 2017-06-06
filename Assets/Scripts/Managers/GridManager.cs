using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {

  public GameObject gridObj, legalBlock;
  public Vector2 gridBlockSize;
  public int gridSizeX, gridSizeY;
  public float spacing;
  public GameObject[] legalBlockFeedback;
  private GameObject illegalBlock;
  private List<NodeBlock> specialBlocks;

  // Use this for initialization
  void Awake () {
    // Gen illegal block
    illegalBlock = Instantiate(legalBlock);
    illegalBlock.GetComponent<SpriteRenderer>().color = Color.red;
    ResetIllegalBlock();

    // Gen legal block
    legalBlockFeedback = new GameObject[4];
    for(int i = 0; i < legalBlockFeedback.Length; ++i)
    {
      legalBlockFeedback[i] = Instantiate(legalBlock);
    }
    ResetLegalBlocks();

    int curLevel = GameManager.Instance.currentLevel;
    ConstructLevel(LevelData.puzzleData[curLevel]);
  }

  public void ConstructLevel(PuzzleData puzzleToLoad)
  {
    // Init grid man data
    gridSizeX = puzzleToLoad.gridSize[0];
    gridSizeY = puzzleToLoad.gridSize[1];

    // Gen special block list
    specialBlocks = puzzleToLoad.startBlocks;
    specialBlocks.AddRange(puzzleToLoad.endBlocks);

    ConstructGrid(gridSizeX, gridSizeY);
    // Generate the start and end blocks
    GetComponent<LevelManager>().GenerateLevel(puzzleToLoad);

    // Setting camera size according to size of grid (wip)
    if (gridSizeX >= 7 || gridSizeY >= 6) Camera.main.orthographicSize = 9;
    else if (gridSizeX >= 5 || gridSizeY >= 4) Camera.main.orthographicSize = 7;
    else Camera.main.orthographicSize = 5;
  }

  // Update is called once per frame
  void Update () {
		
	}

  public void ResetAllBlocks()
  {
    ResetIllegalBlock();
    ResetLegalBlocks();
  }

  public void ResetIllegalBlock()
  {
    illegalBlock.transform.position = new Vector3(100, 100, 0);
  }

  public void ResetLegalBlocks()
  {
    for (int i = 0; i < legalBlockFeedback.Length; ++i)
    {
      legalBlockFeedback[i].transform.position = new Vector3(100, 100, 0);
    }
  }

  public void ConstructGrid(int[] gridSizes)
  {
    ConstructGrid(gridSizes[0], gridSizes[1]);
  }

  void ConstructGrid(int gridX, int gridY)
  {
    GameManager.Instance.ResetGrid();

    //Vector3 gridBlockDim = gridObj.transform.localScale;

    int halfGridX = gridX / 2;
    int halfGridY = gridY / 2;
    int xOffset = gridSizeX % 2;
    int yOffset = gridSizeY % 2;
    float posOffsetX = xOffset == 0 ? gridBlockSize.x : 0;
    float posOffsetY = yOffset == 0 ? gridBlockSize.y : 0;

    for (int x = -halfGridX; x < halfGridX + xOffset; ++x)
    {
      // Update GameManager
      GameManager.Instance.grid.Add(new List<GameObject>(new GameObject[gridSizeY]));

      for (int y = -halfGridY; y < halfGridY + yOffset; ++y)
      {
        bool skip = false;
        // Check if there is an end block there
        for (int i = 0; i < specialBlocks.Count; ++i)
        {
          if ((x + halfGridX) == (int)specialBlocks[i].pos.x &&
            (y + halfGridY) == (int)specialBlocks[i].pos.y)
          {
            skip = true;
            break;
          }
        }
        if (skip) continue;

        Vector3 instPos = new Vector3(x * (gridBlockSize.x + spacing) + posOffsetX, 
          y * (gridBlockSize.y + spacing) + posOffsetY, 0.0f);
        GameObject curGridBlock = Instantiate(gridObj, instPos, Quaternion.identity);
        curGridBlock.transform.localScale = new Vector3(gridBlockSize.x, gridBlockSize.y, 1.0f);

        // Add grid coordinates
        curGridBlock.GetComponent<GridBehaviour>().coordinates = new int[2] { x + halfGridX, y + halfGridY};

        // Update GameManager
        GameManager.Instance.grid[x + halfGridX][y + halfGridY] = curGridBlock;
      }
    }
  }

  public void UpdateIllegalBlock(Vector3 pos)
  {
    illegalBlock.transform.position = pos;
  }
}
