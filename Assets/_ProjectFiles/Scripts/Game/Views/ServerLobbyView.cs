﻿using System;
using System.Collections.Generic;
using System.Linq;
using Game.Net;
using Gasanov.Core.Mvp;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class ServerLobbyView : MonoBehaviour, IView
{
    [BoxGroup("Players")]
    [SerializeField] private GameObject playerGrid;
    [BoxGroup("Players")]
    [SerializeField] private GameObject playerBoxPrefab;
    
    [BoxGroup("Chat")]
    [SerializeField] private GameObject chatFlow;

    [SerializeField] private TMP_Text statusTextbox;

    public event Action OnShutdown = delegate {  };

    private List<Tuple<GameObject, User>> _playerBoxBinding;

    private void Awake()
    {
        _playerBoxBinding = new List<Tuple<GameObject, User>>();
    }
    
    public void Shutdown()
    {
        OnShutdown();
    }

    public void AddUser(User user)
    {
        var inst = Instantiate(playerBoxPrefab, playerGrid.transform, false);
        inst.GetComponentInChildren<TMP_Text>().text = user.Name;
        _playerBoxBinding.Add(new Tuple<GameObject, User>(inst, user));
    }

    public void RemoveUser(User user)
    {
        var coincidence = _playerBoxBinding.FirstOrDefault(x =>
            x.Item2.Name == user.Name);

        if (coincidence == null)
            return;
        
        Destroy(coincidence.Item1);
        _playerBoxBinding.Remove(coincidence);
    }
    
    public void ClearUsers()
    {
        foreach (var binding in _playerBoxBinding)
        {
            Destroy(binding.Item1);
        }
        
        _playerBoxBinding.Clear();
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void SetStatus(string text)
    {
        statusTextbox.text = text;
    }
}
