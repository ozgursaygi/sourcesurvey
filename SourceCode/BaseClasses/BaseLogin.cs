using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Security.Cryptography;
using System.Web;
using BaseDB;

namespace BaseClasses
{
    public class BaseLogin
    {
        DataTable userTable;
        DataTable uyeTable;
        public bool UserValidaton(string email, string passWord)
        {
            using (BaseDB.BaseDataAccess baseDataAccess = new BaseDB.BaseDataAccess())
            {
                using (BaseDB.BaseAdapter adapter = new BaseDB.BaseAdapter())
                {
                    string encriptedPassword = this.EncriptText(passWord);

                    System.Text.Encoding enc = System.Text.Encoding.ASCII;
                    //byte[] byteArray = enc.GetBytes(encriptedPassword);


                    BaseCommand cmd = new BaseCommand(baseDataAccess.MsConn);
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "Select * From gnl_users Where email=@email And password=@sifre And active = 1";
                    adapter.SelectCommand = cmd.Command;
                    adapter.SelectCommand.Parameters.AddWithValue("email", email);
                    adapter.SelectCommand.Parameters.AddWithValue("sifre", encriptedPassword);

                    DataSet userDataSet = new DataSet();
                    adapter.Fill(userDataSet, "Table");

                    userTable = userDataSet.Tables[0];
                    
                    if(userTable.Rows.Count >0)
                    {
                        DataSet uyeDataSet=new DataSet();
                        if (userTable.Rows[0]["user_uid"] != System.DBNull.Value && userTable.Rows[0]["user_uid"].ToString() != "") 
                        {
                            uyeDataSet = BaseDB.DBManager.AppConnection.GetDataSet("select * from gnl_uye_kullanicilar where user_uid='" + userTable.Rows[0]["user_uid"].ToString() + "'");
                            uyeTable = uyeDataSet.Tables[0];
                        }
                        
                    }
                    return userTable.Rows.Count == 1;
                }
            }
        }

        public bool UserValidatonWithAktivasyonKey(string email, string passWord,string key)
        {
            using (BaseDB.BaseDataAccess baseDataAccess = new BaseDB.BaseDataAccess())
            {
                using (BaseDB.BaseAdapter adapter = new BaseDB.BaseAdapter())
                {
                    string encriptedPassword = this.EncriptText(passWord);

                    System.Text.Encoding enc = System.Text.Encoding.ASCII;
                    //byte[] byteArray = enc.GetBytes(encriptedPassword);

                    BaseCommand cmd = new BaseCommand(baseDataAccess.MsConn);
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "Select * From gnl_users Where email=@email And password=@sifre And (active = 1 or active is null) and activation_key='" + key+"'";
                    adapter.SelectCommand = cmd.Command;
                    adapter.SelectCommand.Parameters.AddWithValue("email", email);
                    adapter.SelectCommand.Parameters.AddWithValue("sifre", encriptedPassword);

                    DataSet userDataSet = new DataSet();
                    adapter.Fill(userDataSet, "Table");

                    userTable = userDataSet.Tables[0];

                    if (userTable.Rows.Count > 0)
                    {
                        DataSet uyeDataSet = new DataSet();
                        if (userTable.Rows[0]["user_uid"] != System.DBNull.Value && userTable.Rows[0]["user_uid"].ToString() != "")
                        {
                            uyeDataSet = BaseDB.DBManager.AppConnection.GetDataSet("select * from gnl_uye_kullanicilar where user_uid='" + userTable.Rows[0]["user_uid"].ToString() + "'");
                            uyeTable = uyeDataSet.Tables[0];
                        }

                    }

                    return userTable.Rows.Count == 1;
                }
            }
        }

        public Guid GetUserUid()
        {
            Guid resultStr = Guid.Empty;
            
            if (userTable.Rows.Count != 0)
            {
                if (userTable.Rows[0]["user_uid"] != System.DBNull.Value && userTable.Rows[0]["user_uid"].ToString()!="") resultStr = Guid.Parse(userTable.Rows[0]["user_uid"].ToString());
            }

            return resultStr;
        }
        public string GetEmail()
        {
            string resultStr = "";
            if (userTable.Rows.Count != 0)
            {
                if (userTable.Rows[0]["email"] != System.DBNull.Value && userTable.Rows[0]["email"].ToString() != "") resultStr = userTable.Rows[0]["email"].ToString();
            }
            return resultStr;
        }

      
        public string GetUserNameAndSurName()
        {
            string resultStr = "";
            if (userTable.Rows.Count != 0)
            {
                if (userTable.Rows[0]["name"] != System.DBNull.Value && userTable.Rows[0]["name"].ToString() != "" && userTable.Rows[0]["surname"] != System.DBNull.Value && userTable.Rows[0]["surname"].ToString() != "") resultStr = userTable.Rows[0]["name"].ToString() + " " + userTable.Rows[0]["surname"].ToString();
            }
            return resultStr;
        }

        public string GetName()
        {
            string resultStr = "";
            if (userTable.Rows.Count != 0)
            {
                if (userTable.Rows[0]["name"] != System.DBNull.Value && userTable.Rows[0]["name"].ToString() != "" ) resultStr = userTable.Rows[0]["name"].ToString() ;
            }
            return resultStr;
        }

        public string GetSurname()
        {
            string resultStr = "";
            if (userTable.Rows.Count != 0)
            {
                if (userTable.Rows[0]["surname"] != System.DBNull.Value && userTable.Rows[0]["surname"].ToString() != "") resultStr = userTable.Rows[0]["surname"].ToString();
            }
            return resultStr;
        }
        public Guid GetUserGrupUid()
        {
            Guid resultStr = Guid.Empty;
            if (userTable.Rows.Count != 0)
            {
                if (userTable.Rows[0]["group_uid"] != System.DBNull.Value && userTable.Rows[0]["group_uid"].ToString() != "") resultStr = Guid.Parse(userTable.Rows[0]["group_uid"].ToString());
            }
            return resultStr;
        }

        public bool GetUserAdminType()
        {
            bool resultStr = false;
            if (userTable.Rows.Count != 0)
            {
                if (userTable.Rows[0]["is_system_admin"] != System.DBNull.Value && userTable.Rows[0]["is_system_admin"].ToString() != "" && (userTable.Rows[0]["is_system_admin"].ToString()=="true" || userTable.Rows[0]["is_system_admin"].ToString()=="True") ) resultStr = true;
            }
            return resultStr;
        }


        public string EncriptText(string text)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] hashedDataBytes;
            UTF8Encoding encoder = new UTF8Encoding();
            hashedDataBytes = md5Hasher.ComputeHash(encoder.GetBytes(text));
            string encriptedPassword = Convert.ToBase64String(hashedDataBytes);
            return encriptedPassword;
        }

        public bool ByteArrayCompare(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
                return false;

            for (int i = 0; i < a1.Length; i++)
                if (a1[i] != a2[i])
                    return false;

            return true;
        }

        public bool PassWordCompare(string a1, string a2)
        {
            if (a1.Length != a2.Length)
                return false;
            else if (a1 == a2)
                return true;
            else
                return false;
        }

        public bool PasswordControl(string password)
        {
            return this.UserValidaton(BaseDB.SessionContext.Current.ActiveUser.UserName, password);
        }

    }
}
