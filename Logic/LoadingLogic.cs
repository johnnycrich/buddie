#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

using GarageGames.Torque.Core;
using GarageGames.Torque.Sim;
using GarageGames.Torque.T2D;
using GarageGames.Torque.Materials;
using GarageGames.Torque.Platform;
using GarageGames.Torque.GFX;
#endregion

namespace BuddieMain.Logic
{
    [TorqueXmlSchemaType]
    public class LoadingLogic
    {
        public LoadingLogic()
        {
            //Game._globalInputMap = new InputMap();
            //Game._globalInputMap.BindAction(Game.Instance._gamepadID, (int)XGamePadDevice.GamePadObjects.X, KeyUpListen);
            
            //Game._globalInputMap.BindAction(Game.Instance._keyboardID, (int)Keys.Delete, KeyUpListen);

            //InputManager.Instance.PushInputMap(Game._globalInputMap);
        }

        void KeyUpListen(float val)
        {
            if (val > 0.0f)
            {
                Game.Instance._loadingNewScene = true;
                Game.Instance.SceneLoader.UnloadLastScene();
            }
        }
    }
}