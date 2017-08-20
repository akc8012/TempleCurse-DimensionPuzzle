using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
	public static SoundManager instance = null;

	public AudioSource fxSource;
	public AudioSource musicSource;
	public AudioSource rumbleSource;
	public AudioSource ladderSource;

	public AudioClip jumpSound;
	public AudioClip collectSound;
	public AudioClip collectGemSound;
	public AudioClip collectCPSound;
	public AudioClip hurtSound;
	public AudioClip scoreTally;
	public AudioClip tick;
	public AudioClip click;
	public AudioClip woosh;
	public AudioClip landing;
	public AudioClip panelStep;
	public AudioClip snapWoosh;
	public AudioClip loseGuy;
	public AudioClip crank;
	public AudioClip lowBounce;
	public AudioClip highBounce;
	public AudioClip ceilingSlide;

	bool sfxMuted = false;
	bool musicMuted = false;
	public bool SfxMuted { get { return sfxMuted; } }
	public bool MusicMuted { get { return musicMuted; } }
	enum RepeatSounds { scoreTally };
	enum CanPlayState { no, limbo, yes };
	CanPlayState[] canPlayClips = new CanPlayState[] { CanPlayState.yes, CanPlayState.yes, CanPlayState.yes };
	float rumbleFadeSpeed = 1.25f;
	float musicFadeSpeed = 0.75f;
	bool rumbleCanFadeIn = true;
	bool rumbleCanFadeOut = false;
	bool musicCanFadeIn = true;
	bool musicCanFadeOut = false;
	bool canUseKeys = true;

	void Awake()
	{
		if (instance == null)
			instance = this;

		else if (instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}

	void OnLevelWasLoaded(int level)
	{
		if (level == 10)
			StartCoroutine("FadeOutMusic");
	}

	void Update()
	{
		if (canUseKeys)
		{
			if (Input.GetKeyDown(KeyCode.M))
				MuteMusic(!musicSource.mute);

			if (Input.GetKeyDown(KeyCode.N))
				MuteSound(!fxSource.mute);
		}
	}

	public void PlaySound(AudioClip clip, bool repeatClip = false)
	{
		if (repeatClip)
		{
			int i = GetRepeatSoundIndex(clip);

			if (canPlayClips[i] == CanPlayState.no)
				return;
			else
				StartCoroutine(WaitForClipEnd(clip, i));
		}

		fxSource.PlayOneShot(clip);
	}

	IEnumerator WaitForClipEnd(AudioClip clip, int i)
	{
		if (canPlayClips[i] == CanPlayState.limbo)
		{
			canPlayClips[i] = CanPlayState.no;
			yield break;
		}

		canPlayClips[i] = CanPlayState.no;
		yield return new WaitForSeconds(clip.length - 0.03f);
		canPlayClips[i] = CanPlayState.yes;
	}

	public void StopSound(AudioClip clip)
	{
		int i = GetRepeatSoundIndex(clip);

		if (canPlayClips[i] == CanPlayState.yes)
			return;
		
		clip.UnloadAudioData();
		clip.LoadAudioData();
		StopCoroutine("WaitForClipEnd");
		canPlayClips[i] = CanPlayState.limbo;
	}

	public void MuteSound(bool enable)
	{
		fxSource.mute = enable;
		sfxMuted = enable;
	}

	public void PlayClickThenMute()
	{
		// fxSource.PlayOneShot(click);		the input module plays click before we mute
		StartCoroutine("WaitThenMute");
	}

	IEnumerator WaitThenMute()
	{
		yield return new WaitForSeconds(click.length);
		MuteSoundSecretly(true);
	}

	public void MuteSoundSecretly(bool enable)		// mute or unmute the sound, BUT DON'T TELL ANYONE ABOUT IT
	{
		if (sfxMuted)		// lol if we actually are muted, we don't wanna unmute
			return;

		fxSource.mute = enable;
		// sfxMuted = enable;
	}

	public void MuteMusic(bool enable)
	{
		musicSource.mute = enable;
		musicMuted = enable;
	}

	int GetRepeatSoundIndex(AudioClip clip)
	{
		int index = (int)System.Enum.Parse(typeof(RepeatSounds), clip.name);
		return index;
	}

	public IEnumerator FadeInMusic()
	{
		if (!musicCanFadeIn)
			yield break;

		musicCanFadeIn = false;
		musicCanFadeOut = true;    // make sure that before we end, we'll still be able to fade out
		StopCoroutine("FadeOutMusic");
		while (musicSource.volume < 0.45f)
		{
			musicSource.volume += musicFadeSpeed * Time.deltaTime;
			yield return null;
		}
		musicSource.volume = 0.5f;
	}

	public IEnumerator FadeOutMusic()
	{
		if (!musicCanFadeOut)
			yield break;

		musicCanFadeOut = false;
		StopCoroutine("FadeInMusic");
		while (musicSource.volume > 0.15f)
		{
			musicSource.volume -= musicFadeSpeed * Time.deltaTime;
			yield return null;
		}
		musicSource.volume = 0;
		musicCanFadeIn = true;
	}

	public IEnumerator FadeInRumble()
	{
		if (!rumbleCanFadeIn || fxSource.mute)
			yield break;

		rumbleCanFadeIn = false;
		rumbleCanFadeOut = true;	// make sure that before we end, we'll still be able to fade out
		StopCoroutine("FadeOutRumble");
		while (rumbleSource.volume < 0.95f)
		{
			rumbleSource.volume += rumbleFadeSpeed * Time.deltaTime;
			yield return null;
		}
		rumbleSource.volume = 1;
	}

	public IEnumerator FadeOutRumble()
	{
		if (!rumbleCanFadeOut)
			yield break;

		rumbleCanFadeOut = false;
		StopCoroutine("FadeInRumble");
		while (rumbleSource.volume > 0.15f)
		{
			rumbleSource.volume -= rumbleFadeSpeed * Time.deltaTime;
			yield return null;
		}
		rumbleSource.volume = 0;
		rumbleCanFadeIn = true;
	}

	public void PlayLadderSound()
	{
		if (sfxMuted) return;

		ladderSource.volume = 0.5f;
	}

	public void StopLadderSound()
	{
		ladderSource.volume = 0;
	}

	public void SetCanUseKeys(bool enable)
	{
		canUseKeys = enable;
	}
}