// Copyright 2014 #jacksoftszone
// Licensed under GPLv3
// Refer to the LICENSE.txt file included.

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Shapes;

namespace LiteralWpf.Controls {
    public class FramelessWindow : Window {

        private HwndSource _hwndSource;

        protected override void OnInitialized(EventArgs e) {
            SourceInitialized += OnSourceInitialized;
            base.OnInitialized(e);
        }

        private void OnSourceInitialized(object sender, EventArgs e) {
            _hwndSource = (HwndSource)PresentationSource.FromVisual(this);
        }

        static FramelessWindow() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FramelessWindow), new FrameworkPropertyMetadata(typeof(FramelessWindow)));
        }
        public FramelessWindow()
            : base() {
            PreviewMouseMove += OnPreviewMouseMove;
        }

        protected void OnPreviewMouseMove(object sender, MouseEventArgs e) {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
                Cursor = Cursors.Arrow;
        }

        protected void Minimize(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        protected void Restore(object sender, RoutedEventArgs e) {
            WindowState = (WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
        }

        protected void Close(object sender, RoutedEventArgs e) {
            Close();
        }

        public override void OnApplyTemplate() {
            Button minimizeButton = GetTemplateChild("tbar_minimize") as Button;
            if (minimizeButton != null) {
                minimizeButton.Click += Minimize;
            }

            Button restoreButton = GetTemplateChild("tbar_restore") as Button;
            if (restoreButton != null) {
                restoreButton.Click += Restore;
            }

            Button closeButton = GetTemplateChild("tbar_close") as Button;
            if (closeButton != null) {
                closeButton.Click += Close;
            }

            Rectangle dragzone = GetTemplateChild("dragzone") as Rectangle;
            if (dragzone != null) {
                dragzone.PreviewMouseDown += DownDrag;
            }

            Grid resizeGrid = GetTemplateChild("resizeGrid") as Grid;
            if (resizeGrid != null) {
                foreach (UIElement element in resizeGrid.Children) {
                    Rectangle resizeRectangle = element as Rectangle;
                    if (resizeRectangle != null) {
                        resizeRectangle.PreviewMouseDown += ResizeRectangle_PreviewMouseDown;
                        resizeRectangle.MouseMove += ResizeMouseMove;
                    }
                }
            }

            base.OnApplyTemplate();
        }

        private void DownDrag(object sender, MouseButtonEventArgs e) {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        protected void ResizeMouseMove(Object sender, MouseEventArgs e) {
            Rectangle rectangle = sender as Rectangle;
            switch (rectangle.Name) {
                case "top":
                    Cursor = Cursors.SizeNS;
                    break;
                case "bottom":
                    Cursor = Cursors.SizeNS;
                    break;
                case "left":
                    Cursor = Cursors.SizeWE;
                    break;
                case "right":
                    Cursor = Cursors.SizeWE;
                    break;
                case "topLeft":
                    Cursor = Cursors.SizeNWSE;
                    break;
                case "topRight":
                    Cursor = Cursors.SizeNESW;
                    break;
                case "bottomLeft":
                    Cursor = Cursors.SizeNESW;
                    break;
                case "bottomRight":
                    Cursor = Cursors.SizeNWSE;
                    break;
                default:
                    break;
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

        protected void ResizeRectangle_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            Rectangle rectangle = sender as Rectangle;
            switch (rectangle.Name) {
                case "top":
                    Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Top);
                    break;
                case "bottom":
                    Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Bottom);
                    break;
                case "left":
                    Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Left);
                    break;
                case "right":
                    Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Right);
                    break;
                case "topLeft":
                    Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.TopLeft);
                    break;
                case "topRight":
                    Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.TopRight);
                    break;
                case "bottomLeft":
                    Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.BottomLeft);
                    break;
                case "bottomRight":
                    Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.BottomRight);
                    break;
                default:
                    break;
            }
        }

        private void ResizeWindow(ResizeDirection direction) {
            SendMessage(_hwndSource.Handle, 0x112, (IntPtr)(61440 + direction), IntPtr.Zero);
        }

        private enum ResizeDirection {
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5,
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8,
        }

    }
}
