using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GFramework
{
    public class AudioMgr : Singleton<AudioMgr>
    {
        private AudioSource bgm = null;
        private float bgmVolume = 1;

        private GameObject soundObj = null;
        private List<AudioSource> soundList = new List<AudioSource>();
        private float soundVolume = 1;

        public AudioMgr()
        {
            MonoMgr.Instance.AddUpdateListner(Update);
        }

        private void Update()
        {
            for (int i = soundList.Count - 1; i >= 0; i--)
            {
                if (!soundList[i].isPlaying)
                {
                    Object.Destroy(soundList[i]);
                    soundList.RemoveAt(i);
                }
            }
        }

        public void PlayBGM(string name)
        {
            if (bgm == null)
            {
                GameObject obj = new GameObject("BGM");
                bgm = obj.AddComponent<AudioSource>();
            }

            ResMgr.Instance.RealLoadAsyn<AudioClip>("Music/BGM/" + name, (clip) =>
            {
                bgm.clip = clip;
                bgm.loop = true;
                bgm.volume = bgmVolume;
                bgm.Play();
            });
        }

        public void PauseBGM(string name)
        {
            if (bgm == null)
                return;
            bgm.Pause();
        }

        public void StopBGM(string name)
        {
            if (bgm == null)
                return;
            bgm.Stop();
        }

        public void ChangeBGMVolume(float volume)
        {
            bgmVolume = volume;
            if (bgm == null)
                return;
            bgm.volume = bgmVolume;
        }

        public void PlaySound(string name, bool isLoop, UnityAction<AudioSource> callBack = null)
        {
            if (soundObj == null)
                soundObj = new GameObject("Sound");
            ResMgr.Instance.RealLoadAsyn<AudioClip>("Music/Sound/" + name, (clip) =>
            {
                AudioSource source = soundObj.AddComponent<AudioSource>();
                source.clip = clip;
                source.volume = soundVolume;
                source.loop = isLoop;
                source.Play();
                soundList.Add(source);
                callBack?.Invoke(source);
            });
        }

        public void ChangeSoundVolume(float volume)
        {
            soundVolume = volume;
            for (int i = 0; i < soundList.Count; i++)
                soundList[i].volume = soundVolume;
        }

        public void StopSound(AudioSource source)
        {
            if (soundList.Contains(source))
            {
                soundList.Remove(source);
                source.Stop();
                Object.Destroy(source);
            }
        }
    }
}
