using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Line
{
  public Line()
  {
    exists = false;
    linePath = new List<LineController>();
    lineCouplers = new List<LineCoupler>(new LineCoupler[2] { null, null });
  }

  public Line(bool isExists)
  {
    exists = isExists;
    linePath = new List<LineController>();
    lineCouplers = new List<LineCoupler>(new LineCoupler[2] { null, null });
  }

  public void AddCoupler(LineCoupler coupler, int index)
  {
    lineCouplers[index] = coupler;
  }

  public bool CanConnect()
  {
    return lineCouplers[0] == null || lineCouplers[1] == null;
  }

  public void ResetPath()
  {
    for (int i = 0; i < linePath.Count; ++i)
    {
      linePath[i].UpdateCurrentLine(null);
    }

    for (int i = 0; i < lineCouplers.Count; ++i)
    {
      if (lineCouplers[i] == null) continue;
      lineCouplers[i].ResetCoupler();
      lineCouplers[i] = null;
    }

    if (lineRenderer != null) lineRenderer.positionCount = 0;
    linePath.Clear();
  }

  public void UpdateLineValue(int val)
  {
    for (int i = 0; i < linePath.Count; ++i)
    {
      linePath[i].SetValue(val);
    }
    value = val;
  }

  public bool exists;
  public int value;
  public LineRenderer lineRenderer;
  public List<LineCoupler> lineCouplers;
  public List<LineController> linePath;
};

public class LineManager : MonoBehaviour {

  public GameObject lineObj;
  public List<Line> lines;
  public Line currentLineBeingDrawn;

  private GridManager gridMan;
  private GameManager gm;

	// Use this for initialization
	void Start () {
    currentLineBeingDrawn = null;
    gridMan = GameObject.Find("GridManager").GetComponent<GridManager>();
    gm = GameManager.Instance;
  }

  // Update is called once per frame
  void Update () {
    // To reset path when nothing is clicked
    if (Input.GetMouseButtonUp(0) && gm.objsIntersect <= 0)
    {
      gm.ChangeUserState(USER_STATE.IDLE);
      if (currentLineBeingDrawn != null) currentLineBeingDrawn.ResetPath();
      gridMan.ResetAllBlocks();
    }

    UpdateLineToMouseFeedback();
  }

  void UpdateLineToMouseFeedback()
  {
    // Toggle player input state between idle and drawing
    switch (GameManager.Instance.userState)
    {
      case USER_STATE.IDLE:

        break;
      case USER_STATE.DRAWING:
        if (currentLineBeingDrawn == null || currentLineBeingDrawn.lineRenderer == null) return;

        Vector3[] listArr = new Vector3[currentLineBeingDrawn.lineRenderer.positionCount];
        currentLineBeingDrawn.lineRenderer.GetPositions(listArr);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0.0f;

        // Add position to the line renderer
        List<Vector3> listPos = new List<Vector3>(listArr);
        listPos.RemoveAt(listPos.Count - 1);
        listPos.Add(mousePos);
        currentLineBeingDrawn.lineRenderer.positionCount = listPos.Count;
        currentLineBeingDrawn.lineRenderer.SetPositions(listPos.ToArray());
        break;
    }
  }

  public void AddMousePointToLine()
  {
    if (currentLineBeingDrawn == null || currentLineBeingDrawn.lineRenderer == null) return;

    Vector3[] listArr = new Vector3[currentLineBeingDrawn.lineRenderer.positionCount];
    currentLineBeingDrawn.lineRenderer.GetPositions(listArr);

    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    mousePos.z = 0.0f;

    // Add position to the line renderer
    List<Vector3> listPos = new List<Vector3>(listArr);
    listPos.Add(mousePos);
    currentLineBeingDrawn.lineRenderer.positionCount = listPos.Count;
    currentLineBeingDrawn.lineRenderer.SetPositions(listPos.ToArray());

  }

  public void RemoveMousePointFromLine()
  {
    if (currentLineBeingDrawn == null || currentLineBeingDrawn.lineRenderer == null) return;

    Vector3[] listArr = new Vector3[currentLineBeingDrawn.lineRenderer.positionCount];
    currentLineBeingDrawn.lineRenderer.GetPositions(listArr);
    // Add position to the line renderer
    List<Vector3> listPos = new List<Vector3>(listArr);
    if (listPos.Count > 0) listPos.RemoveAt(listPos.Count - 1);
    currentLineBeingDrawn.lineRenderer.positionCount = listPos.Count;
    currentLineBeingDrawn.lineRenderer.SetPositions(listPos.ToArray());
  }

  public void AddPointToLine(Vector3 pt)
  {
    Vector3[] listArr = new Vector3[currentLineBeingDrawn.lineRenderer.positionCount];
    currentLineBeingDrawn.lineRenderer.GetPositions(listArr);

    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    mousePos.z = 0.0f;

    // Add position to the line renderer
    List<Vector3> listPos = new List<Vector3>(listArr);
    if (listPos.Count > 0) listPos.RemoveAt(listPos.Count - 1);
    listPos.Add(pt);
    listPos.Add(mousePos);
    currentLineBeingDrawn.lineRenderer.positionCount = listPos.Count;
    currentLineBeingDrawn.lineRenderer.SetPositions(listPos.ToArray());
  }

  public void ResetLines()
  {
    foreach(Line line in lines)
    {
      Destroy(line.lineRenderer.gameObject);
    }

    lines.Clear();
  }

  public Line AddLine()
  {
    GameObject newObj = Instantiate(lineObj);
    Line newLine = new Line(true);
    //newLine.exists = true;
    newLine.lineRenderer = newObj.GetComponent<LineRenderer>();
    lines.Add(newLine);
    return newLine;
  }

  public void RemoveLine(Line line)
  {
    Destroy(line.lineRenderer.gameObject);
    lines.Remove(line);
  }

  public void RemoveLine()
  {
    Destroy(currentLineBeingDrawn.lineRenderer.gameObject);
    lines.Remove(currentLineBeingDrawn);
  }

  public void DrawLegalBlocks(GameObject go)
  {
    int[] coord = go.GetComponentInParent<GridBehaviour>().coordinates;
    List<List<GameObject>> grid = GameManager.Instance.grid;
    gridMan.ResetLegalBlocks();

    if (coord[0] + 1 < gridMan.gridSizeX)
    {
      GameObject gridObj = grid[coord[0] + 1][coord[1]];
      if (CheckGridForLegalBlock(gridObj))
        gridMan.legalBlockFeedback[0].transform.position = gridObj.transform.position;
    }
    if (coord[0] - 1 >= 0)
    {
      GameObject gridObj = grid[coord[0] - 1][coord[1]];
      if (CheckGridForLegalBlock(gridObj))
        gridMan.legalBlockFeedback[1].transform.position = gridObj.transform.position;
    }
    if (coord[1] + 1 < gridMan.gridSizeY)
    {
      GameObject gridObj = grid[coord[0]][coord[1] + 1];
      if (CheckGridForLegalBlock(gridObj))
        gridMan.legalBlockFeedback[2].transform.position = gridObj.transform.position;
    }
    if (coord[1] - 1 >= 0)
    {
      GameObject gridObj = grid[coord[0]][coord[1] - 1];
      if (CheckGridForLegalBlock(gridObj))
        gridMan.legalBlockFeedback[3].transform.position = gridObj.transform.position;
    }
  }

  public void DrawLegalBlockFromCoupler(LineCoupler lc)
  {
    GameObject gridObj = null;
    int[] coord = lc.GetComponentInParent<GridBehaviour>().coordinates;
    List<List<GameObject>> grid = GameManager.Instance.grid;
    gridMan.ResetLegalBlocks();

    switch (lc.dir)
    {
      case DIRECTION.LEFT:
        gridObj = grid[coord[0] - 1][coord[1]];
        break;
      case DIRECTION.RIGHT:
        gridObj = grid[coord[0] + 1][coord[1]];
        break;
      case DIRECTION.TOP:
        gridObj = grid[coord[0]][coord[1] + 1];
        break;
      case DIRECTION.BOTTOM:
        gridObj = grid[coord[0]][coord[1] - 1];
        break;
    }

    if (CheckGridForLegalBlock(gridObj))
    {
      gridMan.legalBlockFeedback[0].transform.position = gridObj.transform.position;
    }
  }

  bool CheckGridForLegalBlock(GameObject gridObj)
  {
    if (gridObj == null) return false;
    LineController lineC = gridObj.GetComponent<LineController>();
    GridBehaviour gridB = gridObj.GetComponent<GridBehaviour>();
    return lineC.currentLine == null && gridB.currentBlockType.blockType == BLOCK.EMPTY;

  }

}
