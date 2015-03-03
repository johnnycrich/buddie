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
    public class BootSplash4 : GUISplash, IGUIScreen
    {
        //======================================================
        #region Constructors
        public BootSplash4()
        {
            GUISplashStyle backgroundStyle = new GUISplashStyle();

            backgroundStyle.PreserveAspectRatio = true;
            backgroundStyle.FadeInSec = 0.5f;
            backgroundStyle.FadeWaitSec = 2.0f;
            backgroundStyle.FadeOutSec = 0.5f;
            backgroundStyle.Bitmap = @"data\images\gui\intro\4";

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
            GUICanvas.Instance.PopDialogControl(this);
        }

        #endregion
    }
}
