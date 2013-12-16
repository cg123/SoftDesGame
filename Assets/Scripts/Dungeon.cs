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
    long lastReadIdx;
    MemoryStream stdout;


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
        Debug.Log(res);
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
