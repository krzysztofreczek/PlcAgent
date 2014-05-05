﻿using System;
using System.Windows;
using System.Windows.Controls;

namespace _3880_80_FlashStation.Visual
{
    static class GuiFactory
    {
        #region Grid

        public static Grid CreateGrid()
        {
            return new Grid();
        }

        public static Grid CreateGrid(int xPosition, int yPosition, HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment)
        {
            return new Grid
            {
                HorizontalAlignment = horizontalAlignment,
                VerticalAlignment = verticalAlignment,
                Margin = new Thickness(xPosition, yPosition, 0, 0)
            };
        }

        public static Grid CreateGrid(int xPosition, int yPosition, HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment, int height)
        {
            return new Grid
            {
                HorizontalAlignment = horizontalAlignment,
                VerticalAlignment = verticalAlignment,
                Height = height,
                Margin = new Thickness(xPosition, yPosition, 0, 0)
            };
        }

        public static Grid CreateGrid(int xPosition, int yPosition, HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment, int height, int width)
        {
            return new Grid
            {
                HorizontalAlignment = horizontalAlignment,
                VerticalAlignment = verticalAlignment,
                Height = height,
                Width = width,
                Margin = new Thickness(xPosition, yPosition, 0, 0)
            };
        }

        #endregion

        #region GroupBox

        public static GroupBox CreateGroupBox(string header, int xPosition, int yPosition,
            HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment, int height, int width)
        {
            return new GroupBox
            {
                Header = header,
                HorizontalAlignment = horizontalAlignment,
                VerticalAlignment = verticalAlignment,
                Height = height,
                Width = width,
                Margin = new Thickness(xPosition, yPosition, 179, 0)
            };
        }

        #endregion

        #region Label

        public static Label CreateLabel(string content, int xPosition, int yPosition,
            HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment, int height, int width)
        {
            return new Label
            {
                Content = content,
                HorizontalAlignment = horizontalAlignment,
                VerticalAlignment = verticalAlignment,
                Height = height,
                Width = width,
                Margin = new Thickness(xPosition, yPosition, 0, 0)
            };
        }

        public static Label CreateLabel(string name, string content, int xPosition, int yPosition,
            HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment,
            HorizontalAlignment horizontalContentAlignment,
            int height, int width)
        {
            return new Label
            {
                Name = name,
                Content = content,
                HorizontalAlignment = horizontalAlignment,
                VerticalAlignment = verticalAlignment,
                HorizontalContentAlignment = horizontalContentAlignment,
                Height = height,
                Width = width,
                Margin = new Thickness(xPosition, yPosition, 0, 0)
            };
        }

        #endregion

        #region Button

        public static Button CreateButton(string name, string content, int xPosition, int yPosition,
            HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment,
            int height, int width, RoutedEventHandler click)
        {
            var button = new Button
            {
                Name = name,
                Content = content,
                HorizontalAlignment = horizontalAlignment,
                VerticalAlignment = verticalAlignment,
                Height = height,
                Width = width,
                Margin = new Thickness(xPosition, yPosition, 0, 0),
                ClickMode = ClickMode.Release
            };

            button.Click += click;
            return button;
        }

        #endregion

        #region CheckBox

        public static CheckBox CreateCheckBox(string name, string content, int xPosition, int yPosition,
            HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment,
            int width, RoutedEventHandler checkedUnchecked)
        {
            var checkBox = new CheckBox
            {
                Name = name,
                Content = content,
                HorizontalAlignment = horizontalAlignment,
                VerticalAlignment = verticalAlignment,
                Width = width,
                Margin = new Thickness(xPosition, yPosition, 5, 0),
                FlowDirection = FlowDirection.RightToLeft
            };

            checkBox.Checked += checkedUnchecked;
            checkBox.Unchecked += checkedUnchecked;

            return checkBox;
        }

        #endregion

    }
}
