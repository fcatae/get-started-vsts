using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleVsts
{
    class VsTaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AssignedTo { get; set; }
        public string State { get; set; }
        public DateTime? StartDate { get; set; }
        public float? Duration { get; set; }
    }
}
