using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class RewardItem : MonoBehaviour 
{
    private Button _button;
    private GameObject _popup;
    private TextMeshProUGUI _ItemName;
    private TextMeshProUGUI _ItemDescription;
    private TextMeshProUGUI _ItemDateTaken;
    private RectTransform _popupRectTransform;
    private RectTransform _itemRectTransform;
    
    private string _itemID;
    void Start()
    {   
        _itemID = transform.name;
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonClick);
        _popup = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(x => x.name == "Popup");
        _popup.SetActive(false);
        // popup has RectTransform and item also has RectTransform
        _popupRectTransform = _popup.GetComponent<RectTransform>();
        _itemRectTransform = GetComponent<RectTransform>();
        // find in child of _popup 
        _ItemName = _popup.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        _ItemDescription = _popup.transform.Find("Des").GetComponent<TextMeshProUGUI>();
        _ItemDateTaken = _popup.transform.Find("DateTaken").GetComponent<TextMeshProUGUI>();   

        var badges = LocalDataManager.Instance.LoadBadgesByStudent(AuthManager.Instance.GetCurrentUserId());
        bool hasBadge = badges.Any(b => b.BadgeID == _itemID);

        if (hasBadge) {
            Debug.Log("Đã nhận badge: " + _itemID);
            this.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        } else {
            this.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        }
    }

       void Update()
    {
        
    }
    void OnButtonClick()
    {   
        BadgeDatabase.LoadBadgeDefinitions();
        var badge = BadgeDatabase.GetBadgeDefinition(_itemID);
        _popupRectTransform.position = _itemRectTransform.position;
        _popup.SetActive(true);
        if (badge != null) {
            _ItemName.text = badge.Name;
            _ItemDescription.text = badge.Description;
            // Load badge data
            var badgeData = LocalDataManager.Instance.LoadBadgesByStudent(AuthManager.Instance.GetCurrentUserId()).FirstOrDefault(x => x.BadgeID == _itemID);
            if (badgeData != null) {
                Debug.Log("badgeData: " + badgeData.Name + " " + badgeData.Description + " " + badgeData.ObtainedAt);
                _ItemDateTaken.text = "Date Taken: " + badgeData.ObtainedAt;
            }
            else {
                _ItemDateTaken.text = "Not Taken";
            }
        }
        else {
            _ItemName.text = "Unknown";
            _ItemDescription.text = "Unknown";
            _ItemDateTaken.text = "Unknown";
        }
        // timer 5s then hide popup
        StartCoroutine(HidePopup());
    }
    IEnumerator HidePopup()
    {
        yield return new WaitForSeconds(5f);
        _popup.SetActive(false);
    }
  
}
