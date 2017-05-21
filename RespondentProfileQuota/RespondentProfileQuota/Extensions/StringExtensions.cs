using System;
using System.Data;
using Newtonsoft.Json;

namespace RespondentProfileQuota
{
    public static class StringExtensions
    {
            public static System.Data.DataTable JSONToDataTable(this string value)
            {
            	if (value == null || value == "null")
            		return null;
            	return (DataTable)JsonConvert.DeserializeObject(value, (typeof(DataTable)));
              }
            private static readonly string[] VietnameseSigns = new string[]

            {

    		"aAeEoOuUiIdDyY",

    		"áàạảãâấầậẩẫăắằặẳẵ",

    		"ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

    		"éèẹẻẽêếềệểễ",

    		"ÉÈẸẺẼÊẾỀỆỂỄ",

    		"óòọỏõôốồộổỗơớờợởỡ",

    		"ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

    		"úùụủũưứừựửữ",

    		"ÚÙỤỦŨƯỨỪỰỬỮ",

    		"íìịỉĩ",

    		"ÍÌỊỈĨ",

    		"đ",

    		"Đ",

    		"ýỳỵỷỹ",

    		"ÝỲỴỶỸ"

        };
        public static string RemoveSign4VietnameseString(this string str)
        {

        	//Tiến hành thay thế , lọc bỏ dấu cho chuỗi

        	for (int i = 1; i < VietnameseSigns.Length; i++)
        	{

        		for (int j = 0; j < VietnameseSigns[i].Length; j++)

        			str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);

        	}

        	return str;

                }

            }
                
}
