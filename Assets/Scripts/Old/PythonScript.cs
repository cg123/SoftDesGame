using UnityEngine;
using System.Collections;
using System.Reflection;

using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

using System.IO;

public class PythonScript : MonoBehaviour {
    public TextAsset script;

    public ScriptEngine engine;
    public ScriptScope scope;
    long lastReadIdx;
    MemoryStream stdout;

    void Awake()
    {
        Reload();
	}

    public void Reload()
    {
        stdout = new MemoryStream();
        lastReadIdx = 0;
        engine = Python.CreateEngine();
        engine.Runtime.IO.SetOutput(stdout, System.Text.Encoding.UTF8);
        scope = engine.CreateScope();
        engine.Runtime.LoadAssembly(Assembly.GetAssembly(typeof(GameObject)));


        scope.SetVariable("transform", transform);
        scope.SetVariable("gameObject", gameObject);

        ScriptSource src = engine.CreateScriptSourceFromString(script.text, script.name, SourceCodeKind.File);
        src.Execute(scope);
    }

    delegate void VoidFunction();

    void Start()
    {
        VoidFunction startFunc;
        if (scope.TryGetVariable<VoidFunction>("Start", out startFunc))
        {
            startFunc();
        }
        HandleOutput();
    }
    void Update()
    {
        VoidFunction updateFunc;
        if (scope.TryGetVariable<VoidFunction>("Update", out updateFunc))
        {
            updateFunc();
        }
        HandleOutput();
    }

    void HandleOutput()
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
