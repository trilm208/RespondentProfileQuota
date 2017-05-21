using System;
using System.Collections.Generic;
using System.Data;
using DevExpress.Mobile.DataGrid;
using Extensions;
using Shell.Core;
using Xamarin.Forms;
using DevExpress.Mobile.DataGrid.Theme;
using Acr.UserDialogs;

namespace RespondentProfileQuota
{
    public partial class ProjectListPage : ContentPage
    {
        ClientServices Services;
        DataTable dataTable;

        public ProjectListPage()
        {
            InitializeComponent();
        }

    

        public ProjectListPage(ClientServices services, DataTable dataTable) 
        {
           
            ThemeManager.ThemeName = Themes.Light;
            InitializeComponent();
            this.Services = services;
             gProjects.ItemsSource = dataTable;
        }

        private async void OnSelectedProject(object sender, RowEventArgs e)
        {

            DataRow row = (gProjects.ItemsSource as DataTable).Rows[e.RowHandle];
            if (row == null)
            {
                return;
            }


            Services.SetInformation("MinYOB", row["MinYOB"]);
            Services.SetInformation("ProjectNo", row["ProjectNo"]);
            Services.SetInformation("ProjectName", row["ProjectNo"]);
            Services.SetInformation("MaxYOB", row["MaxYOB"]);
            Services.SetInformation("AcceptGender", row["AcceptGender"]);
            Services.SetInformation("MonthICMA", row["MonthICMA"]);

             Services.SetInformation("CityHandle", row["CityHandle"]);
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

    
    }
}
