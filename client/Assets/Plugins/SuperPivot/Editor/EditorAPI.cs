﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SuperPivot
{
    public class EditorAPI
    {
        TargetWrapper m_TargetWrapper = null;
        string m_ErrorMsg = "";
        Tool m_prevTool;
        bool m_Snap = false;
        float m_SnapGridSize = 1f;
        const string m_EditorPrefsPrefix = "SuperPivot_";

        static GUIContent ms_SnapGUIContent = new GUIContent("Snap", "Snap to a virtual grid while moving the pivot with gizmos handlers in the scene view.");

        public event System.Action askToRepaintWindowDelegate;

        static public Transform[] GetSelectedTransforms()
        {
            return Selection.GetTransforms(SelectionMode.ExcludePrefab);
        }

        void AskToRepaintWindow()
        {
            if (askToRepaintWindowDelegate != null)
                askToRepaintWindowDelegate();
        }
        
        void SetTargets(Transform[] targets)
        {
            m_TargetWrapper = null;
            m_ErrorMsg = null;

            if (targets != null && targets.Length == 1)
                if (API.CanChangePivot(targets[0], out m_ErrorMsg))
                    m_TargetWrapper = new TargetWrapper(targets[0]);

            if (targets != null && targets.Length > 1)
                m_ErrorMsg = "Free version doesn't support multi-object editing";
        }

        void ResetTargets()
        {
            SetTargets(null);
        }

        bool StartMove()
        {
            StopMove();
            StartListeningSceneGUI();

            SetTargets(GetSelectedTransforms());
            if (m_TargetWrapper != null)
            {
                m_prevTool = Tools.current;
                Tools.current = Tool.None;
                SceneView.RepaintAll();
            }
            AskToRepaintWindow();
            return m_TargetWrapper != null;
        }

        public void StopMove()
        {
            StopListeningSceneGUI();
            if (m_TargetWrapper != null)
                Tools.current = m_prevTool;

            ResetTargets();
            AskToRepaintWindow();
        }

        void OnSelectionChanged()
        {
            StopMove();
        }

        public void DrawGUI(Transform[] selectedTransforms)
        {
            if (m_TargetWrapper == null)
            {
                if (GUILayout.Button("Move Pivot"))
                    StartMove();

                if (!string.IsNullOrEmpty(m_ErrorMsg))
                    EditorGUILayout.LabelField(m_ErrorMsg);
            }
            else
            {
                if (GUILayout.Button("Stop Moving Pivot"))
                {
                    Tools.current = m_prevTool;
                    ResetTargets();
                    SceneView.RepaintAll();
                }

                if (Tools.pivotMode == PivotMode.Center)
                {
                    EditorGUILayout.Separator();
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.HelpBox("Be careful, the Unity tool handle is in 'Center' mode, which may be confusing.\nWe recommend you to work in 'Pivot' mode instead.", MessageType.Warning, true);
                        if (GUILayout.Button("Switch to\n'Pivot' mode"))
                            Tools.pivotMode = PivotMode.Pivot;
                    }
                    EditorGUILayout.Separator();
                }

                if (m_TargetWrapper != null)
                {
                    GUISelection();

                    m_TargetWrapper.GUIWorldPosition();
                    m_TargetWrapper.GUILocalPosition();

                    EditorGUILayout.Separator();
                    {
                        EditorGUILayout.LabelField("Scene view", EditorStyles.boldLabel);

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUIUtility.labelWidth = 50f;

                            m_Snap = EditorGUILayout.Toggle(ms_SnapGUIContent, EditorPrefs.GetBool(m_EditorPrefsPrefix + "SnapToggle", m_Snap));
                            EditorPrefs.SetBool(m_EditorPrefsPrefix + "SnapToggle", m_Snap);

                            m_SnapGridSize = EditorGUILayout.FloatField(EditorPrefs.GetFloat(m_EditorPrefsPrefix + "SnapSize", m_SnapGridSize));
                            EditorPrefs.SetFloat(m_EditorPrefsPrefix + "SnapSize", m_SnapGridSize);
                        }
                    }
                }
            }
        }

        void GUISelection()
        {
            string header = "";
            if (m_TargetWrapper != null)
                header = string.Format("Currently moving '{0}\' pivot", m_TargetWrapper.name);

            EditorGUILayout.LabelField(header, EditorStyles.boldLabel);

            EditorGUILayout.Separator();
        }

        Vector3 GetSnappedPosition(Vector3 pivotPos)
        {
            if (m_Snap)
                pivotPos = new Vector3(
                    Mathf.Round(pivotPos.x / m_SnapGridSize) * m_SnapGridSize,
                    Mathf.Round(pivotPos.y / m_SnapGridSize) * m_SnapGridSize,
                    Mathf.Round(pivotPos.z / m_SnapGridSize) * m_SnapGridSize);
            return pivotPos;
        }

        public void OnWindowUpdate()
        {
            if (m_TargetWrapper != null && Tools.current != Tool.None)
            {
                StopMove();
            }
        }

        void StartListeningSceneGUI()
        {
#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui -= this.OnSceneGUI;
            SceneView.duringSceneGui += this.OnSceneGUI;
#else
            SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
            SceneView.onSceneGUIDelegate += this.OnSceneGUI;
#endif
        }

        void StopListeningSceneGUI()
        {
#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui -= this.OnSceneGUI;
#else
            SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
#endif
        }

        void OnSceneGUI(SceneView sceneView)
        {
            Handles.color = Color.yellow;
            Handles.matrix = Matrix4x4.identity;

            if(m_TargetWrapper != null)
            {
#if UNITY_5_6_OR_NEWER
                Handles.SphereHandleCap(0, m_TargetWrapper.transform.position, Quaternion.identity, HandleUtility.GetHandleSize(m_TargetWrapper.transform.position) * 0.3f, Event.current.type);
#else
                Handles.SphereCap(0, m_TargetWrapper.transform.position, Quaternion.identity, HandleUtility.GetHandleSize(m_TargetWrapper.transform.position) * 0.3f);
#endif

                if (m_TargetWrapper.TargetTransformHasChanged())
                    m_TargetWrapper.UpdateTargetCachedData();

                EditorGUI.BeginChangeCheck();
                var newPos = Handles.PositionHandle(m_TargetWrapper.transform.position, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    m_TargetWrapper.SetPivot(GetSnappedPosition(newPos), API.Space.Global);
                    AskToRepaintWindow(); // ask to repaint window
                }
            }
        }

        void Handles_DrawWireCube(Bounds bounds)
        {
#if UNITY_5_4_OR_NEWER
            Handles.DrawWireCube(bounds.center, bounds.size);
#else
            Vector3[] corners =
            {
                bounds.center + Vector3.Scale(bounds.extents, new Vector3(1, 1, 1)),
                bounds.center + Vector3.Scale(bounds.extents, new Vector3(-1, 1, 1)),
                bounds.center + Vector3.Scale(bounds.extents, new Vector3(-1, 1, -1)),
                bounds.center + Vector3.Scale(bounds.extents, new Vector3(1, 1, -1)),
                bounds.center + Vector3.Scale(bounds.extents, new Vector3(1, -1, 1)),
                bounds.center + Vector3.Scale(bounds.extents, new Vector3(-1, -1, 1)),
                bounds.center + Vector3.Scale(bounds.extents, new Vector3(-1, -1, -1)),
                bounds.center + Vector3.Scale(bounds.extents, new Vector3(1, -1, -1))
            };

            for (int i = 0; i < 4; i++)
            {
                Handles.DrawLine(corners[i], corners[(i + 1) % 4]);
                Handles.DrawLine(corners[i], corners[i + 4]);
                Handles.DrawLine(corners[i + 4], corners[4 + (i + 1) % 4]);
            }
#endif
        }

        // Singleton
        private static readonly EditorAPI instance = new EditorAPI();
        public static EditorAPI Instance { get { return instance; } }

        EditorAPI()
        {
            Selection.selectionChanged -= OnSelectionChanged;
            Selection.selectionChanged += OnSelectionChanged;

            EditorPrefs.SetFloat(m_EditorPrefsPrefix + "Version", API.Version);
        }
    }
}
