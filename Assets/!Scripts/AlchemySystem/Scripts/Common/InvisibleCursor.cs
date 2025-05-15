using UnityEngine;

public class InvisibleCursor : MonoBehaviour
{
    public void TurnCursorInsivible()
    {
        Cursor.visible = false;
    }
    public void TurnCursorVisible() 
    { 
        Cursor.visible = true;
    }
}
