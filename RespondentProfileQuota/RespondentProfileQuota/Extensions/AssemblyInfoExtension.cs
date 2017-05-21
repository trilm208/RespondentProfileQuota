using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;

using Shell.Core;
using Xamarin.Forms;

namespace Extensions
{
    public static class AssemblyInfoExtension
    {
        public static string GetAssemblyCopyright(this Assembly asm)
        {
            if (asm == null)
                return "";

            try
            {
                var attrib = asm.GetCustomAttribute<AssemblyCopyrightAttribute>();

                return attrib == null ? "" : attrib.Copyright;
            }
            catch (Exception)
            {
                return "";
            }
        }
        public static string GetAssemblyTitle(this Assembly asm)
        {
            if (asm == null)
                return "";

            try
            {
                var attrib = asm.GetCustomAttribute<AssemblyTitleAttribute>();

                return attrib == null ? "" : attrib.Title;
            }
            catch (Exception)
            {
                return "";
            }
        }
        public static string GetAssemblyCompanyName(this Assembly asm)
        {
            if (asm == null)
                return "";

            try
            {
                var attrib = asm.GetCustomAttribute<AssemblyCompanyAttribute>();

                return attrib == null ? "" : attrib.Company;
            }
            catch (Exception)
            {
                return "";
            }
        }
        public static string GetAssemblyVersionName(this Assembly asm )
        {
            //Assembly asm = this.GetType().GetTypeInfo().Assembly;
            string name = asm.GetAssemblyTitle();
            string copyright = asm.GetAssemblyCopyright();
            return name + "\n" + copyright + "\n" + asm.FullName;
        }
       
    }
}
