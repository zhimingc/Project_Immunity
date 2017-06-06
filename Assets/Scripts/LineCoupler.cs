using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DIRECTION
{
  LEFT,
  RIGHT,
  TOP,
  BOTTOM
};

public class LineCoupler : MonoBehaviour {

  public Line currentLine;
  public GameObject currentBlock;
  public DIRECTION dir;
  public bool isInput;
  public bool inUse;
  public bool canSpawnLines;

  public GridBehaviour gridB;
  private LineManager lineMan;
  private GridManager gridMan;

  // Animation data
  //private Vector3 originScale;
  private ParticleSystem couplerParticles;
  //private Color lineCol;

  // Use this for initialization
  void Start () {
    inUse = false;
    isInput = false;
    currentLine = null;
    currentBlock = GetComponentsInParent<Transform>()[1].gameObject;

    lineMan = GameObject.Find("LineManager").GetComponent<LineManager>();
    gridMan = GameObject.Find("GridManager").GetComponent<GridManager>();
    gridB = GetComponentInParent<GridBehaviour>();

    // Set animation data
    couplerParticles = GetComponentInChildren<ParticleSystem>();
    //UpdateCouplerColor();
    var psMain = couplerParticles.main;
    psMain.startSize = 1.2f;
  }

  // Update is called once per frame
  void Update () {
    UpdateCouplerFeedback();
  }

  void UpdateCouplerFeedback()
  {
    ParticleSystem.EmissionModule em = couplerParticles.emission;
    ParticleSystem.ColorOverLifetimeModule colorLifetime = couplerParticles.colorOverLifetime;

    //if (gridB) colorLifetime.color = gridB.currentBlockType.lineCol;

    switch (GameManager.Instance.userState)
    {
      case USER_STATE.IDLE:
        if (!inUse && canSpawnLines && gridB.CanSpawnOutput())
        {
          couplerParticles.Play();
        }
        else
        {
          couplerParticles.Clear();
          couplerParticles.Stop();
        }
        break;
      case USER_STATE.DRAWING:
        // don't turn on feedback if it belongs to the same grid
        if (!inUse &&
          lineMan.currentLineBeingDrawn.lineCouplers[0].currentBlock != currentBlock &&
           gridB.CanSpawnInput())
        {
          couplerParticles.Play();
        }
        else
        {
          couplerParticles.Clear();
          couplerParticles.Stop();
        }
        break;
    }
  }

  void OnMouseEnter()
  {
    GameManager.Instance.UpdateObjsIntersect(1);

    // Update line if the mouse returns to a grid holding the current line
    if (GameManager.Instance.userState == USER_STATE.DRAWING)
    {
      gridMan.ResetIllegalBlock();
      bool legalConnect = CheckLegalConnect();

      if (legalConnect)
      {
        // Track line for this controller and add grid pos
        UpdateCoupler(lineMan.currentLineBeingDrawn);
        UpdateLine(lineMan.currentLineBeingDrawn, 1);
      }
    }
  }

  void OnMouseExit()
  {
    GameManager.Instance.UpdateObjsIntersect(-1);

    if (GameManager.Instance.userState == USER_STATE.DRAWING &&
      lineMan.currentLineBeingDrawn.lineCouplers[1] == this)
    {
      RemoveCouplerFromLine(currentLine);
    }
  }

  void OnMouseOver()
  {
    // Toggle player input state between idle and drawing
    if (Input.GetMouseButtonUp(0))
    {
      switch (GameManager.Instance.userState)
      {
        case USER_STATE.IDLE:
          DefaultBlockMouseDown();
          //lineMan.DrawLegalBlocks(gameObject);
          break;
        case USER_STATE.DRAWING:
          if (currentLine == null || currentLine.lineCouplers[0] == this)
          {
            ResetPath();
          }
          else  // Update block at end coupler
          {
            inUse = true;
            isInput = true;
            UpdateCouplerValue();
          }

          GameManager.Instance.ChangeUserState(USER_STATE.IDLE);
          gridMan.ResetAllBlocks();
          lineMan.currentLineBeingDrawn = null;
          break;
      }
    }
  }

  bool CheckForLoop()
  {
    bool success = false;

    return success;
  }

  public void UpdateCouplerValue()
  {
    gridB.UpdateOtherCouplers();
    gridB.UpdateBlockValue();
  }

  bool CheckLegalConnect()
  {
    bool success = true;
    Line curLine = lineMan.currentLineBeingDrawn;

    if (curLine == currentLine)
    {
      return false;
    }

    GridBehaviour gridB = GetComponentInParent<GridBehaviour>();
    if (!curLine.CanConnect() ||
      curLine.lineCouplers.Contains(this) ||
      gridB.currentBlockType.blockType == BLOCK.START ||
      gridB.IsLineExisting(curLine))
    {
      success = false;
    }
    else
    {
      List<LineController> linePath = curLine.linePath;
      int[] thisCoord = gridB.coordinates;
      int[] prevCoord;

      if (curLine.linePath.Count == 0)
      {
        prevCoord = curLine.lineCouplers[0].GetComponentInParent<GridBehaviour>().coordinates;
      }
      else
      {
        prevCoord = linePath[linePath.Count - 1].GetComponentInParent<GridBehaviour>().coordinates;
      }

      int xDiff = Mathf.Abs(thisCoord[0] - prevCoord[0]);
      int yDiff = Mathf.Abs(thisCoord[1] - prevCoord[1]);

      if (!CheckLegalDirection(xDiff, yDiff))
      {
        success = false;
      }
    }

    if (!success)
    {
      gridMan.UpdateIllegalBlock(transform.position);
    }

    return success;
  }

  // Ensure a line coming into a coupler is from the right direction
  public bool CheckLegalDirection(int diffX, int diffY)
  {
    bool success = false;

    switch(dir)
    {
      case DIRECTION.LEFT:
      case DIRECTION.RIGHT:
        success = diffX == 1 && diffY == 0;
        break;
      case DIRECTION.TOP:
      case DIRECTION.BOTTOM:
        success = diffX == 0 && diffY == 1;
        break;
    }

    return success;
  }

  void ResetPath()
  {
    lineMan.currentLineBeingDrawn.ResetPath();
  }

  void DefaultBlockMouseDown()
  {
    if (!canSpawnLines && currentLine == null) return;

    GameManager.Instance.ChangeUserState(USER_STATE.DRAWING);

    if (currentLine == null || currentLine.exists == false)
    {
      currentLine = lineMan.AddLine();
      InitLine(currentLine);
      UpdateLine(currentLine, 0);
    }
    else if (currentLine.lineCouplers[0] == this)
    {
      Line tmpLine = currentLine;
      currentLine.ResetPath();
      currentLine = tmpLine;
      InitLine(currentLine);
      UpdateLine(currentLine, 0);
    }

    inUse = true;
    lineMan.currentLineBeingDrawn = currentLine;
    gridB.UpdateOtherCouplers();
    gridB.UpdateBlockValue();
    currentLine.value = gridB.GetGridData().value;
    lineMan.DrawLegalBlockFromCoupler(this);
  }

  void UpdateCoupler(Line line)
  {
    currentLine = line;
  }

  //public void UpdateCouplerColor()
  //{
  //  switch (gridB.currentBlockType.blockType)
  //  {
  //    case BLOCK.START:
  //    case BLOCK.END:
  //      // Teal
  //      lineCol = new Color(0.0f, 1.0f, 1.0f, 0.5f);
  //      break;
  //    //case BLOCK.ADD:
  //    //  // Coral
  //    //  lineCol = new Color(1.0f, 0.5f, 80.0f / 255.0f, 0.5f);
  //    //  break;
  //    case BLOCK.COPY:
  //      lineCol = new Color(1.0f, 215.0f / 255.0f, 0.0f, 0.5f);
  //      break;
  //    //case BLOCK.SPLIT:
  //    //  // Lime green
  //    //  lineCol = new Color(50.0f / 255.0f, 205.0f / 255.0f, 50.0f / 255.0f, 0.5f);
  //    //  break;
  //  }
  //}

  void InitLine(Line line)
  {
    // Update value
    line.value = gridB.GetGridData().value;

    // Update color
    //UpdateCouplerColor();
    Color lineCol = gridB.currentBlockType.lineCol;

    line.lineRenderer.startColor = lineCol;
    line.lineRenderer.endColor = lineCol;
    GameManager.Instance.mouseCursor.GetComponent<SpriteRenderer>().color = lineCol;
  }


  void UpdateLine(Line line, int index)
  {
    lineMan.currentLineBeingDrawn = currentLine;

    Vector3[] listArr = new Vector3[line.lineRenderer.positionCount];
    line.lineRenderer.GetPositions(listArr);

    // Add position to the line renderer
    List<Vector3> listPos = new List<Vector3>(listArr);
    if (!listPos.Contains(transform.position))
    {
      lineMan.AddPointToLine(transform.position);
    }

    // Update line's couplers
    line.AddCoupler(GetComponent<LineCoupler>(), index);
  }

  void RemoveCouplerFromLine(Line line)
  {
    currentLine = null;
    line.lineCouplers[1] = null;
    inUse = false;
    isInput = false;

    // Remove position to the line renderer
    Vector3[] listArr = new Vector3[line.lineRenderer.positionCount];
    line.lineRenderer.GetPositions(listArr);
    List<Vector3> listPos = new List<Vector3>(listArr);
    listPos.Remove(transform.position);
    line.lineRenderer.positionCount = listPos.Count;
    line.lineRenderer.SetPositions(listPos.ToArray());

    GetComponentInParent<GridBehaviour>().UpdateBlockValue();
  }

  public void ResetCoupler()
  {
    inUse = false;
    currentLine = null;
    isInput = false;
    UpdateCouplerValue();
  }
}
