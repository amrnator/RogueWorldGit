using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof (MapGennerator))]
public class NewBehaviourScript : Editor {

    public override void OnInspectorGUI()
    {
        MapGennerator mapGen = (MapGennerator)target;

        if (DrawDefaultInspector()) {
            if (mapGen.autoUpdate) {
                mapGen.GenerateMap();
            }
        }

        if (GUILayout.Button("Generate")) {
            mapGen.GenerateMap();
        }
    }
}
