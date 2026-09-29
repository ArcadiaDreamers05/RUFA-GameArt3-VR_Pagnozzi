using UnityEngine;

namespace Rufa
{
    /// <summary>
    /// Gives a scene its own quality level (and so its own URP asset) while it plays, so one project can hold a scene
    /// with deliberately wrong URP settings next to scenes that keep the project's. Switching the level, instead of
    /// replacing the asset of the current level, leaves the project settings untouched: in the editor, Play mode
    /// changes to QualitySettings would otherwise be saved.
    /// </summary>
    [DefaultExecutionOrder(-1000)] // before anything that reads UniversalRenderPipeline.asset in Awake
    public sealed class ScenePipeline : MonoBehaviour
    {
        public string qualityLevel = "Demo";
        int previous = -1;

        void Awake()
        {
            int level = System.Array.IndexOf(QualitySettings.names, qualityLevel);
            if (level < 0) { Debug.LogError($"ScenePipeline: quality level '{qualityLevel}' not found (Project Settings > Quality)."); return; }
            previous = QualitySettings.GetQualityLevel();
            QualitySettings.SetQualityLevel(level, true);
        }

        void OnDestroy()
        {
            if (previous >= 0) QualitySettings.SetQualityLevel(previous, true);
        }
    }
}
