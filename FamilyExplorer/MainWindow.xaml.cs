﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FamilyExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private bool isMouseCaptured;
        private double mouseVerticalPosition;
        private double mouseHorizontalPosition;

        public Family family;        

        public MainWindow()
        {
            InitializeComponent();            
            family = new Family();
            this.DataContext = family;            
        }

        private void AddMother_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            family.AddMother_CanExecute(sender, e);
        }

        private void AddMother_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            family.AddMother_Executed(sender, e);
        }

        private void AddFather_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            family.AddFather_CanExecute(sender, e);
        }

        private void AddFather_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            family.AddFather_Executed(sender, e);
        }

        private void AddSiblingEqualParents_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            family.AddSiblingEqualParents_CanExecute(sender, e);
        }

        private void AddSiblingEqualParents_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            family.AddSiblingEqualParents_Executed(sender, e);
        }

        private void AddSiblingByMother_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            family.AddSiblingByMother_CanExecute(sender, e);
        }

        private void AddSiblingByMother_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            family.AddSiblingByMother_Executed(sender, e);
        }

        private void AddSiblingByFather_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            family.AddSiblingByFather_CanExecute(sender, e);
        }

        private void AddSiblingByFather_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            family.AddSiblingByFather_Executed(sender, e);
        }

        private void AddFriend_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            family.AddFriend_CanExecute(sender, e);
        }

        private void AddFriend_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            family.AddFriend_Executed(sender, e);
        }

        private void AddPartner_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            family.AddPartner_CanExecute(sender, e);
        }

        private void AddPartner_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            family.AddPartner_Executed(sender, e);
        }

        private void AddChild_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            family.AddChild_CanExecute(sender, e);
        }

        private void AddChild_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            family.AddChild_Executed(sender, e);
        }

        private void AddAbuser_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            family.AddAbuser_CanExecute(sender, e);
        }

        private void AddAbuser_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            family.AddAbuser_Executed(sender, e);
        }

        private void AddVictim_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            family.AddVictim_CanExecute(sender, e);
        }

        private void AddVictim_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            family.AddVictim_Executed(sender, e);
        }

        private void SetMother_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            family.SetMother_CanExecute(sender, e);
        }

        private void SetMother_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            family.SetMother_Executed(sender, e);
        }

        private void SetFather_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            family.SetFather_CanExecute(sender, e);
        }

        private void SetFather_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            family.SetFather_Executed(sender, e);
        }

        private void SetFriend_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            family.SetFriend_CanExecute(sender, e);
        }

        private void SetFriend_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            family.SetFriend_Executed(sender, e);
        }

        private void SetPartner_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            family.SetPartner_CanExecute(sender, e);
        }

        private void SetPartner_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            family.SetPartner_Executed(sender, e);
        }

        private void SetChild_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            family.SetChild_CanExecute(sender, e);
        }

        private void SetChild_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            family.SetChild_Executed(sender, e);
        }

        private void SetAbuser_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            family.SetAbuser_CanExecute(sender, e);
        }

        private void SetAbuser_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            family.SetAbuser_Executed(sender, e);
        }

        private void SetVictim_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            family.SetVictim_CanExecute(sender, e);
        }

        private void SetVictim_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            family.SetVictim_Executed(sender, e);
        }

        private void PersonItem_MouseEnter(object sender, MouseEventArgs e)
        {            
            family.EnterSetCommandRelation((Person)((FrameworkElement)sender).DataContext);
        }

        private void PersonItem_MouseLeave(object sender, MouseEventArgs e)
        {
            family.ExitSetCommandRelation();
        }

        private void PersonItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            family.FinalizeSetCommand((Person)((FrameworkElement)sender).DataContext);           
        }
       
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            family.EndSetCommand();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            family.EndSetCommand();
        }
       
        private void TreeCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            family.SetWindowSize(TreeCanvas.ActualWidth, TreeCanvas.ActualHeight);                      
        }
       
        private void FamilyTreeScrollViewer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ScrollViewer FamilyTreeScrollViewer = sender as ScrollViewer;
            mouseVerticalPosition = e.GetPosition(null).Y;
            mouseHorizontalPosition = e.GetPosition(null).X;
            isMouseCaptured = true;
            FamilyTreeScrollViewer.CaptureMouse();
        }

        private void FamilyTreeScrollViewer_MouseMove(object sender, MouseEventArgs e)
        {
            
            if (isMouseCaptured)
            {
                // Calculate distance moved
                double deltaX = e.GetPosition(null).X - mouseHorizontalPosition;
                double deltaY = e.GetPosition(null).Y - mouseVerticalPosition;
                // Move Tree
                family.MoveTreePositionInWindow(deltaX, deltaY);
                // Record current position
                mouseHorizontalPosition = e.GetPosition(null).X;
                mouseVerticalPosition = e.GetPosition(null).Y;
                
            }
            // Tooltip for set commands
            ToolTipPopup.HorizontalOffset = e.GetPosition(TreeCanvas).X + 20;
            ToolTipPopup.VerticalOffset = e.GetPosition(TreeCanvas).Y + 30;
        }

        private void FamilyTreeScrollViewer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ScrollViewer FamilyTreeScrollViewer = sender as ScrollViewer;
            isMouseCaptured = false;
            FamilyTreeScrollViewer.ReleaseMouseCapture();
            mouseVerticalPosition = -1;
            mouseHorizontalPosition = -1;
        }

        private void FamilyTreeScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //FamilyTreeListBoxScaleTransform.CenterX = e.GetPosition(null).X;
            //FamilyTreeListBoxScaleTransform.CenterY = e.GetPosition(null).Y;
           
            family.ScaleTree(Convert.ToDouble(e.Delta), e.GetPosition(TreeCanvas).X, e.GetPosition(TreeCanvas).Y);           
        }

      
    }
}
