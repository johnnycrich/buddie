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
#endregion

namespace BuddieMain.GUI
{
    public class PlayView : GUISceneview, IGUIScreen
    {
        #region Constructors
        public PlayView()
        {
            int _keyboardID = InputManager.Instance.FindDevice("keyboard");

            GUIStyle playStyle = new GUIStyle();
            playStyle.PreserveAspectRatio = true;

            this.Style = playStyle;
            this.Size = new Vector2(1280.0f, 720.0f);
            this.Name = "PlayView";
        }
        #endregion
    }
}
