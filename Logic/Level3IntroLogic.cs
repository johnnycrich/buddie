using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using GarageGames.Torque.Core;
using GarageGames.Torque.Sim;
using GarageGames.Torque.T2D;
using GarageGames.Torque.Materials;
using GarageGames.Torque.Platform;
using GarageGames.Torque.GFX;
using GarageGames.Torque.GUI;

using BuddieMain.GUI;

namespace BuddieMain.Logic
{
    public class Level3IntroLogic
    {
        #region Variable Definitions
        public bool _initialized = false;
        bool _choiceState = false;
        int _speechSection;
        int _dialogueCue = 1;
        int[] clothesSort = Game.Instance._clothesSort;

        TorqueSafePtr<T2DSceneObject> speech_bubble;
        TorqueSafePtr<T2DSceneObject> speech_object;

        TorqueSafePtr<T2DSceneObject> select_yes;
        TorqueSafePtr<T2DSceneObject> select_no;
        TorqueSafePtr<T2DSceneObject> select_arrow;

        TorqueSafePtr<T2DSceneObject> level_overlay;
        TorqueSafePtr<T2DSceneObject> dad;

        TorqueSafePtr<T2DAnimatedSprite> _shirts;
        TorqueSafePtr<T2DAnimatedSprite> _pants;
        TorqueSafePtr<T2DAnimatedSprite> _shoes;

        Stopwatch dialogueWait;

        LevelSplash Level3Intro;
        #endregion
        
        public Level3IntroLogic()
        {
            Level3Intro = new LevelSplash();
            GUICanvas.Instance.PushDialogControl(Level3Intro, 51);
        }

        public void Initialize()
        {
            _initialized = true;

            Game._audioHandler.LoadSounds("level3_intro");

            _shirts.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("shirts");
            _pants.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("pants");
            _shoes.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("shoes");
            speech_bubble.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("speech_bubble");
            
            select_yes.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("select_yes");
            select_no.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("select_no");
            select_arrow.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("select_arrow");

            level_overlay.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("level_overlay");
            dad.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("dad");

            _speechSection = 1;
            
            Game._audioHandler.PlayDialogue(false, 1);
            
            _shirts.Object.SetAnimationFrame((uint)clothesSort[0]);
            _pants.Object.SetAnimationFrame((uint)clothesSort[1]);
            _shoes.Object.SetAnimationFrame((uint)clothesSort[2]);

            _shirts.Object.Visible = Game.Instance._clothesShirt;
        }

        private void SetupInput()
        {
            Game._globalInputMap = new InputMap();

            Game._globalInputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.LeftThumbLeftButton, PressLeft);
            Game._globalInputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.LeftThumbRightButton, PressRight);
            Game._globalInputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.RightThumbLeftButton, PressLeft);
            Game._globalInputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.RightThumbRightButton, PressRight);
            Game._globalInputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.A, PressSelect);

            // keyboard controls
            Game._globalInputMap.BindAction(Game.Instance._keyboardID, (int)Keys.Right, PressRight);
            Game._globalInputMap.BindAction(Game.Instance._keyboardID, (int)Keys.Left, PressLeft);
            Game._globalInputMap.BindAction(Game.Instance._keyboardID, (int)Keys.Space, PressSelect);
            
            InputManager.Instance.PushInputMap(Game._globalInputMap);
        }

        private void DialogueLogic()
        {
            dialogueWait = new Stopwatch();

            dialogueWait.Start();

            _dialogueCue++;

            while (dialogueWait.IsRunning)
            {
                if (dialogueWait.Elapsed.Seconds > 1)
                {
                    dialogueWait.Stop();

                    switch (_dialogueCue)
                    {
                        case 5:
                            speech_bubble.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("speech_bubble_dad");
                            
                            _speechSection = _dialogueCue;
                            Game._audioHandler.PlayDialogue(false, _dialogueCue);
                            break;
                        case 8:
                            SetupInput();

                            _speechSection = _dialogueCue;
                            Game._audioHandler.PlayDialogue(false, _dialogueCue);

                            _choiceState = true;
                            
                            dad.Object.Visible = true;
                            level_overlay.Object.Visible = true;
                            select_arrow.Object.Visible = true;
                            select_no.Object.Visible = true;
                            select_yes.Object.Visible = true;
                            break;
                        case 10:
                            _initialized = false;
                            // Stop level music
                            Game._audioHandler.KillSounds(true);

                            Game.Instance._currentScene = "level3_play";
                            Game.Instance.LoadScene();
                            break;
                        default:
                            _speechSection = _dialogueCue;
                            Game._audioHandler.PlayDialogue(false, _dialogueCue);
                            break;
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if (Game.Instance.Level3 != null && _initialized == true)
            {
                if (!_choiceState)
                {
                    if (Game._audioHandler._dialogueCue.IsPlaying == false)
                    {
                        speech_bubble.Object.Visible = false;
                        speech_object.Object.Visible = false;
                        DialogueLogic();
                    }
                }

                if (Game._audioHandler._dialogueCue != null)
                {
                    if (Game._audioHandler._dialogueCue.IsPlaying == true)
                    {
                        speech_object.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("speech_" + _speechSection);
                        if (speech_object.Object != null)
                        {
                            speech_object.Object.Position = new Vector2(speech_bubble.Object.Position.X, speech_bubble.Object.Position.Y - 2);
                            speech_object.Object.Visible = true;
                        }
                        if (speech_bubble.Object != null)
                        {
                            speech_bubble.Object.Visible = true;
                        }
                    }
                }
            }
        }

        #region Button Listeners
        void PressRight(float val)
        {
            if (val > 0.0f)
            {
               select_arrow.Object.Mount(select_no, "mount", false);
               Game.Instance._level3Veggie = false;
            }
        }

        void PressLeft(float val)
        {
            if (val > 0.0f)
            {
              select_arrow.Object.Mount(select_yes, "mount", false);
              Game.Instance._level3Veggie = true;
            }
        }

        void PressSelect(float val)
        {
            if (val > 0.0f)
            {
                InputManager.Instance.PopInputMap(Game._globalInputMap);

                _choiceState = false;

                dad.Object.Visible = false;
                level_overlay.Object.Visible = false;
                select_arrow.Object.Visible = false;
                select_no.Object.Visible = false;
                select_yes.Object.Visible = false;

                speech_bubble.Object.Visible = false;
                speech_object.Object.Visible = false;
            }
        }
        #endregion
    }
}
