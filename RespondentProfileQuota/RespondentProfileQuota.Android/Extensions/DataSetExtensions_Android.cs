using System.Data;
using Extensions;
using RespondentProfileQuota.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(DataSetExtensions_Android))]
namespace RespondentProfileQuota.Droid
{
    public class DataSetExtensions_Android : IDataSetExtension
    {
        public bool IsNull(DataSet ds)
        {
            if (ds == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
