using FISCA.Data;
using FISCA.Presentation.Controls;
using FISCA.UDT;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KCBSSubjectScoreCalc
{
    public partial class KCBSCalcSetting : BaseForm
    {
        AccessHelper _A;
        List<KCBSSubjectScoreConfig> _config;
        public KCBSCalcSetting()
        {
            InitializeComponent();

            _A = new AccessHelper();

            _config = _A.Select<KCBSSubjectScoreConfig>();

            foreach (KCBSSubjectScoreConfig conf in _config)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgv, conf.Subject + "", conf.Level + "", conf.Percentage + "");
                dgv.Rows.Add(row);
            }

            string sql = "select subject from course group by subject order by subject";

            QueryHelper q = new QueryHelper();
            DataTable dt = q.Select(sql);

            foreach (DataRow row in dt.Rows)
            {
                string subj = row["subject"] + "";
                colSubject.Items.Add(subj);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool pass = true;
            List<string> check = new List<string>();
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow)
                    continue;

                string subject_name = row.Cells[colSubject.Index].Value + "";
                string str_level = row.Cells[colLevel.Index].Value + "";
                string str_percentage = row.Cells[colPercentage.Index].Value + "";
                string key = subject_name + "#" + str_level;
                int i;

                row.ErrorText = string.Empty;
                if (string.IsNullOrWhiteSpace(subject_name))
                {
                    row.ErrorText = "科目名稱不可為空白";
                    pass = false;
                }
                else
                {
                    if (check.Contains(key))
                    {
                        row.ErrorText = "科目名稱與級別的組合不能重覆";
                        pass = false;
                    }
                    check.Add(key);
                }

                row.Cells[colLevel.Index].ErrorText = string.Empty;
                if (!int.TryParse(str_level, out i))
                {
                    row.Cells[colLevel.Index].ErrorText = "必須為整數數字";
                    pass = false;
                }

                row.Cells[colPercentage.Index].ErrorText = string.Empty;
                if (!int.TryParse(str_percentage, out i))
                {
                    row.Cells[colPercentage.Index].ErrorText = "必須為整數數字";
                    pass = false;
                }
            }

            if (!pass)
                return;


            if (_config.Count > 0)
                _A.DeletedValues(_config);

            List<KCBSSubjectScoreConfig> insert = new List<KCBSSubjectScoreConfig>();

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow)
                    continue;

                KCBSSubjectScoreConfig conf = new KCBSSubjectScoreConfig();
                conf.Subject = row.Cells[colSubject.Index].Value + "";

                int level;
                int percentage;
                int.TryParse(row.Cells[colLevel.Index].Value + "", out level);
                int.TryParse(row.Cells[colPercentage.Index].Value + "", out percentage);
                conf.Level = level;
                conf.Percentage = percentage;

                insert.Add(conf);
            }

            if (insert.Count > 0)
            {
                _A.InsertValues(insert);
            }

            this.Close();

        }
    }
}
