using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
  private int stack_limit = 64;
  public GameObject[,] UISlots;
  public GameObject[] UIHSlots;
  public InventorySlot[,] inventorySlots;
  public InventorySlot[] hotbarSlots;
  public float slot_scale { get; private set; }
  public Vector2 start_grid { get; private set; }
  private Vector2 offset;
  //private Vector2 multiplier;
  public Vector2 barOffset;
  public Vector2 barmultiplier;
  public Sprite uisprite;
  public Sprite uisprite2;
  public GameObject invUI;
  public GameObject barUI;
  public GameObject UISlot;
  public GameObject UISlot2;
  public GameObject LayerSlot;
  public Sprite[] LayerSprites;

  public int Width;
  public int Height;


  public void Start()
  {
    inventorySlots = new InventorySlot[Width, Height];
    UISlots = new GameObject[Width, Height];
    SetupUI();
    UpdateInvUI();
    UpdateBarUI();
  }
  void SetupUI()
  {
    slot_scale = 105;
    offset = new Vector2(slot_scale + 5, slot_scale + 5);
    start_grid = new Vector2(-(offset.x * (Width - 1) / 2), -(offset.y * Height / 2));
    SetupUIInventory(start_grid);
    //SetupUIHotbar();
  }

  /*public void Update()
  {
    for (int x = 0; x < Width; x++)
    {
      for (int y = 0; y < Height; y++)
      {
        UISlots[x,y].GetComponent<RectTransform>().localPosition = new Vector3((x * multiplier.x) + offset.x, (y * multiplier.y) + offset.y);
      }
    }
  }*/

  void SetupUIInventory(Vector2 start_grid)
  {
    for(int x = 0; x < Width; x++)
    {
      for(int y = 0; y < Height; y++)
      {
        GameObject invSlot = Instantiate(UISlot, invUI.transform.GetChild(0).transform);
        Vector2 spawn_pos = new Vector2(offset.x * x, offset.y * y);
        invSlot.GetComponent<RectTransform>().localPosition = new Vector3(start_grid.x + spawn_pos.x, start_grid.y + spawn_pos.y);
        invSlot.GetComponent<Image>().enabled = true;
        UISlots[x,y] = invSlot;
        inventorySlots[x,y] = null;
      }
    }
  }

  /*void SetupUIHotbar()
  {
    for(int x = 0; x < Width; x++)
    {
      GameObject barSlot = Instantiate(UISlot2, barUI.transform.GetChild(0).transform);
      barSlot.GetComponent<RectTransform>().localPosition = new Vector3((x * barmultiplier.x) + barOffset.x, barOffset.y);
      UIHSlots[x] = barSlot;
      hotbarSlots[x] = null;
    }
  }*/

  public void UpdateInvUI()
  {
    for(int x = 0; x < Width; x++)
    {
      for(int y = 0; y < Height; y++)
      {
        if(inventorySlots[x,y] == null)
        {
          UISlots[x,y].transform.GetChild(0).GetComponent<Image>().sprite = null;
          UISlots[x,y].transform.GetChild(0).GetComponent<Image>().enabled = false;
          UISlots[x,y].transform.GetChild(1).GetComponent<Text>().text = "";
          UISlots[x,y].transform.GetChild(1).GetComponent<Text>().enabled = false;
          UISlots[x,y].GetComponent<Image>().color = new Color32(255, 255, 255, 180);
        }
        else
        {
          UISlots[x,y].transform.GetChild(0).GetComponent<Image>().enabled = true;
          UISlots[x,y].transform.GetChild(0).GetComponent<Image>().sprite = inventorySlots[x,y].item.Sprite;
          if(inventorySlots[x,y].item.StackUse)
          {
            UISlots[x,y].transform.GetChild(1).GetComponent<Text>().enabled = true;
            UISlots[x,y].transform.GetChild(1).GetComponent<Text>().text = inventorySlots[x,y].quantity.ToString();
          }
          UISlots[x,y].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
      }
    }
  }

  public void UpdateBarUI()
  {
    for(int x = 0; x < Width; x++)
    {
      if(inventorySlots[x, Height - 1] == null)
      {
        UIHSlots[x].transform.GetChild(0).GetComponent<Image>().sprite = null;
        UIHSlots[x].transform.GetChild(0).GetComponent<Image>().enabled = false;
        UIHSlots[x].transform.GetChild(1).GetComponent<Text>().text = "";
        UIHSlots[x].transform.GetChild(1).GetComponent<Text>().enabled = false;
        UIHSlots[x].GetComponent<Image>().color = new Color32(255, 255, 255, 180);
      }
      else
      {
        UIHSlots[x].transform.GetChild(0).GetComponent<Image>().enabled = true;
        UIHSlots[x].transform.GetChild(0).GetComponent<Image>().sprite = inventorySlots[x, Height - 1].item.Sprite;
        if(inventorySlots[x, Height - 1].item.StackUse)
        {
          UIHSlots[x].transform.GetChild(1).GetComponent<Text>().enabled = true;
          UIHSlots[x].transform.GetChild(1).GetComponent<Text>().text = inventorySlots[x, Height - 1].quantity.ToString();
        }
        UIHSlots[x].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
      }
    }
  }

  public void ClearSlot(int x, int y)
  {
    if(y == Height - 1)
    {
      UIHSlots[x].transform.GetChild(0).GetComponent<Image>().sprite = null;
      UIHSlots[x].transform.GetChild(0).GetComponent<Image>().enabled = false;
      UIHSlots[x].transform.GetChild(1).GetComponent<Text>().text = "";
      UIHSlots[x].transform.GetChild(1).GetComponent<Text>().enabled = false;
      UIHSlots[x].GetComponent<Image>().color = new Color32(255, 255, 255, 180);
    }
    UISlots[x,y].transform.GetChild(0).GetComponent<Image>().sprite = null;
    UISlots[x,y].transform.GetChild(0).GetComponent<Image>().enabled = false;
    UISlots[x,y].transform.GetChild(1).GetComponent<Text>().text = "";
    UISlots[x,y].transform.GetChild(1).GetComponent<Text>().enabled = false;
    UISlots[x,y].GetComponent<Image>().color = new Color32(255, 255, 255, 180);
    inventorySlots[x,y] = null;
  }

  public void SetupSlot(int x, int y, InventorySlot fixed_item)
  {
    UISlots[x,y].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
    UISlots[x,y].transform.GetChild(0).GetComponent<Image>().enabled = true;
    UISlots[x,y].transform.GetChild(0).GetComponent<Image>().sprite = fixed_item.item.Sprite;
    UISlots[x,y].transform.GetChild(1).GetComponent<Text>().enabled = false;
    if(fixed_item.item.StackUse)
    {
      UISlots[x,y].transform.GetChild(1).GetComponent<Text>().enabled = true;
      UISlots[x,y].transform.GetChild(1).GetComponent<Text>().text = fixed_item.quantity.ToString();
    }
    if(y == Height - 1)
    {
      UIHSlots[x].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
      UIHSlots[x].transform.GetChild(0).GetComponent<Image>().enabled = true;
      UIHSlots[x].transform.GetChild(0).GetComponent<Image>().sprite = fixed_item.item.Sprite;
      UIHSlots[x].transform.GetChild(1).GetComponent<Text>().enabled = false;
      if(fixed_item.item.StackUse)
      {
        UIHSlots[x].transform.GetChild(1).GetComponent<Text>().enabled = true;
        UIHSlots[x].transform.GetChild(1).GetComponent<Text>().text = fixed_item.quantity.ToString();
      }
    }
  }

  public void UpdateLayerUI(int tile_layer)
  {
    if(tile_layer == 0)
      LayerSlot.transform.GetChild(0).GetComponent<Image>().sprite = LayerSprites[0];
    else if(tile_layer == 1)
      LayerSlot.transform.GetChild(0).GetComponent<Image>().sprite = LayerSprites[1];
    else if(tile_layer == 2)
      LayerSlot.transform.GetChild(0).GetComponent<Image>().sprite = LayerSprites[2];
  }

  public bool Add(ItemClass Item)
  {
    bool added = true;
    for(int y = Height - 1; y > 0; y--)
    {
      for(int x = 0; x < Width; x++)
      {
        if(added == false)
          break;
        if(inventorySlots[x,y] == null)
        {
          inventorySlots[x,y] = new InventorySlot{item = Item, position = new Vector2Int(x,y), quantity = 1};
          if(Item.DropUse || Item.OreDrop)
            inventorySlots[x,y].item.Sprite = Item.tileDrop[0];
          added = false;
          break;
        }
        else if(inventorySlots[x,y] != null)
        {
          if(inventorySlots[x,y].item.Sprite == Item.tileDrop[0] || inventorySlots[x,y].item.Sprite == Item.Sprite)
          {
            if(inventorySlots[x,y].item.StackUse)
            {
              if(inventorySlots[x,y].quantity < 64)
              {
                inventorySlots[x,y].quantity += 1;
                added = false;
                break;
              }
            }
          }
        }
      }
    }
    UpdateInvUI();
    UpdateBarUI();
    return added;
  }

  public void Remove(int x, int y)
  {
    if(inventorySlots[x,y].quantity > 0 && inventorySlots[x,y].quantity <= stack_limit && inventorySlots[x,y].item.StackUse)
    {
      inventorySlots[x,y].quantity -= 1;
      if(inventorySlots[x,y].quantity == 0)
      {
        inventorySlots[x,y] = null;
      }
      UpdateInvUI();
      UpdateBarUI();
    }
  }
}
