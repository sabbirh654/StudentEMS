using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StudentEMS.Models
{
    public class ColumnModel
    {
        public string HeaderText { get; set; }
        public string BindingPath { get; set; }
        public Visibility Visibility { get; set; }
        public ListSortDirection Sortable { get; set; }
        public double Size { get; set; }
    }
}
