using System;
using System.Collections.Generic;
using System.Linq;
using Egsp.Core.Ui;
using Game.Net;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ServerInLobbyView : SerializedView
{
    [BoxGroup("Players")]
    [SerializeField] private GameObject playerGrid;
    [BoxGroup("Players")]
    [SerializeField] private GameObject playerBoxPrefab;
    
    [BoxGroup("Chat")]
    [SerializeField] private GameObject chatFlow;

    [BoxGroup("Scene")]
    [SerializeField] private TMP_InputField sceneField;

    [BoxGroup("Settings")]
    public Toggle spawnControllerToggle;
    
    [SerializeField] private TMP_Text statusTextbox;

    public event Action OnShutdown = delegate {  };
    public event Action<string> OnLoadScene = delegate {  };

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

    public void LoadScene()
    {
        if (sceneField.text.IsNullOrWhitespace())
            return;

        OnLoadScene(sceneField.text);
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
