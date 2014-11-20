﻿using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using _PlcAgent.General;
using _PlcAgent.Vector;

namespace _PlcAgent.Visual.Gui.Vector
{
    /// <summary>
    /// Interaction logic for GuiVFlashPathBank.xaml
    /// </summary>
    public partial class GuiVFlashPathBank
    {
        private readonly VFlashTypeBank _vFlashTypeBank;
        private readonly VFlashTypeBankFile _vFlashTypeBankFile;

        public GuiVFlashPathBank(VFlashTypeBank vFlashTypeBank)
        {
            _vFlashTypeBank = vFlashTypeBank;
            _vFlashTypeBankFile = _vFlashTypeBank.VFlashTypeBankFile;

            InitializeComponent();

            VersionDataGrid.ItemsSource = _vFlashTypeBank.Children;
            VersionDataGrid.Foreground = Brushes.Black;
        }

        public void UpdateSizes(double height, double width)
        {
            Height = height;
            Width = width;

            GeneralGrid.Height = height;
            GeneralGrid.Width = width;

            VersionDataGrid.Height = height - 30;
            VersionDataGrid.Width = 400;
            SequenceDataGrid.Height = height - 30;
            SequenceDataGrid.Width = Limiter.DoubleLimit(width - VersionDataGrid.Width - 4, 0);
        }

        private void TypeCreation(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".vflashpack",
                Filter = "Flash Path (.vflashpack)|*.vflashpack"
            };

            var result = dlg.ShowDialog();
            if (result != true) return;
            _vFlashTypeBank.Add(new VFlashTypeBank.VFlashTypeComponent(TypeVersionBox.Text));
            UpdateVFlashProjectCollection();
        }

        private void UpdateVFlashProjectCollection()
        {
            /*_vFlashTypeBankFile.TypeBank[_vFlashTypeBank.Header.Id] = VFlashTypeBank.VFlashTypeConverter.VFlashTypesToStrings(_vFlashTypeBank.Children);
            _vFlashTypeBankFile.Save();

            _vFlashProjectCollection.Clear();
            foreach (var type in _vFlashTypeBank.Children.Cast<VFlashTypeBank.VFlashTypeComponent>())
            {
                _vFlashProjectCollection.Add(new VFlashTypeBank.VFlashDisplayProjectData
                {
                    Type = type.Type,
                    Version = type.Version,
                    Path = type.Path
                });
            }*/
        }

        private void VFlashProjectbankListViewSelection(object sender, SelectionChangedEventArgs e)
        {
            var listView = (ListView)sender;
            /*var projectdata = (VFlashTypeBank.VFlashDisplayProjectData)listView.SelectedItem;
            if (projectdata != null) TypeNumberBox.Text = projectdata.Type.ToString(CultureInfo.InvariantCulture);
            if (projectdata != null) TypeVersionBox.Text = projectdata.Version;*/
        }
    }
}
