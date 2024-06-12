using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MazeGenerator2D))]
[CanEditMultipleObjects]
public class VisualizatorComponentEditor : Editor
{
    Texture2D boxBack;
    MazeGenerator2D subject;

    SerializedProperty MazeMode, wall, Mwidth, Mheight, TunnelWidth, WallWidth;
    
    SerializedProperty CreateMazeOnStart;
    SerializedProperty UseSeed, MazeSeed;
    SerializedProperty startPos, UseStart, UseEndWall, StartWallPrefab, EndWallPrefab;
    
    void OnEnable()
    {
        subject = target as MazeGenerator2D;
       
        MazeMode = serializedObject.FindProperty("MazeMode");
        wall = serializedObject.FindProperty("WallPrefab");
       
        Mwidth = serializedObject.FindProperty("MazeWidth");
        Mheight = serializedObject.FindProperty("MazeHeight");
        TunnelWidth = serializedObject.FindProperty("TunnelWidth");
        WallWidth = serializedObject.FindProperty("WallWidth");
        
       

        
        CreateMazeOnStart = serializedObject.FindProperty("CreateOnStart");       

        UseSeed = serializedObject.FindProperty("IsUseSeed");
        MazeSeed = serializedObject.FindProperty("MazeSeed");

        startPos = serializedObject.FindProperty("StartPosition");
        UseStart= serializedObject.FindProperty("UseStartWall");
        UseEndWall = serializedObject.FindProperty("UseEndWall");
        
        StartWallPrefab = serializedObject.FindProperty("StartWallPrefab");
        EndWallPrefab = serializedObject.FindProperty("EndWallPrefab");

        boxBack = (Texture2D)Resources.Load("box");



    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.BeginVertical();

        Rect r1 = EditorGUILayout.BeginVertical();
        EditorGUILayout.Vector2Field("Maze position", subject.StartPosition);
        
        EditorGUILayout.PropertyField(Mwidth);
        EditorGUILayout.PropertyField(Mheight);
        
        EditorGUILayout.PropertyField(TunnelWidth);
        EditorGUILayout.PropertyField(WallWidth);
        EditorGUILayout.EndVertical();
        
        //GUI.Box(r1, boxBack);
        EditorGUILayout.Separator();
        Rect r2 = EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(MazeMode);
        if (subject.MazeMode == MazeGenerator2D.MazeGeneratorMode.WithPrefabs)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(wall,new GUIContent("Wall prefabs:"));
            
            GUILayout.EndHorizontal();
            
            
        }
        EditorGUILayout.EndVertical();
        //GUI.Box(r2, "");
        EditorGUILayout.Separator();
        Rect r3 = EditorGUILayout.BeginVertical();
        
        EditorGUILayout.EndVertical();
        //GUI.Box(r3, "");
        EditorGUILayout.Separator();
        EditorGUILayout.PropertyField(CreateMazeOnStart);
        Rect r6 = EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(UseSeed);
        if(subject.IsUseSeed) EditorGUILayout.PropertyField(MazeSeed);
        EditorGUILayout.EndVertical();
        //GUI.Box(r6, "");
        EditorGUILayout.Separator();
        Rect r4 = EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(UseEndWall);

        if (subject.UseEndWall)
        {
            
            EditorGUILayout.PropertyField(EndWallPrefab);
        }
        EditorGUILayout.EndVertical();
        //GUI.Box(r4, "");
        EditorGUILayout.Separator();
        Rect r5 = EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(UseStart);
        if (subject.UseStartWall)
        {
            
            EditorGUILayout.PropertyField(StartWallPrefab);
        }
        EditorGUILayout.EndVertical();
        //GUI.Box(r5, "");

        

        EditorGUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();
    }


}
