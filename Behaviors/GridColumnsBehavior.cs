using StudentEMS.Models;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;

namespace StudentEMS.Behaviors
{
    public static class GridColumnsBehavior
    {
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.RegisterAttached(
                "Columns",
                typeof(ObservableCollection<ColumnModel>),
                typeof(GridColumnsBehavior),
                new PropertyMetadata(null, ColumnsPropertyChanged));

        public static ObservableCollection<ColumnModel> GetColumns(DependencyObject obj)
        {
            return (ObservableCollection<ColumnModel>)obj.GetValue(ColumnsProperty);
        }

        public static void SetColumns(DependencyObject obj, ObservableCollection<ColumnModel> value)
        {
            obj.SetValue(ColumnsProperty, value);
        }

        private static void ColumnsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid dataGrid)
            {
                if (e.NewValue is ObservableCollection<ColumnModel> columns)
                {
                    dataGrid.Columns.Clear();

                    foreach (var column in columns)
                    {
                        var sz = column.Size;
                        var binding = new Binding(column.BindingPath);
                        var dataGridColumn = new DataGridTextColumn
                        {
                            Header = column.HeaderText,
                            Binding = binding,
                           
                        };

                        dataGrid.Columns.Add(dataGridColumn);
                    }
                }
            }
        }
    }
}
