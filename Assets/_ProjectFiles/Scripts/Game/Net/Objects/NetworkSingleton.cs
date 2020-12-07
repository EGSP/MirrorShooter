using System;
using System.Collections;
using System.Collections.Generic;
using Game.Net.Objects;
using UnityEngine;

/// <summary>
/// При появлении объект сразу устанавливает на себя ссылку.
/// Нужен для объектов, создание которых контроллирует сервер.
/// </summary>
public class NetworkSingleton<TSingleton> : DualNetworkBehaviour
    where TSingleton : NetworkSingleton<TSingleton>
{
    /// <summary>
    /// Текущий экземпляр объекта. Может быть NULL, до момента создания.
    /// </summary>
    public static TSingleton Instance { get; private set; }

    /// <summary>
    /// Вызывается при появлении в мире объекта данного типа.
    /// </summary>
    public static event Action<TSingleton> OnInstanceCreated;

    protected override void Awake()
    {
        Instance = this as TSingleton;
        OnInstanceCreated(Instance);
        
        base.Awake();
    }
}
