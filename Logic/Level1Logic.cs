#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

using GarageGames.Torque.Core;
using GarageGames.Torque.Sim;
using GarageGames.Torque.T2D;
using GarageGames.Torque.Materials;
using GarageGames.Torque.Platform;
using GarageGames.Torque.GFX;
using GarageGames.Torque.GUI;


using BuddieMain.GUI;
#endregion

namespace BuddieMain.Logic
{
    [TorqueXmlSchemaType]
    public class Level1Logic
    {
        LevelSplash level1splash;

        KeyboardState keyState = Keyboard.GetState();

        TorqueSafePtr<T2DAnimatedSprite> buddie;
        TorqueSafePtr<T2DAnimatedSprite> selectArrow;
        TorqueSafePtr<T2DAnimatedSprite> leftArrow;
        TorqueSafePtr<T2DAnimatedSprite> rightArrow;

        TorqueSafePtr<T2DAnimatedSprite> pants;
        TorqueSafePtr<T2DAnimatedSprite> shoes;
        TorqueSafePtr<T2DAnimatedSprite> shirts;

        TorqueSafePtr<T2DSceneObject> mount_shoes;
        TorqueSafePtr<T2DSceneObject> mount_pants;
        TorqueSafePtr<T2DSceneObject> mount_shirts;
        TorqueSafePtr<T2DSceneObject> finish_ui;

        TorqueSafePtr<T2DSceneObject> submount_shirts_lt;
        TorqueSafePtr<T2DSceneObject> submount_pants_lt;
        TorqueSafePtr<T2DSceneObject> submount_shoes_lt;
        TorqueSafePtr<T2DSceneObject> submount_shirts_rt;
        TorqueSafePtr<T2DSceneObject> submount_pants_rt;
        TorqueSafePtr<T2DSceneObject> submount_shoes_rt;

        TorqueSafePtr<T2DSceneObject> finish_keyboard;
        TorqueSafePtr<T2DSceneObject> finish_pad;

        TorqueSafePtr<T2DSceneObject> speech_bubble;
        TorqueSafePtr<T2DSceneObject> speech_object;

        TorqueSafePtr<T2DSceneObject> level_overlay;

        Stopwatch dialogueWait;

        int _currentClothesType = 0;
        int _speechSection;
        float _blinkTimer = 0f;
        float _blinkTime = 0f;
        float _blinkDelay = 5000.0f;

        public bool _initialized = false;

        bool _clothesPicked = false;
        bool _shirtsPicked = false;
        bool _pantsPicked = false;
        bool _shoesPicked = false;
        bool _intro = false;
        bool _tutorial = false;
        bool _exiting = false;
        bool _exitNow = false;
        
        public Level1Logic()
        {
            level1splash = new LevelSplash();
            GUICanvas.Instance.PushDialogControl(level1splash, 51);
        }

        public void Initialize()
        {
            buddie.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("buddie");
            speech_bubble.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("speech_bubble");

            selectArrow.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("select");
            leftArrow.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("subselect_Left");
            rightArrow.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("subselect_Right");

            mount_shoes.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("mount_shoes");
            mount_pants.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("mount_pants");
            mount_shirts.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("mount_shirts");

            submount_shirts_lt.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("submount_shirts_lt");
            submount_pants_lt.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("submount_pants_lt");
            submount_shoes_lt.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("submount_shoes_lt");
            submount_shirts_rt.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("submount_shirts_rt");
            submount_pants_rt.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("submount_pants_rt");
            submount_shoes_rt.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("submount_shoes_rt");

            pants.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("pants");
            shoes.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("shoes");
            shirts.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("shirts");

            Game._audioHandler.LoadSounds("level1");

            // Mount selection arrows to initial mountees
            selectArrow.Object.Mount(mount_shirts, "mount", false);
            leftArrow.Object.Mount(submount_shirts_lt, "mount", false);
            rightArrow.Object.Mount(submount_shirts_rt, "mount", false);

            selectArrow.Object.Visible = false;
            leftArrow.Object.Visible = false;
            rightArrow.Object.Visible = false;

            pants.Object.Visible = false;
            shoes.Object.Visible = false;
            shirts.Object.Visible = false;

            Game._audioHandler.PlayDialogue(false, 20);
            _speechSection = 20;

            _initialized = true;
            _intro = true;
        }

        public void Start()
        {
            _tutorial = false;
            _intro = false;
            _initialized = true;

            finish_ui.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("finish_ui");

            level_overlay.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("level_overlay");

            if (Game.controllerFlag == true)
            {
                finish_pad.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("finish_pad");
                finish_pad.Object.Visible = true;
            }
            else
            {
                finish_keyboard.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("finish_keyboard");
                finish_keyboard.Object.Visible = true;
            }
        }

        private void Exit()
        {
            //Game.Instance.SaveUserData(1);
            if (!shirts.Object.Visible)
            {
                Game.Instance._clothesShirt = false;
            }
            Game.Instance._clothesSort = new int[3] { shirts.Object.CurrentFrame, pants.Object.CurrentFrame, shoes.Object.CurrentFrame };
            Game.Instance._levelStartMount = 1;
            Game.Instance.ExitLevel();
        }

        private void SetupInput()
        {
            Game._globalInputMap = new InputMap();

            Game._globalInputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.LeftThumbLeftButton, KeyLeftListen);
            Game._globalInputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.LeftThumbRightButton, KeyRightListen);
            Game._globalInputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.LeftThumbDownButton, KeyDownListen);
            Game._globalInputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.LeftThumbUpButton, KeyUpListen);
            Game._globalInputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.RightThumbLeftButton, KeyLeftListen);
            Game._globalInputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.RightThumbRightButton, KeyRightListen);
            Game._globalInputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.RightThumbDownButton, KeyDownListen);
            Game._globalInputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.RightThumbUpButton, KeyUpListen);
            Game._globalInputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.A, Finished);

            Game._globalInputMap.BindAction(Game.Instance._keyboardID, (int)Keys.Left, KeyLeftListen);
            Game._globalInputMap.BindAction(Game.Instance._keyboardID, (int)Keys.Right, KeyRightListen);
            Game._globalInputMap.BindAction(Game.Instance._keyboardID, (int)Keys.Down, KeyDownListen);
            Game._globalInputMap.BindAction(Game.Instance._keyboardID, (int)Keys.Up, KeyUpListen);
            Game._globalInputMap.BindAction(Game.Instance._keyboardID, (int)Keys.Enter, Finished);

            InputManager.Instance.PushInputMap(Game._globalInputMap);
        }

        private void DialogueLogic(int cue)
        {
            dialogueWait = new Stopwatch();
            dialogueWait.Start();

            while (dialogueWait.IsRunning)
            {
                if (dialogueWait.Elapsed.Seconds > 1)
                {
                    dialogueWait.Stop();

                    switch (cue)
                    {
                        case 15:
                            Game._audioHandler.PlayDialogue(false, 15);
                            _speechSection = 15;

                            _exitNow = true;
                            break;
                        case 16:
                            Game._audioHandler.PlayDialogue(_intro, 16);
                            _speechSection = 16;

                            _exiting = true;

                            level_overlay.Object.Visible = true;
                            selectArrow.Object.Visible = false;
                            leftArrow.Object.Visible = false;
                            rightArrow.Object.Visible = false;

                            if (Game.controllerFlag == true)
                            {
                                finish_pad.Object.Visible = false;
                            }
                            else
                            {
                                finish_keyboard.Object.Visible = false;
                            }
                            break;
                        case 17:
                            Game._audioHandler.PlayDialogue(false, 17);
                            _speechSection = 17;

                            _exitNow = true;
                            break;
                        case 18:
                            Game._audioHandler.PlayDialogue(false, 18);
                            _speechSection = 18;

                            _exitNow = true;
                            break;
                        case 19:
                            Game._audioHandler.PlayDialogue(false, 19);
                            _speechSection = 19;

                            _exitNow = true;
                            break;
                        case 20:
                            Exit();
                            break;
                        case 21:
                            Game._audioHandler.PlayDialogue(false, 21);
                            _speechSection = 21;
                            break;
                        case 22:
                            SetupInput();

                            Game.Instance.LoadIntro();

                            selectArrow.Object.Visible = true;
                            leftArrow.Object.Visible = true;
                            rightArrow.Object.Visible = true;

                            _initialized = false;
                            _tutorial = true;
                            break;
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            double time = gameTime.TotalRealTime.TotalSeconds;
            _blinkTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_blinkTimer > _blinkDelay)
            {
                buddie.Object.SetAnimationFrame(1);
                _blinkTime = _blinkTimer;
                _blinkTimer = 0f;
            }
            else
            {
                buddie.Object.SetAnimationFrame(0);
            }

            if (Game._audioHandler._dialogueCue != null)
            {
                if (Game._audioHandler._dialogueCue.IsPlaying == true)
                {
                    speech_object.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("speech_"+_speechSection);
                    if (speech_object.Object.Visible == true)
                    {
                        speech_object.Object.Position = new Vector2(speech_bubble.Object.Position.X, speech_bubble.Object.Position.Y - 2);
                    }
                    else
                    {
                        speech_object.Object.Visible = true;
                    }
                    speech_bubble.Object.Visible = true;
                }
                else
                {
                    speech_bubble.Object.Visible = false;
                    speech_object.Object.Visible = false;
                }
            }

            if(_exiting == true)
            {
                if (Game._audioHandler._dialogueCue.IsPlaying == false)
                {
                    if (_exitNow == true)
                    {
                        DialogueLogic(20);
                    }
                    else
                    {
                        if (shoes.Object.CurrentFrame == 1)
                        {
                            DialogueLogic(17);
                        }
                        else if (shirts.Object.CurrentFrame == 4)
                        {
                            DialogueLogic(18);
                        }
                        else if (_shirtsPicked == false)
                        {
                            DialogueLogic(15);   
                        }
                        else if (shirts.Object.CurrentFrame == 0 || shirts.Object.CurrentFrame == 1 || shirts.Object.CurrentFrame == 2)
                        {
                            DialogueLogic(19);
                        }
                        else
                        {
                            DialogueLogic(20);
                        }
                    }
                }
            }

            if (_intro == true)
            {
                if (Game._audioHandler._dialogueCue.IsPlaying == false)
                {
                    if (_speechSection == 20)
                    {
                        DialogueLogic(21);
                    }
                    else
                    {
                        DialogueLogic(22);
                    }
                }
            }
        }

        private void ClothesCycle(int type, string direction)
        {
            _clothesPicked = true;
            if (speech_object.Object != null)
            {
                speech_object.Object.Visible = false;
            }

            Game._audioHandler.PlaySFX(3);

            switch(type) {
                case 0:
                    _shirtsPicked = true;

                    if (direction == "left")
                    {
                        leftArrow.Object.SetAnimationFrame(1);

                        if (shirts.Object.Visible == false)
                        {
                            shirts.Object.SetAnimationFrame(6);
                            shirts.Object.Visible = true;
                        }
                        else
                        {

                            if (shirts.Object.CurrentFrame != shirts.Object.StartFrame)
                            {
                                if (shirts.Object.CurrentFrame >= 0)
                                {
                                    shirts.Object.SetAnimationFrame((uint)(shirts.Object.CurrentFrame - 1));
                                    
                                }
                            }
                            else
                            {
                                _shirtsPicked = false;
                                shirts.Object.Visible = false;
                            }
                        }
                    }
                    else
                    {
                        rightArrow.Object.SetAnimationFrame(1);

                        if (shirts.Object.Visible == false)
                        {
                            shirts.Object.SetAnimationFrame(0);
                            shirts.Object.Visible = true;
                        }
                        else
                        {
                            if (shirts.Object.CurrentFrame != shirts.Object.FinalFrame)
                            {
                                if (shirts.Object.CurrentFrame >= 0)
                                {
                                    shirts.Object.AdvanceFrame(); 
                                }
                            }
                            else
                            {
                                _shirtsPicked = false;
                                shirts.Object.Visible = false;
                            }
                        }


                    }

                    if (shirts.Object.Visible == true)
                    {
                        if (_tutorial == false)
                        {
                            Game._audioHandler.PlayDialogue(false, shirts.Object.CurrentFrame + 1);
                            _speechSection = shirts.Object.CurrentFrame + 1;
                        }
                    }
                break;
                case 1:
                    _pantsPicked = true;

                    if (direction == "left")
                    {
                        leftArrow.Object.SetAnimationFrame(1);

                        if (pants.Object.Visible == false)
                        {
                            pants.Object.SetAnimationFrame(3);
                            pants.Object.Visible = true;
                        }
                        else
                        {
                            if (pants.Object.CurrentFrame != pants.Object.StartFrame)
                            {
                                if (pants.Object.CurrentFrame >= 0)
                                {
                                    pants.Object.SetAnimationFrame((uint)(pants.Object.CurrentFrame - 1));
                                }
                            }
                            else
                            {
                                _pantsPicked = false;
                                pants.Object.Visible = false;
                            }
                        }
                    }
                    else
                    {
                        rightArrow.Object.SetAnimationFrame(1);

                        if (pants.Object.Visible == false)
                        {
                            pants.Object.SetAnimationFrame(0);
                            pants.Object.Visible = true;
                        }
                        else
                        {
                            if (pants.Object.CurrentFrame != pants.Object.FinalFrame)
                            {
                                if (pants.Object.CurrentFrame >= 0)
                                {
                                    pants.Object.SetAnimationFrame((uint)(pants.Object.CurrentFrame + 1));
                                }
                            }
                            else
                            {
                                _pantsPicked = false;
                                pants.Object.Visible = false;
                            }
                        }
                    }
                break;
            case 2:
                _shoesPicked = true;

                if (direction == "left")
                {
                    leftArrow.Object.SetAnimationFrame(1);

                    if (shoes.Object.Visible == false)
                    {
                        shoes.Object.SetAnimationFrame(2);
                        shoes.Object.Visible = true;
                    }
                    else
                    {
                        if (shoes.Object.CurrentFrame != shoes.Object.StartFrame)
                        {
                            if (shoes.Object.CurrentFrame >= 0)
                            {
                                shoes.Object.SetAnimationFrame((uint)(shoes.Object.CurrentFrame - 1));
                            }
                        }
                        else
                        {
                            _shoesPicked = false;
                            shoes.Object.Visible = false;
                        }
                    }
                }
                else
                {
                    rightArrow.Object.SetAnimationFrame(1);

                    if (shoes.Object.Visible == false)
                    {
                        shoes.Object.SetAnimationFrame(0);
                        shoes.Object.Visible = true;
                    }
                    else
                    {
                        if (shoes.Object.CurrentFrame != shoes.Object.FinalFrame)
                        {
                            if (shoes.Object.CurrentFrame >= 0)
                            {
                                shoes.Object.SetAnimationFrame((uint)(shoes.Object.CurrentFrame + 1));
                            }
                        }
                        else
                        {
                            _shoesPicked = false;
                            shoes.Object.Visible = false;
                        }
                    }
                }

                if (shoes.Object.Visible == true)
                {
                    if (_tutorial == false)
                    {
                        Game._audioHandler.PlayDialogue(false, shoes.Object.CurrentFrame + 9);
                        _speechSection = shoes.Object.CurrentFrame + 9;
                    }
                }

                break;
            }
        }

        private void ClothesPick(int current, string direction) {

            if (speech_object.Object != null)
            {
                speech_object.Object.Visible = false;
                speech_bubble.Object.Visible = false;
            }

            Game._audioHandler.KillSounds(false);
            
            switch(direction) {
                case "up":
                    if (_currentClothesType == 1)
                    {
                        selectArrow.Object.Mount(mount_shirts, "mount", false);
                        selectArrow.Object.Size = new Vector2(selectArrow.Object.Size.X * 1.2f, selectArrow.Object.Size.Y * 1.2f);
                        leftArrow.Object.Mount(submount_shirts_lt, "mount", false);
                        rightArrow.Object.Mount(submount_shirts_rt, "mount", false);

                        _currentClothesType = 0;

                        Game._audioHandler.PlaySFX(1);
                    }
                    else if (_currentClothesType == 2) {
                        selectArrow.Object.Mount(mount_pants, "mount", false);
                        selectArrow.Object.Size = new Vector2(selectArrow.Object.Size.X * 1.2f, selectArrow.Object.Size.Y * 1.2f);
                        leftArrow.Object.Mount(submount_pants_lt, "mount", false);
                        rightArrow.Object.Mount(submount_pants_rt, "mount", false);

                        _currentClothesType = 1;

                        Game._audioHandler.PlaySFX(1);
                        Game._audioHandler.PlayDialogue(false, 8);
                        _speechSection = 8;
                    }
                break;
                case "down":
                    if (_currentClothesType == 0)
                    {
                        selectArrow.Object.Mount(mount_pants, "mount", false);
                        selectArrow.Object.Size = new Vector2(selectArrow.Object.Size.X / 1.2f, selectArrow.Object.Size.Y / 1.2f);
                        leftArrow.Object.Mount(submount_pants_lt, "mount", false);
                        rightArrow.Object.Mount(submount_pants_rt, "mount", false);

                        _currentClothesType = 1;

                        Game._audioHandler.PlaySFX(1);
                        if (_tutorial == false)
                        {
                            Game._audioHandler.PlayDialogue(false, 8);
                            _speechSection = 8;
                        }
                    } else if (_currentClothesType == 1)
                    {
                        selectArrow.Object.Mount(mount_shoes, "mount", false);
                        selectArrow.Object.Size = new Vector2(selectArrow.Object.Size.X / 1.2f, selectArrow.Object.Size.Y / 1.2f);
                        leftArrow.Object.Mount(submount_shoes_lt, "mount", false);
                        rightArrow.Object.Mount(submount_shoes_rt, "mount", false);

                        _currentClothesType = 2;

                        Game._audioHandler.PlaySFX(1);
                    }
                break;
            }
        }

        #region Button Listeners
        void Finished(float val)
        {
            if (_speechSection != 20 && val > 0.0f)
            {
                if (_tutorial == false)
                {
                    if (speech_object.Object != null)
                    {
                        if (speech_object.Object.Visible == true)
                        {
                            speech_object.Object.Visible = false;
                            Game._audioHandler.KillSounds(false);
                        }
                    }

                    if (_clothesPicked == false)
                    {
                        Game._audioHandler.PlayDialogue(false, 12);
                        _speechSection = 12;
                    }
                    else if (_pantsPicked == false)
                    {
                        Game._audioHandler.PlayDialogue(false, 13);
                        _speechSection = 13;
                    }
                    else if (_shoesPicked == false)
                    {
                        Game._audioHandler.PlayDialogue(false, 14);
                        _speechSection = 14;
                    }
                    else
                    {
                        InputManager.Instance.PopInputMap(Game._globalInputMap);
                        DialogueLogic(16);
                    }
                }
                else
                {
                    Game.Instance._levelIntro.SetStep();
                }
            }
        }

        void KeyLeftListen(float val)
        {
            /* (
                 (Game.Instance._levelIntro != null && Game.Instance._levelIntro._initialized) ||
                 (Game.Instance._levelIntro == null)
                )
                &&
             */
            if (_speechSection != 20 && val > 0.0f)
            {
                ClothesCycle(_currentClothesType, "left");
                
                if (_tutorial == true)
                {
                    Game.Instance._levelIntro.SetStep();
                }
            }
            else
            {
                leftArrow.Object.SetAnimationFrame(0);
            }
        }

        void KeyRightListen(float val)
        {
            if (_speechSection != 20 && val > 0.0f)
            {
                ClothesCycle(_currentClothesType, "right");
                if (_tutorial == true)
                {
                    Game.Instance._levelIntro.SetStep();
                }
            }
            else
            {
                rightArrow.Object.SetAnimationFrame(0);
            }
        }

        void KeyDownListen(float val)
        {
            if (_speechSection != 20 && val > 0.0f)
            {
                ClothesPick(_currentClothesType, "down");

                if (_tutorial == true)
                {
                    Game.Instance._levelIntro.SetStep();
                }
            }
        }

        void KeyUpListen(float val)
        {
            if (_speechSection != 20 && val > 0.0f)
            {
                ClothesPick(_currentClothesType, "up");

                if (_tutorial == true)
                {
                    Game.Instance._levelIntro.SetStep();
                }
            }
        }
        #endregion   
       
    }
}