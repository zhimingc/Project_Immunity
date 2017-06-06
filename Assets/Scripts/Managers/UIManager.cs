using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

  public Button[] controlBlocks;
  public BLOCK uiBlockClicked;
  public GameObject uiDragging;

  public bool isDragging;

  // levels to show ui
  private int[] toShow;

  // ui objects
  private Text progressText;
  private Text levelCompleteText;
  private Text bestScoreText;

  void Awake()
  {
    // init text objects
    progressText = GameObject.Find("Progress").GetComponent<Text>();
    levelCompleteText = GameObject.Find("Continue").GetComponent<Text>();
    bestScoreText = GameObject.Find("Best").GetComponent<Text>();

    // init which levels to show ui
    toShow = new int[3];
  }

  // Use this for initialization
  void Start () {
    isDragging = false;

    // init which levels to show ui
    toShow[0] = 2; // to show add
    toShow[1] = 4; // to show copy
    toShow[2] = 6; // to show split
  }
	
	// Update is called once per frame
	void Update () {
    UpdatePlayerInputUI();
	}

  void UpdatePlayerInputUI()
  {
    if (isDragging)
    {
      Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      mousePos.z = 0.0f;
      uiDragging.transform.position = mousePos;

      if (Input.GetMouseButtonUp(0))
      {
        isDragging = false;
        uiDragging.GetComponent<SpriteRenderer>().enabled = false;
      }
    }
  }

  void UpdateControlBlockUIDisplay()
  {
    int curLevel = GameManager.Instance.currentLevel;
    GameObject.Find("Toggle_Blocks").GetComponent<Text>().enabled = true;
    GameObject.Find("Quick_Instructs").GetComponent<Text>().enabled = false;
    //GameObject.Find("Drag_Instructs").GetComponent<Text>().enabled = true;

    for (int i = 0; i < controlBlocks.Length; ++i)
    {
      controlBlocks[i].gameObject.SetActive(true);
    }
    //if (curLevel >= 6)
    //{
    //  for (int i = 0; i < 3; ++i)
    //  {
    //    controlBlocks[i].gameObject.SetActive(true);
    //  }
    //}
    //else if (curLevel >= 4)
    //{
    //  for (int i = 0; i < 2; ++i)
    //  {
    //    controlBlocks[i].gameObject.SetActive(true);
    //  }
    //}
    //else if (curLevel >= 2)
    //{
    //  controlBlocks[0].gameObject.SetActive(true);
    //}
    //else
    if (curLevel >= 9)
    {
      GameObject.Find("Drag_Instructs").GetComponent<Text>().enabled = false;
    }
    else if (curLevel < 2)
    {
      GameObject.Find("Drag_Instructs").GetComponent<Text>().enabled = false;

      if (curLevel == 1)
      {
        GameObject.Find("Quick_Instructs").GetComponent<Text>().text = "Get the right value to the white blocks";
      }
      else if (curLevel == 0)
      {
        GameObject.Find("Quick_Instructs").GetComponent<Text>().text = "Left click to draw lines";
      }

      for (int i = 0; i < controlBlocks.Length; ++i)
        controlBlocks[i].gameObject.SetActive(false);

      GameObject.Find("Quick_Instructs").GetComponent<Text>().enabled = true;
      GameObject.Find("Toggle_Blocks").GetComponent<Text>().enabled = false;
    }
  }

  public void UpdateLevelUI()
  {
    // Update level UI
    progressText.text = "Progress\n" + (GameManager.Instance.currentLevel + 1).ToString() + "/" + LevelData.puzzleData.Count.ToString();
    int bestScore = LevelData.puzzleData[GameManager.Instance.currentLevel].best;
    if (bestScore == 0) bestScoreText.text = "";
    else bestScoreText.text = "Best: " + bestScore.ToString();

    UpdateControlBlockUIDisplay();
    UpdateLevelCompleteUI();
  }

  public void UpdateLevelCompleteUI()
  {
    // Level completed UI
    if (GameManager.Instance.levelCompleted)
    {
      GameObject.Find("Quick_Instructs").GetComponent<Text>().enabled = false;
      GameObject.Find("Toggle_Blocks").GetComponent<Text>().enabled = false;

      levelCompleteText.enabled = true;
      if (Input.anyKeyDown)
      {
        UpdateControlBlockUIDisplay();
        levelCompleteText.enabled = false;
      }
    }
  }

  public BLOCK GetBlockClicked()
  {
    return uiBlockClicked;
  }

  public void OnControlBlockClick(GameObject btn)
  {
    string btnName = btn.name;
    isDragging = true;
    uiDragging.GetComponent<SpriteRenderer>().enabled = true;
    uiDragging.GetComponent<SpriteRenderer>().sprite = btn.GetComponent<Image>().sprite;

    if (btnName == "Base")
    {
      uiBlockClicked = BLOCK.BASE;
    }
    else if (btnName == "Copy")
    {
      uiBlockClicked = BLOCK.COPY;
    }
    else if (btnName == "Minus")
    {
      uiBlockClicked = BLOCK.MINUS;
    }
    else if (btnName == "Multi")
    {
      uiBlockClicked = BLOCK.MULTI;
    }
  }
}
