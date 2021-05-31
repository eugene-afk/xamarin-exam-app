using ExamApp.Tests;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ExamApp.ViewModel
{
    public class DemoViewModel : Base
    {
        private bool _waiting;
        public bool waiting
        {
            get { return _waiting; }
            set { _waiting = value; OnPropertyChanged(); }
        }
        private string _stateMsg;
        public string stateMsg
        {
            get { return _stateMsg; }
            set { _stateMsg = value; OnPropertyChanged(); }
        }
        public ICommand FillDemoCommand { protected set; get; }

        public DemoViewModel()
        {
            FillDemoCommand = new Command(async () => await FillDemo());
        }

        private async Task FillDemo()
        {
            waiting = true;
            stateMsg = "Please wait before demo data will be created...";
            Demo d = new Demo();
            try
            {
                await d.FillDemoData();
            }
            catch(Exception ex)
            {
                waiting = false;
                Debug.Print("*FillDemo* msg: " + ex);
                stateMsg = "Something go wrong. Error: " + ex.Message;
                return;
            }
            waiting = false;
            stateMsg = "Success. Demo data added.";
        }

    }
}
