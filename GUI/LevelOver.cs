#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GarageGames.Torque.Platform;
using GarageGames.Torque.Core;
using GarageGames.Torque.Sim;
using GarageGames.Torque.GUI;
using GarageGames.Torque.MathUtil;
using GarageGames.Torque.T2D;
#endregion

namespace BuddieMain.GUI
{
    /// <summary>
    /// Displays the splash ad for TankBuster, first GUI screen to become visible.
    /// </summary>
    public class LevelOver : GUIBitmap, IGUIScreen
    {
        //======================================================
        #region Constructors
            public LevelOver()
            {

                GUIBitmapStyle backgroundStyle = new GUIBitmapStyle();
                backgroundStyle.SizeToBitmap = false;
                backgroundStyle.PreserveAspectRatio = true;

                _btnImageStyle = new GUIBitmapStyle();
                _btnImageStyle.SizeToBitmap = false;

                this.Name = "LevelOver";
                this.Style = backgroundStyle;
                this.Bitmap = @"data\images\gui\main_bg";
                this.Size = new Vector2(1024.0f, 768.0f);

                // create an image with a picture of the gamepad's A button
                _btnImageA = new GUIBitmap();
                _btnImageA.Style = _btnImageStyle;
                _btnImageA.Bitmap = @"data\images\gui\level_lev1";
                _btnImageA.Position = new Vector2(25, 366);
                _btnImageA.Size = new Vector2(490, 61);
                _btnImageA.Visible = true;
                _btnImageA.Folder = this;

                // create an image with a picture of the gamepad's A button
                _btnImageX = new GUIBitmap();
                _btnImageX.Style = _btnImageStyle;
                _btnImageX.Bitmap = @"data\images\gui\level_lev2";
                _btnImageX.Position = new Vector2(25, 421);
                _btnImageX.Size = new Vector2(490, 61);
                _btnImageX.Visible = true;
                _btnImageX.Folder = this;

                mainMenuInputMap = new InputMap();

                mainMenuInputMap.BindAction(Game.Instance.gamepadId, (int)XGamePadDevice.GamePadObjects.A, _sendToLevel1);
                mainMenuInputMap.BindAction(Game.Instance.gamepadId, (int)XGamePadDevice.GamePadObjects.X, _sendToLevel2);
                mainMenuInputMap.BindAction(Game.Instance.gamepadId, (int)XGamePadDevice.GamePadObjects.Back, _mainMenuQuit);

                mainMenuInputMap.BindAction(Game.Instance.keyboardId, (int)Microsoft.Xna.Framework.Input.Keys.Left, _sendToLevel1);
                mainMenuInputMap.BindAction(Game.Instance.keyboardId, (int)Microsoft.Xna.Framework.Input.Keys.Right, _sendToLevel2);
                mainMenuInputMap.BindAction(Game.Instance.keyboardId, (int)Microsoft.Xna.Framework.Input.Keys.Escape, _mainMenuQuit);

                InputManager.Instance.PushInputMap(mainMenuInputMap);
            }
        #endregion

        //======================================================
        #region Private, protected, internal methods

            private void _sendToLevel1(float val)
            {
                InputManager.Instance.PopInputMap(mainMenuInputMap);
                Game.Instance._currentScene = "level1";
                Game.Instance.LoadScene();
            }

            private void _sendToLevel2(float val)
            {
                InputManager.Instance.PopInputMap(mainMenuInputMap);
                Game.Instance._currentScene = "level2";
                Game.Instance.LoadScene();
            }

            private void _mainMenuQuit(float val)
            {
                Game.Instance.Exit();
            }

        #endregion

        //======================================================
        #region Private, protected, internal fields

        InputMap mainMenuInputMap;

        GUIBitmapStyle _btnImageStyle = null;
        GUIBitmap _btnImageA = null;
        GUIBitmap _btnImageX = null;

        #endregion
    }
}
