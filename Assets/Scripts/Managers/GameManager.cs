using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum USER_STATE
{
  IDLE,
  DRAWING
};

public class GameManager : Singleton<GameManager>
{

  // guarantee this will be always a singleton only - can't use the constructor!
  protected GameManager()
  {
    grid = new List<List<GameObject>>();
  }

  public USER_STATE userState;
  public List<List<GameObject>> grid;
  public List<GameObject> endBlocks;
  public GameObject mouseCursor;
  public int objsIntersect;
  public bool levelCompleted;
  public int currentLevel;

  // For ui controls
  public UIManager uiMan;

  private ScoreManager scoreMan;
  private GridManager gridMan;
  private LineManager lineMan;
  private PuzzleData currentPuzzle;

  public LineManager GetLineMan() { return lineMan; }

  void Start()
  {
    endBlocks = new List<GameObject>();
    mouseCursor = (GameObject)Instantiate(Resources.Load("Prefabs/DrawingIcon"));
    DontDestroyOnLoad(mouseCursor);
    currentLevel = 14;

    uiMan = GameObject.Find("UI").GetComponent<UIManager>();
    lineMan = GameObject.Find("LineManager").GetComponent<LineManager>();
    gridMan = GameObject.Find("GridManager").GetComponent<GridManager>();
    scoreMan = gameObject.AddComponent<ScoreManager>();

    //ChangeUserState(USER_STATE.IDLE);
    // Init. state for mouse feedback
    lineMan.RemoveMousePointFromLine();
    mouseCursor.GetComponent<SpriteRenderer>().enabled = false;

    // Init proc. gen. variables
    ProcGenManager.InitSequentialGen();
    ProcGenManager.CaptureInitPGW();
    currentPuzzle = new PuzzleData();
    currentPuzzle = ProcGenManager.RandomGenQueuedLevel(currentPuzzle);
    gridMan.ConstructLevel(currentPuzzle);

    // Initialize ui
    uiMan.UpdateLevelUI();
  }

  void Update()
  {
    // Testing
    if (Input.GetKey(KeyCode.Z))
    {
      ProcGenManager.IncreaseGridSize(1, 1);
      LoadNextScene();
    }
    if (Input.GetKey(KeyCode.Q) || Input.GetKeyDown(KeyCode.W))
    {
      LoadNextScene();
    }
    // Reset level
    if (Input.GetKeyDown(KeyCode.R))
    {
      //LoadScene(currentLevel);
      ResetProcGenScene();
    }
    // Restart game
    if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
    {
      RestartProcGenScene();
    }

    if (Input.GetKeyDown(KeyCode.Escape))
    {
      Application.Quit();
    }

    StateUpdate();

    // Check for level change
    if (levelCompleted)
    {
      if (Input.anyKeyDown) LoadNextScene();
    }
  }

  public void UpdateObjsIntersect(int amt)
  {
    objsIntersect += amt;
  }

  void StateUpdate()
  {
    switch (userState)
    {
      case USER_STATE.IDLE:
        break;
      case USER_STATE.DRAWING:
        break;
    }
  }

  public void ResetGrid()
  {
    foreach (List<GameObject> col in grid)
    {
      foreach (GameObject go in col) Destroy(go);
      col.Clear();
    }

    grid.Clear();
  }

  public void ChangeUserState(USER_STATE state)
  {
    userState = state;

    switch (userState)
    {
      case USER_STATE.IDLE:
        lineMan.RemoveMousePointFromLine();
        mouseCursor.GetComponent<SpriteRenderer>().enabled = false;
        CheckLevelComplete();
        break;
      case USER_STATE.DRAWING:
        lineMan.AddMousePointToLine();
        mouseCursor.GetComponent<SpriteRenderer>().enabled = true;

        //mouseCursor.SetActive(true);
        break;
    }
  }

  public void CheckLevelComplete()
  {
    // Return if any of the end blocks are incomplete
    foreach (GameObject obj in endBlocks)
    {
      EndBlockScript eb = obj.GetComponent<EndBlockScript>();
      if (eb.endStatus != 1) return;
    }

    levelCompleted = true;
    uiMan.UpdateLevelUI();

    // Trigger start blocks which have been used
    foreach (GameObject obj in endBlocks)
    {
      EndBlockScript eb = obj.GetComponent<EndBlockScript>();
      eb.BeginTraceToStartBlock();

      // Update score
      scoreMan.AddScore(1);
    }
  }

  public void RestartProcGenScene()
  {
    ProcGenManager.LoadInitPGW();
    currentPuzzle = new PuzzleData();
    currentPuzzle = ProcGenManager.RandomGenQueuedLevel(currentPuzzle);
    scoreMan.ResetScoreMan();

    ResetProcGenScene();
  }

  public void ResetProcGenScene()
  {
    lineMan.ResetLines();
    lineMan.currentLineBeingDrawn = null;
    gridMan.ConstructLevel(currentPuzzle);
    uiMan.UpdateLevelUI();

    // Reset number of objects intersection
    objsIntersect = 0;
    levelCompleted = false;
  }

  public void LoadScene(int level)
  {
    //currentLevel = level;
    currentLevel = 14;
    lineMan.ResetLines();
    lineMan.currentLineBeingDrawn = null;

    // Hand crafted levels
    //gridMan.ConstructLevel(LevelData.puzzleData[currentLevel]);

    // Full random level gen
    //PuzzleData puzzle = ProcGenManager.RandomGenLevel();

    // Proc. queued level gen
    // Remove used start blocks here
    for (int i = 0; i < currentPuzzle.startBlocks.Count; ++i)
    {
      NodeBlock startB = currentPuzzle.startBlocks[i];
      StartBlockScript sbs = grid[(int)startB.pos.x][(int)startB.pos.y].GetComponent<StartBlockScript>();

      if (sbs.isInUse)
      {
        currentPuzzle.startBlocks.Remove(startB);
        --i;
      }
    }
    currentPuzzle = ProcGenManager.RandomGenQueuedLevel(currentPuzzle);

    gridMan.ConstructLevel(currentPuzzle);
    uiMan.UpdateLevelUI();

    // Reset number of objects intersection
    objsIntersect = 0;
    levelCompleted = false;
  }

  public void LoadNextScene()
  {
    currentLevel = (currentLevel + 1) % LevelData.puzzleData.Count;
    LoadScene(currentLevel);
  }

  public void SetEndBlocks(List<GameObject> ebs)
  {
    endBlocks = new List<GameObject>(ebs);
  }

}


