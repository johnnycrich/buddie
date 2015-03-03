#region Using Statements
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

using GarageGames.Torque.Platform;
using GarageGames.Torque.Core;
using GarageGames.Torque.Sim;
using GarageGames.Torque.GUI;
using GarageGames.Torque.MathUtil;
using GarageGames.Torque.T2D;
using GarageGames.Torque.Materials;
#endregion

namespace BuddieMain.GUI
{
    /// <summary>
    /// Displays the tutorial dialogs
    /// </summary>
    public class LevelIntros : GUIControl, IGUIScreen
    {

        //======================================================
        #region Constructors
        public LevelIntros()
        {
            _level = Game._currentLevel;

            GUIStyle _baseStyle = new GUIStyle();
            _baseStyle.Focusable = true;
          
            _introGUIStyle = new GUIBitmapStyle();
            _introGUIStyle.SizeToBitmap = true;

            this.Name = "LevelIntro";
            this.Layer = 50;
            this.FocusOnWake = true;
            this.Visible = true;
            this.Style = _baseStyle;
            this.Size = new Vector2(1280.0f, 720.0f);

            if (Game.Instance._currentScene == "level3_play")
            {
                _guiPosition = new Vector2(50.0f, 300.0f);
            }

            else
            {
               _guiPosition = new Vector2(50.0f, 50.0f);
            }

            if (Game.controllerFlag == true)
            {
                _guiSetPath = "pad";
            }
            else
            {
                _guiSetPath = "keyboard";
            }

            _initialized = true;
        }
        #endregion

        //======================================================
        #region Public properties, functions

        public int currentStep;

        public void SetStep()
        {
            _currentStep++;

            switch (_currentStep)
            {
                case 1:

                   _introGUI1 = new GUIBitmap();
                   _introGUI1.Style = _introGUIStyle;
                   _introGUI1.HorizSizing = HorizSizing.Relative;
                   _introGUI1.VertSizing = VertSizing.Relative;

                   _introGUI1.Position = _guiPosition;
                   _introGUI1.Folder = this;

                   _introGUI1.Bitmap = @"data\images\gui\tutorial\level" + _level + "\\intro_1_" + _guiSetPath;

                    break;

                case 2:
 
                   _introGUI1.Visible = false;

                   _introGUI2 = new GUIBitmap();
                   _introGUI2.Style = _introGUIStyle;
                   _introGUI2.HorizSizing = HorizSizing.Relative;
                   _introGUI2.VertSizing = VertSizing.Relative;
                   
                   _introGUI2.Position = _guiPosition;
                   _introGUI2.Folder = this;

                   _introGUI2.Bitmap = @"data\images\gui\tutorial\level" + _level + "\\intro_2_" + _guiSetPath;                   

                    break;

                case 3:

                   _introGUI2.Visible = false;

                   _introGUI3 = new GUIBitmap();
                   _introGUI3.Style = _introGUIStyle;
                   _introGUI3.HorizSizing = HorizSizing.Relative;
                   _introGUI3.VertSizing = VertSizing.Relative;
                   
                   
                   _introGUI3.Position = _guiPosition;
                   _introGUI3.Folder = this;

                   _introGUI3.Bitmap = @"data\images\gui\tutorial\level" + _level + "\\intro_3_" + _guiSetPath;
                   
                   break;

                case 4:

                    Game.Instance.UnloadIntro(_level);
    
                    break;
            }

            if (_currentStep != 4)
            {
                Game._audioHandler.PlayDialogue(true, _currentStep);
            }
            
        }
        
        #endregion

        //======================================================
        #region Private, protected, internal fields

        GUIBitmapStyle _introGUIStyle;

        GUIBitmap _introGUI1;
        GUIBitmap _introGUI2;
        GUIBitmap _introGUI3;

        private int _currentStep = 0;
        private int _level;
        public bool _initialized = false;
        private Vector2 _guiPosition;
        private string _guiSetPath;

        #endregion
    }
}