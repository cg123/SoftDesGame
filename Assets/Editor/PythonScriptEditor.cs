using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Reflection;
using System.Linq;

[CustomEditor(typeof(PythonScript))]
public class PythonScriptEditor : Editor
{
    void ShowField(PythonScript s, string name)
    {
        object o = s.scope.GetVariable(name);
        if (o == null) return;

        if (typeof(float).IsInstanceOfType(o))
        {
            s.scope.SetVariable(name, EditorGUILayout.FloatField(name, (float)o));
        }
        else if (typeof(double).IsInstanceOfType(o))
        {
            s.scope.SetVariable(name, EditorGUILayout.FloatField(name, (float)(double)o));
        }
        else if (typeof(int).IsInstanceOfType(o))
        {
            s.scope.SetVariable(name, EditorGUILayout.IntField(name, (int)o));
        }
        else if (typeof(string).IsInstanceOfType(o))
        {
            s.scope.SetVariable(name, EditorGUILayout.TextField(name, (string)o));
        }
        else if (typeof(Vector2).IsInstanceOfType(o))
        {
            s.scope.SetVariable(name, EditorGUILayout.Vector2Field(name, (Vector2)o));
        }
        else if (typeof(Vector3).IsInstanceOfType(o))
        {
            s.scope.SetVariable(name, EditorGUILayout.Vector3Field(name, (Vector3)o));
        }
        else if (typeof(Vector4).IsInstanceOfType(o))
        {
            s.scope.SetVariable(name, EditorGUILayout.Vector3Field(name, (Vector4)o));
        }
    }


    HashSet<string> excludedNames = new HashSet<string>(Assembly.GetAssembly(typeof(GameObject)).GetTypes().Select((t) => t.Name));
    bool showPrivate = false;
    public override void OnInspectorGUI()
    {
        PythonScript s = (PythonScript)target;
        s.script = (TextAsset)EditorGUILayout.ObjectField("Script", s.script, typeof(TextAsset), false);
        EditorGUI.indentLevel++;
        if (s.scope != null)
        {
            IEnumerable<string> names = s.scope.GetVariableNames();
            IEnumerable<string> privateNames = names.Where((str) => str.StartsWith("_"));
            IEnumerable<string> publicNames = names.Where((str) => !excludedNames.Contains(str) && !str.StartsWith("_"));
            
            showPrivate = EditorGUILayout.Foldout(showPrivate, "Private Variables");
            if (showPrivate)
            {
                EditorGUI.indentLevel++;
                foreach (string name in privateNames)
                {
                    ShowField(s, name);
                }
                EditorGUI.indentLevel--;
            }
            foreach (string name in publicNames)
            {
                ShowField(s, name);
            }
        }
        EditorGUI.indentLevel--;
        if (GUILayout.Button("Reload"))
        {
            s.Reload();
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
