using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExamApp.Data.Model
{
    [Table("RESULTS_BODY")]
    public class ResultBody
    {
        [PrimaryKey, AutoIncrement, Column("id")]
        public int id { get; set; }
        public int headID { get; set; }
        public int questionID { get; set; }
        public int chosenVariantID { get; set; }
        public bool isRight { get; set; } = false;
    }

    public class ResultBodyView
    {
        private string _questionName;
        public string questionName 
        {
            get { return _questionName; }
            set
            {
                _questionName = CheckTextLength(value);
            }
        }
        private string _chosenVariantName;
        public string chosenVariantName 
        {
            get { return _chosenVariantName; }
            set
            {

                _chosenVariantName = CheckTextLength(value);
            }
        }
        public bool isRight { get; set; } = false;
        public string color
        {
            get
            {
                if (isRight)
                {
                    return "#34A853";
                }
                return "#EA4335";
            }
        }

        private string CheckTextLength(string str)
        {
            string res = str;
            if (res.Length > 22)
            {
                res = res.Substring(0, 22) + "...";
            }
            return res;
        }

    }
}
