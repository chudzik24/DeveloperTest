using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailingLib.Models
{
    public class EmailHeader
    {
        public string Subject { get; set; }
        public List<string> From { get; set; } = new List<string>();
        public List<string> To { get; set; } = new List<string>();
        public string Id { get; set; }
        public DateTime? Date { get; set; }
    }
}
