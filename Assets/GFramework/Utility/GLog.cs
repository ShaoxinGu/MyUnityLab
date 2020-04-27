using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GFramework
{
	public class GLog : MonoBehaviour
	{
		static List<string> mWriteTxt = new List<string>();
		string DevicePersistentPath, LogPath, logPath;
		void Awake()
		{

#if UNITY_EDITOR
			DevicePersistentPath = Application.persistentDataPath;
#elif UNITY_STANDALONE_WIN
			DevicePersistentPath = Application.dataPath + "/PersistentPath";
#elif UNITY_STANDALONE_OSX
			DevicePersistentPath = Application.dataPath + "/PersistentPath";
#else
			DevicePersistentPath = Application.persistentDataPath;
#endif
			LogPath = "Log";
			string logName = DateTime.Now.ToString("MMdd_HHmmss");
			logPath = string.Format("{0}/{1}/{2}.txt", DevicePersistentPath, LogPath, logName);
			if (File.Exists(logPath))
				File.Delete(logPath);

			//在这里做一个Log的监听
			Application.logMessageReceivedThreaded += HandleLog;
		}
		void Update()
		{
			if (mWriteTxt.Count > 0)
			{
				string[] temp = mWriteTxt.ToArray();
				foreach (string t in temp)
				{
					using (StreamWriter writer = new StreamWriter(logPath, true, Encoding.UTF8))
					{
						writer.WriteLine(t);
					}
					mWriteTxt.Remove(t);
				}
			}
		}

		void HandleLog(string logString, string stackTrace, LogType type)
		{
			mWriteTxt.Add(logString);
		}
	}
}