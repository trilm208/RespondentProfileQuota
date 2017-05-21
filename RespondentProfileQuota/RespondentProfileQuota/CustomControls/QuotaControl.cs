using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;
using Xamarin.Forms;
using XLabs;

using XLabs.Forms;
using XLabs.Forms.Controls;

namespace RespondentProfileQuota
{
    public class QuotaControl:StackLayout
    {
        private String QuotaName;
        public String QuotaFieldName;
        XLabs.Forms.Controls.BindableRadioGroup groupOptions;
        public int Type = 1;
        public string ShowCode = "1";
        private Picker pick;
        List<string> listID = new List<string>();

        public string QuotaFieldValue 
        { 
            get 
            {
                if(Type==1)
                    {
                        if (groupOptions==null) return "";
                        foreach(var item in groupOptions.Children)
                            {
                                if(item.GetType()==typeof(MyRadioButton))
                                {
                                    if ((item as MyRadioButton).Checked == true)
                                        return (item as MyRadioButton).FieldValue;
                                }
                            }
                        return "";
                    }
                if(Type==2)
                    {
                    if (pick == null)
                        return "";
                    if (pick.SelectedItem == null)
                           return "";

                    return listID[pick.SelectedIndex];
                       
                       
                    }
                return "";
             }
        }   
        public QuotaControl(DataRow row,string QuotaFieldValue)
        {
            var label = new Label();
            label.Text = row["Caption"].ToString();
            label.TextColor = Color.Black;
            label.FontSize = 15;

            QuotaFieldName = row["FieldName"].ToString();
            try
            {
                Type = (int)row["Type"];
            }
            catch
            {

            }

             try
            {
                ShowCode = row["ShowCode"].ToString();
            }
            catch
            {

            }

            string valueOptions = row["OptionValues"].ToString();


            if (Type == 2)
            {
               
                try
                {
                    int indexSelected = -1;
                 
                    var tblValue = JsonConvert.DeserializeObject<List<QuotaControlField>>(valueOptions);
                    var items = new List<String>();
                    for (int j = 0; j < tblValue.Count;j++)
                        {
                            var item = tblValue[j];
                        if(ShowCode=="1")
                              items.Add(item.OptionFieldValue+". "+ item.OptionFieldName);
                        else{
                             items.Add( item.OptionFieldName);
                            }

                        listID.Add(item.OptionFieldValue);

                        if(item.OptionFieldValue==QuotaFieldValue)
                            {
                            indexSelected = j;
                            }
                        }
                   
                    pick = new Picker();
                    pick.ItemsSource = items;

                    try
                    {
                        pick.SelectedIndex = indexSelected;
                    }
                    catch(Exception ex)
                    {

                    }
                 
                    this.Children.Add(label);
                    this.Children.Add(pick);

                }
                catch (Exception ex)
                {

                }

               
            }
            if (Type == 1)
            {
                try
                {
                    var tblValue = JsonConvert.DeserializeObject<List<QuotaControlField>>(valueOptions);
                     groupOptions = new XLabs.Forms.Controls.BindableRadioGroup();
                    foreach (QuotaControlField item in tblValue)
                    {
                        MyRadioButton op = new MyRadioButton();
                        if(ShowCode=="1")
                        op.Text =item.OptionFieldValue+". "+ item.OptionFieldName;
                        else{
                             op.Text = item.OptionFieldName;
                            }
                        op.FieldValue = item.OptionFieldValue;
                        if (item.OptionFieldValue.Trim().ToUpper() == QuotaFieldValue.Trim().ToUpper())
                            op.Checked = true;

                        op.CheckedChanged += option_CheckedChange;
                        groupOptions.Children.Add(op);

                    }
                     this.Children.Add(label);
                      this.Children.Add(groupOptions);
                }
                catch (Exception ex)
                {

                }
            }

          
        }

        void option_CheckedChange(object sender, EventArgs<bool> e)
        {
            if(sender!=null && (sender as MyRadioButton).Checked==true)
            {
                foreach(var item in groupOptions.Children)
                    {
                    if(item.GetType()==typeof(MyRadioButton))
                           {
                              if(item!=sender)
                               {
                                   (item as MyRadioButton).Checked = false;
                               }
                           }
                    }
            }
        }

        void groupOptions_CheckedChanged(object sender, int e)
        {
        	var radio = sender as MyRadioButton;

        	if (radio == null || radio.Id == -1) return;

        	// Display selected value in Label   
        	

        }

       
   }
}
