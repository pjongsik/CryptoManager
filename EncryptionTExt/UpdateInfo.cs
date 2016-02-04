using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EncryptionTest
{
    public class UpdateInfo
    {
        public string Table
        {
            get;
            set;
        }

        public string Column
        {
            get;
            set;
        }

        public string PlainText
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }

        public override string ToString()
        {
            string query = "UPDATE {0} SET {1} = '{2}' WHERE {1} = '{3}'";
            query = string.Format(query, Table, Column, Text, PlainText);

            return query;
        }
    }
}
