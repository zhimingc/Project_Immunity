using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public enum BLOCK
{
  EMPTY,
  BASE,
  COPY,
  MINUS,
  MULTI,
  ADD,
  SPLIT,
  NUM_USER_BLOCKS,
  END,
  START
};

public delegate Block BlockCreate();

public class EndBlock : Block
{
  public EndBlock()
  {
    // Teal
    lineCol = new Color(0.0f, 1.0f, 1.0f, 0.5f);
    blockType = BLOCK.END;
    maxIn = 1;
    maxOut = 0;
  }

  override public void UpdateGridValue(GridBehaviour obj)
  {
    // Look for input lines
    foreach (GameObject go in obj.couplers)
    {
      LineCoupler lc = go.GetComponent<LineCoupler>();
      if (!lc.inUse || !lc.isInput) continue;
      obj.data = new GridData(lc.currentLine.value);
      obj.GetComponent<EndBlockScript>().UpdateEndStatus(obj.data.value);
      return;
    }

    // If no input was found
    obj.GetComponent<EndBlockScript>().SetNoInput();
  }

  new static public Block Create() { return new EndBlock(); }
};

public class StartBlock : Block
{
  public StartBlock()
  {
    // Teal
    lineCol = new Color(0.0f, 1.0f, 1.0f, 0.5f);
    blockType = BLOCK.START;
    maxIn = 0;
    maxOut = 1;
  }

  override public void UpdateGridValue(GridBehaviour obj)
  {
    
  }

  new static public Block Create() { return new StartBlock(); }
};

public class CopyBlock : Block
{
  public CopyBlock()
  {
    lineCol = new Color(1.0f, 215.0f / 255.0f, 0.0f, 0.5f);
    blockSprite = Resources.Load<Sprite>("Sprites/block_copy");
    blockType = BLOCK.COPY;
    maxIn = 1;
    maxOut = 3;
  }

  override public void UpdateGridValue(GridBehaviour obj)
  {
    int curAmt = 0;
    foreach (GameObject go in obj.couplers)
    {
      LineCoupler lc = go.GetComponent<LineCoupler>();
      if (!lc.inUse || !lc.isInput) continue;

      curAmt += lc.currentLine.value;
      break;
    }
    obj.data = new GridData(curAmt);
  }

  new static public Block Create() { return new CopyBlock(); }
};

public class BaseBlock : Block
{
  public BaseBlock()
  {
    // Coral
    lineCol = new Color(1.0f, 0.5f, 80.0f / 255.0f, 0.5f);
    blockSprite = Resources.Load<Sprite>("Sprites/block_addsplit");
    blockType = BLOCK.BASE;
    maxIn = 3;
    maxOut = 3;
  }

  override public void UpdateGridValue(GridBehaviour obj)
  {
    int divideAmt = 0;
    int curAmt = 0;
    foreach (GameObject go in obj.couplers)
    {
      LineCoupler lc = go.GetComponent<LineCoupler>();
      if (!lc.inUse) continue;

      if (!lc.isInput)
        ++divideAmt;
      else if (lc.isInput)
        curAmt += lc.currentLine.value;
    }

    // Check to avoid division by 0
    if (divideAmt > 0) obj.data = new GridData(curAmt / divideAmt);
    else obj.data = new GridData(curAmt);
  }

  new static public Block Create() { return new BaseBlock(); }
};

public class MinusBlock : Block
{
  public MinusBlock()
  {
    // Coral
    lineCol = new Color(1.0f, 0.5f, 80.0f / 255.0f, 0.5f);
    blockSprite = Resources.Load<Sprite>("Sprites/block_minus");
    blockType = BLOCK.MINUS;
    maxIn = 3;
    maxOut = 1;
  }

  override public void UpdateGridValue(GridBehaviour obj)
  {
    int minusFrom = 0;
    int minusAmt = 0;
    foreach (GameObject go in obj.couplers)
    {
      LineCoupler lc = go.GetComponent<LineCoupler>();
      if (!lc.inUse || !lc.isInput) continue;

      if (lc.currentLine.value > minusFrom)
      {
        minusAmt += minusFrom;
        minusFrom = lc.currentLine.value;
      }
      else
      {
        minusAmt += lc.currentLine.value;
      }
    }

    // Final minus amount cant go lower than 0
    int newVal = Mathf.Max(minusFrom - minusAmt, 0);
    obj.data = new GridData(newVal);
  }

  new static public Block Create() { return new MinusBlock(); }
};

public class MultiBlock : Block
{
  public MultiBlock()
  {
    // Coral
    lineCol = new Color(1.0f, 0.5f, 80.0f / 255.0f, 0.5f);
    blockSprite = Resources.Load<Sprite>("Sprites/block_multi");
    blockType = BLOCK.MULTI;
    maxIn = 3;
    maxOut = 1;
  }

  override public void UpdateGridValue(GridBehaviour obj)
  {
    bool anyInput = false;
    int finalVal = 1;
    foreach (GameObject go in obj.couplers)
    {
      LineCoupler lc = go.GetComponent<LineCoupler>();
      if (!lc.inUse || !lc.isInput) continue;
      anyInput = true;

      finalVal *= lc.currentLine.value;
    }

    // Set the value to 0 if there are no inputs
    if (anyInput == false) finalVal = 0;
    obj.data = new GridData(finalVal);
  }

  new static public Block Create() { return new MultiBlock(); }
};

public class AddBlock : Block
{
  public AddBlock()
  {
    // Coral
    lineCol = new Color(1.0f, 0.5f, 80.0f / 255.0f, 0.5f);
    blockSprite = Resources.Load<Sprite>("Sprites/block_add");
    blockType = BLOCK.ADD;
    maxIn = 3;
    maxOut = 1;
  }

  override public void UpdateGridValue(GridBehaviour obj)
  {
    int finalVal = 0;
    foreach (GameObject go in obj.couplers)
    {
      LineCoupler lc = go.GetComponent<LineCoupler>();
      if (!lc.inUse || !lc.isInput) continue;

      finalVal += lc.currentLine.value;
    }
    obj.data = new GridData(finalVal);
  }

  new static public Block Create() { return new AddBlock(); }
};

public class SplitBlock : Block
{
  public SplitBlock()
  {
    // Coral
    lineCol = new Color(1.0f, 0.5f, 80.0f / 255.0f, 0.5f);
    blockSprite = Resources.Load<Sprite>("Sprites/block_split");
    blockType = BLOCK.SPLIT;
    maxIn = 1;
    maxOut = 3;
  }

  override public void UpdateGridValue(GridBehaviour obj)
  {
    int divideAmt = 0;
    int curAmt = 0;
    foreach (GameObject go in obj.couplers)
    {
      LineCoupler lc = go.GetComponent<LineCoupler>();
      if (!lc.inUse) continue;

      if (!lc.isInput)
        ++divideAmt;
      else if (lc.isInput)
        curAmt = lc.currentLine.value;
    }

    // Check to avoid division by 0
    if (divideAmt > 0) obj.data = new GridData(curAmt / divideAmt);
    else obj.data = new GridData(curAmt);
  }

  new static public Block Create() { return new SplitBlock(); }
};

public class Block {

  // Behavior
  public BLOCK blockType;
  public int maxIn, maxOut;

  // Display
  public Sprite blockSprite;
  public Color lineCol;

  public Block()
  {
    // Teal
    lineCol = new Color(0.0f, 1.0f, 1.0f, 0.5f);
    blockSprite = Resources.Load<Sprite>("Sprites/grid_block");
    blockType = BLOCK.EMPTY;
  }

  public Block(BLOCK block)
  {
    // Teal; assume these blocks are either end/start
    lineCol = new Color(0.0f, 1.0f, 1.0f, 0.5f);
    blockType = block;
  }

  // Base block will create empty blocks
  static public Block Create() { return new Block(); }

  // Register block map
  static public Dictionary<BLOCK, BlockCreate> blockMap =
    new Dictionary<BLOCK, BlockCreate>
  {
    { BLOCK.END, (BlockCreate) Delegate.CreateDelegate(typeof(BlockCreate), typeof(EndBlock).GetMethod("Create"))},
    { BLOCK.START, (BlockCreate) Delegate.CreateDelegate(typeof(BlockCreate), typeof(StartBlock).GetMethod("Create"))},
    { BLOCK.EMPTY, (BlockCreate) Delegate.CreateDelegate(typeof(BlockCreate), typeof(Block).GetMethod("Create"))},
    { BLOCK.COPY, (BlockCreate) Delegate.CreateDelegate(typeof(BlockCreate), typeof(CopyBlock).GetMethod("Create"))},
    { BLOCK.BASE, (BlockCreate) Delegate.CreateDelegate(typeof(BlockCreate), typeof(BaseBlock).GetMethod("Create"))},
    { BLOCK.MINUS, (BlockCreate) Delegate.CreateDelegate(typeof(BlockCreate), typeof(MinusBlock).GetMethod("Create"))},
    { BLOCK.MULTI, (BlockCreate) Delegate.CreateDelegate(typeof(BlockCreate), typeof(MultiBlock).GetMethod("Create"))},
    { BLOCK.ADD, (BlockCreate) Delegate.CreateDelegate(typeof(BlockCreate), typeof(AddBlock).GetMethod("Create"))},
    { BLOCK.SPLIT, (BlockCreate) Delegate.CreateDelegate(typeof(BlockCreate), typeof(SplitBlock).GetMethod("Create"))},

  };

  virtual public void UpdateGridValue(GridBehaviour obj) { }
  static public Block BlockFactory(BLOCK block)
  {
    if (blockMap.ContainsKey(block)) return blockMap[block].Invoke();
    return null;
  }
}
