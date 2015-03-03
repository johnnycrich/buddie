#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace BuddieMain.Logic
{
    public class Audio
    {
        public AudioEngine _sfxAudioEngine;
        WaveBank _sfxWaveBank;
        SoundBank _sfxSoundBank;

        public AudioEngine _currentAudioEngine;
        WaveBank _currentWaveBank;
        SoundBank _currentSoundBank;

        public Cue _dialogueCue;
        public Cue _musicCue;
        Cue _sfxCue;

        string inputSetPath;
        
        public Audio()
        {
            // Initialize audio objects
            _sfxAudioEngine = new AudioEngine("data\\sound\\sfx.xgs");
            _sfxWaveBank = new WaveBank(_sfxAudioEngine, "data\\sound\\sfx_wave_bank.xwb");
            _sfxSoundBank = new SoundBank(_sfxAudioEngine, "data\\sound\\sfx_sound_bank.xsb");

            // Get global controller flag for tutorials
            if (Game.controllerFlag == true)
            {
                inputSetPath = "pad";
            }
            else
            {
                inputSetPath = "keyboard";
            }
        }

        public void LoadSounds(string scene)
        {
            _currentAudioEngine = new AudioEngine("data\\sound\\" + scene + ".xgs");
            _currentWaveBank = new WaveBank(_currentAudioEngine, "data\\sound\\" + scene + "_wave_bank.xwb");
            _currentSoundBank = new SoundBank(_currentAudioEngine, "data\\sound\\" + scene + "_sound_bank.xsb");

            _musicCue = _currentSoundBank.GetCue("music");
            _musicCue.Play();
        }

        public void PlayDialogue(bool intro, int cue)
        {
            string cueName = null;

            if (intro == true)
            {
                cueName = "tutorial_" + cue + "_" + inputSetPath;
            }
            else
            {
                cueName = cue.ToString();
            }

            if (_dialogueCue != null)
            {
                _dialogueCue.Stop(AudioStopOptions.Immediate);
            }
            
            _dialogueCue = _currentSoundBank.GetCue(cueName);
            _dialogueCue.Play();
        }

        public void PlaySFX(int cue)
        {
            string cueName = null;
            
            switch (cue)
            {
                case 1:
                    cueName = "arrow_move";
                    break;
                case 2:
                    cueName = "select";
                    break;
                case 3:
                    cueName = "switch";
                    break;
                case 4:
                    cueName = "bell";
                    break;
                case 5:
                    cueName = "oil";
                    break;
                case 6:
                    cueName = "chop";
                    break;
                case 7:
                    cueName = "meat";
                    break;
            }

            _sfxCue = _sfxSoundBank.GetCue(cueName);
            _sfxCue.Play();
        }

        public void KillSounds(bool killMusic)
        {
            if (killMusic == true)
            {
                if (_musicCue != null)
                {
                   _musicCue.Stop(AudioStopOptions.Immediate);
                    _musicCue = null;
                }
            }

            if (_dialogueCue != null)
            {
                _dialogueCue.Stop(AudioStopOptions.Immediate);
                _dialogueCue = null;
            }
        }
    }
}
