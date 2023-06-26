using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrGeneration : MonoBehaviour
{
  [Header("Illumination")]
  public Texture2D worldMap;
  public GameObject background;
  public GameObject overlay;
  public Material lightShader;
  public float lightRadius = 12f;
  public float GroundLight;
  public float CaveLight;
  List<Vector2Int> unlits = new List<Vector2Int>();
  List<Vector3Int> threshlights = new List<Vector3Int>();

  public PlayerControl player;
  public CameraControl camera;
  public GameObject tileDrop;
  public PhysicsMaterial2D GroundPhysic;
  public GameObject GameTile;
  public Inventory Inv;

  [Header("TileAtlas")]
  public float seed;
  public TileAtlas tileAtlas;
  public BiomeClass[] biomes;

  [Header("Biomes")]
  public float biomeFrequency;
  public Gradient biomeGradient;
  public Texture2D biomeMap;

  [Header("Generation Sets")]
  public int chunkSize = 16;
  public int worldSize = 100;
  public bool generateCaves = true;
  public float surfaceValue = 0.25f;
  public int heightAddition = 25;

  [Header("Noise Sets")]
  public float terrainFreq = 0.05f;
  public float caveFreq = 0.05f;
  public Texture2D caveNTexture;

  [Header("Ore Sets")]
  public ToolClass[] Ores;

  private GameObject[] worldChunks;
  private GameObject[,,] worldObjects;
  private TileClass[,] background_world_tiles;
  private TileClass[,] frontground_world_tiles;
  private BiomeClass curBiome;

  private void OnValidate()
  {
    DrawTextures();
  }

  public void Start()
  {
    background.SetActive(true);
    overlay.SetActive(true);
    background_world_tiles = new TileClass[worldSize,worldSize];
    frontground_world_tiles = new TileClass[worldSize,worldSize];
    worldObjects = new GameObject[worldSize,worldSize,2];
    IlluminateInstall();
    seed = Random.Range(-10000, 10000);
    DrawTextures();
    CreateChunks();
    GenerateTerrain();
    IlluminateGenerate();
    camera.SpawnPoint(new Vector3(player.spawnPos.x, player.spawnPos.y, camera.transform.position.z));
    camera.worldSize = worldSize;
    player.SpawnPoint();
    RefreshChunks();
  }

  public void IlluminateInstall()
  {
    worldMap = new Texture2D(worldSize, worldSize);
    lightShader.SetTexture("_ShadowTex", worldMap);
    for(int x = 0; x < worldSize; x++)
    {
      for(int y = 0; y < worldSize; y++)
      {
        worldMap.SetPixel(x,y,Color.white);
      }
    }
    worldMap.Apply();
  }

  public void IlluminateGenerate()
  {
    for(int x = 0; x < worldSize; x++)
    {
      for(int y = 0; y < worldSize; y++)
      {
        if(worldMap.GetPixel(x,y) == Color.white)
        {
          LightTile(x,y,1f);
        }
      }
    }
    worldMap.Apply();
  }

  public void DrawTextures()
  {
    DrawBiomeTexture();
    caveNTexture = new Texture2D(worldSize, worldSize);
    GenerateNTexture(caveFreq, surfaceValue, caveNTexture);
    for (int i = 0; i < biomes.Length; i++)
    {
      for (int o = 0; o < biomes[i].ores.Length; o++)
      {
        biomes[i].ores[o].texture = new Texture2D(worldSize, worldSize);
        GenerateNTexture(biomes[i].ores[o].rarity, biomes[i].ores[o].size, biomes[i].ores[o].texture);
      }
    }
  }

  private void Update()
  {
    RefreshChunks();
  }

  void RefreshChunks()
  {
    for(int i = 0; i < worldChunks.Length; i++)
    {
      if(Vector2.Distance(new Vector2((i * chunkSize) + (chunkSize / 2), 0), new Vector2(player.transform.position.x, 0)) > camera.oSize * 4f)
      {
        worldChunks[i].SetActive(false);
      }
      else
      {
        worldChunks[i].SetActive(true);
      }
    }
  }

  public void DrawBiomeTexture()
  {
    float v;
    Color col;
    biomeMap = new Texture2D(worldSize, worldSize);
    for (int x = 0; x < biomeMap.width; x++)
    {
      for (int y = 0; y < biomeMap.height; y++)
      {
        v = Mathf.PerlinNoise((x + seed) * biomeFrequency, (y + seed) * biomeFrequency);
        col = biomeGradient.Evaluate(v);
        biomeMap.SetPixel(x,y,col);
      }
    }
    biomeMap.Apply();
  }

  public void GenerateNTexture(float frequency, float limit, Texture2D NTexture)
  {
    for(int x = 0; x < NTexture.width; x++)
    {
      for(int y = 0; y < NTexture.height; y++)
      {
        float v = Mathf.PerlinNoise((x + seed) * frequency, (y + seed) * frequency);
        if(v > limit)
          NTexture.SetPixel(x,y, Color.white);
        else
          NTexture.SetPixel(x,y, Color.black);
      }
    }
    NTexture.Apply();
  }

  public void CreateChunks()
  {
    int numChunks = worldSize / chunkSize;
    worldChunks = new GameObject[numChunks];
    for(int i = 0; i < numChunks; i++)
    {
      GameObject newChunk = new GameObject();
      newChunk.name = i.ToString();
      newChunk.transform.parent = this.transform;
      worldChunks[i] = newChunk;
    }
  }

  public BiomeClass GetCurrentBiome(int x, int y)
  {
    for(int i = 0; i < biomes.Length; i++)
    {
      if(biomes[i].biomeCol == biomeMap.GetPixel(x,y))
      {
        return biomes[i];
      }
    }
    return curBiome;
  }

  public void GenerateTerrain()
  {
    TileClass tile;
    for(int x = 0; x < worldSize; x++)
    {
      curBiome = GetCurrentBiome(x,0);
      float height = Mathf.PerlinNoise((x + seed) * terrainFreq, seed * terrainFreq) * curBiome.heightMultiplier + heightAddition;
      for(int y = 0; y < height; y++)
      {
        curBiome = GetCurrentBiome(x,y);
        if(x == worldSize / 2)
        {
          player.spawnPos = new Vector2(x, height + 1);
        }
        if(y < height - curBiome.dirtLayerHeight && y > 0)
        {
          tile = curBiome.tileAtlas.stone;
          if(curBiome.ores[0].texture.GetPixel(x,y).r > 0.5f && height - y > curBiome.ores[0].maxSpawnHeight)
            tile = curBiome.tileAtlas.coal;
          if(curBiome.ores[1].texture.GetPixel(x,y).r > 0.5f && height - y > curBiome.ores[1].maxSpawnHeight)
            tile = curBiome.tileAtlas.iron;
          if(curBiome.ores[2].texture.GetPixel(x,y).r > 0.5f && height - y > curBiome.ores[2].maxSpawnHeight)
            tile = curBiome.tileAtlas.gold;
          if(curBiome.ores[3].texture.GetPixel(x,y).r > 0.5f && height - y > curBiome.ores[3].maxSpawnHeight)
            tile = curBiome.tileAtlas.diamond;
        }
        else if(y < height - 1 && y > 0)
        {
          tile = curBiome.tileAtlas.dirt;
        }
        else if(y == 0)
        {
          tile = tileAtlas.bedrock;
        }
        else
        {
          tile = curBiome.tileAtlas.grass;
        }
        if(caveNTexture.GetPixel(x,y).r > 0.5f)
        {
          PlaceTile(tile,x,y,false,0);
          if(tile.wallVariant != null)
            PlaceTile(tile,x,y,true,0);
          if(y > height - 1)
          {
            int t = Random.Range(0, curBiome.treeChance);
            if(t == 1)
            {
              if(curBiome.tileAtlas.leaf != null)
              {
                GenerateTree(x, y + 1, Random.Range(curBiome.minTreeHeight, curBiome.maxTreeHeight));
              }
            }
            else
            {
              int i = Random.Range(0, curBiome.tallGrassChance);
              if(i == 1)
              {
                if(curBiome.tileAtlas.tall_grass != null && curBiome.biomeName != "Snowland")
                {
                  PlaceTile(curBiome.tileAtlas.tall_grass,x,y + 1,true,0);
                }
              }
              else if (curBiome.tileAtlas.cactus != null)
              {
                int b = Random.Range(0, 8);
                if(b == 1)
                {
                  int v = Random.Range(1, 4);
                  for(int o = 0; o <= v; o++)
                  {
                    PlaceTile(curBiome.tileAtlas.cactus,x,y+o+1,true,0);
                  }
                }
              }
              else if(curBiome.tileAtlas.snow != null)
              {
                int a = Random.Range(0, 100);
                int b = Random.Range(0, 100);
                int c = Random.Range(0, 100);
                if(a <= 60)
                {
                  PlaceTile(curBiome.tileAtlas.snow,x,y+1, true,0);
                  if(b <= 60)
                  {
                    if(background_world_tiles[x,y+1])
                    {
                      PlaceTile(curBiome.tileAtlas.snow,x,y+1, true,0);
                    }
                  }
                }
                if(c <= 25)
                {
                  if(curBiome.tileAtlas.tall_grass != null && curBiome.biomeName == "Snowland")
                  {
                    if(!background_world_tiles[x,y+1])
                    {
                      PlaceTile(curBiome.tileAtlas.tall_grass,x,y+1,true,0);
                    }
                  }
                }
              }
            }
          }
        }
        else
        {
          if(tile.wallVariant != null)
            PlaceTile(tile,x,y,false,0);
          if(tile == tileAtlas.bedrock)
            PlaceTile(tile,x,y,false,0);
        }
      }
    }
    worldMap.Apply();
  }

  void GenerateTree(int x, int y, int treeHeight)
  {
    for(int i = 0; i <= treeHeight; i++)
    {
      if(!background_world_tiles[x,y+i])
        PlaceTile(tileAtlas.log,x,y+i,true,0);
    }
    for(int i = 0; i < 3; i++)
    {
      PlaceTile(tileAtlas.leaf,x,y+treeHeight+i,true,0);
      if(i < 2)
      {
        PlaceTile(tileAtlas.leaf,x-1,y+treeHeight+i,true,0);
        PlaceTile(tileAtlas.leaf,x+1,y+treeHeight+i,true,0);
      }
    }
  }

  public void DestroyTile(TileClass tile, int x, int y, int n)
  {
    if(tile.DropUse)
    {
      if(!tile.OreDrop)
      {
        GameObject newTileDrop = Instantiate(tileDrop, new Vector2(x, y + 0.5f), Quaternion.identity);
        newTileDrop.GetComponent<SpriteRenderer>().sprite = tile.tileDrop[0];
        ItemClass DropItem = new ItemClass(tile);
        newTileDrop.GetComponent<TileDrop>().item = DropItem;
        newTileDrop.GetComponent<TileDrop>().item.tile = tile;
        newTileDrop.GetComponent<TileDrop>().added = true;
      }
      else
      {
        GameObject newTileDrop = Instantiate(tileDrop, new Vector2(x, y + 0.5f), Quaternion.identity);
        newTileDrop.GetComponent<SpriteRenderer>().sprite = tile.tileDrop[0];
        ToolClass tool = Ores[0];
        if(tile.tileName == "coal" || tile.tileName == "coal_wall")
        {
          tool = Ores[0];
        }
        if(tile.tileName == "iron" || tile.tileName == "iron_wall")
        {
          tool = Ores[1];
        }
        if(tile.tileName == "gold" || tile.tileName == "gold_wall")
        {
          tool = Ores[2];
        }
        if(tile.tileName == "diamond" || tile.tileName == "diamond_wall")
        {
          tool = Ores[3];
        }
        ItemClass DropItem = new ItemClass(tool);
        newTileDrop.GetComponent<TileDrop>().item = DropItem;
        newTileDrop.GetComponent<TileDrop>().item.tool = tool;
        newTileDrop.GetComponent<TileDrop>().added = true;
      }
    }
    Destroy(worldObjects[x,y,n]);
  }

  public void RemoveinFrontTile(int x, int y)
  {
    TileClass tile = background_world_tiles[x,y];
    DestroyTile(tile,x,y,0);
  }

  public void RemoveTile(int x, int y, int tile_layer)
  {
    if(x >= 0 && x <= worldSize && y >= 0 && y <= worldSize)
    {
      if(tile_layer == 0)
      {
        if(background_world_tiles[x,y])
        {
          TileClass tile = background_world_tiles[x,y];
          if(!frontground_world_tiles[x,y] && background_world_tiles[x,y])
          {
            tile = background_world_tiles[x,y];
            if(background_world_tiles[x,y].toBreak)
            {
              DestroyTile(tile,x,y,0);
              RemoveTileFromWorld(x,y,tile_layer);
              worldMap.SetPixel(x,y,Color.white);
              LightTile(x,y,1f);
            }
          }
          else if(frontground_world_tiles[x,y] && background_world_tiles[x,y])
          {
            tile = frontground_world_tiles[x,y];
            if(frontground_world_tiles[x,y].toBreak)
            {
              DestroyTile(tile,x,y,1);
              RemoveTileFromWorld(x,y,tile_layer);
              RemoveLightSource(x,y);
            }
          }
        }
      }
      else if(tile_layer == 1)
      {
        if(frontground_world_tiles[x,y])
        {
          TileClass tile = frontground_world_tiles[x,y];
          if(frontground_world_tiles[x,y].toBreak)
          {
            DestroyTile(tile,x,y,1);
            RemoveTileFromWorld(x,y,tile_layer);
            if(background_world_tiles[x,y])
            {
              RemoveLightSource(x,y);
            }
            else if(!background_world_tiles[x,y])
            {
              worldMap.SetPixel(x,y,Color.white);
              LightTile(x,y,1f);
            }
          }
        }
      }
      else if(tile_layer == 2)
      {
        if(background_world_tiles[x,y])
        {
          if(background_world_tiles[x,y].toBreak)
          {
            TileClass tile = background_world_tiles[x,y];
            DestroyTile(tile,x,y,0);
            RemoveTileFromWorld(x,y,tile_layer);
            worldMap.SetPixel(x,y,Color.white);
            LightTile(x,y,1f);
          }
        }
      }
    }
    worldMap.Apply();
  }

  public void BreakTile(int x, int y, int tile_layer, ItemClass item)
  {
    if(x >= 0 && x <= worldSize && y >= 0 && y <= worldSize)
    {
      if(background_world_tiles[x,y])
      {
        if(tile_layer == 0)
        {
          TileClass tile = background_world_tiles[x,y];
          if(frontground_world_tiles[x,y])
          {
            tile = frontground_world_tiles[x,y];
          }
          if(tile.tool_tobreak == item.toolType)
            RemoveTile(x,y,tile_layer);
        }
        if(tile_layer == 1)
        {
          if(frontground_world_tiles[x,y])
          {
            TileClass tile = frontground_world_tiles[x,y];
            if(tile.tool_tobreak == item.toolType)
              RemoveTile(x,y,tile_layer);
          }
        }
        if(tile_layer == 2)
        {
          if(background_world_tiles[x,y])
          {
            TileClass tile = background_world_tiles[x,y];
            if(tile.tool_tobreak == item.toolType)
              RemoveTile(x,y,tile_layer);
          }
        }
      }
    }
  }

  public void CheckTile(TileClass tile, int x, int y, int tile_layer, int invX, int invY)
  {
    if (Inv.inventorySlots[invX, invY] != null)
    {
      if (x >= 0 && x <= worldSize && y >= 0 && y <= worldSize)
      {
        if (tile_layer == 0)
        {
          if (!frontground_world_tiles[x, y])
          {
            if (background_world_tiles[x, y])
            {
              if (background_world_tiles[x, y].inFront)
                RemoveinFrontTile(x, y);
            }
            PlaceTile(tile, x, y, true, tile_layer);
            Inv.Remove(invX, invY);
            RemoveLightSource(x, y);
          }
        }
        else if (tile_layer == 1)
        {
          if (!frontground_world_tiles[x, y])
          {
            PlaceTile(tile, x, y, true, tile_layer);
            Inv.Remove(invX, invY);
            RemoveLightSource(x, y);
          }
        }
        else if (tile_layer == 2)
        {
          if (!background_world_tiles[x, y])
          {
            PlaceTile(tile, x, y, true, tile_layer);
            Inv.Remove(invX, invY);
            if (!background_world_tiles[x, y].inFront)
              RemoveLightSource(x, y);
          }
        }
      }
      worldMap.Apply();
    }
  }

  void GenerateTile(TileClass tile, int x, int y, bool Light, int variation)
  {
    GameObject newTile = Instantiate(GameTile, transform, false);
    float chunkCoord = Mathf.Round(x / chunkSize) * chunkSize;
    chunkCoord /= chunkSize;
    newTile.transform.parent = worldChunks[(int)chunkCoord].transform;
    if(!background_world_tiles[x,y] && variation != 2)
    {
      newTile.GetComponent<SpriteRenderer>().sortingOrder = -10;
      Destroy(newTile.GetComponent<BoxCollider2D>());
      if(!tile.inFront)
      {
        newTile.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
      }
    }
    else if(background_world_tiles[x,y] && variation != 3)
    {
      newTile.tag = "Ground";
      newTile.GetComponent<SpriteRenderer>().sortingOrder = -5;
    }
    int spriteIndex = Random.Range(0, tile.tileSprites.Length);
    newTile.GetComponent<SpriteRenderer>().sprite = tile.tileSprites[spriteIndex];
    newTile.name = tile.tileName;
    newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);
    if(!worldObjects[x,y,0] && variation != 2)
      worldObjects[x,y,0] = newTile;
    else if(!worldObjects[x,y,1] && variation != 3)
      worldObjects[x,y,1] = newTile;
    if(!Light)
    {
      worldMap.SetPixel(x,y,Color.black);
    }
    TileClass newTileClass = TileClass.CreateInstance(tile);
    AddTileToWorld(x,y,newTileClass,variation-1);
  }

  public void PlaceTile(TileClass tile, int x, int y, bool Light, int tile_layer)
  {
    if(x >= 0 && x <= worldSize && y >= 0 && y <= worldSize)
    {
      if(tile_layer == 2)
      {
        if(!background_world_tiles[x,y])
        {
          GenerateTile(tile,x,y,Light,3);
        }
      }
      else if(tile_layer == 1)
      {
        if(!frontground_world_tiles[x,y])
        {
          GenerateTile(tile,x,y,Light,2);
        }
      }
      else if(tile_layer == 0)
      {
        if(!frontground_world_tiles[x,y])
        {
          GenerateTile(tile,x,y,Light,1);
        }
      }
    }
  }

  void AddTileToWorld(int x, int y, TileClass tile, int tile_layer)
  {
    if(tile_layer == 0)
    {
      if(!frontground_world_tiles[x,y])
      {
        if(background_world_tiles[x,y])
        {
          frontground_world_tiles[x,y] = tile;
        }
        else if(!background_world_tiles[x,y])
        {
          background_world_tiles[x,y] = tile;
        }
      }
    }
    else if(tile_layer == 1)
    {
      if(!frontground_world_tiles[x,y])
      {
        frontground_world_tiles[x,y] = tile;
      }
    }
    else if(tile_layer == 2)
    {
      if(!background_world_tiles[x,y])
      {
        background_world_tiles[x,y] = tile;
      }
    }
  }

  void RemoveTileFromWorld(int x, int y, int tile_layer)
  {
    if(tile_layer != 2)
    {
      if(frontground_world_tiles[x,y] != null)
      {
        frontground_world_tiles[x,y] = null;
      }
      else
      {
        if(background_world_tiles[x,y] != null)
        {
          background_world_tiles[x,y] = null;
        }
      }
    }
    else
    {
      if(background_world_tiles[x,y] != null)
      {
        background_world_tiles[x,y] = null;
      }
    }
  }

  int ReLightTile(int ix, int iy, int light_radius, float intensity, float dist, float thresh)
  {
    light_radius = 8;
    float ti = (intensity - (dist / lightRadius)) * thresh;
    if(worldMap.GetPixel(ix, iy).r < ti)
    {
      LightTile(ix,iy,ti);
    }
    return light_radius;
  }

  void LightGenerate(int x, int y, int light_radius, int ax, int ay, int bx, int by, float intensity)
  {
    for(int ix = x - ax; ix < x + bx; ix++)
    {
      for(int iy = y - ay; iy < y + by; iy++)
      {
        if(ix != x || iy != y)
        {
          if(ix >= 0 && ix < worldSize && iy >= 0 && iy < worldSize)
          {
            float dist = Vector2.Distance(new Vector2(x, y), new Vector2(ix, iy));
            float thresh = 0.8f;
            if(background_world_tiles[ix,iy] && !frontground_world_tiles[ix,iy])
            {
              lightRadius = CaveLight;
              thresh = 0.8f;
              if(ix > x + 7)
              {
                ReLightTile(ix,iy,light_radius,intensity,dist,thresh);
                break;
              }
            }
            else if(background_world_tiles[ix,iy] && frontground_world_tiles[ix,iy])
            {
              lightRadius = GroundLight;
              thresh = 1f;
              if(ix > x + 5)
              {
                ReLightTile(ix,iy,light_radius,intensity,dist,thresh);
                break;
              }
            }
            float target_intensity = (intensity - (dist / lightRadius)) * thresh;
            if(worldMap.GetPixel(ix, iy) != null)
            {
              if(worldMap.GetPixel(ix, iy).r < target_intensity)
              {
                worldMap.SetPixel(ix,iy,Color.white * target_intensity);
              }
            }
          }
        }
      }
    }
    worldMap.Apply();
  }

  void LightTile(int x, int y, float intensity)
  {
    if(x >= 0 && x < worldSize && y >= 0 && y < worldSize)
    {
      worldMap.SetPixel(x,y,Color.white * intensity);
      int light_radius = 7;
      if(background_world_tiles[x,y] && !frontground_world_tiles[x,y])
        light_radius = 8;
      else if(background_world_tiles[x,y] && frontground_world_tiles[x,y])
        light_radius = 5;
      if(x - light_radius >= 0 && x + light_radius < worldSize && y - light_radius >= 0 && y + light_radius < worldSize)
      {
        int a = light_radius;
        LightGenerate(x,y,a,a,a,a,a,intensity);
      }
      else
      {
        int ax = light_radius;
        int bx = light_radius;
        int ay = light_radius;
        int by = light_radius;
        LightParametres(x,y,light_radius,ax,ay,bx,by);
        LightGenerate(x,y,light_radius,ax,ay,bx,by,intensity);
      }
    }
  }

  public void ReloadLight(int x, int y, int light_radius, int ax, int ay, int bx, int by, List<Vector2Int> toRelight)
  {
    for(int ix = x - light_radius; ix < x + light_radius; ix++)
    {
      for(int iy = y - light_radius; iy < y + light_radius; iy++)
      {
        if(ix >= 0 && ix < worldSize && iy >= 0 && iy < worldSize)
        {
          if(worldMap.GetPixel(ix, iy).r > 0f)
          {
            if(frontground_world_tiles[ix,iy] || background_world_tiles[ix,iy] && !background_world_tiles[ix,iy].inFront)
            {
              worldMap.SetPixel(ix,iy, Color.black);
            }
          }
        }
      }
    }
    for(int ix = x - ax; ix < x + bx; ix++)
    {
      for(int iy = y - ay; iy < y + by; iy++)
      {
        if(ix >= 0 && ix < worldSize && iy >= 0 && iy < worldSize)
        {
          if(worldMap.GetPixel(ix, iy).r > 0f)
          {
            if(!background_world_tiles[ix,iy] && !frontground_world_tiles[ix,iy])
            {
              if(ix+1 < worldSize && ix-1 >= 0 && iy+1 < worldSize && iy-1 >= 0)
              {
                if(background_world_tiles[ix+1,iy] || background_world_tiles[ix-1,iy] || background_world_tiles[ix,iy+1] || background_world_tiles[ix,iy-1] ||
                   frontground_world_tiles[ix+1,iy] || frontground_world_tiles[ix-1,iy] || frontground_world_tiles[ix,iy+1] || frontground_world_tiles[ix,iy-1])
                {
                  toRelight.Add(new Vector2Int(ix,iy));
                }
              }
            }
            else if(background_world_tiles[ix,iy])
            {
              if(background_world_tiles[ix,iy].inFront)
              {
                toRelight.Add(new Vector2Int(ix,iy));
              }
            }
          }
        }
      }
    }
    foreach(Vector2Int source in toRelight)
    {
      LightTile(source.x, source.y, 1f);
    }
  }

  void RemoveLightSource(int x, int y)
  {
    List<Vector2Int> toRelight = new List<Vector2Int>();
    worldMap.SetPixel(x,y, Color.black);
    int light_radius = 8;
    if(background_world_tiles[x,y] && !frontground_world_tiles[x,y])
      light_radius = 8;
    else if(background_world_tiles[x,y] && frontground_world_tiles[x,y])
      light_radius = 5;
    if(x >= 0 && x < worldSize && y >= 0 && y < worldSize)
    {
      if(x - 12 >= 0 && x + 12 < worldSize && y - 12 >= 0 && y + 12 < worldSize)
      {
        ReloadLight(x,y,light_radius,12,12,12,12,toRelight);
      }
      else
      {
        int ax = 12;
        int bx = 12;
        int ay = 12;
        int by = 12;
        LightParametres(x,y,light_radius,ax,ay,bx,by);
        ReloadLight(x,y,light_radius,ax,ay,bx,by,toRelight);
      }
      worldMap.Apply();
    }
  }

  int LightParametres(int x, int y, int light_radius, int ax, int ay, int bx, int by)
  {
    if(x - 12 < 0)
    {
      ax = x;
      if(light_radius > ax)
      {
        light_radius = ax;
      }
    }
    if(x + 12 > worldSize)
    {
      bx = worldSize - x;
      if(light_radius > bx)
      {
        light_radius = bx;
      }
    }
    if(y - 12 < 0)
    {
      ay = y;
      if(light_radius > ay)
      {
        light_radius = ay;
      }
    }
    if(y + 12 > worldSize)
    {
      by = worldSize - y;
      if(light_radius > by)
      {
        light_radius = by;
      }
    }
    return ax;
    return ay;
    return bx;
    return by;
    return light_radius;
  }
}
