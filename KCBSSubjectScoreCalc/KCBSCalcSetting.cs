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
        Dictionary<string, int> _refDic;

        public KCBSCalcSetting(Dictionary<string, int> dic)
        {
            InitializeComponent();
            _refDic = dic;
            _A = new AccessHelper();
            SetSubjectItems();
        }

        private void SetSubjectItems()
        {
            List<string> students = K12.Presentation.NLDPanels.Student.SelectedSource;
            string id = string.Join(",", students);
            string sql = "select subject from course where id in (select ref_course_id from sc_attend where ref_student_id=" + id + ") group by subject order by subject";

            QueryHelper q = new QueryHelper();
            DataTable dt = q.Select(sql);
            List<string> check = new List<string>();

            foreach (DataRow row in dt.Rows)
            {
                //去除空白
                string subj = (row["subject"] + "").Trim();
                if (!check.Contains(subj))
                {
                    check.Add(subj);
                }
            }

            colSubject.Items.AddRange(check.ToArray());
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (DataValidated())
            {
                BuildData();
                this.Close();
            }
        }

        /// <summary>
        /// 驗證資料
        /// </summary>
        /// <returns></returns>
        private bool DataValidated()
        {
            bool pass = true;
            List<string> check = new List<string>();

            //檢查資料
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

            return pass;
        }

        /// <summary>
        /// 整理資料
        /// </summary>
        private void BuildData()
        {
            //先清除字典內容
            _refDic.Clear();

            //將資料整理到字典中
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow)
                    continue;

                string subj = (row.Cells[colSubject.Index].Value + "").Trim();
                string level = (row.Cells[colLevel.Index].Value + "").Trim();
                string percentage = (row.Cells[colPercentage.Index].Value + "").Trim();

                string key = subj + "#" + level;

                if (!_refDic.ContainsKey(key))
                    _refDic.Add(key, int.Parse(percentage));
            }
        }
    }
}
