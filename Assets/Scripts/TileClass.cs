using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "newtileclass", menuName = "Tile Class")]
public class TileClass : ScriptableObject
{
  public string tileName;
  public TileClass wallVariant;
  public Sprite[] tileSprites;
  public bool inFront = true;
  public Sprite[] tileDrop;
  public bool DropUse;
  public bool StackUse;
  public bool OreDrop;
  public bool toBreak;
  public ItemClass.ToolType tool_tobreak;

  public static TileClass CreateInstance(TileClass tile)
  {
    var thisTile = ScriptableObject.CreateInstance<TileClass>();
    thisTile.Init(tile);
    return thisTile;
  }

  public void Init (TileClass tile)
  {
    tileName = tile.tileName;
    wallVariant = tile.wallVariant;
    tileSprites = tile.tileSprites;
    DropUse = tile.DropUse;
    OreDrop = tile.OreDrop;
    if(tile.DropUse)
      tileDrop = tile.tileDrop;
    toBreak = tile.toBreak;
    inFront = tile.inFront;
    StackUse = tile.StackUse;
    tool_tobreak = tile.tool_tobreak;
  }
}
