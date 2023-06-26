using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileAtlas", menuName = "Tile Atlas")]
public class TileAtlas : ScriptableObject
{
  [Header("Environment")]
  public TileClass grass;
  public TileClass dirt;
  public TileClass stone;
  public TileClass log;
  public TileClass leaf;
  public TileClass cactus;
  public TileClass tall_grass;
  public TileClass snow;
  public TileClass sand;
  public TileClass bedrock;

  [Header("Ores")]
  public TileClass coal;
  public TileClass iron;
  public TileClass gold;
  public TileClass diamond;

  [Header("Background")]
  public TileClass stone_wall;
  public TileClass dirt_wall;
}
