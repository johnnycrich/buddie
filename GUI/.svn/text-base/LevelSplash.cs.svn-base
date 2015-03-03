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
    /// Displays the splash for levels
    /// </summary>
    public class LevelSplash : GUISplash, IGUIScreen
    {
        //======================================================
        #region Constructors
        public LevelSplash()
        {
            GUISplashStyle backgroundStyle = new GUISplashStyle();
            
            backgroundStyle.PreserveAspectRatio = true;
            backgroundStyle.FadeInSec = 1.0f;
            backgroundStyle.FadeWaitSec = 4.0f;
            backgroundStyle.FadeOutSec = 1.0f;
            backgroundStyle.Bitmap = @"data\images\gui\level" + Game._currentLevel + "_splash";

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
            if (Game._currentLevel == 1)
            {
                Game.Instance.Level1.Initialize();
            }
            else if (Game._currentLevel == 2)
            {
                if (Game.Instance.Level2._initialized == false)
                {
                    Game.Instance.Level2.Initialize();
                }
            }
            else
            {
                if (Game.Instance.Level3._initialized == false)
                {
                    Game.Instance.Level3.Initialize();
                }
            }
            GUICanvas.Instance.PopDialogControl(this);
        }

        #endregion
    }
}
