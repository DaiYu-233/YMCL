﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using MinecaftOAuth;
using MinecraftLaunch.Modules.Toolkits;
using Newtonsoft.Json;
using Panuon.UI.Silver;
using YMCL.Class;

namespace YMCL.Pages.SettingPages
{
    /// <summary>
    /// Account.xaml 的交互逻辑
    /// </summary>
    public partial class Account : Page
    {
        public int indexAnd;
        public List<AccountInfo> accountInfos = new List<AccountInfo>();
        

        public Account()
        {
            InitializeComponent();

            LoginTypeComboBox.SelectedItem = LoginTypeComboBox.Items[0];

            var obj = JsonConvert.DeserializeObject<SettingInfo>(File.ReadAllText("./YMCL/YMCL.Setting.json"));

            datagrid();
            if((accountInfos.Count) == 0)
            {
                System.IO.File.WriteAllText(@".\YMCL\YMCL.Account.json", "[{\"AccountType\": \"离线登录\",\"Name\": \"Steve\",\"AddTime\": \"Null\"}]");
                string tstr = System.IO.File.ReadAllText(@".\YMCL\YMCL.Account.json");
                accountInfos = JsonConvert.DeserializeObject<List<AccountInfo>>(tstr);
            }
            DataGr.SelectedItem = DataGr.Items[Convert.ToInt32(obj.LoginIndex)];
            


            System.IO.File.WriteAllText(@".\YMCL\Temp\LoginName.log", accountInfos[DataGr.SelectedIndex].Name.ToString());
            System.IO.File.WriteAllText(@".\YMCL\Temp\LoginType.log", accountInfos[DataGr.SelectedIndex].AccountType.ToString());

        }
        private async void MicrosoftLogin()
        {
            var V = MessageBoxX.Show("确定开始验证您的账户\n需要打开浏览器进行登录", "验证", MessageBoxButton.OKCancel);
            if (V == MessageBoxResult.OK)
            {
                addacbro.Visibility = Visibility.Hidden;
                mojangbro.Visibility = Visibility.Hidden;
                lxdl.Visibility = Visibility.Hidden;
            }
            else
            {
                addacbro.Visibility = Visibility.Hidden;
                mojangbro.Visibility = Visibility.Hidden;
                lxdl.Visibility = Visibility.Hidden;
                return;
            }

            MicrosoftAuthenticator microsoftAuthenticator = new(MinecaftOAuth.Module.Enum.AuthType.Access) { ClientId = "c06d4d68-7751-4a8a-a2ff-d1b46688f428" };
            var code = await microsoftAuthenticator.GetDeviceInfo();
            Clipboard.SetText(code.UserCode);
            MessageBoxX.Show(code.UserCode, "您的一次性访问代码(已复制到剪切板)"); 
            Debug.WriteLine("Link:{0} - Code:{1}", code.VerificationUrl, code.UserCode);
            if (V == MessageBoxResult.OK) { Process.Start(new ProcessStartInfo(code.VerificationUrl) { UseShellExecute = true, Verb = "open" }); }
            var token = await microsoftAuthenticator.GetTokenResponse(code);
            var user = await microsoftAuthenticator.AuthAsync(x => { Debug.WriteLine(x); });
            var a = JsonConvert.SerializeObject(user, Newtonsoft.Json.Formatting.Indented);
            MessageBoxX.Show(a,"账户Json信息");
            System.IO.File.WriteAllText("./YMCL/Accounts/Microsoft-"+user.Name+".json",a);
            accountInfos.Add(new AccountInfo() { AccountType = "微软登录", Name = user.Name.ToString(),AddTime = DateTime.Now.ToString()});
            WriteFile();

            string str = System.IO.File.ReadAllText(@".\YMCL\YMCL.Account.json");
            dynamic model = JsonConvert.DeserializeObject<List<AccountInfo>>(str);
            DataGr.ItemsSource = model;


        }












        private void datagrid()
        {
            if (System.IO.File.Exists(@".\YMCL\YMCL.Account.json"))
            {
                string tstr = System.IO.File.ReadAllText(@".\YMCL\YMCL.Account.json");
                accountInfos = JsonConvert.DeserializeObject<List<AccountInfo>>(tstr);
            }
            else
            {
                System.IO.File.WriteAllText(@".\YMCL\YMCL.Account.json", "[{\"AccountType\": \"离线登录\",\"Name\": \"Steve\",\"AddTime\": \"Null\"}]");
                string tstr = System.IO.File.ReadAllText(@".\YMCL\YMCL.Account.json");
                accountInfos = JsonConvert.DeserializeObject<List<AccountInfo>>(tstr);
            }
            string str = System.IO.File.ReadAllText(@".\YMCL\YMCL.Account.json");
            dynamic model = JsonConvert.DeserializeObject<List<AccountInfo>>(str);
            accountInfos = JsonConvert.DeserializeObject<List<AccountInfo>>(str);
            DataGr.ItemsSource = model;
        }
        private void AddAcount_Click(object sender, RoutedEventArgs e)
        {
            addacbro.Visibility = Visibility.Visible;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            addacbro.Visibility = Visibility.Hidden;
            mojangbro.Visibility = Visibility.Hidden;
            lxdl.Visibility = Visibility.Hidden;
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (LoginTypeComboBox.SelectedItem.ToString() == "System.Windows.Controls.ComboBoxItem: 离线登录")
            {
                lxdl.Visibility = Visibility.Visible;
                addacbro.Visibility = Visibility.Hidden;
            }
            else if (LoginTypeComboBox.SelectedItem.ToString() == "System.Windows.Controls.ComboBoxItem: Mojang迁移")
            {
                Process.Start("explorer.exe", "https://www.minecraft.net/zh-hans/account-security");
                addacbro.Visibility = Visibility.Hidden;
            }
            else if (LoginTypeComboBox.SelectedItem.ToString() == "System.Windows.Controls.ComboBoxItem: 微软登录")
            {
                MicrosoftLogin();
            }
            else
            {
                MessageBoxX.Show("请选择登录方式");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if(yhm2.Text != string.Empty)
            {
                accountInfos.Add(new AccountInfo() { AccountType = "离线登录",Name = yhm2.Text, AddTime = DateTime.Now.ToString()});
                WriteFile();
                addacbro.Visibility = Visibility.Hidden;
                mojangbro.Visibility = Visibility.Hidden;
                lxdl.Visibility = Visibility.Hidden;
                datagrid();
            }
            else
            {
                MessageBoxX.Show("用户名为空！");
            }

        }
        private void WriteFile()
        {
            string str = JsonConvert.SerializeObject(accountInfos, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText(@".\YMCL\YMCL.Account.json", str);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (yhm1.Text != string.Empty && mmk.Password != string.Empty)
            {
                accountInfos.Add(new AccountInfo() { AccountType = "Mojang", Name = yhm1.Text, AddTime=DateTime.Now.ToString() });
                WriteFile();
                addacbro.Visibility = Visibility.Hidden;
                mojangbro.Visibility = Visibility.Hidden;
                lxdl.Visibility = Visibility.Hidden;
                datagrid();
            }
            else
            {
                MessageBoxX.Show("账户或密码为空！");
            }
        }

        private void DataGr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGr.SelectedIndex >= 0)
            {

                Del.IsEnabled = true;
                NowLoginType.Text = accountInfos[DataGr.SelectedIndex].AccountType.ToString()+" - "+accountInfos[DataGr.SelectedIndex].Name.ToString();
                System.IO.File.WriteAllText(@".\YMCL\Temp\LoginName.log", accountInfos[DataGr.SelectedIndex].Name.ToString());
                System.IO.File.WriteAllText(@".\YMCL\Temp\LoginType.log", accountInfos[DataGr.SelectedIndex].AccountType.ToString());

                var obj = JsonConvert.DeserializeObject<SettingInfo>(File.ReadAllText("./YMCL/YMCL.Setting.json"));
                obj.LoginIndex = DataGr.SelectedIndex.ToString();
                obj.LoginName = accountInfos[DataGr.SelectedIndex].Name.ToString();
                obj.LoginType = accountInfos[DataGr.SelectedIndex].AccountType.ToString();
                System.IO.File.WriteAllText(@"./YMCL/YMCL.Setting.json",JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented) );

            }
            else
            {
                if ((accountInfos.Count) == 0)
                {
                    System.IO.File.WriteAllText(@".\YMCL\YMCL.Account.json", "[{\"AccountType\": \"离线登录\",\"Name\": \"Steve\",\"AddTime\": \"Null\"}]");
                    string tstr = System.IO.File.ReadAllText(@".\YMCL\YMCL.Account.json");
                    accountInfos = JsonConvert.DeserializeObject<List<AccountInfo>>(tstr);
                }
                DataGr.SelectedItem = DataGr.Items[0];
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            accountInfos.RemoveAt(DataGr.SelectedIndex);
            if ((accountInfos.Count) == 0)
            {
                System.IO.File.WriteAllText(@".\YMCL\YMCL.Account.json", "[{\"AccountType\": \"离线登录\",\"Name\": \"Steve\",\"AddTime\": \"Null\"}]");
                string tstr = System.IO.File.ReadAllText(@".\YMCL\YMCL.Account.json");
                accountInfos = JsonConvert.DeserializeObject<List<AccountInfo>>(tstr);
            }
            
            WriteFile();
            datagrid();
            DataGr.SelectedItem = DataGr.Items[0];
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Toast.Show("启动逻辑尚未完善,建议使用离线模式", ToastPosition.Top);
        }
    }
}
