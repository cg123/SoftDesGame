using UnityEngine;
using System.Collections;

public class TestDungeon : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Dungeon d = new Dungeon(50, 50);
        d.Regenerate(new Dungeon.GenerationOptions());
    }

    // Update is called once per frame
    void Update()
    {

    }
}
