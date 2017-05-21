using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Extensions;
using Shell.Core;
using Xamarin.Forms;
using XLabs;

namespace RespondentProfileQuota
{
    public partial class QuotaControlPage : ContentPage
    {
        void btnFresh_Clicked(object sender, System.EventArgs e)
        {

        }


        public QuotaControlPage()
        {
            InitializeComponent();

           
        }
        ClientServices Services;
        String ProjectID;
        private int IsAll = 0;

        public QuotaControlPage(ClientServices services, string projectID)
        {
            InitializeComponent();
            this.Services = services;
            ProjectID = projectID;

            if(Services.GetInformation("UserID").ToString().ToUpper()!="00000000-0000-0000-0000-000000000000"  && Services.GetInformation("UserID").ToString().ToUpper()!="C641CEC7-4411-415F-8A0A-393E0A92714F")
                {
                    colCity.IsVisible = false;
                    IsAll = 0;
                }
            else{
                IsAll = 1;
                }
                
            rQC.CheckedChanged += option_CheckedChange;
            rRercuit.CheckedChanged += option_CheckedChange;
            rInterview.CheckedChanged += option_CheckedChange;
            rCLTRercuit.CheckedChanged += option_CheckedChange;

            //LoadData();
        }
        void option_CheckedChange(object sender, EventArgs<bool> e)
        {
            if (sender != null && (sender as XLabs.Forms.Controls.CustomRadioButton).Checked == true)
            {
                foreach (var item in groupOptions.Children)
                {
                    if (item.GetType() == typeof(XLabs.Forms.Controls.CustomRadioButton))
                    {
                        if (item != sender)
                        {
                            (item as XLabs.Forms.Controls.CustomRadioButton).Checked = false;
                        }

                    }
                }
            }

            if (sender != null && (sender as XLabs.Forms.Controls.CustomRadioButton).Checked == true)
            {
                LoadData();
            }

        }
        private string FindValue(DataTable tbl, string AnswerID, string inputColumn, string inputValue, string outputColumn)
        {
            foreach (DataRow item in tbl.Rows)
            {
                if (item["AnswerID"].ToString().ToUpper() == AnswerID.Trim().ToUpper() && item[inputColumn].ToString() == inputValue)
                {
                    return item[outputColumn].ToString();
                }
            }
            return "";
        }

        private bool IsFieldQuota(string field, DataTable data)
        {
            foreach (DataRow row in data.Rows)
            {
                if (row["FieldName"].ToString().Trim().ToUpper() == field)
                    return true;
            }

            return false;
        }
        async Task LoadData()
        {
            //danh sach phieu Ok


            using (UserDialogs.Instance.Loading("Đang lấy dữ liệu quota"))
            {
                try
                {
                    var CityHandle = Services.GetInformation("CityHandle");
                    var query = new DataAccess.RequestCollection();
                    int checkChecked = 0;
                    int TypeView = 0;
                    if (rRercuit.Checked == true)
                    {
                        TypeView = 1;

                        checkChecked += 1;
                    }
                    if (rCLTRercuit.Checked == true)
                    {
                        TypeView = 2;
                     
                        checkChecked += 1;
                    }

                    if (rQC.Checked == true)
                    {
                        TypeView = 3;
                       
                        checkChecked += 1;
                    }
                    if (rInterview.Checked == true)
                    {
                        TypeView = 4;
                      
                        checkChecked += 1;
                    }

                    if (checkChecked != 1)
                    {

                        await DisplayAlert("Lỗi check option", Services.LastError, "Ok");
                        return;
                    }
                    //data  quota value
                    //query += DataAccess.DataQuery.Create("Docs", "ws_DOC_QuotaControl_List", new
                    //{
                    //    ProjectID

                    //});
                    ////data quota field name
                    //query += DataAccess.DataQuery.Create("Docs", "ws_DOC_QuotaControlValues_List", new
                    //{
                    //    ProjectID
                    //});
                    //query += DataAccess.DataQuery.Create("Docs", "ws_DOC_QuotaConditionExpression_ListWithCityHandle", new
                    //{
                    //    ProjectID,
                    //    CityHandle=Services.GetInformation("CityHandle")
                    //});
                    query = DataAccess.DataQuery.Create("Docs", "ws_DOC_Answers_ListCalculateQuotaWithCity", new
                        {
                            ProjectID,
                            TypeView,
							CityHandle = CityHandle,
                            IsAll=IsAll

                        });
                    var ds = Services.Execute(query);
                    if (DependencyService.Get<IDataSetExtension>().IsNull(ds) == true)
                    {

                        await DisplayAlert("Lỗi load dữ liệu", Services.LastError, "Ok");
                        return;
                    }

                    gData.ItemsSource = ds.Tables[0];
                    //var tblProfile = ds.Tables[0];
                    //var tblQuotaValue = ds.Tables[2];
                    //var tblQuotaField = ds.Tables[1];
                    //var tblExpression = ds.Tables[3];

                    //foreach (DataRow row in tblQuotaField.Rows)
                    //{
                    //    tblProfile.Columns.Add(row["FieldName"].ToString().ToUpper(), typeof(String));

                    //}

                    //foreach (DataRow row in tblProfile.Rows)
                    //{
                    //    for (int i = 0; i < row.ItemArray.Length; i++)
                    //    {
                    //        string AnswerID = row["AnswerID"].ToString();
                    //        string columnName = row.Table.Columns[i].ColumnName.ToString().ToUpper();
                    //        if (IsFieldQuota(columnName, tblQuotaField) == true)
                    //        {
                    //            row[i] = FindValue(tblQuotaValue, AnswerID, "FieldName", columnName, "FieldValue");
                    //        }

                    //    }
                    //}

                    //foreach (DataRow row in tblExpression.Rows)
                    //{
                    //    string queryCondition = row["ConditionExpression"].ToString();
                    //    var items = DependencyService.Get<IDataTableExtension>().SelectQuery(tblProfile, queryCondition);

                    //    int currentCount = items.Length;
                    //    row["CurrentCount"] = currentCount;

                    //    int minValue = (int)row["MinValue"];
                    //    int maxValue = (int)row["MaxValue"];

                    //    if (currentCount <= minValue && minValue != maxValue)
                    //        row["NeedQty"] = String.Format("Còn {0}-{1}", minValue - currentCount, maxValue - currentCount);
                        
                    //    if (currentCount <= minValue && minValue == maxValue)
                    //        row["NeedQty"] = String.Format("Còn {0}", minValue - currentCount);
                        
                    //    if (currentCount > minValue && minValue == maxValue)
                    //        row["NeedQty"] = String.Format("Dư {0}", currentCount - minValue);
                        
                    //    if (currentCount > minValue && minValue != maxValue && currentCount <= maxValue)
                    //        row["NeedQty"] = String.Format("Dư +{0}", maxValue - currentCount);

                    //    if (currentCount > minValue && minValue != maxValue && currentCount > maxValue)
                    //        row["NeedQty"] = String.Format("Dư +{0}", currentCount - maxValue);



                    //}
                    //gData.ItemsSource = tblExpression;
                }
                catch (Exception ex)
                {

                    await DisplayAlert(ex.StackTrace.ToString(), ex.Message, "Ok");
                    return;
                }
            }


        }

        internal async void Process()
        {
            if ((rQC.Checked == null || rQC.Checked == false) && (rRercuit.Checked == null || rRercuit.Checked == false) && (rInterview.Checked == null || rInterview.Checked == false))
            {
                rRercuit.Checked = true;

            }

            await LoadData();

        }
    }
}