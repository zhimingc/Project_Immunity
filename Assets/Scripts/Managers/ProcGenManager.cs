using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProcGenManager {

  public static PuzzleData RandomGenLevel()
  {
    PuzzleData retPuzzle = new PuzzleData();

    // Randomize grid size
    int[] gridSize = new int[2] { Random.Range(3, 6), Random.Range(3, 5) };
    retPuzzle.gridSize = gridSize;

    // Randomize start and end blocks
    int numStart = Random.Range(1, 3);
    retPuzzle.startBlocks = new NodeBlock[numStart];
    for(int i = 0; i < numStart; ++i)
    {
      retPuzzle.startBlocks[i] = new NodeBlock();
    }
    int numEnd = Random.Range(1, 2);
    retPuzzle.endBlocks = new NodeBlock[numEnd];
    for (int i = 0; i < numEnd; ++i)
    {
      retPuzzle.endBlocks[i] = new NodeBlock();
    }

    while (numStart > 0)
    {
      Vector2 randPos = new Vector2(Random.Range(0, gridSize[0] - gridSize[0] / 2), 
        Random.Range(0, gridSize[1] - 1));
      bool skip = false;
      foreach(NodeBlock block in retPuzzle.startBlocks)
      {
        if ((block.pos - randPos).magnitude < 1.0f)
          skip = true;
      }
      if (skip) continue;

      retPuzzle.startBlocks[numStart - 1].pos = randPos;
      retPuzzle.startBlocks[numStart - 1].value = Random.Range(1, 10);
      --numStart;
    }

    while (numEnd > 0)
    {
      Vector2 randPos = new Vector2(Random.Range((gridSize[0] - gridSize[0] / 2) - 1, gridSize[0]),
        Random.Range(0, gridSize[1] - 1));
      bool skip = false;
      foreach (NodeBlock block in retPuzzle.startBlocks)
      {
        if ((block.pos - randPos).magnitude < 1.0f)
          skip = true;
      }
      if (skip) continue;


      retPuzzle.endBlocks[numEnd - 1].pos = randPos;
      retPuzzle.endBlocks[numEnd - 1].value = Random.Range(1, 15);
      --numEnd;
    }

    return retPuzzle;
  }
}
