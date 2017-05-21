using DataAccess;

using System.Collections.Generic;
using System.Data;
using Xamarin.Forms;
using System;
using System.Threading.Tasks;
using Extensions;
using DataAcesss;

namespace Shell.Core
{
    public class ClientServices : IClientServices
    {
        private Config Config { get; set; }

        private HttpDataServices DataService { get; set; }

        private DataTable Localization { get; set; }

        private DataTable Setting { get; set; }

        private List<string> Permissions { get; set; }

        private List<string> UserPermissions { get; set; }

        public string LastError { get; private set; }

        private Dictionary<string, object> SharedInfo = new Dictionary<string, object>();

        public ClientServices()
        {
            //this.Config = Shell.Core.Config.Load();
            this.DataService = new HttpDataServices("http://10.0.2.2:8080/DataAccess.ashx");
            this.Permissions = new List<string>();
            this.UserPermissions = new List<string>();
        }

        public void Initialize()
        {
      
        }

        public async void LoadPermissions(string FacID)
        {
              
        }



        public string LoadSettings(string FacID)
        {
            var query = DataQuery.Create("Application", "ws_Settings_List");
            var ds = this.Execute(query);

            if (DependencyService.Get<IDataSetExtension>().IsNull(ds) == true)
            {
                return this.LastError;
                //return "Ok";
            
            }
            this.Setting = ds.Tables[0];
            return "Ok";
        }

        public string Localize(string name)
        {
            return "";
        }

        public string Localize(string category, string name)
        {
           
            return "";
        }

        public string GetSetting(string name)
        {

            foreach(DataRow row in Setting.Rows)
            {
                if(row["Name"].ToString()==name)
                {
                    return row["Value"].ToString();
                }    
                    
            }
            return "";
        }

        public string GetSetting(string category, string name)
        {
          
            return "";
        }

        public DataSet Execute(RequestCollection requests)
        {
            this.LastError = "";
            var ds = DataService.Execute(requests);

            var table = ds.Tables[0];
            try
            {
                if (table.TableName == "Error")
                {
                    var row = table.Rows[0];

                    var message = row["Message"].ToString();
                    var source = row["Source"].ToString();
                    var stackTrace = row["StackTrace"].ToString();
                    var helpLink = row["HelpLink"].ToString();

                    this.LastError = message;
                    ds = null;
                }
            }
            catch
            {

            }
            return ds;
           
        }

        public void SetInformation(string key, object value)
        {
            if (SharedInfo.ContainsKey(key))
                SharedInfo[key] = value;
            else
                SharedInfo.Add(key, value);
        }

        public object GetInformation(string key)
        {
            if (SharedInfo.ContainsKey(key))
                return SharedInfo[key];

            return null;
        }

        public object this[string key]
        {
            get { return GetInformation(key); }
            set { SetInformation(key, value); }
        }

     
        public bool HasPermission(string key)
        {
            key = key.ToUpper();
            if (UserPermissions.Contains(key))
                return true;

            if (Permissions.Contains(key))
                return false;

            var query = DataQuery.Create("Security", "ws_Permissions_Create", new { Key = key });
            this.Execute(query);

            Permissions.Add(key);

            return false;
        }
    }
}