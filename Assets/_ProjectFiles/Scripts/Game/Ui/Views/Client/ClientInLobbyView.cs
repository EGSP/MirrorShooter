using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Egsp.Core.Ui;
using Game.Net;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClientInLobbyView : SerializedView
{
    [BoxGroup("Players")]
    [SerializeField] private GameObject playerGrid;
    [BoxGroup("Players")]
    [SerializeField] private GameObject playerBoxPrefab;
    
    [BoxGroup("Session")]
    [SerializeField] private TMP_Text sessionStarted;
    [BoxGroup("Session")] 
    [SerializeField] private Button sessionReadyButton;
    

    [SerializeField] private TMP_Text statusTextbox;

    public event Action OnDisconnect = delegate {  };
    public event Action OnReady = delegate {  };

    private List<Tuple<GameObject, User>> _playerBoxBinding;

    private void Awake()
    {
        _playerBoxBinding = new List<Tuple<GameObject, User>>();
    }
    
    public void AddUser(User user)
    {
        var inst = Instantiate(playerBoxPrefab, playerGrid.transform, false);
        inst.GetComponentInChildren<TMP_Text>().text = user.name;
        _playerBoxBinding.Add(new Tuple<GameObject, User>(inst, user));
    }

    public void RemoveUser(User user)
    {
        var coincidence = _playerBoxBinding.FirstOrDefault(x =>
            x.Item2.id == user.id);

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
    
    public void Ready()
    {
        OnReady();
    }

    public void ShowSession(SessionStateMessage msg)
    {
        sessionStarted.text = $"Started = {msg.IsStarted}.";

        if (msg.IsStarted)
        {
            sessionReadyButton.interactable = true;
        }
        else
        {
            sessionReadyButton.interactable = false;
        }
    }

    public void Disconnect()
    {
        OnDisconnect();
    }

    public void SetStatus(string status)
    {
        statusTextbox.text = status;
    }
    
    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
