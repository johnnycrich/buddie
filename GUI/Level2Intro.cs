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
    /// Displays the splash for level 2
    /// </summary>
    public class LevelSplash : GUISplash, IGUIScreen
    {
        //======================================================
        #region Constructors
            public LevelSplash()
            {
                int keyboardId = InputManager.Instance.FindDevice("keyboard");

                GUISplashStyle backgroundStyle = new GUISplashStyle();
                backgroundStyle.PreserveAspectRatio = true;
                backgroundStyle.FadeInSec = 1.0f;
                backgroundStyle.FadeWaitSec = 1.0f;
                backgroundStyle.FadeOutSec = 1.0f;
                backgroundStyle.Bitmap = @"data\images\gui\level2_splash";
                

                this.Name = "Level2Intro";
                this.Style = backgroundStyle;
                this.Size = new Vector2(1280.0f, 720.0f);

               // GUIBitmapStyle splashStyle = new GUIBitmapStyle();
               // splashStyle.SizeToBitmap = true;

              /*  GUIBitmap splashText = new GUIBitmap();
                splashText.Style = splashStyle;
                splashText.HorizSizing = HorizSizing.Relative;
                splashText.VertSizing = VertSizing.Relative;
                splashText.Bitmap = @"data\images\gui\level2_splash";
                splashText.Position = new Vector2(0.0f, 0.0f);
                splashText.Folder = this; */


                introInputMap = new InputMap();
                introInputMap.BindAction(Game.Instance.gamepadId, (int)XGamePadDevice.GamePadObjects.A, LoadLevel);
                introInputMap.BindAction(Game.Instance.keyboardId, (int)Microsoft.Xna.Framework.Input.Keys.Right, LoadLevel);

                InputManager.Instance.PushInputMap(introInputMap);
            }
        #endregion

        //======================================================
        #region Private, protected, internal methods

            private void LoadLevel(float val)
            {
                InputManager.Instance.PopInputMap(introInputMap);
                Game.Instance.LoadScene();
            }

        #endregion

        //======================================================
        #region Private, protected, internal fields

        InputMap introInputMap;

        #endregion
    }
}
