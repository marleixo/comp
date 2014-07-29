using System;
using System.Windows.Forms;
using DMExport.Library.Helpers;
using DMExport.Library.Services;

namespace DMExport
{
    public partial class FormResults : Form
    {
        private readonly IDataService _dataStorage;

        public FormResults(IDataService dataStorage)
        {
            InitializeComponent();

            _dataStorage = dataStorage;
            components = null;
        }

        private void Warnings_Load(object sender, EventArgs e)
        {
            lblExportResult.Text =
                "The export completed successfully. However, the following items were detected as required objects but were not selected to be exported. If these objects are not present on the import server, the export will fail for the objects depending on these missing items.";

            _dataStorage
                .ExportWarningItems
                .ForEach(item => 
                    lstItems.Items.Add(String.Format("{0}: {1}", item.Type.ToString(), item.Name)));
        }

        private void BtnOkClick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
