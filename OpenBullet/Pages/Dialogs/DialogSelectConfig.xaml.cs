﻿using OpenBullet.ViewModels;
using RuriLib;
using RuriLib.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace OpenBullet
{
    /// <summary>
    /// Logica di interazione per DialogSelectConfig.xaml
    /// </summary>
    public partial class DialogSelectConfig : Page
    {
        private ObservableCollection<ConfigViewModel> configsList;
        public ConfigManagerViewModel vm = new ConfigManagerViewModel();
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;
        private object Caller { get; set; }
        public ConfigViewModel Current { get { return Globals.mainWindow.ConfigsPage.CurrentConfig; } set { Globals.mainWindow.ConfigsPage.CurrentConfig = value; vm.RefreshCurrent(); } }

        public DialogSelectConfig(object caller)
        {
            InitializeComponent();
            Caller = caller;
            DataContext = Globals.mainWindow.ConfigsPage.ConfigManagerPage.DataContext;
        }

        private void selectButton_Click(object sender, RoutedEventArgs e)
        {
            if (configsListView.SelectedItems.Count == 0) return;
            if (Caller.GetType() == typeof(Runner))
            {
                if (Globals.obSettings.General.LiveConfigUpdates) ((Runner)Caller).SetConfig(((ConfigViewModel)configsListView.SelectedItem).Config);
                else ((Runner)Caller).SetConfig(IOManager.CloneConfig(((ConfigViewModel)configsListView.SelectedItem).Config));
            }
            ((MainDialog)Parent).Close();
        }

        private void listViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                configsListView.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
            configsListView.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }

        private void ListViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            selectButton_Click(this, null);
        }

        private void ListViewItem_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                selectButton_Click(this, null);
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            configsList = new ObservableCollection<ConfigViewModel>(vm.ConfigsList.Where(c => c.Name.ToLower().Contains(searchBox.Text.ToLower())));
            configsListView.ItemsSource = configsList;
        }
    }
}