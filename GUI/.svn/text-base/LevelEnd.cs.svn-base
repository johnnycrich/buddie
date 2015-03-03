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
    public class LevelEnd : GUIBitmap, IGUIScreen
    {
        InputMap levelEndInputMap;

        //======================================================
        #region Constructors
        public LevelEnd()
        {

            GUIBitmapStyle backgroundStyle = new GUIBitmapStyle();
            backgroundStyle.SizeToBitmap = false;
            backgroundStyle.PreserveAspectRatio = true;
            //backgroundStyle.FadeInSec = 1.0f;
            //backgroundStyle.FadeWaitSec = 15.0f;
            //backgroundStyle.FadeOutSec = 1.0f;

            this.Name = "LevelEnd";
            this.Style = backgroundStyle;
            this.Bitmap = @"data\images\gui\level_complete";
            this.Size = new Vector2(1280.0f, 720.0f);

            levelEndInputMap = new InputMap();

            levelEndInputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.A, _sendMain);
            levelEndInputMap.BindAction(Game.Instance._keyboardID, (int)Microsoft.Xna.Framework.Input.Keys.Space, _sendMain);

            InputManager.Instance.PushInputMap(levelEndInputMap);

        }

        private void _sendMain(float val)
        {
            Game.Instance.ExitLevel();
        }
        #endregion

        //======================================================
        #region Private, protected, internal fields

        LevelOverlay _levelOverlay = null;

        #endregion;
    }
}
