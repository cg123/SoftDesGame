using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

public class PyDungeon : MonoBehaviour {
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

    static char GetTile(string[] lines, int i, int j)
    {
        if (j < 0 || j >= lines.Length || i < 0 || i >= lines[j].Length)
        {
            return 'x';
        }
        return lines[j][i];
    }

    public void Awake()
    {
        stdout = new MemoryStream();
        lastReadIdx = 0;
        engine = Python.CreateEngine();
        engine.Runtime.IO.SetOutput(stdout, System.Text.Encoding.UTF8);

        ICollection<string> searchPath = engine.GetSearchPaths();
        searchPath.Add(Application.streamingAssetsPath + "/IronPythonLib");
        Debug.Log(Application.streamingAssetsPath + "/IronPythonLib");
        engine.SetSearchPaths(searchPath);
        scope = engine.CreateScope();
        ScriptSource src = engine.CreateScriptSourceFromString(dungeonDotPy.text, dungeonDotPy.name, SourceCodeKind.File);
        src.Execute(scope);
        Debug.Log(dungeonDotPy.name);

        string res = engine.Execute<string>("make_dungeon_map()", scope);
        string[] lines = res.Trim().Split('\r', '\n');
        int width = lines[0].Length,
            height = lines.Length;

        tiles = new GameObject[width+2, height+2];
        int i, j;
        for (j = -1; j < height + 1; j++)
        {
            for (i = -1; i < width + 1; i++)
            {
                tiles[i+1, j+1] = new GameObject(i.ToString() + ", " + j.ToString());
                GameObject tileObj = tiles[i + 1, j + 1];
                tileObj.transform.parent = transform;
                tileObj.transform.localPosition = new Vector3(i, -j, 50);
                tileObj.transform.localScale = new Vector3(1, 1, 1);
                tileObj.AddComponent<SpriteRenderer>();

                char tile = GetTile(lines, i, j);
                if (tile == '@')
                {
                    GameObject jesus = Instantiate(jesusTemplate) as GameObject;
                    jesus.transform.parent = transform;
                    jesus.transform.localPosition = new Vector3(i, -j, 40);
                    jesus.transform.localScale = Vector3.one;
                }
                else if (tile == '#')
                {
                    GameObject bacon = Instantiate(baconTemplate) as GameObject;
                    bacon.GetComponent<Pickup>().ui = ui;
                    bacon.transform.parent = transform;
                    bacon.transform.localPosition = new Vector3(i, -j, 40);
                    bacon.transform.localScale = Vector3.one;
                    ui.baconCount++;
                }
                else if (tile == '%')
                {
                    player.rigidbody2D.isKinematic = true;
                    player.position = new Vector3(i, -j, 40);
                    player.rigidbody2D.isKinematic = false;
                }

                Sprite sprite;
                if (tile == 'x')
                {
                    sprite = wallSprite;
                    tileObj.AddComponent<BoxCollider2D>();
                    tileObj.GetComponent<BoxCollider2D>().size = new Vector2(1, 1);
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
                tileObj.GetComponent<SpriteRenderer>().sprite = sprite;
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
