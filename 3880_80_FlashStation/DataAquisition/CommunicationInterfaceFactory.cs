﻿using System;
using System.Collections;

namespace _3880_80_FlashStation.DataAquisition
{
    static public class CommunicationInterfaceFactory
    {
        public static CommunicationInterfaceVariable CreateVariable(string name, int pos, CommunicationInterfaceComponent.VariableType type)
        {
            switch (type)
            {
                case CommunicationInterfaceComponent.VariableType.BitArray:
                    return new CiBitArray(name, pos, type, new BitArray(2));
                case CommunicationInterfaceComponent.VariableType.Integer:
                    return new CiInteger(name, pos, type, 0);
                case CommunicationInterfaceComponent.VariableType.DoubleInteger:
                    return new CiDoubleInteger(name, pos, type, 0);
                case CommunicationInterfaceComponent.VariableType.Real:
                    return new CiReal(name, pos, type, 0.0f);
                case CommunicationInterfaceComponent.VariableType.String:
                    return new CiString(name, pos, type, "", 0);
                default:
                    throw new FactoryException("Cannot create a variable");
            }
        }

        public static CommunicationInterfaceVariable CreateVariable(string name, int pos, int posBit, CommunicationInterfaceComponent.VariableType type)
        {
            switch (type)
            {
                case CommunicationInterfaceComponent.VariableType.BitArray:
                    return new CiSingleBit(name, pos, posBit, type, false);
                default:
                    throw new FactoryException("Cannot create a variable");
            }
        }

        #region Auxiliaries

        public class FactoryException : ApplicationException
        {
            public FactoryException(string info) : base(info) { }
        }

        #endregion
    }
}
