using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PuzzleData
{
  public PuzzleData()
  {
    startBlocks = new List<NodeBlock>();
    endBlocks = new List<NodeBlock>();
  }

  public int best = 0;
  public int[] gridSize;
  public List<NodeBlock> startBlocks;
  public List<NodeBlock> endBlocks;
}

public static class LevelData
{
  static public List<PuzzleData> puzzleData =
    new List<PuzzleData>(new PuzzleData[] {
      new PuzzleData  // lines - 0
      {
        gridSize = new int[2] { 3, 1 },
        startBlocks = new List<NodeBlock> ( new NodeBlock[]
        { new NodeBlock { pos = new Vector2(0.0f, 0.0f), value = 1 } } ),
        endBlocks = new List<NodeBlock> ( new NodeBlock[]
        { new NodeBlock { pos = new Vector2(2.0f, 0.0f), value = 1 } })
      },
      new PuzzleData  // two lines - 1
      {
        gridSize = new int[2] { 3, 3 },
        startBlocks = new List<NodeBlock> ( new NodeBlock[]
        {
          new NodeBlock { pos = new Vector2(0.0f, 0.0f), value = 1 },
          new NodeBlock { pos = new Vector2(0.0f, 2.0f), value = 2 }
        }),
        endBlocks = new List<NodeBlock> ( new NodeBlock[]
        {
          new NodeBlock { pos = new Vector2(2.0f, 0.0f), value = 1 },
          new NodeBlock { pos = new Vector2(2.0f, 2.0f), value = 2 }
        })
      },
      new PuzzleData  // add - 2
      {
        gridSize = new int[2] { 3, 3 },
        startBlocks = new List<NodeBlock> ( new NodeBlock[]
        {
          new NodeBlock { pos = new Vector2(0.0f, 0.0f), value = 1 },
          new NodeBlock { pos = new Vector2(0.0f, 2.0f), value = 1 }
        }),
        endBlocks = new List<NodeBlock> ( new NodeBlock[]
        {
          new NodeBlock { pos = new Vector2(2.0f, 1.0f), value = 2 }
        })
      },
      new PuzzleData  // add3 - 3
      {
        gridSize = new int[2] { 3, 3 },
        startBlocks = new List<NodeBlock> ( new NodeBlock[]
        {
          new NodeBlock { pos = new Vector2(0.0f, 0.0f), value = 1 },
          new NodeBlock { pos = new Vector2(0.0f, 1.0f), value = 2},
          new NodeBlock { pos = new Vector2(0.0f, 2.0f), value = 1 },
        }),
        endBlocks = new List<NodeBlock> ( new NodeBlock[]
        {
          new NodeBlock { pos = new Vector2(2.0f, 1.0f), value = 4 }
        })
      },
      new PuzzleData  // copy - 4
      {
        gridSize = new int[2] { 3, 3 },
        startBlocks = new List<NodeBlock> ( new NodeBlock[]
        {
          new NodeBlock { pos = new Vector2(0.0f, 1.0f), value = 1 }
        }),
        endBlocks = new List<NodeBlock> ( new NodeBlock[]
        {
          new NodeBlock { pos = new Vector2(2.0f, 0.0f), value = 1 },
          new NodeBlock { pos = new Vector2(2.0f, 2.0f), value = 1 }
        })
      },
      new PuzzleData  // copyadd - 5
      {
        gridSize = new int[2] { 4, 3 },
        startBlocks = new List<NodeBlock> ( new NodeBlock[]
        {
          new NodeBlock { pos = new Vector2(0.0f, 1.0f), value = 1 }
        }),
        endBlocks = new List<NodeBlock> ( new NodeBlock[]
        {
          new NodeBlock { pos = new Vector2(3.0f, 1.0f), value = 3 }
        })
      },
      new PuzzleData  // split - 6
      {
        gridSize = new int[2] { 3, 3 },
        startBlocks = new List<NodeBlock> ( new NodeBlock[]
        {
          new NodeBlock { pos = new Vector2(0.0f, 1.0f), value = 2 },
        }),
        endBlocks = new List<NodeBlock> ( new NodeBlock[]
        {
          new NodeBlock { pos = new Vector2(2.0f, 0.0f), value = 1 },
          new NodeBlock { pos = new Vector2(2.0f, 2.0f), value = 1 }
        })
      },
      new PuzzleData  // split3 - 7
      {
        gridSize = new int[2] { 4, 3 },
        startBlocks = new List<NodeBlock> ( new NodeBlock[]
        {
          new NodeBlock { pos = new Vector2(0.0f, 1.0f), value = 7 },
        }),
        endBlocks = new List<NodeBlock> ( new NodeBlock[]
        {
          new NodeBlock { pos = new Vector2(3.0f, 0.0f), value = 2 },
          new NodeBlock { pos = new Vector2(3.0f, 1.0f), value = 2 },
          new NodeBlock { pos = new Vector2(3.0f, 2.0f), value = 2 },
        })
      },
      new PuzzleData  // splitcopy - 8
      {
        gridSize = new int[2] { 4, 3 },
        startBlocks = new List<NodeBlock> ( new NodeBlock[]
        {
          new NodeBlock { pos = new Vector2(0.0f, 1.0f), value = 4 }
        }),
        endBlocks = new List<NodeBlock> ( new NodeBlock[]
        {
          new NodeBlock { pos = new Vector2(3.0f, 0.0f), value = 2 },
          new NodeBlock { pos = new Vector2(3.0f, 1.0f), value = 2 },
          new NodeBlock { pos = new Vector2(3.0f, 2.0f), value = 2 },
        })
      },
      new PuzzleData  // onedown - 9
      {
        best = 2,
        gridSize = new int[2] { 5, 5 },
        startBlocks = new List<NodeBlock> ( new NodeBlock[]
        {
          new NodeBlock { pos = new Vector2(0.0f, 2.0f), value = 10 },
        }),
        endBlocks = new List<NodeBlock> ( new NodeBlock[]
        {
          new NodeBlock { pos = new Vector2(4.0f, 2.0f), value = 9 },
        })
      },
      new PuzzleData  // use3 - 10
      {
        best = 3,
        gridSize = new int[2] { 5, 5 },
        startBlocks = new List<NodeBlock> ( new NodeBlock[]
        {
          new NodeBlock { pos = new Vector2(0.0f, 2.0f), value = 4 },
        }),
        endBlocks = new List<NodeBlock> ( new NodeBlock[]
        {
          new NodeBlock { pos = new Vector2(4.0f, 2.0f), value = 9 },
        })
      },
      new PuzzleData  // divide-copy-add - 11
      {
        best = 3,
        gridSize = new int[2] { 5, 5 },
        startBlocks = new List<NodeBlock> ( new NodeBlock[]
        {
          new NodeBlock { pos = new Vector2(0.0f, 2.0f), value = 12 },
        }),
        endBlocks = new List<NodeBlock> ( new NodeBlock[]
        {
          new NodeBlock { pos = new Vector2(4.0f, 1.0f), value = 8 },
          new NodeBlock { pos = new Vector2(4.0f, 3.0f), value = 8 },
        })
      },
      new PuzzleData  // crossover - 12
      {
        best = 3,
        gridSize = new int[2] { 5, 5 },
        startBlocks = new List<NodeBlock> ( new NodeBlock[]
        {
          new NodeBlock { pos = new Vector2(0.0f, 1.0f), value = 4 },
          new NodeBlock { pos = new Vector2(0.0f, 3.0f), value = 5 },
        }),
        endBlocks = new List<NodeBlock> ( new NodeBlock[]
        {
          new NodeBlock { pos = new Vector2(4.0f, 1.0f), value = 5 },
          new NodeBlock { pos = new Vector2(4.0f, 3.0f), value = 4 },
        })
      },
      new PuzzleData  // onewall - 13
      {
        best = 3,
        gridSize = new int[2] { 6, 5 },
        startBlocks = new List<NodeBlock> ( new NodeBlock[]
        {
          new NodeBlock { pos = new Vector2(1.0f, 2.0f), value = 5 },
          new NodeBlock { pos = new Vector2(4.0f, 1.0f), value = 1 },
          new NodeBlock { pos = new Vector2(4.0f, 2.0f), value = 1 },
          new NodeBlock { pos = new Vector2(4.0f, 3.0f), value = 1 },
        }),
        endBlocks = new List<NodeBlock> ( new NodeBlock[]
        {
          new NodeBlock { pos = new Vector2(5.0f, 1.0f), value = 4 },
          new NodeBlock { pos = new Vector2(5.0f, 3.0f), value = 4 },
        })
      },
      new PuzzleData  // bigSplitAdd - 14
      {
        best = 4,
        gridSize = new int[2] { 6, 5 },
        startBlocks = new List<NodeBlock> ( new NodeBlock[]
        {
          new NodeBlock { pos = new Vector2(0.0f, 1.0f), value = 4 },
          new NodeBlock { pos = new Vector2(0.0f, 3.0f), value = 9 },
        }),
        endBlocks = new List<NodeBlock> ( new NodeBlock[]
        {
          new NodeBlock { pos = new Vector2(3.0f, 2.0f), value = 8 },
          new NodeBlock { pos = new Vector2(5.0f, 2.0f), value = 5 },
        })
      },
    });



};
