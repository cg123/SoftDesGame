using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

public class Dungeon : MonoBehaviour {
    public TextAsset dungeonDotPy;

    public ScriptEngine engine;
    public ScriptScope scope;
    public UserInterface ui;
    long lastReadIdx;
    MemoryStream stdout;

    public Sprite[] floorSprites;
    public Sprite wallSprite, questSprite, itemSprite;
    GameObject[,] tiles;
    public GameObject jesusTemplate;
    public GameObject baconTemplate;

    public Transform player;

    public void Awake()
    {
        stdout = new MemoryStream();
        lastReadIdx = 0;
        engine = Python.CreateEngine();
        engine.Runtime.IO.SetOutput(stdout, System.Text.Encoding.UTF8);

        ICollection<string> searchPath = engine.GetSearchPaths();
        searchPath.Add(Application.dataPath + "/IronPythonLib");
        Debug.Log(Application.dataPath + "/IronPythonLib");
        engine.SetSearchPaths(searchPath);
        scope = engine.CreateScope();
        ScriptSource src = engine.CreateScriptSourceFromString(dungeonDotPy.text, dungeonDotPy.name, SourceCodeKind.File);
        src.Execute(scope);
        Debug.Log(dungeonDotPy.name);

        string res = engine.Execute<string>("make_dungeon_map()", scope);
        string[] lines = res.Trim().Split('\r', '\n');
        int width = lines[0].Length,
            height = lines.Length;

        tiles = new GameObject[width, height];
        int i, j;
        for (j = 0; j < height; j++)
        {
            for (i = 0; i < width; i++)
            {
                tiles[i, j] = new GameObject(i.ToString() + ", " + j.ToString());
                tiles[i, j].transform.parent = transform;
                tiles[i, j].transform.localPosition = new Vector3(i, -j, 50);
                tiles[i, j].transform.localScale = new Vector3(1, 1, 1);
                tiles[i, j].AddComponent<SpriteRenderer>();

                if (lines[j][i] == '@')
                {
                    GameObject jesus = Instantiate(jesusTemplate) as GameObject;
                    jesus.transform.parent = transform;
                    jesus.transform.localPosition = new Vector3(i, -j, 40);
                    jesus.transform.localScale = Vector3.one;
                }
                else if (lines[j][i] == '#')
                {
                    GameObject bacon = Instantiate(baconTemplate) as GameObject;
                    bacon.GetComponent<Pickup>().ui = ui;
                    bacon.transform.parent = transform;
                    bacon.transform.localPosition = new Vector3(i, -j, 40);
                    bacon.transform.localScale = Vector3.one;
                    ui.baconCount++;
                }
                else if (lines[j][i] == '%')
                {
                    player.rigidbody2D.isKinematic = true;
                    player.position = new Vector3(i, -j, 40);
                    player.rigidbody2D.isKinematic = false;
                }

                Sprite sprite;
                if (lines[j][i] == 'x')
                {
                    sprite = wallSprite;
                    tiles[i, j].AddComponent<BoxCollider2D>();
                    tiles[i, j].GetComponent<BoxCollider2D>().size = new Vector2(1, 1);
                }
                /*else if (lines[j][i] == ' ')
                {
                    sprite = floorSprite;
                }
                else if (lines[j][i] == '-')
                {
                    sprite = tunnelSprite;
                }*/
                else
                {
                    sprite = floorSprites[Random.Range(0, floorSprites.Length - 1)];
                }
                tiles[i, j].GetComponent<SpriteRenderer>().sprite = sprite;
            }
        }
        ui.started = true;
    }

    void Update()
    {
        string newData = ReadNewData();
        if (newData != null)
        {
            Debug.Log(newData);
        }
    }
    string ReadNewData()
    {
        if (stdout == null) return null;
        long length = stdout.Length - lastReadIdx;
        byte[] data = new byte[length];
        stdout.Seek(lastReadIdx, SeekOrigin.Begin);
        stdout.Read(data, 0, (int)length);
        lastReadIdx = stdout.Position;

        string res = System.Text.Encoding.UTF8.GetString(data);
        if (res == "") return null;
        return res;
    }
}
