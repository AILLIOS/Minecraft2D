using System.Collections;
using UnityEngine;

public class TileDrop : MonoBehaviour
{
  public ItemClass item;
  public bool added;

  private void OnTriggerEnter2D(Collider2D coll)
  {
    if(coll.gameObject.CompareTag("Player"))
    {
      AddItem(coll);
    }
  }

  private void AddItem(Collider2D coll)
  {
    if(added)
    {
      if(!coll.GetComponent<Inventory>().Add(item))
      {
        Destroy(this.gameObject);
      }
      added = false;
    }
  }
}
