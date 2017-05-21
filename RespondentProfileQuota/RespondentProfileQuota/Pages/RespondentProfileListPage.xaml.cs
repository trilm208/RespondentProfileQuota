using System;
using System.Collections.Generic;
using System.Data;
using DevExpress.Mobile.DataGrid;
using Extensions;
using Shell.Core;
using Xamarin.Forms;
using DevExpress.Mobile.DataGrid.Theme;
using Acr.UserDialogs;
using System.Threading.Tasks;

namespace RespondentProfileQuota
{
    public partial class RespondentProfileListPage : ContentPage
    {
     

        DataTable tblHanhChanh;
        DataTable tblNhanSu;
        ClientServices Services;
        private String ProjectID;
        private DataTable tblQuota;

        async void btnNew_Clicked(object sender, System.EventArgs e)
        {
                 var _detailPage = new RespondentDetailPage(Services, null, tblQuota, tblHanhChanh, tblNhanSu, ProjectID, Guid.NewGuid().ToString());
                _detailPage.Saved += DetailPage_Saved;
                await Application.Current.MainPage.Navigation.PushAsync(_detailPage);
                UserDialogs.Instance.HideLoading();
           
        }

        async void btnView_Clicked(object sender, System.EventArgs e)
        {
            try
            {
                    DataRow row;
                    if (RowHandle >= 0)
                    {
                        row = (gData.ItemsSource as DataTable).Rows[RowHandle];
                        if (row == null)
                        {
                            return;
                        }

                    }
                    else
                    {
                        row = (gData.ItemsSource as DataTable).Rows[0];
                        if (row == null)
                        {
                            return;
                        }
                    }
                    var _detailPage = new RespondentDetailPage(Services, row, tblQuota, tblHanhChanh, tblNhanSu, ProjectID, row["AnswerID"].ToString());
                    _detailPage.Saved += DetailPage_Saved;
                     
                    await Application.Current.MainPage.Navigation.PushAsync(_detailPage);
            }
            catch( Exception ex)
            {
                 Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Lỗi chọn", "Chạm một dòng dữ liệu để xem lại", "OK");
                });
               
           }
          
        }

        void DetailPage_Saved(object sender, EventArgs e)
        {
          
            gData.ItemsSource = (sender as RespondentDetailPage).tblResult;
        }

        async void btnRefresh_Clicked(object sender, System.EventArgs e)
        {
                IUserDialogs Dialogs = UserDialogs.Instance;

                Dialogs.ShowLoading("Đang cập nhật");
                await   LoadRespondentList(); 
                Dialogs.HideLoading();
           
        }

        public RespondentProfileListPage()
        {
            InitializeComponent();
             
        }

        public RespondentProfileListPage(ClientServices services, string projectID) 
        {
            ThemeManager.ThemeName = Themes.Light;
            InitializeComponent();
            this.Services = services;
            this.ProjectID = projectID;
            Process();

            lblProjectName.Text = Services.GetInformation("ProjectName").ToString();
        }

        internal async Task Process()
        {
            await LoadRespondentList();         
        }

        async Task LoadRespondentList()
        {          
         
            var query = DataAccess.DataQuery.Create("Docs", "ws_DOC_Answers_GetForWaitingMobile", new
            {
                ProjectID,
                UserID=Services.GetInformation("UserID").ToString()
            });
            query += DataAccess.DataQuery.Create("Docs", "ws_DOC_QuotaControl_Get", new
            {
                ProjectID
            });
            query += DataAccess.DataQuery.Create("KadenceDB", "ws_L_WardDistrictCity_ListFromMobile", new
            {
                ProjectID,
                UserID=Services.GetInformation("UserID").ToString()
            });

            query += DataAccess.DataQuery.Create("KadenceDB", "ws_HR_PTEProject_ListFromMobile", new
            {
                ProjectID,
                UserID=Services.GetInformation("UserID").ToString()
            });
            var ds = Services.Execute(query);
            if (DependencyService.Get<IDataSetExtension>().IsNull(ds) == true)
            {
                await DisplayAlert("Lỗi load dữ liệu", Services.LastError, "Ok");
                return;
            }
           
            gData.ItemsSource = ds.Tables[0];
            tblQuota = ds.Tables[1];
            tblHanhChanh = ds.Tables[2];
            tblNhanSu = ds.Tables[3];

        }
        int RowHandle = -1;
        private async void OnSelectedRespondent(object sender, RowEventArgs e)
        {
            RowHandle=e.RowHandle;
        }
   }
}
