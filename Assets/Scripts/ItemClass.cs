using System.Collections;
using UnityEngine;

[System.Serializable]
public class ItemClass
{
  public enum ItemType
  {
    tile,
    tool
  };

  public enum ToolType
  {
    axe,
    pickaxe,
    showel,
    sword,
    ore,
    arm
  };

  public ItemType itemType;
  public ToolType toolType;

  public TileClass tile;
  public ToolClass tool;

  public string name;
  public Sprite Sprite;
  public Sprite[] tileDrop;
  public bool StackUse;
  public bool DropUse;
  public bool OreDrop;
  public bool toBreak;

  public ItemClass (TileClass Tile)
  {
    name = Tile.tileName;
    Sprite = Tile.tileSprites[0];
    StackUse = Tile.StackUse;
    DropUse = Tile.DropUse;
    OreDrop = Tile.OreDrop;
    tileDrop = Tile.tileDrop;
    toBreak = Tile.toBreak;
    itemType = ItemType.tile;
    tile = Tile;
  }

  public ItemClass (ToolClass Tool)
  {
    name = Tool.name;
    Sprite = Tool.sprite[0];
    StackUse = Tool.StackUse;
    tileDrop = Tool.sprite;
    toBreak = Tool.toBreak;
    itemType = ItemType.tool;
    toolType = Tool.toolType;
    tool = Tool;
  }
}
