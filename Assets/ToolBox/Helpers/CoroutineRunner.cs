using System.Collections;
using UnityEngine;

namespace ToolBox.Helpers
{
    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator routine);
        void StopCoroutine(Coroutine routine);
    }

    public class CoroutineRunner : ICoroutineRunner
    {
        private readonly MonoBehaviour _monoBehaviour;

        public CoroutineRunner(MonoBehaviour monoBehaviour)
        {
            _monoBehaviour = monoBehaviour;
        }

        public Coroutine StartCoroutine(IEnumerator routine) => _monoBehaviour.StartCoroutine(routine);
        
        public void StopCoroutine(Coroutine routine) => _monoBehaviour.StopCoroutine(routine);
    }
}