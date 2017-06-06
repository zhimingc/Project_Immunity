using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct GridData
{
  public GridData(int val)
  {
    value = val;
  }
  public int value;
}

public class GridBehaviour : MonoBehaviour {

  //public Sprite[] blockSprites;
  public BLOCK overrideBlock;
  public Block currentBlockType;
  public int[] coordinates;
  public bool showCouplers;
  public GameObject[] couplers;
  public GridData data;

  public bool isUpdated;   // used to catch loops

  void Awake()
  {
    currentBlockType = Block.BlockFactory(overrideBlock);
  }

  // Use this for initialization
  void Start () {
    isUpdated = false;

    // Set and init couplers
    SetCouplers(showCouplers);
    foreach (GameObject go in couplers)
    {
      go.GetComponent<LineCoupler>().gridB = this;
    }
    couplers[0].GetComponent<LineCoupler>().dir = DIRECTION.LEFT;
    couplers[1].GetComponent<LineCoupler>().dir = DIRECTION.RIGHT;
    couplers[2].GetComponent<LineCoupler>().dir = DIRECTION.TOP;
    couplers[3].GetComponent<LineCoupler>().dir = DIRECTION.BOTTOM;

    if (overrideBlock == BLOCK.END)
    {
      GetComponent<EndBlockScript>().endValue = data.value;
    }

    // Init block
    if (currentBlockType.blockType < BLOCK.NUM_USER_BLOCKS)
      ScrollBlocks(currentBlockType.blockType);
  }

  public bool CanSpawnOutput()
  {
    int numOut = 0;
    foreach (GameObject go in couplers)
    {
      LineCoupler lc = go.GetComponent<LineCoupler>();
      if (lc.inUse && !lc.isInput) ++numOut;
    }

    return numOut < currentBlockType.maxOut;
  }

  public bool CanSpawnInput()
  {
    int numIn = 0;
    foreach (GameObject go in couplers)
    {
      LineCoupler lc = go.GetComponent<LineCoupler>();
      if (lc.inUse && lc.isInput) ++numIn;
    }

    return numIn < currentBlockType.maxIn;
  }

  void UpdateGridData()
  {
    currentBlockType.UpdateGridValue(this);

    // Update value text
    if (overrideBlock != BLOCK.END)
    {
      SetValue(data.value);
    }
  }

  public GridData GetGridData()
  {
    UpdateGridData();
    return data;
  }

  void OnMouseOver()
  {
    if ((int)currentBlockType.blockType < (int)BLOCK.NUM_USER_BLOCKS &&
      GameManager.Instance.userState == USER_STATE.IDLE)
    {
      if (Input.GetMouseButtonUp(0) && 
        GameManager.Instance.uiMan.isDragging)
      {
        ScrollBlocks(GameManager.Instance.uiMan.GetBlockClicked());
      }

      if (Input.GetMouseButtonUp(1))
      {
        ScrollBlocks((int)BLOCK.EMPTY);
      }

      if (currentBlockType.blockType != BLOCK.EMPTY)
      {
        SetCouplers(true);
      }
      else
      {
        SetCouplers(false);
      }
    }
  }

  void ScrollBlocks()
  {
    BLOCK nextBlock = (BLOCK) (((int)currentBlockType.blockType + 1) % (int)BLOCK.NUM_USER_BLOCKS);
    ScrollBlocks(nextBlock); 
  }

  void ScrollBlocks(BLOCK nextBlock)
  {
    currentBlockType = Block.BlockFactory(nextBlock);
    GetComponent<SpriteRenderer>().sprite = currentBlockType.blockSprite;

    LineController lineC = GetComponent<LineController>();
    if (lineC.currentLine != null)
    {
      lineC.currentLine.ResetPath();
    }

    foreach (GameObject go in couplers)
    {
      LineCoupler lc = go.GetComponent<LineCoupler>();

      if (lc.currentLine != null)
      {
        lc.currentLine.ResetPath();
      }
    }

    SetValue(0);

    // Custom collider size depending on block type
    if (currentBlockType.blockType == BLOCK.EMPTY)
      GetComponent<BoxCollider2D>().size = new Vector2(2.0f, 2.0f);
    else
      GetComponent<BoxCollider2D>().size = new Vector2(1.0f, 1.0f);

  }

  public void SetCouplers(bool flag)
  {
    GridManager gridMan = GameObject.Find("GridManager").GetComponent<GridManager>();
    List<List<GameObject>> grid = GameManager.Instance.grid;
    showCouplers = flag;

    // Reset spawners
    foreach (GameObject go in couplers) go.SetActive(false);

    if (coordinates[0] - 1 >= 0)
    {
      GameObject curBlock = grid[coordinates[0] - 1][coordinates[1]];
      LineCoupler lineC = couplers[0].GetComponent<LineCoupler>();

      couplers[0].SetActive(flag);
      lineC.currentBlock = gameObject;
      if (overrideBlock != BLOCK.END) lineC.canSpawnLines = true;
    }

    if (coordinates[0] + 1 < gridMan.gridSizeX)
    {
      GameObject curBlock = grid[coordinates[0] + 1][coordinates[1]];
      LineCoupler lineC = couplers[1].GetComponent<LineCoupler>();

      couplers[1].SetActive(flag);
      lineC.currentBlock = gameObject;
      if (overrideBlock != BLOCK.END) lineC.canSpawnLines = true;
    }

    if (coordinates[1] + 1 < gridMan.gridSizeY)
    {
      GameObject curBlock = grid[coordinates[0]][coordinates[1] + 1];
      LineCoupler lineC = couplers[2].GetComponent<LineCoupler>();

      couplers[2].SetActive(flag);
      lineC.currentBlock = gameObject;
      if (overrideBlock != BLOCK.END) lineC.canSpawnLines = true;
    }

    if (coordinates[1] - 1 >= 0)
    {
      GameObject curBlock = grid[coordinates[0]][coordinates[1] - 1];
      LineCoupler lineC = couplers[3].GetComponent<LineCoupler>();

      couplers[3].SetActive(flag);
      lineC.currentBlock = gameObject;
      if (overrideBlock != BLOCK.END) lineC.canSpawnLines = true;
    }
  }

  public void SetValue(int val, bool overRide = false)
  {
    if ((currentBlockType.blockType == BLOCK.START || currentBlockType.blockType == BLOCK.END) &&
      overRide)
      data = new GridData(val);

    DisplayValue(val);
  }

  public void DisplayValue(int val)
  {
    Text childText = GetComponentInChildren<Text>();
    if (childText)
    {
      childText.text = val.ToString();
    }

    LineController lineC = GetComponent<LineController>();
    if ((lineC.currentLine == null || 
      !lineC.currentLine.exists) &&
      currentBlockType.blockType == BLOCK.EMPTY)
    {
      childText.text = "";
    }
  }

  // Recursively update blocks through couplers
  public void UpdateBlockValue()
  {
    UpdateGridData();

    // Send update to each output couplers
    foreach(GameObject obj in couplers)
    {
      LineCoupler lc = obj.GetComponent<LineCoupler>();
      if (lc.isInput) continue;

      if (lc.currentLine != null)
      {
        int resValue = GetGridData().value;
        //lc.currentLine.value = resValue;
        lc.currentLine.UpdateLineValue(resValue);
        if (lc.currentLine.lineCouplers[1] != null)
          lc.currentLine.lineCouplers[1].UpdateCouplerValue();
      }

    }
  }

  public void UpdateOtherCouplers()
  {
    switch(currentBlockType.blockType)
    {
      case BLOCK.START:
        foreach (GameObject go in couplers)
        {
          LineCoupler lc = go.GetComponent<LineCoupler>();
          if (!lc.inUse) continue;
          ToggleOtherCouplers(false);
          return;
        }
        break;
      case BLOCK.END:
        // End blocks should have only 1 input and 1 output

        foreach (GameObject go in couplers)
        {
          LineCoupler lc = go.GetComponent<LineCoupler>();
          if (!lc.inUse) continue;
          ToggleOtherCouplers(false);
          return;
        }
        break;
    }

    // If no coupler is in use, activate all couplers
    if (currentBlockType.blockType != BLOCK.EMPTY) SetCouplers(true);
  }

  // Toggle the other couplers which are not in use
  public void ToggleOtherCouplers(bool flag)
  {
    foreach (GameObject go in couplers)
    {
      LineCoupler lc = go.GetComponent<LineCoupler>();
      if (lc.inUse) continue;
      go.SetActive(flag);
    }
  }

  // Check other couplers for existing line
  public bool IsLineExisting(Line curLine)
  {
    GameObject fromBlock = curLine.lineCouplers[0].currentBlock;

    foreach (GameObject go in couplers)
    {
      LineCoupler lc = go.GetComponent<LineCoupler>();
      if (!lc.inUse) continue;

      if (lc.currentLine.lineCouplers[1] != null &&
        lc.currentLine.lineCouplers[1].currentBlock == fromBlock)
      {
        return true;
      }
    }
    return false;
  }
}
