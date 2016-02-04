using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EncryptionTest
{
    public class CryptoManager
    {
        // 데이블 및 필드 목록 정의
        List<string> _tableList = null;
        Dictionary<string, List<string>> _tableFieldDictionary = null;

        Func<string, string> _encryption = null;
        Func<string, string> _decryption = null;

        List<UpdateInfo> _updateList = null;

        public CryptoManager()
        {
        }

        public CryptoManager(Func<string, string> encryption, Func<string, string> decryption)
        {
            _encryption = encryption;
            _decryption = decryption;
        }

        public void SetTableFieldList(string tableName, List<string> fieldList)
        {
            if (_tableFieldDictionary == null)
                _tableFieldDictionary = new Dictionary<string, List<string>>();

            if (_tableList == null)
                _tableList = new List<string>();

            _tableList.Add(tableName);
            _tableFieldDictionary.Add(tableName, fieldList);
        }

        // 1. 암호화할 데이터 조회 
        // 2. UPDATE SQL 만들기
        // 3. SQL 문 실행

        #region Run

        public void Run()
        {
            if (_tableFieldDictionary == null)
                throw new NullReferenceException("암호화 적용할 테이블/필드명이 입력되지 않았습니다.");

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        
            worker.RunWorkerAsync();
        }

        string connectionString = string.Format("Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}", "192.168.1.43,1510", "NFDZQ4-1CHFCP-GF1TKW", "sa", "iqst63214");
         
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // 암화화할 데이터 조회
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                foreach (string tableName in _tableList)
                {
                    List<string> fieldList = _tableFieldDictionary[tableName] as List<string>;

                    foreach (string field in fieldList)
                    {
                        UpdateProcess(connection, tableName, field);
                    }
                }
            }
        }

        private void UpdateProcess(SqlConnection connection, string tableName, string field)
        {
            List<string> plainSocialSecurityNoList = new List<string>();
            List<UpdateInfo> updateList = new List<UpdateInfo>();

            // 여기할차려
            var queryString = string.Format("SELECT {0} FROM {1} WHERE LEN({0}) <= 14", field, tableName);

            SqlCommand command = new SqlCommand(queryString, connection);
            command.Connection.Open();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                plainSocialSecurityNoList.Add(reader["SocialSecurityNo"] as string);
            }

            reader.Close();
          
            // update 문 만들기
            foreach (string value in plainSocialSecurityNoList)
            {
                UpdateInfo updateInfo = new UpdateInfo();
                updateInfo.Table = tableName;
                updateInfo.Column = field;
                updateInfo.PlainText = value;
                updateInfo.Text = _encryption(value);

                updateList.Add(updateInfo);
            }

            // SQL문 실행
            foreach (var updateInfo in updateList)
            {
                // 여기할차례
                SqlCommand commandUpdate = new SqlCommand(updateInfo.ToString(), connection);
                commandUpdate.ExecuteNonQuery();
            }

            command.Connection.Close();
        }


        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // 에러 확인
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            
            // 끝
            OnProcessCompleted();
        }

        #endregion 

        #region ProcessCompleted event things for C# 3.0
        public event EventHandler<ProcessCompletedEventArgs> ProcessCompleted;

        protected virtual void OnProcessCompleted(ProcessCompletedEventArgs e)
        {
            if (ProcessCompleted != null)
                ProcessCompleted(this, e);
        }

        private ProcessCompletedEventArgs OnProcessCompleted()
        {
            ProcessCompletedEventArgs args = new ProcessCompletedEventArgs();
            OnProcessCompleted(args);

            return args;
        }

        private ProcessCompletedEventArgs OnProcessCompletedForOut()
        {
            ProcessCompletedEventArgs args = new ProcessCompletedEventArgs();
            OnProcessCompleted(args);

            return args;
        }

        public class ProcessCompletedEventArgs : EventArgs
        {
            public ProcessCompletedEventArgs()
            {

            }
        }
        #endregion
    }
}
