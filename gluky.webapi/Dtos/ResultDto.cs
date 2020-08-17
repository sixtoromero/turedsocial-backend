using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gluky.webapi.Dtos
{
    public class ResultDto
    {
        public int code { get; set; }
        public object Meta { get; set; }
        public List<postsDto> data { get; set; }
    }
}
