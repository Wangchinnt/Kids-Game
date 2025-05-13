using UnityEngine;

public interface IHaveCountingElement
{
    int Count { get; set; } 
    void IncreaseCountAndPlaySoundCount(GameObject countingElement);
    void ResetCount();
}