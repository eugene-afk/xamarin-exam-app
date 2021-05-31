using System;

namespace ExamApp.Common.AppMasterDetail
{
    public class AppMasterDetailPageMasterMenuItem
    {
        public AppMasterDetailPageMasterMenuItem()
        {
            TargetType = typeof(AppMasterDetailPageMasterMenuItem);
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }
    }
}