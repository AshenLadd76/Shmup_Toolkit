using System.Collections;
using ToolBox.Messaging;
using UnityEngine;

namespace CodeBase.Audio
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
            if (Input.GetKeyDown(KeyCode.M))
               MessageBus.Broadcast( AudioServiceMessages.RequestPlayMusicTrack, sfxCode );
            
            if(Input.GetKeyDown(KeyCode.C))
               MessageBus.Broadcast(AudioServiceMessages.RequestAudioCrossFade, "stage1and4");
            
            if(Input.GetKeyDown(KeyCode.R))
                MessageBus.Broadcast(AudioServiceMessages.RequestAudioCrossFade, sfxCode);
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
