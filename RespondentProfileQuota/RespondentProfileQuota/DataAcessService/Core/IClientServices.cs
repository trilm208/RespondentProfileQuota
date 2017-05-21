using DataAccess;
using System.Data;

namespace DataAcesss
{
    public interface IClientServices
    {
        string LastError { get; }

        DataSet Execute(RequestCollection requests);

        object this[string key] { get; set; }

        void SetInformation(string key, object value);

        object GetInformation(string key);

        string Localize(string name);

        string Localize(string category, string name);

        string GetSetting(string name);

        string GetSetting(string category, string name);

        string LoadSettings(string FacID);

        bool HasPermission(string type);
        
    }
}