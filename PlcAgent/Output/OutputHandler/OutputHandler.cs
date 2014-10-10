﻿using System;
using System.Threading;
using System.Windows;
using _PlcAgent.DataAquisition;
using _PlcAgent.General;
using _PlcAgent.Log;

namespace _PlcAgent.Output.OutputHandler
{
    public class OutputHandler : ExecutiveModule
    {
        #region Variables

        private OutputWriter _outputWriter;
        private readonly Thread _communicationThread;

        #endregion


        #region Properties

        public OutputWriter OutputWriter
        {
            get { return _outputWriter; }
            set { _outputWriter = value; }
        }

        public OutputHandlerFile OutputHandlerFile { get; set; }
        public OutputHandlerInterfaceAssignmentFile OutputHandlerInterfaceAssignmentFile { get; set; }

        #endregion


        #region Constructors

        public OutputHandler(uint id, string name, CommunicationInterfaceHandler communicationInterfaceHandler, OutputHandlerFile outputHandlerFile, OutputHandlerInterfaceAssignmentFile outputHandlerInterfaceAssignmentFile)
            : base(id, name)
        {
            CommunicationInterfaceHandler = communicationInterfaceHandler;
            OutputHandlerFile = outputHandlerFile;
            OutputHandlerInterfaceAssignmentFile = outputHandlerInterfaceAssignmentFile;

            _communicationThread = new Thread(OutputCommunicationThread);
            _communicationThread.SetApartmentState(ApartmentState.STA);
            _communicationThread.IsBackground = true;

            if (OutputHandlerInterfaceAssignmentFile.Assignment == null) OutputHandlerInterfaceAssignmentFile.Assignment = new string[9][];
            Assignment = OutputHandlerInterfaceAssignmentFile.Assignment[Header.Id];
            CreateInterfaceAssignment();
        }

        #endregion


        #region Methods

        public override void Initialize()
        {
            _communicationThread.Start();
            Logger.Log("ID: " + Header.Id + " Output Handler Initialized");
        }

        public override void Deinitialize()
        {
            _communicationThread.Abort();
            Logger.Log("ID: " + Header.Id + " Output Handler Deinitialized");
        }

        public void CreateOutput()
        {
            var fileName = OutputHandlerFile.Default.FileNameSuffixes[Header.Id];
            var directoryName = OutputHandlerFile.Default.DirectoryPaths[Header.Id];

            var interfaceVariable = fileName.Split('%');
            if (interfaceVariable.Length > 1)
            {
                var interfaceInputComponent = CommunicationInterfaceHandler.ReadInterfaceComposite.ReturnVariable(interfaceVariable[1]);
                if (interfaceInputComponent != null)
                {
                    if (interfaceInputComponent.TypeOfVariable == CommunicationInterfaceComponent.VariableType.String)
                    {
                        fileName = (string) interfaceInputComponent.Value;
                    }
                }
                var interfaceOutputComponent = CommunicationInterfaceHandler.WriteInterfaceComposite.ReturnVariable(interfaceVariable[1]);
                if (interfaceOutputComponent != null)
                {
                    if (interfaceOutputComponent.TypeOfVariable == CommunicationInterfaceComponent.VariableType.String)
                    {
                        fileName = (string) interfaceOutputComponent.Value;
                    }
                }
            }

            if (_outputWriter == null) return;
            try{
                _outputWriter.CreateOutput(fileName, directoryName,
                    _outputWriter.InterfaceToStrings(CommunicationInterfaceHandler.ReadInterfaceComposite,
                        OutputHandlerFile.Default.StartAddress[Header.Id],
                        OutputHandlerFile.Default.EndAddress[Header.Id]));
            }
            catch (Exception)
            {
                MessageBox.Show("ID: " + Header.Id + " : Output File creation Failed!", "Error");
                Logger.Log("ID: " + Header.Id + " : Output File creation Failed");
            }
        }

        #endregion


        #region Background methods

        private void OutputCommunicationThread()
        {
            Int16 counter = 0;
            Int16 caseAuxiliary = 0;

            while (_communicationThread.IsAlive)
            {
                PcControlModeChangeAllowed = false;

                Int16 antwort;
                Int16 status;

                if (!PcControlMode && CheckInterface())
                {
                    var inputCompositeCommand = (Int16)CommunicationInterfaceHandler.ReadInterfaceComposite.ReturnVariable(InterfaceAssignmentCollection.GetAssignment("Command")).Value;

                    switch (inputCompositeCommand)
                    {
                        case 100:
                            if (caseAuxiliary != 100)
                            {
                                Logger.Log("ID: " + Header.Id + " : Output file creation requested from PLC");
                                CreateOutput();
                            }
                            caseAuxiliary = 100;
                            antwort = 100;
                            status = 100;
                            break;
                        default:
                            caseAuxiliary = 0;
                            antwort = 0;
                            status = 0;
                            break;
                    }
                }
                else
                {
                    antwort = 999;
                    status = 999;
                    PcControlModeChangeAllowed = true;
                }

                if (CommunicationInterfaceHandler.WriteInterfaceComposite != null && CheckInterface())
                {
                    CommunicationInterfaceHandler.WriteInterfaceComposite.ModifyValue(InterfaceAssignmentCollection.GetAssignment("Life Counter"), counter);
                    counter++;
                    CommunicationInterfaceHandler.WriteInterfaceComposite.ModifyValue(InterfaceAssignmentCollection.GetAssignment("Reply"), antwort);
                    CommunicationInterfaceHandler.WriteInterfaceComposite.ModifyValue(InterfaceAssignmentCollection.GetAssignment("Status"), status);
                }
                Thread.Sleep(200);
            }
        }

        #endregion


        #region Auxiliaries

        public class OutputHandlerException : ApplicationException
        {
            public OutputHandlerException(string info) : base(info) { }
        }

        protected override void AssignmentFileUpdate()
        {
            OutputHandlerInterfaceAssignmentFile.Assignment[Header.Id] = Assignment;
            OutputHandlerInterfaceAssignmentFile.Save();
        }

        protected override sealed void CreateInterfaceAssignment()
        {
            if (Assignment.Length == 0) Assignment = new string[4];
            InterfaceAssignmentCollection = new InterfaceAssignmentCollection();

            InterfaceAssignmentCollection.Add(0, "Command", CommunicationInterfaceComponent.VariableType.Integer, InterfaceAssignment.Direction.In, Assignment);
            InterfaceAssignmentCollection.Add(1, "Life Counter", CommunicationInterfaceComponent.VariableType.Integer, InterfaceAssignment.Direction.Out, Assignment);
            InterfaceAssignmentCollection.Add(2, "Reply", CommunicationInterfaceComponent.VariableType.Integer, InterfaceAssignment.Direction.Out, Assignment);
            InterfaceAssignmentCollection.Add(3, "Status", CommunicationInterfaceComponent.VariableType.Integer, InterfaceAssignment.Direction.Out, Assignment);
        }

        #endregion
   
    }
}
