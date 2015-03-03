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
using GarageGames.Torque.T2D;
using GarageGames.Torque.MathUtil;
#endregion
namespace BuddieMain.GUI
{
    /// <summary>
    /// Displays the splash for game start
    /// </summary>
    public class BootSplash3 : GUISplash, IGUIScreen
    {
        //======================================================
        #region Constructors
        public BootSplash3()
        {
            GUISplashStyle backgroundStyle = new GUISplashStyle();

            backgroundStyle.PreserveAspectRatio = true;
            backgroundStyle.FadeInSec = 0.5f;
            backgroundStyle.FadeWaitSec = 3.0f;
            backgroundStyle.FadeOutSec = 0.5f;
            backgroundStyle.Bitmap = @"data\images\gui\intro\3";

            this.Name = "LevelIntro";
            this.Style = backgroundStyle;
            this.Size = new Vector2(1280.0f, 720.0f);
            this.OnFadeFinished = OnSplashFinished;
        }
        #endregion

        //======================================================
        #region Public methods

        public void OnSplashFinished()
        {
            Game.Instance._bootSplash4 = new BootSplash4();
            GUICanvas.Instance.PopDialogControl(Game.Instance._bootSplash3);
            GUICanvas.Instance.PushDialogControl(Game.Instance._bootSplash4, 53);
        }

        #endregion
    }
}
