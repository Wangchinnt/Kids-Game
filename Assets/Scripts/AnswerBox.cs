using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class AnswerBox: MonoBehaviour
{
    [SerializeField] private string _value;
    [SerializeField] private GameObject textMeshPro;
    [SerializeField] private GameObject[] particles;
    [SerializeField] private bool isAnimateTextEvery5Seconds = true;
    private TextMeshProUGUI _textMeshPro;
    private List<GameObject> _instantiatedParticles = new List<GameObject>();
    
    void Start()
    {
        _textMeshPro = textMeshPro.GetComponent<TextMeshProUGUI>();
        if (isAnimateTextEvery5Seconds) StartCoroutine(AnimateTextEvery5Seconds());
    }

    private IEnumerator AnimateTextEvery5Seconds()
    {
        while (true)
        {
            foreach (GameObject particlePrefab in particles)
            {
                GameObject particle = Instantiate(particlePrefab, transform.position + new Vector3(0, 0, -90), Quaternion.identity);
                _instantiatedParticles.Add(particle); 
            }
            var sequence = DOTween.Sequence();
            sequence.Append(_textMeshPro.transform.DOScale(1.5f, 0.8f).SetLoops(2, LoopType.Yoyo));
            sequence.Join(_textMeshPro.transform.DOShakeRotation(0.8f, new Vector3(0, 0, 10), 10, 90, true, ShakeRandomnessMode.Harmonic));

            yield return sequence.WaitForCompletion();

            // Sau khi sequence hoàn tất, xóa các particle đã tạo
            foreach (GameObject particle in _instantiatedParticles)
            {
                Destroy(particle); // Xóa particle
            }

            // Chờ thêm để đủ 5 giây
            yield return new WaitForSeconds(3f);
        }
    }

    public string GetText()
    {
        return _textMeshPro.text;
    }
    
    public void SetText(string text)
    {
        _textMeshPro.text = text;
        StopAllCoroutines();
        if (_instantiatedParticles.Count > 0)
        {
            foreach (GameObject particle in _instantiatedParticles)
            {
                Destroy(particle);
            }
        }
    }

    public void SetValue(string value)
    {
        _value = value;
    }
    
    public string GetValue()
    {
        return _value; 
    }
    
    
}
