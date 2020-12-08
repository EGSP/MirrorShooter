using UnityEngine;

namespace Egsp.Utils.MeshUtilities
{
    public class PropertyBlockProxy
    {
        public PropertyBlockProxy(Renderer targetRenderer)
        {
            this.targetRenderer = targetRenderer;
            propertyBlock = new MaterialPropertyBlock();
            targetRenderer.GetPropertyBlock(propertyBlock);
        }

        /// <summary>
        /// Материал, которому будет устанавливаться блок значений.
        /// </summary>
        private readonly Renderer targetRenderer; 
        
        private readonly MaterialPropertyBlock propertyBlock;
        
        public PropertyBlockProxy SetFloat(string fieldName, float value)
        {
            propertyBlock.SetFloat(fieldName,value);
            return this;
        }

        public PropertyBlockProxy SetColor(string fieldName, Color value)
        {
            propertyBlock.SetColor(fieldName,value);
            return this;
        }

        public void Apply()
        {
            targetRenderer.SetPropertyBlock(propertyBlock);
        }
    }
}