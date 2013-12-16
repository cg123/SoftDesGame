using UnityEngine;
using System.Collections;

public class UserInterface : MonoBehaviour
{
    public PlayerHealthController pc;
    void OnGUI()
    {
        GUI.Box(new Rect(0, 0, 256, 100), pc.health.ToString());
    }
}
