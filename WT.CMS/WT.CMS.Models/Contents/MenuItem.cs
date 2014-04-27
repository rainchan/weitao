using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WT.CMS.Models.Contents
{
    public class MenuItem
    {
        public long Id { get; set; }

        public long ParentId { get; set; }

        public string Name { get; set; }

        public string Command { get; set; }
    }
}
