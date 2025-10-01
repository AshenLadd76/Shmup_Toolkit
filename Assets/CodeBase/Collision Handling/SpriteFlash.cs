using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(SpriteRenderer))]


public class SpriteFlash : MonoBehaviour
{

    [SerializeField] private Color flashColour;
    [SerializeField] private float flashDuration;

    private Material _material;

    private IEnumerator _flashCoRoutine;
    private static readonly int FlashAmount = Shader.PropertyToID("_FlashAmount");
    private static readonly int FlashColor = Shader.PropertyToID("_FlashColor");

    private void Awake()
    {
        _material = GetComponent<SpriteRenderer>().material;
    }

    private void Start()
    {
        _material.SetColor( FlashColor, flashColour );
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Flash();
    }

    public void Flash()
    {
        if( _flashCoRoutine != null  )
            StopCoroutine( _flashCoRoutine );

        _flashCoRoutine = FlashCo();
            
        StartCoroutine( _flashCoRoutine );

    }

    private IEnumerator FlashCo()
    {
        float lerpTime = 0;

        while (lerpTime < flashDuration)
        {
            lerpTime += Time.deltaTime;

            float perc = lerpTime / flashDuration;

            SetFlashAmount( 1f - perc );
                
            yield return null;
        }
    }

    private void SetFlashAmount(float flashAmount)
    {
        if( _material )
            _material.SetFloat( FlashAmount, flashAmount );
    }
}

