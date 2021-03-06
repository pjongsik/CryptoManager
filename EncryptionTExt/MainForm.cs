﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EncryptionTest.Utility;
using Framework.Core;

namespace EncryptionTest
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (DesignMode == true)
                return;

            Work();
        }

        /// <summary>
        ///  암호화 키
        /// </summary>
        const string _key = "thisiskey^^!";

        private void Work()
        {
            try
            {
                //Func<string, string> funcEncrypt = Cryptographer.Encrypt;
                //Func<string, string> funcDecrypt = Cryptographer.Decrypt;

                // 암복호화 메소드
                CryptoManager manager = new CryptoManager(AESEncryption, AESDecryption);

                // 암호화할 테이블의 필드들
                manager.SetTableFieldList("Employee", new List<string> { "SocialSecurityNo" });
                manager.SetTableFieldList("EmployeeFamily", new List<string> { "SocialSecurityNo" });

                // 종료이벤트
                manager.ProcessCompleted += manager_ProcessCompleted;

                // 시작
                manager.Run();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }
        }

        void manager_ProcessCompleted(object sender, CryptoManager.ProcessCompletedEventArgs e)
        {
            this.Close();
        }

        public string AESEncryption(string text)
        {
            return AESCrypto.Encrypt(text, _key);
        }

        public string AESDecryption(string text)
        {
            return AESCrypto.Decrypt(text, _key);
        }

    }
}
