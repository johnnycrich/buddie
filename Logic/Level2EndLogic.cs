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

namespace BuddieMain.Logic
{
    public class Level2EndLogic
    {
        #region Variable Definitions
        public bool _initialized = false;
        bool _restart = true;
        bool _levelLost = false;
        float gameTimeElapsed;
        int[] clothesSort;

        TorqueSafePtr<T2DSceneObject> bubble;
        TorqueSafePtr<T2DSceneObject> select_arrow;
        TorqueSafePtr<T2DSceneObject> select_yes;
        TorqueSafePtr<T2DSceneObject> select_no;
        TorqueSafePtr<T2DSceneObject> speech_01;
        TorqueSafePtr<T2DSceneObject> speech_02;
        TorqueSafePtr<T2DSceneObject> speech_03;
        TorqueSafePtr<T2DSceneObject> speech_04;

        TorqueSafePtr<T2DAnimatedSprite> _shirts;
        TorqueSafePtr<T2DAnimatedSprite> _pants;
        TorqueSafePtr<T2DAnimatedSprite> _shoes;

        Stopwatch stopWatch;
        Vector2 _speechPos;
        #endregion

        public Level2EndLogic()
        {
            _initialized = true;

            clothesSort = Game.Instance._clothesSort;

            bubble.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("speech_bubble");

            _shirts.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("shirts");
            _pants.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("pants");
            _shoes.Object = TorqueObjectDatabase.Instance.FindObject<T2DAnimatedSprite>("shoes");

            _speechPos = new Vector2(bubble.Object.Position.X, bubble.Object.Position.Y - 3);

            speech_01.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("speech_01");
            speech_02.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("speech_02");
            speech_03.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("speech_03");
            speech_04.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("speech_04");

            _shirts.Object.SetAnimationFrame((uint)clothesSort[0]);
            _pants.Object.SetAnimationFrame((uint)clothesSort[1]);
            _shoes.Object.SetAnimationFrame((uint)clothesSort[2]);

            _shirts.Object.Visible = Game.Instance._clothesShirt;
        }

        public void Initialize(bool lost)
        {
            if (lost == false)
            {
                speech_02.Object.Visible = true;
                speech_02.Object.Position = _speechPos;
                stopWatch = new Stopwatch();
                stopWatch.Start();
                Game._audioHandler.PlayDialogue(false, 8);
            }
            else
            {
                _levelLost = true;
                speech_01.Object.Visible = true;
                speech_01.Object.Position = _speechPos;
                Game._audioHandler.PlayDialogue(false, 7);

                select_arrow.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("select_arrow");
                select_yes.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("select_yes");
                select_no.Object = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("select_no");

                select_arrow.Object.Visible = true;
                select_yes.Object.Visible = true;
                select_no.Object.Visible = true;

                SetupInput();
            }
        }

        private void SetupInput()
        {
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

        public void Update(GameTime gameTime)
        {
            if (_levelLost == false)
            {
                double time = stopWatch.Elapsed.TotalSeconds;
                if (Math.Round(time, 1) == 7.0f)
                {
                    if (speech_02.Object != null)
                    {
                        speech_02.Object.MarkForDelete = true;
                    }

                    speech_03.Object.Visible = true;
                    speech_03.Object.Position = _speechPos;
                    Game._audioHandler.PlayDialogue(false, 9);
                }

                if (Math.Round(time, 1) == 10.0f)
                {
                    if (speech_03.Object != null)
                    {
                        speech_03.Object.MarkForDelete = true;
                    }
                    speech_04.Object.Visible = true;
                    speech_04.Object.Position = _speechPos;
                }

                if (Math.Round(time, 1) == 17.0f)
                {
                    Game.Instance._levelStartMount = 2;
                    Game.Instance.ExitLevel();
                }
            }
        }

        #region Button Listeners
        void PressRight(float val)
        {
            if (val > 0.0f)
            {
                if (_restart == true)
                {
                    select_arrow.Object.Mount(select_no, "mount", false);
                    _restart = false;
                }
            }
        }

        void PressLeft(float val)
        {
            if (val > 0.0f)
            {
                if (_restart == false)
                {
                    select_arrow.Object.Mount(select_yes, "mount", false);
                    _restart = true;
                }
            }
        }

        void PressSelect(float val)
        {
            if (val > 0.0f)
            {
                InputManager.Instance.PopInputMap(Game._globalInputMap);
                
                if (_restart == true)
                {    
                    Game.Instance._currentScene = "level2";
                    Game.Instance.LoadScene();
                }
                else
                {
                    Game.Instance.ExitLevel();
                }
            }
        }
        #endregion


    }
}
