using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Utage
{
	public interface SoundManagerSystemInterface
	{
		bool IsLoading { get; }

		void Init(SoundManager soundManager, List<string> saveStreamNameList);

		void Play(string groupName, string label, SoundData soundData, float fadeInTime, float fadeOutTime);

		void Stop(string groupName, string label, float fadeTime);

		bool IsPlaying(string groupName, string label);

		AudioSource GetAudioSource(string groupName, string label);

		float GetSamplesVolume(string groupName, string label);

		void StopGroup(string groupName, float fadeTime);

		void StopGroupIgnoreLoop(string groupName, float fadeTime);

		void StopAll(float fadeTime);

		void StopAllLoop(float fadeTime);

		float GetMasterVolume(string groupName);

		void SetMasterVolume(string groupName, float volume);

		float GetGroupVolume(string groupName);

		void SetGroupVolume(string groupName, float volume, float fadeTime = 0f);

		bool IsMultiPlay(string groupName);

		void SetMultiPlay(string groupName, bool multiPlay);

		void WriteSaveData(BinaryWriter writer);

		void ReadSaveDataBuffer(BinaryReader reader);
	}
}
