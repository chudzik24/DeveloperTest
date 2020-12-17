using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
namespace DeveloperTest.Models
{
    public class EmailHeaderModel : ObservableObject
    {
        public string HeaderId { get; set; }
        public DateTime? Date
        {
            get
            {
                return _date;
            }
            set
            {
                Set(() => Date, ref _date, value);
            }
        }
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                Set(() => Title, ref _title, value);
            }
        }
        public string From
        {
            get
            {
                return _from;
            }
            set
            {
                Set(() => From, ref _from, value);
            }
        }
        private string _title;
        private string _from;
        private DateTime? _date;
    }
}
