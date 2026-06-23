using UnityEngine;

namespace CyberGuardian
{
    public static class CyberGuardianRuntimeAudio
    {
        private const int SampleRate = 44100;

        private static AudioSource source;
        private static AudioClip buttonClickClip;
        private static AudioClip healthPowerClip;
        private static AudioClip energyPowerClip;
        private static AudioClip shieldPowerClip;
        private static AudioClip overclockPowerClip;

        public static void PlayButtonClick()
        {
            Play(GetButtonClickClip(), 0.72f);
        }

        public static void PlayPowerUp(CyberGuardianPowerUpType type)
        {
            switch (type)
            {
                case CyberGuardianPowerUpType.Health:
                    Play(GetHealthPowerClip(), 0.84f);
                    break;
                case CyberGuardianPowerUpType.Boost:
                    Play(GetEnergyPowerClip(), 0.82f);
                    break;
                case CyberGuardianPowerUpType.Firewall:
                    Play(GetShieldPowerClip(), 0.86f);
                    break;
                case CyberGuardianPowerUpType.Overclock:
                    Play(GetOverclockPowerClip(), 0.90f);
                    break;
                default:
                    Play(GetEnergyPowerClip(), 0.82f);
                    break;
            }
        }

        public static void PlayFallbackSfx()
        {
            Play(GetButtonClickClip(), 0.55f);
        }

        private static void Play(AudioClip clip, float volumeScale)
        {
            if (clip == null || !CyberGuardianMainMenu.IsSfxEnabled())
            {
                return;
            }

            AudioSource audioSource = GetSource();
            if (audioSource == null)
            {
                return;
            }

            float volume = Mathf.Clamp01(CyberGuardianMainMenu.GetSfxVolume() * Mathf.Clamp01(volumeScale));
            audioSource.mute = false;
            audioSource.volume = 1f;
            audioSource.PlayOneShot(clip, volume);
        }

        private static AudioSource GetSource()
        {
            if (source != null)
            {
                return source;
            }

            GameObject audioObject = new GameObject("Cyber Guardian Runtime Audio");
            Object.DontDestroyOnLoad(audioObject);
            source = audioObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
            source.spatialBlend = 0f;
            return source;
        }

        private static AudioClip GetButtonClickClip()
        {
            if (buttonClickClip == null)
            {
                buttonClickClip = CreateToneClip("CyberGuardian_UI_Click", 760f, 1240f, 0.07f, 0.55f, true);
            }

            return buttonClickClip;
        }

        private static AudioClip GetHealthPowerClip()
        {
            if (healthPowerClip == null)
            {
                healthPowerClip = CreateToneClip("CyberGuardian_Power_Health", 460f, 880f, 0.18f, 0.62f, false);
            }

            return healthPowerClip;
        }

        private static AudioClip GetEnergyPowerClip()
        {
            if (energyPowerClip == null)
            {
                energyPowerClip = CreateToneClip("CyberGuardian_Power_Energy", 680f, 1320f, 0.16f, 0.60f, false);
            }

            return energyPowerClip;
        }

        private static AudioClip GetShieldPowerClip()
        {
            if (shieldPowerClip == null)
            {
                shieldPowerClip = CreateToneClip("CyberGuardian_Power_Shield", 360f, 700f, 0.20f, 0.64f, false);
            }

            return shieldPowerClip;
        }

        private static AudioClip GetOverclockPowerClip()
        {
            if (overclockPowerClip == null)
            {
                overclockPowerClip = CreateToneClip("CyberGuardian_Power_Overclock", 720f, 1640f, 0.22f, 0.62f, true);
            }

            return overclockPowerClip;
        }

        private static AudioClip CreateToneClip(string name, float startFrequency, float endFrequency, float duration, float gain, bool digitalEdge)
        {
            int sampleCount = Mathf.Max(1, Mathf.RoundToInt(SampleRate * Mathf.Max(0.02f, duration)));
            float[] samples = new float[sampleCount];
            float phase = 0f;
            for (int i = 0; i < sampleCount; i++)
            {
                float t = sampleCount <= 1 ? 1f : i / (float)(sampleCount - 1);
                float frequency = Mathf.Lerp(startFrequency, endFrequency, t);
                phase += 2f * Mathf.PI * frequency / SampleRate;

                float attack = Mathf.Clamp01(t / 0.12f);
                float release = Mathf.Clamp01((1f - t) / 0.22f);
                float envelope = Mathf.SmoothStep(0f, 1f, attack) * Mathf.SmoothStep(0f, 1f, release);
                float wave = Mathf.Sin(phase);
                if (digitalEdge)
                {
                    wave = Mathf.Sign(wave) * 0.58f + Mathf.Sin(phase * 2.01f) * 0.18f;
                }
                else
                {
                    wave += Mathf.Sin(phase * 1.51f) * 0.18f;
                }

                samples[i] = Mathf.Clamp(wave * envelope * gain, -1f, 1f);
            }

            AudioClip clip = AudioClip.Create(name, sampleCount, 1, SampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}
