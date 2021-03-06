﻿using System.Windows;
using System.Windows.Controls;

namespace _PlcAgent.Visual
{
    /// <summary>
    /// Interaction logic for ComponentCreationWindowTreeView.xaml
    /// </summary>
    public partial class ComponentCreationWindowTreeView
    {
        public ComponentCreationWindowTreeView()
        {
            InitializeComponent();
        }

        private void ComponentManagerSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var treeView = (TreeView)sender;
            var selection = (TreeViewItem)treeView.SelectedItem;
            if (selection != null) ComponentManagerSelectionLabel.Content = selection.Header;
        }
    }
}
