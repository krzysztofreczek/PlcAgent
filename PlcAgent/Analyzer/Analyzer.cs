﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using OxyPlot.Axes;
using _PlcAgent.DataAquisition;
using _PlcAgent.General;
using _PlcAgent.Log;
using _PlcAgent.Visual.Gui.Analyzer;

namespace _PlcAgent.Analyzer
{
    public class Analyzer : ExecutiveModule
    {
        #region Variables

        private Boolean _recording;

        private double _startRecordingTime;
        private double _recordingTime;
        private Visibility _dataCursorVisibility;

        private readonly Thread _analysisThread;
        private readonly Thread _communicationThread;

        #endregion


        #region Properties

        public CommunicationInterfaceHandler CommunicationInterfaceHandler { get; set; }
        public AnalyzerAssignmentFile AnalyzerAssignmentFile { get; set; }
        public AnalyzerSetupFile AnalyzerSetupFile { get; set; }
        public AnalyzerChannelList AnalyzerChannels { get; set; }

        public GuiAnalyzerDataCursor AnalyzerDataCursorRed;
        public GuiAnalyzerDataCursor AnalyzerDataCursorBlue;

        public AnalyzerDataCursorPointCollection AnalyzerDataCursorPointCollection;
        public AnalyzerCsvHandler AnalyzerCsvHandler;

        public bool Recording
        {
            get { return _recording; }
            private set
            {
                if (value == _recording) return;
                _recording = value;
                OnPropertyChanged();
            }
        }

        public double RecordingTime
        {
            get { return _recordingTime; }
            private set
            {
                if (Math.Abs(value - _recordingTime) < 1) return;
                _recordingTime = value;
                OnPropertyChanged();
            }
        }

        public double TimeRange
        {
            set
            {
                foreach (var analyzerChannel in AnalyzerChannels.Children.Where(analyzerChannel => analyzerChannel.AnalyzerObservableVariable != null))
                { analyzerChannel.AnalyzerObservableVariable.TimeRange = value; }
                TimeObservableVariable.TimeRange = value;
                if (OnUpdatePlotsDelegate != null) OnUpdatePlotsDelegate();
            }
        }

        public Visibility DataCursorsVisibility
        {
            get { return _dataCursorVisibility; }
            set
            {
                if (value == _dataCursorVisibility) return;
                _dataCursorVisibility = value;
                OnPropertyChanged();
            }
        }

        public AnalyzerObservableVariable TimeObservableVariable { get; set; }

        public delegate void UpdatePlotsDelegate();

        public UpdatePlotsDelegate OnUpdatePlotsDelegate;

        #endregion


        #region Constructors

        public Analyzer(uint id, string name, CommunicationInterfaceHandler communicationInterfaceHandler, AnalyzerAssignmentFile analyzerAssignmentFile, AnalyzerSetupFile analyzerSetupFile) : base(id, name)
        {
            PcControlModeChangeAllowed = true;
            PcControlMode = false;

            CommunicationInterfaceHandler = communicationInterfaceHandler;
            CommunicationInterfaceHandler.OnInterfaceUpdatedDelegate += Clear;

            AnalyzerAssignmentFile = analyzerAssignmentFile;
            AnalyzerSetupFile = analyzerSetupFile;

            TimeObservableVariable = new AnalyzerObservableVariable(this, new CiInteger("Time", 0, CommunicationInterfaceComponent.VariableType.Integer, 0))
            {
                MainViewModel = new TimeMainViewModel(),
                Brush = Brushes.Black
            };

            AnalyzerChannels = new AnalyzerChannelList(0, this);
            AnalyzerChannels.RetriveConfiguration();

            AnalyzerDataCursorRed = new GuiAnalyzerDataCursor(this)
            {
                Brush = Brushes.Red
            };
            AnalyzerDataCursorBlue = new GuiAnalyzerDataCursor(this)
            {
                Brush = Brushes.Blue
            };

            AnalyzerDataCursorPointCollection = new AnalyzerDataCursorPointCollection();

            TimeRange = AnalyzerSetupFile.TimeRange[Header.Id];
            DataCursorsVisibility = AnalyzerSetupFile.ShowDataCursors[Header.Id] ? Visibility.Visible : Visibility.Hidden;

            AnalyzerCsvHandler = new AnalyzerCsvHandler(this);

            _analysisThread = new Thread(AnalyzeThread);
            _analysisThread.SetApartmentState(ApartmentState.STA);
            _analysisThread.IsBackground = true;

            _communicationThread = new Thread(OutputCommunicationThread);
            _communicationThread.SetApartmentState(ApartmentState.STA);
            _communicationThread.IsBackground = true;

            CreateInterfaceAssignment(id, AnalyzerAssignmentFile.Assignment);
        }

        #endregion


        #region Methods

        public override void Initialize()
        {
            try { AnalyzerCsvHandler.RetrivePointsFromCsvFile(); }
            catch (Exception) { Logger.Log("ID: " + Header.Id + " Plot could not be retrieved"); }

            if (AnalyzerSetupFile.SampleTime[Header.Id] < 10) AnalyzerSetupFile.SampleTime[Header.Id] = 100;
            if (AnalyzerSetupFile.TimeRange[Header.Id] < 1000) AnalyzerSetupFile.TimeRange[Header.Id] = 10000;

            _analysisThread.Start();
            _communicationThread.Start();

            Logger.Log("ID: " + Header.Id + " Analyzer Initialized");
        }

        public override void Deinitialize()
        {
            Recording = false;
            Thread.Sleep(AnalyzerSetupFile.SampleTime[Header.Id]);

            _analysisThread.Abort();
            _communicationThread.Abort();

            Logger.Log("ID: " + Header.Id + " Analyzer Deinitialized");
        }

        public void StartStopRecording()
        {
            if (!Recording)
            {
                Clear();
                AnalyzerCsvHandler.InitCsvFile();
            }
            Recording = !Recording;
        }

        public void Clear()
        {
            AnalyzerChannels.Clear();
            TimeObservableVariable.Clear();

            RecordingTime = 0.0;
            _startRecordingTime = DateTime.Now.TimeOfDay.TotalMilliseconds;
            
            Logger.Log("ID: " + Header.Id + " Analysis cleared");

            AnalyzerSetupFile.Save();
        }

        public void AddNewChannel()
        {
            AnalyzerSetupFile.NumberOfChannels[Header.Id] += 1;

            uint id = 1;
            for (uint i = 1; i < AnalyzerSetupFile.NumberOfChannels[Header.Id]; i++)
            {
                if (AnalyzerChannels.GetChannel(i) != null) continue;
                id = i;
                break;
            }

            AnalyzerChannels.Add(new AnalyzerChannel(id, this));
            AnalyzerSetupFile.Save();
        }

        public void RemoveChannel(AnalyzerChannel analyzerChannel)
        {
            AnalyzerChannels.Remove(analyzerChannel);

            AnalyzerSetupFile.NumberOfChannels[Header.Id] -= 1;
            AnalyzerSetupFile.Save();
        }

        #endregion

        
        #region Background methods

        private void AnalyzeThread()
        {
            var lastMilliseconds = 0.0;

            while (_analysisThread.IsAlive)
            {
                if (Recording)
                {
                    var timeTick = TimeSpanAxis.ToDouble(DateTime.Now.TimeOfDay);

                    TimeObservableVariable.StoreActualValue(timeTick);
                    Parallel.ForEach(AnalyzerChannels.Children,
                        analyzerChannel =>
                    {
                        if (analyzerChannel.AnalyzerObservableVariable == null) return;
                        analyzerChannel.AnalyzerObservableVariable.StoreActualValue(timeTick);
                    });

                    AnalyzerCsvHandler.StorePointsInCsvFile();
                    RecordingTime = lastMilliseconds - _startRecordingTime;
                }

                var timeDifference = (int) (DateTime.Now.TimeOfDay.TotalMilliseconds - lastMilliseconds);
                if (timeDifference > AnalyzerSetupFile.SampleTime[Header.Id]) timeDifference = AnalyzerSetupFile.SampleTime[Header.Id];
                
                Thread.Sleep(AnalyzerSetupFile.SampleTime[Header.Id] - timeDifference);
                lastMilliseconds = DateTime.Now.TimeOfDay.TotalMilliseconds;
            }
        }

        private void OutputCommunicationThread()
        {
            Int16 counter = 0;
            Int16 caseAuxiliary = 0;

            while (_communicationThread.IsAlive)
            {
                Int16 antwort;
                Int16 status = 0;

                if (CheckInterface())
                {
                    var inputCompositeCommand = (Int16)CommunicationInterfaceHandler.ReadInterfaceComposite.ReturnVariable(InterfaceAssignmentCollection.GetAssignment("Command")).Value;

                    switch (inputCompositeCommand)
                    {
                        case 100:
                            if (caseAuxiliary != 100)
                            {
                                Logger.Log("ID: " + Header.Id + " : Start/stop analysis requested from PLC");
                                StartStopRecording();
                            }
                            caseAuxiliary = 100;
                            antwort = 100;
                            break;
                        default:
                            caseAuxiliary = 0;
                            antwort = 0;
                            break;
                    }

                    if (Recording) status = 100;
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

        public class AnalyzerException : ApplicationException
        {
            public AnalyzerException(string info) : base(info) { }
        }

        protected override Boolean CheckInterface()
        {
            CommunicationInterfaceComponent component = CommunicationInterfaceHandler.ReadInterfaceComposite.ReturnVariable(InterfaceAssignmentCollection.GetAssignment("Command"));
            if (component == null || component.Type != CommunicationInterfaceComponent.VariableType.Integer)
                return false;
            component = CommunicationInterfaceHandler.WriteInterfaceComposite.ReturnVariable(InterfaceAssignmentCollection.GetAssignment("Life Counter"));
            if (component == null || component.Type != CommunicationInterfaceComponent.VariableType.Integer)
                return false;
            component = CommunicationInterfaceHandler.WriteInterfaceComposite.ReturnVariable(InterfaceAssignmentCollection.GetAssignment("Reply"));
            if (component == null || component.Type != CommunicationInterfaceComponent.VariableType.Integer)
                return false;
            component = CommunicationInterfaceHandler.WriteInterfaceComposite.ReturnVariable(InterfaceAssignmentCollection.GetAssignment("Status"));
            if (component == null || component.Type != CommunicationInterfaceComponent.VariableType.Integer)
                return false;

            return true;
        }

        protected override sealed void CreateInterfaceAssignment(uint id, string[][] assignment)
        {
            if (assignment[id].Length == 0) assignment[id] = new string[4];

            InterfaceAssignmentCollection = new InterfaceAssignmentCollection();
            InterfaceAssignmentCollection.Children.Add(new InterfaceAssignment
            {
                VariableDirection = InterfaceAssignment.Direction.In,
                Name = "Command",
                Type = CommunicationInterfaceComponent.VariableType.Integer,
                Assignment = AnalyzerAssignmentFile.Assignment[id][0]
            });
            InterfaceAssignmentCollection.Children.Add(new InterfaceAssignment
            {
                VariableDirection = InterfaceAssignment.Direction.Out,
                Name = "Life Counter",
                Type = CommunicationInterfaceComponent.VariableType.Integer,
                Assignment = AnalyzerAssignmentFile.Assignment[id][1]
            });
            InterfaceAssignmentCollection.Children.Add(new InterfaceAssignment
            {
                VariableDirection = InterfaceAssignment.Direction.Out,
                Name = "Reply",
                Type = CommunicationInterfaceComponent.VariableType.Integer,
                Assignment = AnalyzerAssignmentFile.Assignment[id][2]
            });
            InterfaceAssignmentCollection.Children.Add(new InterfaceAssignment
            {
                VariableDirection = InterfaceAssignment.Direction.Out,
                Name = "Status",
                Type = CommunicationInterfaceComponent.VariableType.Integer,
                Assignment = AnalyzerAssignmentFile.Assignment[id][3]
            });
        }

        public override void UpdateAssignment()
        {
            AnalyzerAssignmentFile.Assignment[Header.Id][0] =
                InterfaceAssignmentCollection.GetAssignment("Command");
            AnalyzerAssignmentFile.Assignment[Header.Id][1] =
                InterfaceAssignmentCollection.GetAssignment("Life Counter");
            AnalyzerAssignmentFile.Assignment[Header.Id][2] =
                InterfaceAssignmentCollection.GetAssignment("Reply");
            AnalyzerAssignmentFile.Assignment[Header.Id][3] =
                InterfaceAssignmentCollection.GetAssignment("Status");
            AnalyzerAssignmentFile.Save();
        }

        #endregion
    }
}
