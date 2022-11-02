using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

namespace Render {
    public class SRPScript : RenderPipeline {
        private CommandBuffer cb;
        
        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if (cb != null) {
                cb.Dispose();
                cb = null;
            }
            Debug.LogError("Dispose");
        }

        protected override void Render(ScriptableRenderContext context, Camera[] cameras) {
            if (null == cb) {
                cb = new CommandBuffer();
            }

            foreach (var camera in cameras) {
                context.SetupCameraProperties(camera);
                cb.ClearRenderTarget(true, true, Color.black);
                context.ExecuteCommandBuffer(cb);
                cb.Clear();
                context.Submit();
            }
        }
    }
}