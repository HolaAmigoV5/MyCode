using i3dFdeCore;
using i3dRenderEngine;

namespace I3DMapOperation
{
    public class FeatureLayerMap
    {
        public FeatureLayerMap(IFeatureLayer featureLayer, IFeatureClass featureClass)
        {
            FeatureLayer = featureLayer;
            FeatureClass = featureClass;
        }
        public IFeatureLayer FeatureLayer { get; set; }
        public IFeatureClass FeatureClass { get; set; }

        public void SetViewportMask(bool isVisible)
        {
            var visible = isVisible ? i3dViewportMask.i3dViewAllNormalView : i3dViewportMask.i3dViewNone;
            FeatureLayer.VisibleMask = visible;
        }
    }
}
