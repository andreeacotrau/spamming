﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartTimeJobs.DAL.Models
{
    public class Review : BaseEntity
    {
        public Job Job { get; set; }
        public string OwnerDescription { get; set; }
        public string AssigneeDescription { get; set; }
    }
}
