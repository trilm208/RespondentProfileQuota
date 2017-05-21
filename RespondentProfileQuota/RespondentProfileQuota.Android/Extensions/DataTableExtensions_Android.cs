using System;
using System.Data;
using Extensions;
using RespondentProfileQuota.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(DataTableExtensions_Android))]
namespace RespondentProfileQuota.Droid
{
    public class DataTableExtensions_Android : IDataTableExtension
    {
        public DataRow[] SelectQuery(DataTable dt, string filterString)
        {
            return dt.Select(filterString);
        }
    }
}
