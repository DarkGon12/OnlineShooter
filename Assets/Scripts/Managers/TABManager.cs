using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TABManager : MonoBehaviour
{
    [SerializeField] private List<PlayerTab> _playerTabs = new List<PlayerTab>();

    [SerializeField] private CanvasGroup _tabGroup;
    [SerializeField] private GameObject _tabContent;
    [SerializeField] private GameObject _tabPrefab;

    public void AddPlayerTab(string name)
    {
        foreach (var playerTab in _playerTabs)
        {
            if (playerTab.Name == name)
                return;
        }

        GameObject tab = Instantiate(_tabPrefab, _tabContent.transform);
        tab.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;

        PlayerTab newPlayerTab = new PlayerTab{
            Name = name,
            Score = 0,
            TabObject = tab
        };

        _playerTabs.Add(newPlayerTab);
        tab.name = name;
    }

    public void RemovePlayer(string name)
    {
        for (int i = 0; i < _playerTabs.Count; i++)
        {
            if (_playerTabs[i].Name == name)
            {
                Destroy(_playerTabs[i].TabObject);
                _playerTabs.RemoveAt(i);
                break;
            }
        }
    }

    public void ChangeScore(string name, int score)
    {
        foreach (var playerTab in _playerTabs)
        {
            if (playerTab.Name == name)
            {
                playerTab.Score = score;
                playerTab.TabObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = score.ToString();
                SortTabsByScore();
                return;
            }
        }
    }

    private void SortTabsByScore()
    {
        _playerTabs.Sort((x, y) => y.Score.CompareTo(x.Score));

        for (int i = 0; i < _playerTabs.Count; i++)
            _playerTabs[i].TabObject.transform.SetSiblingIndex(i);        
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
            _tabGroup.alpha = 1;
        else
            _tabGroup.alpha = 0;
    }
}