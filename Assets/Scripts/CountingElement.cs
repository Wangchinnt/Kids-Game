using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class CountingElement : MonoBehaviour
{
    // Start is called before the first frame update
    private Button _button;
    private CanvasGroup _textCanvasGroup;
    private CanvasGroup _text2CanvasGroup;
    private bool _isClickable = true;
    private IHaveCountingElement gameHaveCountingElement;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI text2;
    [SerializeField] private GameObject[] particles;
    public float fadeInDuration = 0.5f; // Thời gian fade-in
    public float shakeDuration = 0.5f; // Thời gian lắc
    public float moveUpDuration = 0.5f; // Thời gian di chuyển lên
    public float fadeOutDuration = 0.5f; // Thời gian fade-out
    public float moveUpDistance = 50f; // Khoảng cách di chuyển lên
    
    void Start()
    {
        transform.DOShakeScale(1, 0.1f, 1, 0, false).SetLoops(-1);
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonClick);
        _textCanvasGroup = text.GetComponent<CanvasGroup>();
        _text2CanvasGroup = text2.GetComponent<CanvasGroup>();
        MiniGame game = FindObjectOfType<MiniGame>();
        gameHaveCountingElement = game as IHaveCountingElement;
    }
    
    private void OnButtonClick()
    {   
        
        if (_isClickable == false) return;
        gameHaveCountingElement.IncreaseCountAndPlaySoundCount(this.GameObject());
        SpawnParticles();
        transform.DOShakeScale(0.5f, new Vector3(0.5f, 0.5f, 0), 1, 0, true);
        // mark the selected element as counted and disable the button 
        // play animation
        text2.text = gameHaveCountingElement.Count.ToString();
        text.text = gameHaveCountingElement.Count.ToString();
        _text2CanvasGroup.alpha = 0;
        _textCanvasGroup.alpha = 0;
      
        var sequence = DOTween.Sequence();
        sequence.Append(_text2CanvasGroup.DOFade(1, fadeInDuration));
        sequence.Append(text2.rectTransform.DOShakePosition(shakeDuration, new Vector3(10, 10, 0), 1, 0, false));
        sequence.Append(text2.rectTransform.DOLocalMoveY(text2.rectTransform.localPosition.y + moveUpDistance, moveUpDuration).SetEase(Ease.OutQuad));
        sequence.Append(_text2CanvasGroup.DOFade(0, fadeOutDuration));

        
        sequence.OnComplete(() =>
        {   
            text2.GameObject().SetActive(false); 
            _button.interactable = false;
            _textCanvasGroup.DOFade(1, 0.2f);
        });
        _isClickable = false;
    
    }

    public void Reset()
    {
        text.text = "";
        text2.text = "";
        text2.GameObject().SetActive(true);
        text2.rectTransform.localPosition = new Vector3(0, 0, 0);
        _button.interactable = true;
        _isClickable = true;
    }
    
    private void SpawnParticles()
    {
        foreach (GameObject particlePrefab in particles)
        {
            // Instantiate particle tại vị trí ban đầu của button
            GameObject particle = Instantiate(particlePrefab, transform.position + new Vector3(0, 0, -90), Quaternion.identity);

            // // Lấy hướng di chuyển (từ button đến text2)
            // Vector3 targetPosition = text2.rectTransform.position; // Vị trí đích là `text2`
             float moveDuration = fadeInDuration + shakeDuration +moveUpDuration + fadeOutDuration; // Thời gian để particle di chuyển
            //
            //particle.transform.DOMoveY(particle.transform.localPosition.y + moveUpDistance, moveUpDuration);
            // Destroy particle khi text2 bị destroy
            Destroy(particle, moveDuration); // Tự động xoá sau khi di chuyển
        }
    }

}
