using Unity;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveHandle : EditorHandle
{
    public override void BeginDrag()
    {
        
    }

    public override void Drag(Vector2 mouse)
    {
        throw new System.NotImplementedException();
    }

    public override void EndDrag()
    {
        throw new System.NotImplementedException();
    }

    public override bool IsMouseOver()
    {
        if(IsMouseOver())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}