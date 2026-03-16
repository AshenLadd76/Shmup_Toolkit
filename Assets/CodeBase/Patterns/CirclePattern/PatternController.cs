using System.Collections;
using UnityEngine;

namespace CodeBase.Patterns.CirclePattern
{
    [RequireComponent(typeof(PatternGenerator))]
    public class PatternController : MonoBehaviour
    {
       private PatternGenerator _patternGenerator;
       
       private Coroutine _coroutine;

       private void Awake()
       {
           _patternGenerator = GetComponent<PatternGenerator>();
       }
       
       public  void StartPattern()
       {
           if (_coroutine != null) return;
           
           _coroutine = StartCoroutine(ExecutePatternCr());
       }

       public void StopPattern()
       {
           if (_coroutine == null) return;
           
           _patternGenerator?.StopGeneratorCoroutine();
           
           StopCoroutine(_coroutine);
           
           _coroutine = null;
       }
       private IEnumerator ExecutePatternCr()
       {
           while (true)
           {
               _patternGenerator?.StartGeneratorCoroutine();

               yield return new WaitForSeconds(Random.Range(0.5f, 3f));

               _patternGenerator?.StopGeneratorCoroutine();
               
               yield return new WaitForSeconds(Random.Range(0.5f, .75f));

               yield return null;
           }
       }
    }
}
