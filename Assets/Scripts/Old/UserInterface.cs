using UnityEngine;
using System.Collections;

public class UserInterface : MonoBehaviour
{
    public bool started = false;
    public int baconCount = 0;
    public PlayerHealthController phc;
    public Texture2D deadTexture;
    public Texture2D notDeadTexture;

    bool quitStarted = false;

    void OnGUI()
    {
        if (!started) return;
        GUI.Box(new Rect(0, 0, 256, 100), phc.health.ToString() + " HP");
        GUI.Box(new Rect(Screen.width - 256, 0, 256, 100), baconCount.ToString() + " Bacon");
        if (baconCount < 1)
        {
            float x0 = Screen.width / 2.0f - notDeadTexture.width / 2.0f,
                  y0 = Screen.height / 2.0f - notDeadTexture.height / 2.0f;
            GUI.DrawTexture(new Rect(x0, y0, notDeadTexture.width, notDeadTexture.height), notDeadTexture);
            if (!quitStarted)
            {
                StartCoroutine(WaitThenQuit());
                quitStarted = true;
            }
        }
        else if (phc.health < 1)
        {
            float x0 = Screen.width / 2.0f - deadTexture.width / 2.0f,
                  y0 = Screen.height / 2.0f - deadTexture.height / 2.0f;
            GUI.DrawTexture(new Rect(x0, y0, deadTexture.width, deadTexture.height), deadTexture);
            if (!quitStarted)
            {
                StartCoroutine(WaitThenQuit());
                quitStarted = true;
            }
        }
    }
    IEnumerator WaitThenQuit()
    {
        yield return new WaitForSeconds(5);
        Debug.Log("Quit()");
        Application.LoadLevel(0);
        //Application.Quit();
    }
}
