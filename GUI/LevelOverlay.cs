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
    /// Displays the tutorial overlay
    /// </summary>
    public class LevelOverlay : GUIControl, IGUIScreen
    {
        //======================================================
        #region Constructors
        public LevelOverlay(bool paused)
        {
            GUIStyle baseStyle = new GUIStyle();
            baseStyle.Focusable = true;
            GUIBitmapStyle guiBitmapStyle = new GUIBitmapStyle();
            guiBitmapStyle.SizeToBitmap = true;

            this.Name = "LevelOverlay";
            this.Layer = 49;
            this.FocusOnWake = false;
            this.Visible = true;
            this.Style = baseStyle;
            this.Position = new Vector2(0.0f, 0.0f);
            this.Size = new Vector2(1280.0f, 720.0f);

            GUIBitmap guiBitmap = new GUIBitmap();
            guiBitmap.Style = guiBitmapStyle;
            guiBitmap.HorizSizing = HorizSizing.Center;
            guiBitmap.VertSizing = VertSizing.Center;
            guiBitmap.Folder = this;

            if (paused)
            {
                string guiSetPath = null;

                if (Game.platformFlag || (!Game.platformFlag && Game.controllerFlag))
                {
                    guiSetPath = "pad";
                }
                else
                {
                    guiSetPath = "keyboard";
                }

                if (Game.Instance._currentScene != "main")
                {
                    guiBitmap.Bitmap = @"data\images\gui\level_paused_" + guiSetPath;
                }
                else
                {
                    guiBitmap.Bitmap = @"data\images\gui\main_paused_" + guiSetPath;
                }
            }
            else
            {
                guiBitmap.Bitmap = @"data\images\gui\tutorial\level_overlay";
            }
        }
        #endregion
    }
}