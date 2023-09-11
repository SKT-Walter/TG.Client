using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TG.Client.ViewModel.Analysis;

namespace TG.Client.View.Analysis
{
    /// <summary>
    /// AnalysisView.xaml 的交互逻辑
    /// </summary>
    public partial class AnalysisView : UserControl
    {
        private AnalysisViewModel analysisViewModel = null;

        public AnalysisView()
        {
            InitializeComponent();
            
            analysisViewModel = new AnalysisViewModel();
            this.DataContext = analysisViewModel;
            this.analysisViewModel.Init(this);
        }

        private void BtnCollect_Click(object sender, RoutedEventArgs e)
        {
            this.analysisViewModel.StartAnalysis();
        }

        private void DataGrid1_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void MenuItem_CopySelectedRows_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
