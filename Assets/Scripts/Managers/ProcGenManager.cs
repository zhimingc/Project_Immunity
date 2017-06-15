using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class ProcGenWrapper
{
  public int[] staticGridSize = new int[] { 4, 4 };

  public int[] startValueRange = new int[] { 1, 4 };
  public int[] startSpawnNum = new int[] { 1, 3 };
  // Start from max; spawn when total < pacer
  public List<int> startSpawnPacer = new List<int>(new int[] { 6 });

  public int[] endValueRange = new int[] { 1, 3 };
  public int[] endSpawnNum = new int[] { 1, 3 };
  // Start from min; spawn when total > pacer
  public List<int> endSpawnPacer = new List<int>(new int[] { 8 });

  // Use flip flop to alternate between odd and even numbers
  public bool flipFlopStart = false;
  public bool flipFlopEnd = false;
};

public static class ProcGenManager {

  static public List<int> startQueue, endQueue;
  static int startQSize = 3;
  static int endQSize = 3;

  static ProcGenWrapper pgw = new ProcGenWrapper
  {
    staticGridSize = new int[] { 4, 4 },
    startValueRange = new int[] { 1, 4 },
    startSpawnNum = new int[] { 1, 3 },
    // Start from max; spawn when total < pacer
    startSpawnPacer = new List<int>(new int[] { 6 }),

    endValueRange = new int[] { 1, 3 },
    endSpawnNum = new int[] { 1, 3 },
    // Start from min; spawn when total > pacer
    endSpawnPacer = new List<int>(new int[] { 8 }),

    // Use flip flop to alternate between odd and even numbers
    flipFlopStart = false,
    flipFlopEnd = false,
  };

  static ProcGenWrapper initPGW;

  public static void CaptureInitPGW()
  {
    initPGW = pgw;
  }

  public static void LoadInitPGW()
  {
    pgw = initPGW;
  }

  public static void IncreaseStartSpawnNum(int amtMin, int amtMax)
  {
    pgw.startSpawnNum[0] += amtMin;
    pgw.startSpawnNum[1] += amtMax;

    // Update pacer
    if (amtMax != 0) pgw.startSpawnPacer.Add((int)(pgw.startValueRange[1] * 2.5f));
  }

  public static void IncreaseEndSpawnNum(int amtMin, int amtMax)
  {
    pgw.endSpawnNum[0] += amtMin;
    pgw.endSpawnNum[1] += amtMax;

    // Update pacer
    if (amtMax != 0) pgw.endSpawnPacer.Add(pgw.startValueRange[1] * 3);
  }

  public static void IncreaseGridSize(int amtX, int amtY)
  {
    pgw.staticGridSize[0] += amtX;
    pgw.staticGridSize[1] += amtY;

    // Try find equation describing camera size related to grid size
    Camera.main.orthographicSize += Mathf.Max(amtX, amtY) * 2;
  }

  public static void IncreaseStartRange(int amtMin, int amtMax)
  {
    pgw.startValueRange[0] += amtMin;
    pgw.startValueRange[1] += amtMax;
  }

  public static void IncreaseEndRange(int amtMin, int amtMax)
  {
    pgw.endValueRange[0] += amtMin;
    pgw.endValueRange[1] += amtMax;
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

  public static void InitSequentialGen()
  {
    startQueue = new List<int>();
    endQueue = new List<int>();

    List<int> odd = new List<int>();
    List<int> even = new List<int>();
    SortBetweenOddAndEven(pgw.startValueRange, ref odd, ref even);
    for (int i = 0; i < startQSize; ++i)
    {
      //startQueue.Add(Random.Range(startRange[0], startRange[1]));
      int numToAdd = 0;
      if (pgw.flipFlopStart) numToAdd = odd[Random.Range(0, odd.Count)];
      else numToAdd = even[Random.Range(0, even.Count)];

      startQueue.Add(numToAdd);
      pgw.flipFlopStart = !pgw.flipFlopStart;
    }

    SortBetweenOddAndEven(pgw.endValueRange, ref odd, ref even);
    for (int i = 0; i < endQSize; ++i)
    {
      //endQueue.Add(Random.Range(endRange[0], endRange[1]));
      int numToAdd = 0;
      if (pgw.flipFlopEnd) numToAdd = odd[Random.Range(0, odd.Count)];
      else numToAdd = even[Random.Range(0, even.Count)];

      endQueue.Add(numToAdd);
      pgw.flipFlopEnd = !pgw.flipFlopEnd;
    }
  }

  static void TopUpSeqQueue()
  {
    List<int> odd = new List<int>();
    List<int> even = new List<int>();
    SortBetweenOddAndEven(pgw.startValueRange, ref odd, ref even);
    for (int i = startQueue.Count; i < startQSize; ++i)
    {
      //startQueue.Add(Random.Range(startRange[0], startRange[1]));
      int numToAdd = 0;
      if (pgw.flipFlopStart) numToAdd = odd[Random.Range(0, odd.Count)];
      else numToAdd = even[Random.Range(0, even.Count)];

      startQueue.Add(numToAdd);
      pgw.flipFlopStart = !pgw.flipFlopStart;
    }

    SortBetweenOddAndEven(pgw.endValueRange, ref odd, ref even);
    for (int i = endQueue.Count; i < endQSize; ++i)
    {
      //endQueue.Add(Random.Range(endRange[0], endRange[1]));
      int numToAdd = 0;
      if (pgw.flipFlopEnd) numToAdd = odd[Random.Range(0, odd.Count)];
      else numToAdd = even[Random.Range(0, even.Count)];

      endQueue.Add(numToAdd);
      pgw.flipFlopEnd = !pgw.flipFlopEnd;
    }
  }

  public static PuzzleData RandomGenQueuedLevel(PuzzleData curPuzzle)
  {
    // Randomize grid size
    //int[] gridSize = new int[2] { Random.Range(3, 6), Random.Range(3, 5) };
    int[] gridSize = pgw.staticGridSize;

    curPuzzle.gridSize = gridSize;

    // Get total start block value
    int totalStartValue = TotalBlocksValue(curPuzzle.startBlocks);

    // Pace the number of end blocks spawning depending on total start val
    int startSpawnMax = 0;
    for (int i = pgw.startSpawnNum[1] - 1, c = 0; i >= pgw.startSpawnNum[0]; --i, ++c)
    {
      if (pgw.startSpawnPacer.Count <= c)
      {
        startSpawnMax = pgw.startSpawnNum[0];
        break;
      }

      startSpawnMax = i;
      if (totalStartValue < pgw.startSpawnPacer[c])
      {
        break;
      }
    }
    // Number of start spawn depend on total start block value
    int numStart = startSpawnMax;
    int numExist = curPuzzle.startBlocks.Count;
    //int numStart = Random.Range(startSpawnNum[0], startSpawnNum[1]);

    Vector2 startXRange = new Vector2(0, (gridSize[0] / 2) + gridSize[0] % 2);
    RandomGenPositions(startXRange, curPuzzle, ref curPuzzle.startBlocks, numStart);
    // Initialize values here depending on how many blocks there are
    for (int i = numExist; i < curPuzzle.startBlocks.Count; ++i)
    {
      curPuzzle.startBlocks[i].value = startQueue[0];
      startQueue.RemoveAt(0);
    }

    // Check total value of start blocks
    totalStartValue = TotalBlocksValue(curPuzzle.startBlocks);

    // Pace the number of end blocks spawning depending on total start val
    int endSpawnMax = 0;
    for (int i = pgw.endSpawnNum[0], c = 0; i < pgw.endSpawnNum[1]; ++i, ++c)
    {
      if (pgw.endSpawnPacer.Count <= c)
      {
        endSpawnMax = pgw.endSpawnNum[1] - 1;
        break;
      }

      endSpawnMax = i;
      if (totalStartValue < pgw.endSpawnPacer[c])
      {
        break;
      }
    }
    //int numEnd = Random.Range(endSpawnNum[0], endSpawnMax);
    int numEnd = endSpawnMax;

    // End blocks always reset
    curPuzzle.endBlocks = new List<NodeBlock>();
    Vector2 endXRange = new Vector2(gridSize[0] - gridSize[0] / 2, gridSize[0]);
    RandomGenPositions(endXRange, curPuzzle, ref curPuzzle.endBlocks, numEnd);
    for (int i = 0; i < numEnd; ++i)
    {
      curPuzzle.endBlocks[i].value = endQueue[0];
      endQueue.RemoveAt(0);
    }

    // Top up proc. gen. variables
    TopUpSeqQueue();

    return curPuzzle;
  }

  static void RandomGenPositions(Vector2 xRange, PuzzleData puzzle, ref List<NodeBlock> currentBlocks, int num)
  {
    int cap = 1000;
    while (num > 0 && --cap > 0)
    {
      Vector2 randPos = new Vector2(Random.Range(Mathf.RoundToInt(xRange.x), Mathf.RoundToInt(xRange.y)), Random.Range(0, puzzle.gridSize[1]));
      bool skip = false;
      foreach (NodeBlock block in currentBlocks)
      {
        if ((block.pos - randPos).magnitude < 1.0f)
          skip = true;
      }
      if (skip) continue;

      NodeBlock newBlock = new NodeBlock();
      newBlock.pos = randPos;
      currentBlocks.Add(newBlock);
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
    RandomGenPositions(xRange, puzzle, ref objs, num);
    RandomGenValues(valueRange, ref objs);
  }

  static void SortBetweenOddAndEven(int[] range, ref List<int> odd, ref List<int> even)
  {
    odd = new List<int>();
    even = new List<int>();

    for (int i = range[0]; i <= range[1]; ++i)
    {
      if (i % 2 == 0) even.Add(i);
      else odd.Add(i);
    }
  }

  static int TotalBlocksValue(List<NodeBlock> blocks)
  {
    int totalValue = 0;
    foreach (NodeBlock node in blocks)
    {
      totalValue += node.value;
    }

    return totalValue;
  }
}
