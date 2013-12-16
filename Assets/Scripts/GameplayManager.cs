using UnityEngine;
using System.Collections;

public class GameplayManager : MonoBehaviour
{
    public bool started = false;
    public int baconCount = 0;
    public PlayerHealthController phc;
    public Texture2D deadTexture;
    public Texture2D notDeadTexture;

    void OnGUI()
    {
        if (!started) return;
        if (baconCount < 1)
        {
            float x0 = Screen.width / 2.0f - deadTexture.width / 2.0f,
                  y0 = Screen.height / 2.0f - deadTexture.height / 2.0f;
            GUI.DrawTexture(new Rect(x0, y0, deadTexture.width, deadTexture.height), deadTexture);
        }
        else if (phc.health < 1)
        {
            float x0 = Screen.width / 2.0f - notDeadTexture.width / 2.0f,
                  y0 = Screen.height / 2.0f - notDeadTexture.height / 2.0f;
            GUI.DrawTexture(new Rect(x0, y0, notDeadTexture.width, notDeadTexture.height), notDeadTexture);
        }
    }
}
