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
#endregion

namespace BuddieMain.GUI
{
    /// <summary>
    /// Displays the splash for transitions
    /// </summary>
    public class Transition : GUIControl, IGUIScreen
    {
        //======================================================
        #region Constructors
        public Transition()
        {
            this.Name = "Transition";
            this.Size = new Vector2(1280.0f, 720.0f);

            GUIStyle _baseStyle = new GUIStyle();
            _baseStyle.Focusable = true;

            _transitionGUIStyle = new GUIBitmapStyle();
            _transitionGUIStyle.SizeToBitmap = true;

            this.Layer = 50;
            this.FocusOnWake = true;
            this.Visible = true;
            this.Style = _baseStyle;

            GUIBitmap _transitionBitmap;

            _transitionBitmap = new GUIBitmap();
            _transitionBitmap.Style = _transitionGUIStyle;
            _transitionBitmap.HorizSizing = HorizSizing.Relative;
            _transitionBitmap.VertSizing = VertSizing.Relative;

            _transitionBitmap.Position = Vector2.Zero;
            _transitionBitmap.Folder = this;

            _transitionBitmap.Bitmap = @"data\images\gui\level_transition";

            //Game.Instance.LoadScenePostTransition();
        }
        #endregion

        //======================================================
        #region Public methods

        public void EndLevelTransition()
        {
            GUICanvas.Instance.PopDialogControl(this);
        }

        #endregion

        //======================================================
        #region Private, protected, internal fields

        GUIBitmapStyle _transitionGUIStyle;

        #endregion
    }
}
