// Copyright 2014 #jacksoftszone
// Licensed under GPLv3
// Refer to the LICENSE.txt file included.

using Literal;
using LiteralWpf.Controls;
using System;
using System.Windows;
using System.Windows.Shell;

namespace LiteralWpf
{
    /// <summary>
    /// Main window of the client
    /// </summary>
    public partial class MainWindow : Controls.FramelessWindow {
        public MainWindow() {
            InitializeComponent();
            GetOSInformations();
        }

        private void File_Connect_Click(object sender, RoutedEventArgs e) {
            ConnectionManager ConnMan = new ConnectionManager();
            ConnMan.Show();
        }

        //Retreive OS informations, usefull for ProgressTaskBar function and future uses
        public void GetOSInformations() {
            Variables.OSVersionFull = Environment.OSVersion.Version.ToString(); //eg. 6.1.7601.65536 (Maybe will be deprecated)
            Variables.OSVersion = Environment.OSVersion.Version.Major + '.' + Environment.OSVersion.Version.Minor; //eg. 6.1
            Variables.OSBuild = Environment.OSVersion.Version.Build; //eg. 7601
            Variables.OS64bit = Environment.Is64BitOperatingSystem; //true or false
            Variables.OSLanguage = System.Globalization.CultureInfo.CurrentUICulture.ThreeLetterWindowsLanguageName; //eg. ENG
        }

        //ProgressBar colors: could be usefull in future (eg. DCC transfers). Maybe will need a background worker.
        //Status 1-5, Value 0-100
        public void ProgressTaskBar(int status, int value) {
            /*Statuses of the ProgressBar in the TaskBar:
             * 1: None
             * 2: Normal (Green)
             * 3: Paused (Yellow)
             * 4: Error (Red)
             * 5: Indeterminated (Flashing Green)
             */
            if (Variables.OSVersion >= 6.1) {
                switch (status) {
                    case 1:
                        taskBarItemInfo.ProgressState = TaskbarItemProgressState.None;
                        break;
                    case 2:
                        taskBarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
                        taskBarItemInfo.ProgressValue = value / 100.0;
                        break;
                    case 3:
                        taskBarItemInfo.ProgressState = TaskbarItemProgressState.Paused;
                        break;
                    case 4:
                        taskBarItemInfo.ProgressState = TaskbarItemProgressState.Error;
                        break;
                    case 5:
                        taskBarItemInfo.ProgressState = TaskbarItemProgressState.Indeterminate;
                        //To be tested, not fully implemented, maybe.
                        break;
                    default:
                        taskBarItemInfo.ProgressValue = value / 100.0;
                        break;
                }
            }
        }
    }

}
