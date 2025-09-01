using ToolBox.Animation.Lerp;
using UnityEngine;
using ToolBox.Utils.Interpolation;

public class LerpTest : MonoBehaviour
{
    [Header("Interpolation Settings")]
    [SerializeField] private LerpRecipes lerpRecipeSO;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField, Range(0f, 5f)] private float duration = 4f;

    private float _timer;
    private bool _forward = true;

    private float _elapsedTime = 0f;
    
    private void Start()
    {
        EzTween.Move(transform, startPoint.position, endPoint.position, duration)
            .SetEase(EaseType.Linear)
            .SetOnComplete(FinishedFirstTween);
    }

    private void FinishedFirstTween()
    {
        Debug.Log("Finished First Tween");
        EzTween.Move(transform, endPoint.position, startPoint.position, duration).SetEase(EaseType.Linear)
            .SetOnComplete(FinishedSecondTween);

    }

    private void FinishedSecondTween()
    {
        Debug.Log("Finished Secind Tween");
    }
    
    

    // private void Update()
    // {
    //     if (lerpRecipeSO == null || startPoint == null || endPoint == null) return;
    //
    //     // increment timer
    //     _timer += Time.deltaTime;
    //     float t = Mathf.Clamp01(_timer / duration);
    //
    //     // Apply lerp
    //     Vector3 newPos = lerpRecipeSO.Lerp(startPoint.position, endPoint.position, t);
    //     transform.position = newPos;
    //
    //     // Reset timer when reaching end
    //     if (_timer >= duration)
    //     {
    //         _timer = 0f;
    //         // swap start and end for ping-pong
    //         var temp = startPoint;
    //         startPoint = endPoint;
    //         endPoint = temp;
    //     }
    // }
}