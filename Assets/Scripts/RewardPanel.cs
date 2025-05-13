using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class RewardPanel : MonoBehaviour
{   
    [SerializeField]private GameObject _rewardPanel;
    [SerializeField]private Button _rewardButton;
    [SerializeField]private Button _leaderboardButton;
    [SerializeField]private GameObject _leaderboardPanel;
    
    [SerializeField]private Button _badgeButton;
    [SerializeField]private GameObject _badgePanel;

    [SerializeField]private Button _exitButton;


    private void Start()
    {   _rewardPanel = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "RewardPanel");
        _leaderboardPanel = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "LeaderboardPanel");
        _badgePanel = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "BadgePanel");
        _rewardButton = Resources.FindObjectsOfTypeAll<Button>().FirstOrDefault(obj => obj.name == "RewardButton");
        _leaderboardButton = Resources.FindObjectsOfTypeAll<Button>().FirstOrDefault(obj => obj.name == "LeaderboardButton");
        _badgeButton = Resources.FindObjectsOfTypeAll<Button>().FirstOrDefault(obj => obj.name == "BadgeButton");
        _exitButton = Resources.FindObjectsOfTypeAll<Button>().FirstOrDefault(obj => obj.name == "ExitRewardPanelButton");
        _rewardButton.onClick.AddListener(ShowRewardPanel);
        _leaderboardButton.onClick.AddListener(ShowLeaderboardPanel);
        _badgeButton.onClick.AddListener(ShowBadgePanel);
        _exitButton.onClick.AddListener(ExitRewardPanel);
    }

    private void ShowRewardPanel()
    {
        _leaderboardPanel.SetActive(true);
        _badgePanel.SetActive(false);
        _rewardPanel.SetActive(true);
    }
    private void ShowBadgePanel()
    {
        _leaderboardPanel.SetActive(false);
        _badgePanel.SetActive(true);
    }   
    private void ShowLeaderboardPanel()
    {
        _leaderboardPanel.SetActive(true);
        _badgePanel.SetActive(false);
    }
    private void ExitRewardPanel()
    {
        _rewardPanel.SetActive(false);
    }
    
}
