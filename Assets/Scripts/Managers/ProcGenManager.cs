using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProcGenManager {

  static public List<int> startQueue, endQueue;
  static int startQSize = 5;
  static int endQSize = 5;

  public static void InitSequentialGen()
  {
    startQueue = new List<int>();
    endQueue = new List<int>();

    for (int i = 0; i < startQSize; ++i)
    {
      startQueue.Add(Random.Range(1, 10));
    }

    for (int i = 0; i < endQSize; ++i)
    {
      endQueue.Add(Random.Range(1, 15));
    }
  }

  static void TopUpSeqQueue()
  {
    for (int i = startQueue.Count; i < startQSize; ++i)
    {
      startQueue.Add(Random.Range(1, 10));
    }
    for (int i = endQueue.Count; i < endQSize; ++i)
    {
      endQueue.Add(Random.Range(1, 15));
    }
  }

  public static PuzzleData RandomGenLevel()
  {
    PuzzleData retPuzzle = new PuzzleData();

    // Randomize grid size
    //int[] gridSize = new int[2] { Random.Range(3, 6), Random.Range(3, 5) };
    int[] gridSize = new int[2] { 5, 4 };
    retPuzzle.gridSize = gridSize;

    // Randomize start and end blocks
    int numStart = Random.Range(1, 3);
    int numEnd = Random.Range(1, 2);
    retPuzzle.startBlocks = new List<NodeBlock>(new NodeBlock[numStart]);
    retPuzzle.endBlocks = new List<NodeBlock>(new NodeBlock[numEnd]);

    Vector2 startXRange = new Vector2(0, (gridSize[0] / 2) + gridSize[0] % 2);
    Vector2 startValues = new Vector2(1, 10);
    RandomGenObjectives(startXRange, startValues, retPuzzle, ref retPuzzle.startBlocks);

    Vector2 endXRange = new Vector2(gridSize[0] - gridSize[0] / 2, gridSize[0]);
    Vector2 endValues = new Vector2(1, 15);
    RandomGenObjectives(endXRange, endValues, retPuzzle, ref retPuzzle.endBlocks);

    return retPuzzle;
  }

  public static PuzzleData RandomGenQueuedLevel(PuzzleData curPuzzle)
  {
    // Randomize grid size
    //int[] gridSize = new int[2] { Random.Range(3, 6), Random.Range(3, 5) };
    int[] gridSize = new int[2] { 5, 4 };
    curPuzzle.gridSize = gridSize;

    // Randomize start and end blocks
    int numStart = Random.Range(1, 3);
    int numEnd = Random.Range(1, 2);
    // Add start blocks into current start blocks
    curPuzzle.startBlocks.AddRange(new NodeBlock[numStart]);
    // End blocks always reset
    curPuzzle.endBlocks = new List<NodeBlock>(new NodeBlock[numEnd]);

    Vector2 startXRange = new Vector2(0, (gridSize[0] / 2) + gridSize[0] % 2);
    int num = curPuzzle.startBlocks.Count;
    for (int i = 0; i < num; ++i)
    {
      curPuzzle.startBlocks[i] = new NodeBlock();
    }
    RandomGenPositions(startXRange, curPuzzle, ref curPuzzle.startBlocks);
    // Initialize values here depending on how many blocks there are
    for (int i = 0; i < num; ++i)
    {
      curPuzzle.startBlocks[i].value = startQueue[0];
      startQueue.RemoveAt(0);
    }

    Vector2 endXRange = new Vector2(gridSize[0] - gridSize[0] / 2, gridSize[0]);
    num = curPuzzle.endBlocks.Count;
    for (int i = 0; i < num; ++i)
    {
      curPuzzle.endBlocks[i] = new NodeBlock();
    }
    RandomGenPositions(endXRange, curPuzzle, ref curPuzzle.endBlocks);
    for (int i = 0; i < num; ++i)
    {
      curPuzzle.endBlocks[i].value = endQueue[0];
      endQueue.RemoveAt(0);
    }

    // Top up proc. gen. variables
    TopUpSeqQueue();

    return curPuzzle;
  }

  static void RandomGenPositions(Vector2 xRange, PuzzleData puzzle, ref List<NodeBlock> objs)
  {
    int num = objs.Count;
    while (num > 0)
    {
      Vector2 randPos = new Vector2(Random.Range(Mathf.RoundToInt(xRange.x), Mathf.RoundToInt(xRange.y)), Random.Range(0, puzzle.gridSize[1]));
      bool skip = false;
      foreach (NodeBlock block in objs)
      {
        if ((block.pos - randPos).magnitude < 1.0f)
          skip = true;
      }
      if (skip) continue;

      objs[num - 1].pos = randPos;
      --num;
    }
  }

  static void RandomGenValues(Vector2 valueRange, ref List<NodeBlock> objs)
  {
    int num = objs.Count;

    while (num > 0)
    {
      objs[num - 1].value = Random.Range((int)valueRange.x, (int)valueRange.y);
      --num;
    }
  }

  public static void RandomGenObjectives(Vector2 xRange, Vector2 valueRange, PuzzleData puzzle, ref List<NodeBlock> objs)
  {
    int num = objs.Count;
    for (int i = 0; i < num; ++i)
    {
      objs[i] = new NodeBlock();
    }

    RandomGenPositions(xRange, puzzle, ref objs);
    RandomGenValues(valueRange, ref objs);
  }
}
