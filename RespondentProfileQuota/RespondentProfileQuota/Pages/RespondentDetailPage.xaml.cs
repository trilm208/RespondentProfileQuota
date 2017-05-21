using System;
using System.Collections.Generic;
using System.Data;
using Acr.UserDialogs;
using Extensions;
using Shell.Core;
using Xamarin.Forms;

namespace RespondentProfileQuota
{
   
    public partial class RespondentDetailPage : ContentPage
    {

        public event EventHandler IsDone;
        ClientServices Services;

        private DataTable tblHanhChanh;
        private DataTable tblQuotaControl;
        DataTable tblQuota;
        DataTable tblNhanSu;
        DataRow row;

        private string AnswerID;
        private string ProjectID;

        void txtDistrict_SelectedIndexChanged(object sender, System.EventArgs e)
        {

            if (txtDistrict.SelectedItem == null)
            {
                txtWard.ItemsSource = new List<String>();
                txtWard.SelectedIndex = -1;
            }
            else
            {
                var items = new List<String>();
                foreach (DataRow row in tblHanhChanh.Rows)
                {
                    if (row["District"].ToString() == txtDistrict.SelectedItem.ToString())
                    {
                        if (!items.Contains(row["Ward"].ToString()))
                            items.Add(row["Ward"].ToString());
                    }
                }
                txtWard.ItemsSource = items;
                txtWard.SelectedIndex = -1;
            }
        }

        public RespondentDetailPage(ClientServices services, DataRow row, DataTable tblQuota, DataTable tblHanhChanh, DataTable tblNhanSu, string projectID, string answerID)
        {
            this.Services = services;

            this.AnswerID = answerID;
            this.ProjectID = projectID;

            this.tblQuotaControl = tblQuota;
            this.tblHanhChanh = tblHanhChanh;
            this.tblNhanSu = tblNhanSu;

            this.row = row;

            InitializeComponent();

            foreach (DataRow r in tblNhanSu.Rows)
            {
                cbRecuit.Items.Add(r["FullNameCode"].ToString());
            }

            foreach (DataRow r in tblHanhChanh.Rows)
            {
                if (txtDistrict.Items.IndexOf(r["District"].ToString()) < 0)
                    txtDistrict.Items.Add(r["District"].ToString());
            }

            int minYOB = (int)Services.GetInformation("MinYOB");
            int maxYOB = (int)Services.GetInformation("MaxYOB");
            string acceptGender = Services.GetInformation("AcceptGender").ToString();

            var acceptGenderList = acceptGender.Split(',');

            foreach(var gender in acceptGenderList)
            {
                txtGender.Items.Add(gender);    
            }
            if (acceptGenderList.Length == 1)
                txtGender.SelectedIndex = 0;
            for (int i = minYOB; i <= maxYOB; i++)
            {
                txtYoB.Items.Add(i.ToString());
            }

            CreateQuotaField();
            LoadData();

          
        }
       
        async void CreateQuotaField()
        {
            var query = DataAccess.DataQuery.Create("Docs", "ws_DOC_QuotaControlValues_Get", new
            {
                AnswerID
            });

            DataSet ds = Services.Execute(query);
            if (DependencyService.Get<IDataSetExtension>().IsNull(ds) == true)
            {
                await DisplayAlert("Lỗi load dữ liệu", this.Services.LastError, "Ok");
                return;
            }

            foreach (DataRow r in tblQuotaControl.Rows)
            {
               
                string oldValue = FindValue(ds.Tables[0], "FieldName", r["FieldName"].ToString(), "FieldValue");

                var control = new QuotaControl(r, oldValue);
                control.HorizontalOptions = LayoutOptions.FillAndExpand;
                control.Orientation = StackOrientation.Vertical;
               
                //layoutQuota.Children.Add(new QuotaControl(r, oldValue));
                layoutQuota.Children.Add(control);
            }
        }

        private string FindValue(DataTable tbl, string inputColumn, string inputValue, string outputColumn)
        {

            foreach (DataRow item in tbl.Rows)
            {
                if (item[inputColumn].ToString() == inputValue)
                {
                    return item[outputColumn].ToString();
                }
            }
            return "";
        }

        void LoadData()
        {
            if (row != null)
            {
                txtFullName.Text = row["RespondentFullName"].ToString().Trim();
                txtGreenID.Text = row["GreenID"].ToString().Trim();
                txtGender.SelectedItem = row["RespondentGender"].ToString().Trim();
                txtYoB.SelectedItem = row["RespondentYoB"].ToString().Trim();
                //txtCity.SelectedItem = row["RespondentCity"].ToString().Trim();
                txtDistrict.SelectedItem = row["RespondentDistrict"].ToString().Trim();
                txtWard.SelectedItem = row["RespondentWard"].ToString().Trim();
                txtStreet.Text = row["RespondentStreet"].ToString().Trim();
                txtAddress.Text = row["RespondentAddressLandmark"].ToString().Trim();
                txtTelephone.Text = row["RespondentTelephone"].ToString().Trim();
                cbRecuit.SelectedIndex = FindIndexRecruitCode(row["RecruitCode"].ToString().Trim());
                txtStatus.SelectedItem = row["RespondentStatus"].ToString().Trim();
                txtEmail.Text = row["EmailAddress"].ToString().Trim();
            }

        }

        int FindIndexRecruitCode(string oldRecruitCode)
        {
            for (int i = 0; i < cbRecuit.Items.Count; i++)
            {
                if (tblNhanSu.Rows[i]["StaffID"].ToString() == oldRecruitCode)
                {
                    return i;
                }
            }
            return -1;
        }


        public DataTable tblResult { get; private set; }
        public async void btnSave_Clicked(object sender, EventArgs args)
        {
            using (UserDialogs.Instance.Loading("Đang lưu thông tin"))
            {
               if (ValidateForm().Result == false)
            {
                return;
            }
           
            string RecruitCode = FindRecruitCode();

            var query = new DataAccess.RequestCollection();
            query += DataAccess.DataQuery.Create("Docs", "ws_DOC_Answers_SupUpdate", new
            {
                AnswerID,
                GreenID = txtGreenID.Text.Trim().ToUpper(),
                ProjectID,
                RespondentFullName = txtFullName.Text.Trim().ToUpper(),
                RespondentAddressLandmark = txtAddress.Text.Trim().ToUpper(),
                RespondentStreet = txtStreet.Text.Trim().ToUpper(),
                RespondentWard = txtWard.SelectedItem == null ? "" : txtWard.SelectedItem.ToString(),
                RespondentDistrict = txtDistrict.SelectedItem == null ? "" : txtDistrict.SelectedItem.ToString(),
                RespondentCity = tblHanhChanh.Rows[0]["City"].ToString(),
                RespondentTelephone = txtTelephone.Text.Trim(),
                RespondentGender = txtGender.SelectedItem == null ? "" : txtGender.SelectedItem.ToString(),
                RespondentYoB = txtYoB.SelectedItem.ToString(),
                RespondentStatus = txtStatus.SelectedItem == null ? "" : txtStatus.SelectedItem.ToString(),
                RecruitCode = FindRecruitCode(),
                RecruitName = "",
                    EmailAddress=txtEmail.Text,
                UserID = Services.GetInformation("UserID")
            });

            foreach (var ctr in this.layoutQuota.Children)

			{
	if (ctr.GetType() == typeof(QuotaControl))
	{
		if ((ctr as QuotaControl).QuotaFieldValue == null || (ctr as QuotaControl).QuotaFieldValue.ToString().Trim().Length == 0)
		{
			await DisplayAlert("Lỗi nhập dữ liệu quota", "Nhập đầy đủ thông tin quota", "Ok");
			return;
		}
		query += DataAccess.DataQuery.Create("Docs", "ws_DOC_QuotaControlValues_Save", new
		{
			AnswerID,
			ProjectID,
			FieldName = (ctr as QuotaControl).QuotaFieldName,
			FieldValue = (ctr as QuotaControl).QuotaFieldValue,
			UserID = Services.GetInformation("UserID")
		});
	}
}

query += DataAccess.DataQuery.Create("Docs", "ws_DOC_Answers_GetForWaitingMobile", new
             {
                 ProjectID,
                 UserID = Services.GetInformation("UserID").ToString()
            });
            var ds = Services.Execute(query);

            if (DependencyService.Get<IDataSetExtension>().IsNull(ds) == true)
            {
               
                await DisplayAlert("Lỗi load dữ liệu", Services.LastError, "Ok");
                return;
            }
            tblResult = ds.Tables[ds.Tables.Count - 1];
          
            if (Saved!=null)
                {

				  Saved(this, null);
                }

            await Application.Current.MainPage.Navigation.PopAsync();
            }
            

          
        }
        protected override void OnDisappearing()
        {

        	base.OnDisappearing();
        }
        string FindRecruitCode()
        {
            foreach (DataRow r in tblNhanSu.Rows)
            {
                if (r["FullNameCode"].ToString() == cbRecuit.SelectedItem.ToString())
                {
                    return r["StaffID"].ToString();
                }
            }
            return "";
        }

        string FindRecruitName()
        {
            foreach (DataRow r in tblNhanSu.Rows)
            {
                if (r["FullNameCode"].ToString() == cbRecuit.SelectedItem.ToString())
                {
                    return r["FullName"].ToString();
                }
            }
            return "";
        }

        async System.Threading.Tasks.Task<bool> ValidateForm()
        {
            if (txtGreenID.Text == null || txtGreenID.Text.Trim().Length == 0)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Lỗi nhập dữ liệu", "Mã xanh không thể trống", "OK");
                });
                return false;
            }

            if (txtFullName.Text == null || txtFullName.Text.Trim().Length == 0)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Lỗi nhập dữ liệu", "Họ và tên không thể trống", "OK");
                });
                return false;
            }

            if (txtGender.SelectedItem == null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Lỗi nhập dữ liệu", "Giới tính không thể trống", "OK");
                });
                return false;
            }
            if (txtYoB.SelectedItem == null || txtYoB.SelectedItem.ToString().Trim().Length == 0)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Lỗi nhập dữ liệu", "Năm sinh không thể trốn", "OK");
                });
                return false;
            }

            int namsinh = 0;
            if (int.TryParse(txtYoB.SelectedItem.ToString().Trim(), out namsinh) == false)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Lỗi nhập dữ liệu", "Năm sinh phải là số nguyên", "OK");
                });
                return false;
            }
            if (namsinh == 0)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Lỗi nhập dữ liệu", "Năm sinh phải là số nguyên", "OK");
                });
                return false;
            }

           
                foreach (var ctr in this.layoutQuota.Children)

                {
                    if (ctr.GetType() == typeof(QuotaControl))
                    {
                        if ((ctr as QuotaControl).QuotaFieldValue == null || (ctr as QuotaControl).QuotaFieldValue.ToString().Trim().Length == 0)
                        {
                            
                        Device.BeginInvokeOnMainThread(() =>
                        {
                          DisplayAlert("Lỗi nhập dữ liệu quota", "Nhập đầy đủ thông tin quota", "Ok");
                        });
                        return false;
                     
                    }
                    }
                }

                if (txtDistrict.SelectedItem == null)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert("Lỗi nhập dữ liệu", "Quận không thể trống", "OK");
                    });
                    return false;
                }


            if (txtWard.SelectedItem == null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Lỗi nhập dữ liệu", "Phường không thể trốn", "OK");
                });
                return false;
            }
            if (txtStreet.Text == null || txtStreet.Text.Trim().Length == 0)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Lỗi nhập dữ liệu", "Đường không thể trống", "OK");
                });
                return false;
            }


            if (txtAddress.Text == null || txtAddress.Text.Trim().Length == 0)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Lỗi nhập dữ liệu", "Số nhà không thể trốn", "OK");
                });
                return false;
            }


            if (cbRecuit.SelectedItem == null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Lỗi nhập dữ liệu", "Người tuyển không thể trống", "OK");
                });
                return false;
               
            }


            if (txtStatus.SelectedItem == null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Lỗi nhập dữ liệu", "Trạng thái không thể trống", "OK");
                });
                return false;
            }
            return true;
        }

        public event EventHandler Saved;
        public async void btnRunICMA_Clicked(object sender, EventArgs args)
        {
            if(txtFullName.Text==null)
                {
                     Device.BeginInvokeOnMainThread(() =>
                    {
                         DisplayAlert("Lỗi chạy ICMA","Họ và tên phải được nhập trước khi chạy ICMA", "Ok");
                    });
                    return;
                }
            if(txtTelephone.Text==null)
                {
                     Device.BeginInvokeOnMainThread(() =>
                    {
                         DisplayAlert("Lỗi chạy ICMA","Điện thoại phải được nhập trước khi chạy ICMA", "Ok");
                    });
                    return;
                }
            if(txtAddress.Text==null)
                {
                     Device.BeginInvokeOnMainThread(() =>
                    {
                         DisplayAlert("Lỗi chạy ICMA","Số nhà phải được nhập trước khi chạy ICMA", "Ok");
                    });
                    return;
                }
            if(txtStreet.Text==null)
                {
                     Device.BeginInvokeOnMainThread(() =>
                    {
                         DisplayAlert("Lỗi chạy ICMA","Đường phải được nhập trước khi chạy ICMA", "Ok");
                    });
                    return;
                }
            var query = DataAccess.DataQuery.Create("Docs", "ws_DOC_GenericFormValues_SearchMultiRespondentCurrentProject", new
            {
            	STT = "1@",
                ProjectID,
                Name = txtFullName.Text.Trim().ToUpper().RemoveSign4VietnameseString().Replace("  ", String.Empty) + "@",
                Telephone =txtTelephone.Text.Trim().ToUpper().RemoveSign4VietnameseString().Replace("  ", String.Empty) + "@",
            	Address = txtAddress.Text.Trim().ToUpper().RemoveSign4VietnameseString().Replace("  ", String.Empty) + "@",
            	Street = txtStreet.Text.Trim().ToUpper().RemoveSign4VietnameseString().Replace("  ", String.Empty) + "@",
                MonthCheck = Services.GetInformation("MonthICMA").ToString(),
            	ProjectType = "1@2"
            });
            var ds = Services.Execute(query);
            if (DependencyService.Get<IDataSetExtension>().IsNull(ds) == true)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                         DisplayAlert("Lỗi chạy ICMA", Services.LastError, "Ok");
                });
                return;
            }
            if(ds.Tables[0].Rows.Count==0)
                {
                     Device.BeginInvokeOnMainThread(() =>
                    {
                           DisplayAlert("ICMA", String.Format("ICMA không phát hiện data nghi vấn"), "OK");
                    });
                    
                }
            else
            {
                string result = "";

                int i = 1;
                foreach(DataRow r in ds.Tables[0].Rows)
                    {
                        result += string.Format("{7}.Dự án {4}:{0}-{1}- Năm sinh:{2}- Ngày tham gia:{3}- Số nhà:{5}-Đường {6}",
                                                r["RespondentFullName"].ToString(),
                                                r["RespondentTelephone"].ToString(),
                                                r["RespondentYoB"].ToString(),
                                                r["AttendentDate"].ToString(),
                                                r["ProjectName"].ToString(),
                                                r["RespondentAddressLandmark"].ToString(),
                                               r["RespondentStreet"].ToString(), i);
                                                
                        result += Environment.NewLine;
                        i++;                                       
                    }

                    Device.BeginInvokeOnMainThread(() =>
                    {
                           DisplayAlert("ICMA", result, "OK");
                    });
               
                  
            }

        }
                                           

   }
 }