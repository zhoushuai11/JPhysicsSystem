using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using UnityEngine.Rendering;

namespace Render {
    public class SRPScriptAsset : RenderPipelineAsset {
        [MenuItem("Assets/Create/Render Pipeline/Kata01/Pipeline Asset")]
        public static void CreateKata01Pipeline() {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, 
                CreateInstance<CreateKata01PipelineAsset>(), 
                "SRP Pipeline.asset", null, null);
        }

        class CreateKata01PipelineAsset : EndNameEditAction {
            public override void Action(int instanceId, string pathName, string resourceFile) {
                var instance = CreateInstance<SRPScriptAsset>();
                AssetDatabase.CreateAsset(instance, pathName);
            }
        }

        protected override RenderPipeline CreatePipeline() {
            return new SRPScript();
        }
    }
}