using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Egsp.Core.Ui
{
    public abstract class SerializedView: SerializedMonoBehaviour, IView
    {
        [OdinSerialize][InfoBox("Канвас, который будет отключаться или включаться. Если его не будет," +
                                " то эти действия будут применены к текущему объекту.")]
        public Canvas CanvasToUse { get; private set; }
        
        public bool Unloading { get; set; }
        
        public virtual void Enable()
        {
            if (CanvasToUse != null)
            {
                CanvasToUse.gameObject.SetActive(true);  
            }
            else
            {
                gameObject.SetActive(true);
            }
        }

        public virtual void Disable()
        {
            if (CanvasToUse != null)
            {
                CanvasToUse.gameObject.SetActive(false);    
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void OnDestroy()
        {
            if (!Unloading)
                ViewFactory.UnloadFromView(this);
        }
    }
}