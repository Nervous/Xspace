using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FMOD;


/* Interface entre le jeu et l'API Fmod */

namespace Xspace.Son
{
    public delegate void EndMusicEventHandler(object sender, EventArgs e);

    public static class AudioPlayer
    {
        private static FMOD.System system = null;

        private static Sound music = null;
        private static string currentMusicPath;
        private static Channel musicChannel = null;

        private static FMOD.CHANNEL_CALLBACK channelCallback;
        public static event EndMusicEventHandler EndMusic;

        private static void ErrCheck(RESULT result)
        {
            if (result != RESULT.OK)
            {
                // throw new ApplicationException("FMOD : " + result + " - " + Error.String(result));
            }
        }

        public static bool Initialize()
        {
            RESULT result;

            result = Factory.System_Create(ref system);
            ErrCheck(result);

            uint version = 0;

            result = system.getVersion(ref version);
            ErrCheck(result);

            if (version < VERSION.number)
            {
                throw new ApplicationException("Error! You are using an old version of FMOD " + version.ToString("X") + ". This program requires " + VERSION.number.ToString("X") + ".");
            }

            result = system.init(32, INITFLAGS.NORMAL, (IntPtr)null);
            ErrCheck(result);

            channelCallback = new CHANNEL_CALLBACK(OnEndMusic);

            return true;
        }

        public static void Deinitialize()
        {
            if (music != null)
            {
                music.release();
            }

            system.release();
        }

        public static void PlayMusic()
        {
            PlayMusic(currentMusicPath);
        }

        public static void PlayMusic(string path, bool paused = false)
        {
            bool isPlaying = false;
            RESULT result;

            if (musicChannel != null)
            {
                result = musicChannel.isPlaying(ref isPlaying);
            }
            else
            {
                isPlaying = false;
            }

            if (currentMusicPath == path && isPlaying)
            {
                return;
            }
            else if (currentMusicPath == path)
            {
                result = system.playSound(CHANNELINDEX.FREE, music, false, ref musicChannel);
                ErrCheck(result);
            }
            else
            {
                if (music != null)
                {
                    result = music.release();
                    ErrCheck(result);
                }

                result = system.createSound(path, MODE.SOFTWARE | MODE.CREATECOMPRESSEDSAMPLE | MODE.LOOP_OFF, ref music);
                ErrCheck(result);

                result = system.playSound(CHANNELINDEX.FREE, music, paused, ref musicChannel);
                ErrCheck(result);
                musicChannel.setCallback(channelCallback); //musicChannel.setCallback(FMOD.CHANNEL_CALLBACKTYPE.END, channelCallback, 0);

                currentMusicPath = path;
                
            }
        }

        public static RESULT OnEndMusic(IntPtr channelraw, CHANNEL_CALLBACKTYPE type, IntPtr commanddata1, IntPtr commanddata2)
        {
            if (EndMusic != null)
                EndMusic(currentMusicPath, new EventArgs());
            musicChannel = null;
            return RESULT.OK;
        }

        public static void StopMusic()
        {
            if (musicChannel != null)
            {
                RESULT result = musicChannel.stop();
                musicChannel = null;
                ErrCheck(result);
            }
        }

        public static void PauseMusic()
        {
            bool paused = false;
            if (musicChannel != null)
            {
                RESULT result = musicChannel.getPaused(ref paused);
                ErrCheck(result);
                result = musicChannel.setPaused(!paused);
                ErrCheck(result);
            }
        }

        public static void SetVolume(float volume)
        {
            if (musicChannel != null)
            {
                RESULT result = musicChannel.setVolume(volume);
                ErrCheck(result);
            }
        }

        public static void Update()
        {
            system.update();
        }

        public static float[] GetSpectrum(int nb_values)
        {
            float[] spectre = new float[nb_values];
            if (musicChannel != null)
            {
                RESULT result = musicChannel.getSpectrum(spectre, nb_values, 0, DSP_FFT_WINDOW.RECT);
                ErrCheck(result);
            }
            return spectre;
        }

        public static float GetMoySpectrum()
        {
            float[] a = GetSpectrum(1);
            return a[0];
        }

        public static float GetFreq()
        {
            float freq = 0;
            if (musicChannel != null)
            {
                RESULT result = musicChannel.getFrequency(ref freq);
                ErrCheck(result);
            }

            return freq;
        }
    }
}

