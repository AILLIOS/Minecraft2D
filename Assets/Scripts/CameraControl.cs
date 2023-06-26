using System.Collections;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
  public GameObject Overlay;

  [Range(0, 1)]
  public float smoothTime;

  public Transform playerTransform;
  public float oSize;

  [HideInInspector]
  public int worldSize;

  public void SpawnPoint(Vector3 pos)
  {
    GetComponent<Transform>().position = pos;
    oSize = GetComponent<Camera>().orthographicSize;
  }

  public void FixedUpdate()
  {
    Vector3 pos = GetComponent<Transform>().position;
    pos.x = Mathf.Lerp(pos.x, playerTransform.position.x, smoothTime);
    pos.y = Mathf.Lerp(pos.y, playerTransform.position.y, smoothTime);
    pos.x = Mathf.Clamp(pos.x, 0 + (oSize * 1.8f), worldSize - (oSize * 1.8f));
    GetComponent<Transform>().position = pos;
    Overlay.GetComponent<Transform>().position = new Vector3(pos.x, pos.y, 0);
  }
}
