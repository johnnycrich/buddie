using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using GarageGames.Torque.Core;
using GarageGames.Torque.T2D;
using GarageGames.Torque.Sim;
using GarageGames.Torque.Platform;

namespace StarterGame2D
{
    [TorqueXmlSchemaType]
    [TorqueXmlSchemaDependency(Type = typeof(T2DPhysicsComponent))]
    public class ArrowMovementComponent : TorqueComponent, ITickObject
    {
        //======================================================
        #region Constructors
        #endregion

        //======================================================
        #region Public properties, operators, constants, and enums
        public int PlayerNumber
        {
            get { return _playerNumber; }
            set { _playerNumber = value; }
        }
        #endregion

        //======================================================
        #region Public Methods
        public void InterpolateTick(float k)
        {
        }

        public void ProcessTick(Move move, float elapsed)
        {
            if (move != null)
            {
                // set our test object's Velocity based on stick/keyboard input
                if (move.Buttons[0].Pushed)
                {
                    if (_kidNumber < 5)
                    {
                        _kidNumber = _kidNumber + 1;
                    }
                }
                else if (move.Buttons[1].Pushed)
                {
                    if (_kidNumber > 1)
                    {
                        _kidNumber = _kidNumber - 1;
                    }
                }
                _sceneObject.Mount(TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("kid"+_kidNumber), "bottom", new Vector2(0.0f, 0.4f), 0.0f, false);
                Console.WriteLine(_kidNumber);
                _sceneObject.Physics.VelocityX = 10.0f;
            }
        }
        #endregion

        //======================================================
        #region Private, protected, internal methods
        protected override bool _OnRegister(TorqueObject owner)
        {
            if (!base._OnRegister(owner) || !(Owner is T2DSceneObject))
                return false;

            // retain a reference to this component's owner object
            _sceneObject = owner as T2DSceneObject;
            _SetupInputMap(_sceneObject, _playerNumber, "gamepad" + _playerNumber, "keyboard");

            // tell the process list to notifiy us with ProcessTick and InterpolateTick events
            ProcessList.Instance.AddTickCallback(Owner, this);

            return true;
        }

        void _OnBackButton(float val)
        {
            if (val > 0.0f)
                Game.Instance.Exit();
        }


        private void _SetupInputMap(TorqueObject player, int playerIndex, String gamePad, String keyboard)
        {
            // Set player as the controllable object
            PlayerManager.Instance.GetPlayer(playerIndex).ControlObject = player;

            // Get input map for this player and configure it
            InputMap inputMap = PlayerManager.Instance.GetPlayer(playerIndex).InputMap;

            int gamepadId = InputManager.Instance.FindDevice(gamePad);
            if (gamepadId >= 0)
            {
                inputMap.BindMove(gamepadId, (int)XGamePadDevice.GamePadObjects.LeftThumbX, MoveMapTypes.StickAnalogHorizontal, 0);
                inputMap.BindMove(gamepadId, (int)XGamePadDevice.GamePadObjects.LeftThumbY, MoveMapTypes.StickAnalogVertical, 0);
                inputMap.BindAction(gamepadId, (int)XGamePadDevice.GamePadObjects.Back, _OnBackButton);
            }

            // keyboard controls
            int keyboardId = InputManager.Instance.FindDevice(keyboard);
            if (keyboardId >= 0)
            {
                inputMap.BindMove(keyboardId, (int)Keys.Right, MoveMapTypes.Button, 0);
                inputMap.BindMove(keyboardId, (int)Keys.Left, MoveMapTypes.Button, 1);
                inputMap.BindMove(keyboardId, (int)Keys.Up, MoveMapTypes.StickDigitalUp, 0);
                inputMap.BindMove(keyboardId, (int)Keys.Down, MoveMapTypes.StickDigitalDown, 0);
                // WASD
                inputMap.BindMove(keyboardId, (int)Keys.D, MoveMapTypes.StickDigitalRight, 0);
                inputMap.BindMove(keyboardId, (int)Keys.A, MoveMapTypes.StickDigitalLeft, 0);
                inputMap.BindMove(keyboardId, (int)Keys.W, MoveMapTypes.StickDigitalUp, 0);
                inputMap.BindMove(keyboardId, (int)Keys.S, MoveMapTypes.StickDigitalDown, 0);
            }

            // configure fire button to only fire on make
            PlayerManager.Instance.GetPlayer(0).MoveManager.ConfigureButtonRepeat(0, false);
            PlayerManager.Instance.GetPlayer(0).MoveManager.ConfigureButtonRepeat(1, false);
        }
        #endregion

        //======================================================
        #region Private, protected, internal fields
        T2DSceneObject _sceneObject;
        int _playerNumber = 0;
        int _kidNumber = 1;
        #endregion
    }
}
