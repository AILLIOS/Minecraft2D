using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BiomeClass
{
  public string biomeName;
  public Color biomeCol;
  public TileAtlas tileAtlas;

  [Header("Generation Sets")]
  public int dirtLayerHeight = 5;
  public float heightMultiplier = 4f;

  [Header("Trees")]
  public int treeChance = 10;
  public int minTreeHeight = 4;
  public int maxTreeHeight = 6;

  [Header("Tall Grass")]
  public int tallGrassChance = 10;

  [Header("Ore Sets")]
  public OreClass[] ores;
}
