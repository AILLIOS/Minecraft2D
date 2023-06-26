using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ToolClass", menuName = "Tool Class")]
public class ToolClass : ScriptableObject
{
  public string name;
  public Sprite[] sprite;
  public bool toBreak;
  public bool StackUse;
  public bool OreDrop;
  public ItemClass.ToolType toolType;

  /*public static ToolClass CreateInstance(ToolClass tool)
  {
    var thisTool = ScriptableObject.CreateInstance<ToolClass>();
    thisTool.Init(tool);
    return thisTool;
  }

  public void Init (ToolClass tool)
  {
    name = tool.name;
    sprite = tool.sprite;
    toolType = tool.toolType;
    toBreak = tool.toBreak;
  }*/
}
