﻿//using microsoft_launcher;
using System;
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

namespace YMCL.LoginUI
{
    /// <summary>
    /// Microsoft.xaml 的交互逻辑
    /// </summary>
    public partial class Microsoft : Page
    {
        public Microsoft()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MicrosoftLoginWb microsoftlogin = new MicrosoftLoginWb();
            microsoftlogin.ShowDialog();
            
        }
    }
}
