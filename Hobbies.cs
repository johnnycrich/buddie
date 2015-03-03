using System;
using System.Collections.Generic;
using System.Text;

using GarageGames.Torque.Core;
using GarageGames.Torque.T2D;
using GarageGames.Torque.Sim;
using GarageGames.Torque.Platform;

namespace BuddieMain
{
    [TorqueXmlSchemaType]
    public class HobbiesComponent : TorqueComponent
    {

        //======================================================
        #region Static methods, fields, constructors
        public static HobbiesComponent Instance
        {
            get { return _hobbiesClass; }
        }
        #endregion

        //======================================================
        #region Public properties, operators, constants, and enums
        public List<string> _hobbiesMainArray = new List<string>();
        public T2DSceneObject _thoughtBubble;       
        #endregion

        //======================================================
        #region Private, protected, internal methods
        protected void Main()
                {
                    // Create static instance.  
                    _hobbiesClass = new HobbiesComponent();
                    _thoughtBubble = TorqueObjectDatabase.Instance.FindObject<T2DSceneObject>("bubble");
                    Instance._OnRegister();
                }

        private void _OnRegister()
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        //======================================================
        #region Private, protected, internal fields
        static HobbiesComponent _hobbiesClass;
        #endregion
    }
}
