using System.Collections;
using ToolBox.Messaging;
using UnityEngine;

namespace CodeBase.Services.Audio
{
    public class AudioTest : MonoBehaviour
    {
        [SerializeField] private string sfxCode;
        
        [SerializeField] private string[] sfxNames;

        private void Start()
        {
           // StartCoroutine(Fire());
        }
        
        // Update is called once per frame
        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.M))
               MessageBus.Broadcast( AudioServiceMessages.RequestPlayMusicAtPosition, this,"stage1and4", new Vector3(0,-30,0) );
            
            if(UnityEngine.Input.GetKeyDown(KeyCode.C))
                MessageBus.Broadcast(AudioServiceMessages.RequestAudio, new AudioRequest("airshipselect", AudioCommand.CrossFade, this, new Vector3(0, -20, 0)));
            
            if(UnityEngine.Input.GetKeyDown(KeyCode.R))
                MessageBus.Broadcast(AudioServiceMessages.RequestAudio, new AudioRequest("stage1and4", AudioCommand.CrossFade, this, new Vector3(0, -20, 0)));
            
            if(UnityEngine.Input.GetKeyDown(KeyCode.P)) 
                MessageBus.Broadcast(AudioServiceMessages.RequestAudio, new AudioRequest("stage1and4", AudioCommand.Music, this, new Vector3(0, -20, 0)));
            
            if (UnityEngine.Input.GetKeyDown(KeyCode.S))
                MessageBus.Broadcast(AudioServiceMessages.RequestAudio, new AudioRequest("stage1and4", AudioCommand.StopMusic, this, new Vector3(0, -20, 0)));
        }


        private IEnumerator Fire()
        {
            while (true)
            {
                MessageBus.Broadcast(AudioServiceMessages.RequestPlayOneShot, PickRandomSfx());
                yield return new WaitForSeconds(Random.Range(0.2f, .8f));
            }
        }

        private string PickRandomSfx()
        {
            return sfxNames[Random.Range(0, sfxNames.Length)];
        }
    }
}
