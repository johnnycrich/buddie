#region Using Statements
using System;
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
    /// Displays the tutorial dialog for level 1
    /// </summary>
    public class Level1Intro : GUIControl, IGUIScreen
    {
        //======================================================
        #region Constructors
        public Level1Intro()
        {
            int keyboardId = InputManager.Instance.FindDevice("keyboard");

            GUIStyle baseStyle = new GUIStyle();
            baseStyle.Focusable = true;
          
            introGUIStyle = new GUIBitmapStyle();
            introGUIStyle.SizeToBitmap = true;

            this.Name = "Level1Intro";
            this.Layer = 50;
            this.FocusOnWake = true;
            this.Visible = true;
            this.Style = baseStyle;
            this.Size = new Vector2(1280.0f, 720.0f);

            introInputMap = new InputMap();
            introInputMap.BindAction(Game.Instance.gamepadId, (int)XGamePadDevice.GamePadObjects.A, Advance);
            introInputMap.BindAction(Game.Instance.keyboardId, (int)Microsoft.Xna.Framework.Input.Keys.Right, Advance);
            InputManager.Instance.PushInputMap(introInputMap);

            Game._audioHandler.LoadSounds("l1_intro");
            Game._audioHandler.PlayDialogue("l1_intro", 03);
        }
        #endregion

        //======================================================
        #region Public properties, operators, constants, and enums

        public int currentStep;
        
        public void SetStep()
        {
            _currentStep++;

            if (_currentStep == 1)
            {
                GUIBitmap introGUI1 = new GUIBitmap();
                introGUI1.Style = introGUIStyle;
                introGUI1.HorizSizing = HorizSizing.Relative;
                introGUI1.VertSizing = VertSizing.Relative;
                introGUI1.Bitmap = @"data\images\gui\tutorial\level1\intro_1_pad";
                introGUI1.Position = new Vector2(50.0f, 50.0f);
                introGUI1.Folder = this;                
            }
            else if(_currentStep == 2)
            {
                GUIBitmap introGUI2 = new GUIBitmap();
                introGUI2.Style = introGUIStyle;
                introGUI2.HorizSizing = HorizSizing.Relative;
                introGUI2.VertSizing = VertSizing.Relative;
                introGUI2.Bitmap = @"data\images\gui\tutorial\level1\intro_2_pad";
                introGUI2.Position = new Vector2(200.0f, 340.0f);
                introGUI2.Folder = this;
            }
            else if (_currentStep == 3)
            {
                GUIBitmap introGUI3 = new GUIBitmap();
                introGUI3.Style = introGUIStyle;
                introGUI3.HorizSizing = HorizSizing.Relative;
                introGUI3.VertSizing = VertSizing.Relative;
                introGUI3.Bitmap = @"data\images\gui\tutorial\level1\intro_3_pad";
                introGUI3.Position = new Vector2(975.0f, 140.0f);
                introGUI3.Folder = this;

                GUIBitmap introGUI4 = new GUIBitmap();
                introGUI4.Style = introGUIStyle;
                introGUI4.HorizSizing = HorizSizing.Relative;
                introGUI4.VertSizing = VertSizing.Relative;
                introGUI4.Bitmap = @"data\images\gui\continue_button";
                introGUI4.Position = new Vector2(1025.0f, 305.0f);
                introGUI4.Folder = this;
              /*   
                CellCountDivider div = new CellCountDivider();
                div.CellCountX = 5;
                div.CellCountY = 16;

                testAni = new SimpleMaterial();
                testAni.TextureFilename = "data/images/gui/continue_button.png";
                testAni.IsTranslucent = false;
                testAni.IsAdditive = false;
                testAni.TextureDivider = div as TextureDivider;

                T2DAnimationData aniData = new T2DAnimationData();
                aniData.Material = testAni;
                aniData.AnimationFrames = "1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32 33 34 35 36 37 38 39 40 41 42 43 44 45 46 47 48 49 50 51 52 53 54 55 56 57 58 59 60 61 62 63 64 65 66 67 68 69 70 71 72 73 74 75 76 77 78 79";
                aniData.AnimationDuration = 3.3332f;
                aniData.AnimationCycle = true;

                T2DAnimatedSprite aniSprite = new T2DAnimatedSprite();
                aniSprite.AnimationData = aniData;
                aniSprite.Size = new Vector2(96.0f, 98.0f);
                aniSprite.Position = new Vector2(100.0f, 100.0f);
                aniSprite.PlayOnLoad = true;
                aniSprite.StartFrame = 0;
                aniSprite.Layer = 30;
                aniSprite.Visible = true; */
            }
            else if (_currentStep == 4)
            {
                InputManager.Instance.PopInputMap(introInputMap);
                Game.Instance.UnloadIntro(1);
                
            }
            }
        #endregion

        //======================================================
        #region Private, protected, internal methods

        private void Advance(float val)
        {
            if (val > 0.0f)
            {
                SetStep();
            }
        }

        #endregion

        //======================================================
        #region Private, protected, internal fields

        InputMap introInputMap;
        GUIBitmapStyle introGUIStyle;
        int _currentStep = 0;

        SimpleMaterial testAni;

        #endregion
    }
}