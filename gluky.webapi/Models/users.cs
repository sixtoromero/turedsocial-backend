﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace gluky.webapi.Models
{
    public class users
    {
        [Key]
        public int id { get; set; }
        public string user_id { get; set; }
        public string email { get; set; }
        public string password { get; set; }
    }
}
