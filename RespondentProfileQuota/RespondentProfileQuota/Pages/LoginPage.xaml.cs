using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Extensions;

using Shell.Core;
using Version.Plugin;
using Xamarin.Forms;



namespace RespondentProfileQuota
{
   
    public partial class LoginPage : ContentPage
    {
       
        public ClientServices Services;

        public LoginPage()
        {
            
            InitializeService();

            var resultLoadSettings = Services.LoadSettings("1");
            if(resultLoadSettings!="Ok")
             {
                Content = new StackLayout
                {
                    Children =
                        {
                            new Label
                            {
                                Text =resultLoadSettings
                            }
                        }
                    };
                 

                    return;
               
             }
            InitializeComponent();
            txtUsername.Text = SettingHelper.GetSetting("last_username", "");
            txtPassword.Text = SettingHelper.GetSetting("last_password", "");   
            string KadenceQuotaProfileAppVersion = Services.GetSetting("KadenceQuotaProfileAppVersion");
            if (KadenceQuotaProfileAppVersion == CrossVersion.Current.Version)
                {  
                   InitializeComponent();
                   txtUsername.Text = SettingHelper.GetSetting("last_username", "");
                   txtPassword.Text = SettingHelper.GetSetting("last_password", "");                }
            else
            {
                    Content = new StackLayout
                    {
                        Children = 
                        {
                            new Label 
                            {
                                Text = String.Format("Phiên bản hiện tại {0} khác phiên bản mới nhất {1}.Vui lòng cập nhật",CrossVersion.Current.Version, KadenceQuotaProfileAppVersion)
                            }
                        }
                    };
                 

                    return;
                }
        
        }

        public async void btnLogin_Clicked(object sender, EventArgs args)
        {
          

            UserDialogs.Instance.ShowLoading("Đang đăng nhập...");

            var query = DataAccess.DataQuery.Create("Security", "ws_Session_AuthenticateFromMobile", new
            {
            	Username = txtUsername.Text.Trim(),
            	PasswordHash = DependencyService.Get<IMd5HashExtensions>().GetMd5Hash(txtPassword.Text.Trim()),
            	FacID = "1"
            });

            var ds = Services.Execute(query);

            if (DependencyService.Get<IDataSetExtension>().IsNull(ds) == true)
            {
                UserDialogs.Instance.HideLoading();
                await DisplayAlert("Lỗi đăng nhập", Services.LastError, "Thử lại");
                return;
            }

            if (ds.Tables[0].Rows.Count == 0)
            {
                 UserDialogs.Instance.HideLoading();
                await DisplayAlert("Lỗi phân quyền", "Bạn chưa được phân quyền dự án", "Thử lại");
                return;
            }

            if (ds.Tables[0].Rows[0][0].ToString().ToUpper() != "OK")
            {
                UserDialogs.Instance.HideLoading();
                await DisplayAlert("Lỗi đăng nhập", ds.Tables[0].Rows[0][0].ToString().ToUpper(), "Thử lại");
                return;
            }

            Services.SetInformation("UserID", ds.Tables[0].Rows[0]["UserID"].ToString());

            if (chkRememberPassword.Checked == true)
            {
                await Save(txtUsername.Text.Trim(), txtPassword.Text.Trim());
            }
            else
            {
                await Save("", "");
            }

            //if(Authenticated!=null)
            //    {
            //      Authenticated(this, null);
            //    }
                   
            if (ds.Tables[0].Rows.Count == 1)
            {
                //await DisplayAlert("Dự án", string.Format("Bạn đang thực hiện dự án {0}",ds.Tables[0].Rows[0]["ProjectName"].ToString()), "Ok");
                Services.SetInformation("MinYOB", ds.Tables[0].Rows[0]["MinYOB"]);
                Services.SetInformation("ProjectNo", ds.Tables[0].Rows[0]["ProjectNo"]);
                Services.SetInformation("ProjectName", ds.Tables[0].Rows[0]["ProjectNo"]);
                Services.SetInformation("MaxYOB", ds.Tables[0].Rows[0]["MaxYOB"]);
                Services.SetInformation("AcceptGender", ds.Tables[0].Rows[0]["AcceptGender"]);
                Services.SetInformation("MonthICMA", ds.Tables[0].Rows[0]["MonthICMA"]);
                Services.SetInformation("CityHandle", ds.Tables[0].Rows[0]["CityHandle"]);
                var row = ds.Tables[0].Rows[0];

              
                var homePage = new TabbedPage();
               
                var _RespondentProfileListPage = new RespondentProfileListPage(Services, row["ProjectID"].ToString()); 
               
                homePage.Children.Add(_RespondentProfileListPage);
                homePage.Children.Add(new QuotaControlPage(Services, row["ProjectID"].ToString()));
          

                homePage.CurrentPageChanged += (object obj, EventArgs evt) =>
                                {
                                    var i = homePage.Children.IndexOf(homePage.CurrentPage);
                                    if (i == 0)
                                    {
                                        (homePage.CurrentPage as RespondentProfileListPage).Process();
                                    }
                                    if (i == 1)
                                    {
                                        (homePage.CurrentPage as QuotaControlPage).Process();
                                    }

                                };
                			
                  Application.Current.MainPage = new NavigationPage(homePage);
             }
            else
            {
                var page=new ProjectListPage(Services, ds.Tables[0]);
                Application.Current.MainPage = new NavigationPage(page);
				
            }  
             UserDialogs.Instance.HideLoading();    
           
        }
        protected override void OnDisappearing()
        {
        	UserDialogs.Instance.HideLoading();
        	base.OnDisappearing();
        }
        private async Task Save(string last_username, string last_password)
        {
            await SettingHelper.SaveSetting("last_username", last_username);
            await SettingHelper.SaveSetting("last_password", last_password);
        }
        void InitializeService()
        {
            Services = new ClientServices();
            Services.Initialize();
        }
   }
}
