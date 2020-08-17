using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace gluky.webapi.Models
{
    public class posts
    {
        [Key]
        public int id { get; set; }
        public string id_posts { get; set; }
        public int user_id { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string url { get; set; }
    }
}
