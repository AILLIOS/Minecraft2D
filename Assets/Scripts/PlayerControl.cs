using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
  public int selectedSlot = 0;
  public GameObject hotbarSelector;
  public GameObject selected_ui;
  public GameObject fixed_slot;
  private InventorySlot fixed_item;
  public Inventory Inv;
  public bool InvActivate;
  public bool BarActivate;
  public bool MouseSlotActive;
  public TileClass selectedTile;
  public ItemClass selectedItem;
  public int playerRange;
  public Vector2Int CamMousePos;
  public float moveSpeed;
  public float jumpForce;
  public bool onGround;
  private float horizontal;
  public float vertical;
  public float jump;
  public bool hit;
  public bool place;
  public int tile_layer;
  private Rigidbody2D rb;
  private Animator AN;
  public GameObject HandHolder;
  public LayerMask layerMask;
  private Vector3 mouse_position;
  private Vector2 ScreenSize;
  public Vector2 mouseMultiplier;
  private float screenMultiplier;
  private Vector2 m_pos;

  [HideInInspector]
  public Vector2 spawnPos;
  public TerrGeneration Terr;

  private void Start()
  {
    rb = GetComponent<Rigidbody2D>();
    AN = GetComponent<Animator>();
    Inv = GetComponent<Inventory>();
    Inv.barUI.SetActive(true);
    tile_layer = 0;
    ScreenSize = new Vector2(Screen.width, Screen.height);
    screenMultiplier = 1920 / ScreenSize.x;
    fixed_slot.GetComponent<SlotControl>()._fixed = true;
  }

  public void SpawnPoint()
  {
    GetComponent<Transform>().position = spawnPos;
  }

  public void OnCollisionEnter2D(Collision2D col)
  {
    if (col.gameObject.tag == "Ground")
      onGround = true;
  }

  private void FixedUpdate()
  {
    horizontal = Input.GetAxis("Horizontal");
    vertical = Input.GetAxisRaw("Vertical");
    jump = Input.GetAxisRaw("Jump");
    Vector2 movement = new Vector2(horizontal * moveSpeed, rb.velocity.y);
    if(horizontal > 0)
      transform.localScale = new Vector3(-1, 1, 1);
    else if(horizontal < 0)
      transform.localScale = new Vector3(1, 1, 1);
    if(vertical >= 0.1f || jump >= 0.1f)
    {
      if(onGround)
      {
        movement.y = jumpForce;
        onGround = false;
      }
    }
    if(FootRaycast() && !HeadRaycast() && movement.x != 0)
    {
      if(onGround)
      {
        movement.y = jumpForce;
        onGround = false;
      }
    }
    rb.velocity = movement;
  }

  public void InInvUpd()
  {
    int _x = Mathf.RoundToInt(mouse_position.x);
    int _y = Mathf.RoundToInt(mouse_position.y);
    if(_x >= 0 && _x < 10 && _y >= 0 && _y < 5)
    {
      if (Inv.inventorySlots[_x, _y] == null)
      {
        if (fixed_item != null)
        {
          Inv.SetupSlot(_x, _y, fixed_item);
          fixed_slot.transform.GetChild(0).GetComponent<Image>().enabled = false;
          fixed_slot.transform.GetChild(1).GetComponent<Text>().enabled = false;
          Inv.inventorySlots[_x, _y] = fixed_item;
        }
      }
    }
  }

  public void OutInvUpd()
  {
    int _x = Mathf.RoundToInt(mouse_position.x);
    int _y = Mathf.RoundToInt(mouse_position.y);
    if (_x >= 0 && _x < 10 && _y >= 0 && _y < 5)
    {
      if (Inv.inventorySlots[_x, _y] != null)
      {
        InventorySlot selected_slot = Inv.inventorySlots[_x, _y];
        fixed_slot.SetActive(true);
        fixed_slot.GetComponent<RectTransform>().localPosition = new Vector3(m_pos.x, m_pos.y);
        fixed_slot.GetComponent<Image>().enabled = false;
        fixed_slot.transform.GetChild(0).GetComponent<Image>().enabled = true;
        fixed_slot.transform.GetChild(0).GetComponent<Image>().sprite = selected_slot.item.Sprite;
        fixed_slot.transform.GetChild(1).GetComponent<Text>().enabled = false;
        if (Inv.inventorySlots[_x, _y].item.StackUse)
        {
          fixed_slot.transform.GetChild(1).GetComponent<Text>().enabled = true;
          fixed_slot.transform.GetChild(1).GetComponent<Text>().text = selected_slot.quantity.ToString();
        }
        fixed_item = selected_slot;
        Inv.ClearSlot(_x, _y);
      }
    }
  }

  private void Update()
  {
    mouse_position = Input.mousePosition;
    mouse_position = mouse_position * screenMultiplier;
    m_pos = new Vector2(mouse_position.x - (ScreenSize.x * screenMultiplier) / 2, mouse_position.y - (ScreenSize.y * screenMultiplier) / 2);
    mouse_position.x = mouse_position.x + Inv.start_grid.x * 1.06f;
    mouse_position.y = mouse_position.y + Inv.start_grid.y * 1.2f;
    mouse_position = mouse_position * mouseMultiplier;
    mouse_position = mouse_position + new Vector3(-0.5f, -0.5f, 0);
    if (!InvActivate)
    {
      hit = Input.GetMouseButton(0);
      place = Input.GetMouseButtonDown(1);
    }
    else if(InvActivate)
    {
      if(fixed_slot != null)
        fixed_slot.GetComponent<RectTransform>().localPosition = new Vector2(m_pos.x, m_pos.y);
    }
    if(Input.GetAxis("Mouse ScrollWheel") > 0)
    {
      if(selectedSlot < Inv.Width - 1)
        selectedSlot += 1;
    }
    else if(Input.GetAxis("Mouse ScrollWheel") < 0)
    {
      if(selectedSlot > 0)
        selectedSlot -= 1;
    }
    hotbarSelector.transform.position = Inv.UIHSlots[selectedSlot].transform.position;
    if(Inv.inventorySlots[selectedSlot, Inv.Height - 1] != null)
    {
      selectedItem = Inv.inventorySlots[selectedSlot, Inv.Height - 1].item;
      selectedTile = Inv.inventorySlots[selectedSlot, Inv.Height - 1].item.tile;
      HandHolder.GetComponent<SpriteRenderer>().sprite = selectedItem.Sprite;
      if(selectedItem.itemType == ItemClass.ItemType.tool)
      {
        HandHolder.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
      }
      else if(selectedItem.itemType == ItemClass.ItemType.tile)
      {
         HandHolder.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
      }
    }
    else
    {
      selectedItem = null;
      selectedTile = null;
      HandHolder.GetComponent<SpriteRenderer>().sprite = null;
    }
    if(Input.GetKeyDown(KeyCode.R))
    {
      if(tile_layer == 1)
        tile_layer = 2;
      else if(tile_layer == 0)
        tile_layer = 1;
      else if(tile_layer == 2)
        tile_layer = 0;
      Inv.UpdateLayerUI(tile_layer);
    }
    if(Input.GetKeyDown(KeyCode.E))
    {
      InvActivate = !InvActivate;
      if(InvActivate)
        BarActivate = false;
      else
        BarActivate = true;
      Inv.invUI.SetActive(InvActivate);
      Inv.barUI.SetActive(BarActivate);
    }

    if(Vector2.Distance(transform.position, CamMousePos) > -4f && Vector2.Distance(transform.position, CamMousePos) < 4f)
    {
      if(hit)
      {
        if(selectedItem != null)
        {
          if(selectedItem.itemType == ItemClass.ItemType.tool)
            Terr.BreakTile(CamMousePos.x, CamMousePos.y, tile_layer, selectedItem);
          if(selectedItem.itemType == ItemClass.ItemType.tile)
            Terr.RemoveTile(CamMousePos.x, CamMousePos.y, tile_layer);
        }
        if(selectedItem == null)
        {
          Terr.RemoveTile(CamMousePos.x, CamMousePos.y, tile_layer);
        }
      }
      if(place)
      {
        if(selectedItem != null)
        {
          if(selectedItem.itemType == ItemClass.ItemType.tile)
          {
            TileClass newTile = new TileClass
            {
              tileName = selectedTile.tileName,
              wallVariant = selectedTile.wallVariant,
              tileSprites = selectedTile.tileDrop,
              inFront = selectedTile.inFront,
              toBreak = selectedTile.toBreak,
              tileDrop = selectedTile.tileDrop,
              DropUse = selectedTile.DropUse,
              StackUse = selectedTile.StackUse
            };
            Terr.CheckTile(newTile, CamMousePos.x, CamMousePos.y, tile_layer, selectedSlot, Inv.Height - 1);
          }
        }
      }
    }
    CamMousePos.x = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x - 0.5f);
    CamMousePos.y = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - 0.5f);
    AN.SetFloat("horizontal", horizontal);
    AN.SetBool("hit", hit || place);
  }

  public bool FootRaycast()
  {
    RaycastHit2D hit = Physics2D.Raycast(transform.position - (Vector3.up * 0.5f), -Vector2.right * transform.localScale.x, 1.2f, layerMask);
    return hit;
  }

  public bool HeadRaycast()
  {
    RaycastHit2D hit = Physics2D.Raycast(transform.position + (Vector3.up * 0.5f), -Vector2.right * transform.localScale.x, 1.2f, layerMask);
    return hit;
  }
}
