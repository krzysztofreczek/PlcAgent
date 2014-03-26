﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace _3880_80_FlashStation.DataAquisition
{
    #region Component

    public abstract class CommunicationInterfaceComponent
    {
        private string _name;
        private int _pos;
        private string _type;

        public CommunicationInterfaceComponent(string name, int pos, string type)
        {
            _name = name;
            _pos = pos;
            _type = type;
        }

        public string Name
        {
            get { return _name; }
        }

        public int Pos
        {
            get { return _pos; }
        }

        public string Type
        {
            get { return _type; }
        }

        public abstract void Add(CommunicationInterfaceComponent c);
        public abstract void Remove(CommunicationInterfaceComponent c);
        public abstract void ReadValue(byte[] valByte);
        public abstract void WriteValue(byte[] valByte);
    }

    #endregion

    #region Composite

    public class CommunicationInterfaceComposite : CommunicationInterfaceComponent
    {
        private readonly List<CommunicationInterfaceComponent> _children = new List<CommunicationInterfaceComponent>();

        public List<CommunicationInterfaceComponent> Children
        {
            get { return _children; }
        }

        // Constructor
        public CommunicationInterfaceComposite(string name) : base(name, 0, "Composite")
        {
        }

        public override void Add(CommunicationInterfaceComponent component)
        {
            _children.Add(component);
        }

        public override void Remove(CommunicationInterfaceComponent component)
        {
            _children.Remove(component);
        }

        public override void ReadValue(byte[] valByte)
        {
            foreach (CommunicationInterfaceComponent component in _children)
            {
                component.ReadValue(valByte);
            }
        }

        public override void WriteValue(byte[] valByte)
        {
            foreach (CommunicationInterfaceComponent component in _children)
            {
                component.WriteValue(valByte);
            }
        }

        public CommunicationInterfaceVariable ReturnVariable(string name)
        {
            foreach (CommunicationInterfaceComponent component in _children)
            {
                if (component.Name == name)
                {
                    return (CommunicationInterfaceVariable) component;
                }
            }
            throw new CompositeException("Error: Variable not found");
        }
    }

    #endregion

    #region Variables

    public abstract class CommunicationInterfaceVariable : CommunicationInterfaceComponent
    {
        // Constructor
        public CommunicationInterfaceVariable(string name, int pos, string type)
            : base(name, pos, type)
        {
        }

        public override void Add(CommunicationInterfaceComponent c)
        {
            throw new CompositeException("Error: Cannot add to a single variable");
        }

        public override void Remove(CommunicationInterfaceComponent c)
        {
            throw new CompositeException("Error: Cannot remove from a single variable");
        }
    }

    public class CiBitArray : CommunicationInterfaceVariable
    {
        private BitArray _value;

        public BitArray Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public CiBitArray(string name, int pos, string type, BitArray value)
            : base(name, pos, type)
        {
            _value = value;
        }

        public override void ReadValue(byte[] valByte)
        {
            _value = DataMapper.Read16Bits(valByte, Pos);
        }

        public override void WriteValue(byte[] valByte)
        {
            DataMapper.Write16Bits(valByte, Pos, _value);
        }
    }

    public class CiInteger : CommunicationInterfaceVariable
    {
        private Int16 _value;

        public Int16 Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public CiInteger(string name, int pos, string type, Int16 value)
            : base(name, pos, type)
        {
            _value = value;
        }

        public override void ReadValue(byte[] valByte)
        {
            _value = DataMapper.ReadInteger(valByte, Pos);
        }

        public override void WriteValue(byte[] valByte)
        {
            DataMapper.WriteInteger(valByte, Pos, _value);
        }
    }

    public class CiDoubleInteger : CommunicationInterfaceVariable
    {
        private Int32 _value;

        public Int32 Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public CiDoubleInteger(string name, int pos, string type, Int32 value)
            : base(name, pos, type)
        {
            _value = value;
        }

        public override void ReadValue(byte[] valByte)
        {
            _value = DataMapper.ReadDInteger(valByte, Pos);
        }

        public override void WriteValue(byte[] valByte)
        {
            DataMapper.WriteDInteger(valByte, Pos, _value);
        }
    }

    public class CiReal : CommunicationInterfaceVariable
    {
        private float _value;

        public float Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public CiReal(string name, int pos, string type, float value)
            : base(name, pos, type)
        {
            _value = value;
        }

        public override void ReadValue(byte[] valByte)
        {
            _value = DataMapper.ReadReal(valByte, Pos);
        }

        public override void WriteValue(byte[] valByte)
        {
            DataMapper.WriteReal(valByte, Pos, _value);
        }
    }

    public class CiString : CommunicationInterfaceVariable
    {
        private string _value;
        private int _length;

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public CiString(string name, int pos, string type, string value, int length)
            : base(name, pos, type)
        {
            _value = value;
            _length = length;
        }

        public override void ReadValue(byte[] valByte)
        {
            _value = DataMapper.ReadString(valByte, Pos, _length);
        }

        public override void WriteValue(byte[] valByte)
        {
            DataMapper.WriteString(valByte, Pos, _value);
        }
    }

    #endregion

    #region Auxiliaries

    public class CompositeException : ApplicationException
    {
        public CompositeException(string info) : base(info) { }
    }

    #endregion
}
