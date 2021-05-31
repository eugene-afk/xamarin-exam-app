using SQLite;
using System;

namespace ExamApp.Data.Model
{
    [Table("RESULTS_HEAD")]
    public class Result
    {
        [PrimaryKey, AutoIncrement, Column("id")]
        public int id { get; set; }
        public int examID { get; set; }
        public int percent { get; set; }
        public DateTime dateEND { get; set; }
        public int userID { get; set; }
    }

    public class ResultView
    {
        public int id { get; set; }
        public int examID { get; set; }
        public int percent { get; set; }
        public DateTime dateEND { get; set; }
        public int userID { get; set; }
        public string label { get; set; }
        public string labelView
        {
            get
            {
                if(label.Length > 22)
                {
                    return label.Substring(0, 22) + "...";
                }
                return label;
            }
        }
        public decimal percentPB
        {
            get
            {
                return (decimal)percent / 100;
            }
        }
        public string barColor
        {
            get
            {
                if(percent >= 70)
                {
                    return "#34A853";
                }
                else if(percent < 70 && percent >= 40)
                {
                    return "#EF913D";
                }
                else
                {
                    return "#EA4335";
                }
            }
        }
    }
}
