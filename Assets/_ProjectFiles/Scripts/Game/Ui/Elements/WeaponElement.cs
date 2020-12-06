using System;
using Gasanov.Core.Pooling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Ui.Elements
{
    public class WeaponElement : MonoBehaviour, IPoolObject
    {
        public Button Button;
        public TMP_Text TextWithId;
        public Image Image;
        
        public IPoolContainer ParentPool { get; set; }
        public Action ReturnInstruction { get; set; }

        public void Awake()
        {
            Button = GetComponent<Button>();
            TextWithId = GetComponentInChildren<TMP_Text>();
            Image = GetComponentInChildren<Image>();
        }

        public void AwakeFromPool()
        {
        }

        public void ReturnToPool()
        {
        }

        public void DisposeByPool()
        {
        }
    }
}