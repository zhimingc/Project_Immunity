using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineController : MonoBehaviour {

  public Line currentLine = null;

  private LineManager lineMan;
  private GridManager gridMan;

	// Use this for initialization
	void Start () {
    currentLine = null;

    lineMan = GameObject.Find("LineManager").GetComponent<LineManager>();
    gridMan = GameObject.Find("GridManager").GetComponent<GridManager>();
  }

  void OnMouseEnter()
  {
    GameManager.Instance.UpdateObjsIntersect(1);

    // Update line if the mouse returns to a grid holding the current line
    if (GameManager.Instance.userState == USER_STATE.DRAWING &&
      GetComponent<GridBehaviour>().currentBlockType.blockType == BLOCK.EMPTY)
    {
      gridMan.ResetIllegalBlock();
      bool legalMove = CheckIfMoveIsLegal();
      bool updatingExisting = CheckExistingLine(ref lineMan.currentLineBeingDrawn);

      if (!updatingExisting && legalMove)
      {
        if (legalMove)
        {
          // Track line for this controller and add grid pos
          UpdateCurrentLine(lineMan.currentLineBeingDrawn);
          AddPositionToLine(lineMan.currentLineBeingDrawn);
          lineMan.DrawLegalBlocks(gameObject);
        }
        else
        {
          gridMan.UpdateIllegalBlock(transform.position);
        }
      }
    }
  }

  void OnMouseExit()
  {
    GameManager.Instance.UpdateObjsIntersect(-1);
  }

  void OnMouseOver()
  {
    if (GameManager.Instance.levelCompleted == true) return;

    // Toggle player input state between idle and drawing
    if (Input.GetMouseButtonDown(0))
    {
      switch (GameManager.Instance.userState)
      {
        case USER_STATE.IDLE:
          DefaultBlockMouseDown();
          break;
        case USER_STATE.DRAWING:
          break;
      }
    }
    if (Input.GetMouseButtonUp(0))
    {
      switch (GameManager.Instance.userState)
      {
        case USER_STATE.IDLE:
          break;
        case USER_STATE.DRAWING:
          if (GetComponent<GridBehaviour>().currentBlockType.blockType == BLOCK.EMPTY)
          {
            gridMan.ResetAllBlocks();
            lineMan.currentLineBeingDrawn.lineCouplers[0].UpdateCouplerValue();
            GameManager.Instance.ChangeUserState(USER_STATE.IDLE);
            lineMan.currentLineBeingDrawn = null;
          }
          break;
      }
    }
  }

  void DefaultBlockMouseDown()
  {
    if (CheckExistingLine(ref currentLine))
    {
      lineMan.currentLineBeingDrawn = currentLine;
      GameManager.Instance.ChangeUserState(USER_STATE.DRAWING);
      lineMan.DrawLegalBlocks(gameObject);
    }
  }

  void AddPositionToLine(Line line)
  {
    // Update line renderer
    lineMan.AddPointToLine(transform.position);

    // Add GO with the position
    line.linePath.Add(this);
  }

  public void UpdateCurrentLine(Line lineToAdd)
  {
    int[] coord = GetComponent<GridBehaviour>().coordinates;
    List<List<GameObject>> grid = GameManager.Instance.grid;
    LineController curLineC = grid[coord[0]][coord[1]].GetComponent<LineController>();

    currentLine = lineToAdd;

    if (coord[0] >= 0 && coord[0] < gridMan.gridSizeX &&
      coord[1] >= 0 && coord[1] < gridMan.gridSizeY)
    {
      curLineC.currentLine = lineToAdd;
    }

    if (lineToAdd == null)
    {
      SetValue(0);
    }
    else
    {
      SetValue(lineToAdd.value);
    }
  }

  bool CheckIfMoveIsLegal()
  {
    bool success = true;
    gridMan.ResetIllegalBlock();
    Line line = lineMan.currentLineBeingDrawn;

    // Check if the block is already occupied
    if (currentLine != null ||
      GetComponent<GridBehaviour>().currentBlockType.blockType != BLOCK.EMPTY) success = false;
    else
    {
      List<LineController> linePath = line.linePath;
      int[] thisCoord = GetComponentInParent<GridBehaviour>().coordinates;
      int[] prevCoord;
      // Legal move checking from a coupler is more specific
      LineCoupler fromCoupler = null; 

      if (line.linePath.Count == 0)
      {
        fromCoupler = line.lineCouplers[0];
        prevCoord = fromCoupler.GetComponentInParent<GridBehaviour>().coordinates;
      }
      else
      {
        prevCoord = linePath[linePath.Count - 1].GetComponentInParent<GridBehaviour>().coordinates;
      }

      int xDiff = Mathf.Abs(thisCoord[0] - prevCoord[0]);
      int yDiff = Mathf.Abs(thisCoord[1] - prevCoord[1]);

      success = CheckLegalDirection(xDiff, yDiff, fromCoupler);
    }

    return success;
  }

  bool CheckLegalDirection(int xDiff, int yDiff, LineCoupler fromCoupler)
  {
    bool success = false;

    if (fromCoupler != null)
    {
      success = fromCoupler.CheckLegalDirection(xDiff, yDiff);
    }
    else
    {
      success = !((xDiff > 0 && yDiff > 0) || (xDiff > 1 || yDiff > 1));
    }

    return success;
  }

  bool CheckExistingLine(ref Line curLine)
  {
    bool success = false;
    if (curLine == null) return false;

    // Update line positions if it exists in line
    if (curLine.linePath.Contains(this))
    {
      LineRenderer currentRenderer = curLine.lineRenderer;
      Vector3[] listArr = new Vector3[currentRenderer.positionCount];
      currentRenderer.GetPositions(listArr);
      List<Vector3> listPos = new List<Vector3>(listArr);

      if (curLine.lineCouplers[1] != null)
      {
        listPos.RemoveAt(listPos.Count - 1);
        curLine.lineCouplers[1].ResetCoupler();
        curLine.lineCouplers[1] = null;
      }

      int curPosIndex = curLine.linePath.IndexOf(this);
      int removeCount = curLine.linePath.Count - (curPosIndex + 1);
      listPos.RemoveRange(curPosIndex + 2, removeCount);
      currentRenderer.positionCount = listPos.Count;
      currentRenderer.SetPositions(listPos.ToArray());

      // Holds on to reference if line needs to be removed
      if (currentRenderer.positionCount != 0)
      {
        for (int i = curPosIndex + 1; i < curLine.linePath.Count; ++i)
        {
          LineController curLineC = curLine.linePath[i].GetComponent<LineController>();
          curLineC.UpdateCurrentLine(null);
        }
        curLine.linePath.RemoveRange(curPosIndex + 1, removeCount);
      }

      success = true;
      lineMan.DrawLegalBlocks(gameObject);
    }

    return success;
  }

  public void SetValue(int val)
  {
    GetComponent<GridBehaviour>().SetValue(val);
  }
}
