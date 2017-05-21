using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Extensions
{
    public  interface IStringExtension
    {
         
         Byte[] StringBase64ToByteArray(String s);
    }
}
