using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
    public  interface IDataTableExtension

    {
            DataRow[] SelectQuery(DataTable dt, string filterString);
    }
}
