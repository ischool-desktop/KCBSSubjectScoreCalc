using Aspose.Cells;
using FISCA.Presentation.Controls;
using SHSchool.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KCBSSubjectScoreReport
{
    public partial class PrintForm : BaseForm
    {
        BackgroundWorker BGW = new BackgroundWorker();

        List<string> StudentIDList { get; set; }

        string _SchoolYear { get; set; }
        string _Semester { get; set; }

        Dictionary<string, SHStudentRecord> StudentDic { get; set; }

        Dictionary<string, List<SubjectObj>> kcbsDic { get; set; }

        Dictionary<string, List<SHSemesterScoreRecord>> SemesterScoreDic { get; set; }

        Dictionary<string, int> ColumnIndexDic { get; set; }
        public PrintForm()
        {
            InitializeComponent();

            BGW.DoWork += BGW_DoWork;
            BGW.RunWorkerCompleted += BGW_RunWorkerCompleted;

            _SchoolYear = K12.Data.School.DefaultSchoolYear;
            _Semester = K12.Data.School.DefaultSemester;

            intSchoolYear.Value = int.Parse(K12.Data.School.DefaultSchoolYear);
            intSemester.Value = int.Parse(K12.Data.School.DefaultSemester);
        }


        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (!BGW.IsBusy)
            {
                StudentIDList = K12.Presentation.NLDPanels.Student.SelectedSource;
                _SchoolYear = intSchoolYear.Value.ToString();
                _Semester = intSemester.Value.ToString();
                btnPrint.Enabled = false;
                BGW.RunWorkerAsync();
            }
            else
            {
                MsgBox.Show("系統忙碌中請稍後!");
            }
        }

        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            kcbsDic = GetkcbsDic(StudentIDList);

            StudentDic = GetStudent(StudentIDList);

            SemesterScoreDic = GetSemesterScore(StudentIDList);

            foreach (string studentID in kcbsDic.Keys)
            {
                if (SemesterScoreDic.ContainsKey(studentID))
                {
                    Marges(kcbsDic[studentID], SemesterScoreDic[studentID]);
                }
            }

        }

        void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnPrint.Enabled = true;
            if (e.Cancelled)
            {
                MsgBox.Show("報表列印已中止!");
                return;
            }


            if (e.Error != null)
            {
                MsgBox.Show("列印發生錯誤:\n" + e.Error.Message);
                return;
            }


            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "*.xlsx|*.xlsx";
            dialog.FileName = "成績調整資訊報表";
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                MsgBox.Show("已取消儲存!");
                return;
            }

            Workbook book = new Workbook();
            book.Worksheets.Clear();

            Worksheet sheet = book.Worksheets[book.Worksheets.Add()];

            SetSheetColume(sheet);

            List<string> StudentIDList = kcbsDic.Keys.ToList();
            StudentIDList.Sort(SortStudent);

            int rowIndex = 1;
            //每一個學生
            foreach (string studentid in StudentIDList)
            {
                //每一筆資料
                foreach (SubjectObj sub in kcbsDic[studentid])
                {
                    if (sub.student != null)
                    {
                        sheet.Cells[rowIndex, ColumnIndexDic["班級"]].Value = sub.student.Class != null ? sub.student.Class.Name : "";
                        sheet.Cells[rowIndex, ColumnIndexDic["座號"]].Value = sub.student.SeatNo.HasValue ? sub.student.SeatNo.Value.ToString() : "";
                        sheet.Cells[rowIndex, ColumnIndexDic["學號"]].Value = sub.student.StudentNumber;
                        sheet.Cells[rowIndex, ColumnIndexDic["姓名"]].Value = sub.student.Name;
                        sheet.Cells[rowIndex, ColumnIndexDic["學年度"]].Value = sub.kcbs_sore.school_year;
                        sheet.Cells[rowIndex, ColumnIndexDic["學期"]].Value = sub.kcbs_sore.semester;
                        sheet.Cells[rowIndex, ColumnIndexDic["科目名稱"]].Value = sub.kcbs_sore.subject;
                        sheet.Cells[rowIndex, ColumnIndexDic["科目級別"]].Value = sub.kcbs_sore.subject_level;
                        sheet.Cells[rowIndex, ColumnIndexDic["原始成績"]].Value = sub.SubjectScore.Score.HasValue ? sub.SubjectScore.Score.Value.ToString() : "";
                        sheet.Cells[rowIndex, ColumnIndexDic["原原始成績"]].Value = sub.kcbs_sore.originalscore;

                        sheet.Cells[rowIndex, ColumnIndexDic["調整比例"]].Value = GetPercent(sub.kcbs_sore.percentage);
                        sheet.Cells[rowIndex, ColumnIndexDic["使用者"]].Value = sub.kcbs_sore.user;
                        rowIndex++;
                    }
                }
            }

            try
            {
                book.Save(dialog.FileName);
            }
            catch (Exception ex)
            {
                MsgBox.Show("儲存發生錯誤:\n" + ex.Message);
                return;
            }

            Process.Start(dialog.FileName);

        }

        private int SortStudent(string kjr1, string kjr2)
        {
            SHStudentRecord s1 = StudentDic[kjr1];
            SHStudentRecord s2 = StudentDic[kjr2];

            string StudentSort1 = s1.Class != null ? (s1.Class.GradeYear.HasValue ? s1.Class.GradeYear.Value.ToString().PadLeft(2, '0') : "00") : "00";
            string StudentSort2 = s2.Class != null ? (s2.Class.GradeYear.HasValue ? s2.Class.GradeYear.Value.ToString().PadLeft(2, '0') : "00") : "00";

            StudentSort1 += s1.Class != null ? s1.Class.Name.PadLeft(10, '0') : "0000000000";
            StudentSort2 += s2.Class != null ? s2.Class.Name.PadLeft(10, '0') : "0000000000";

            StudentSort1 += s1.SeatNo.HasValue ? s1.SeatNo.Value.ToString().PadLeft(3, '0') : "000";
            StudentSort2 += s2.SeatNo.HasValue ? s2.SeatNo.Value.ToString().PadLeft(3, '0') : "000";

            return StudentSort1.CompareTo(StudentSort2);
        }

        /// <summary>
        /// 取得與轉換%值
        /// </summary>
        private string GetPercent(decimal percentage)
        {
            return percentage + "%";
        }

        /// <summary>
        /// 建立Column相關資訊
        /// </summary>
        private void SetSheetColume(Worksheet sheet)
        {
            List<string> ColumnsName = GetColumnName();

            ColumnIndexDic = new Dictionary<string, int>();

            int x = 0;
            foreach (string each in ColumnsName)
            {
                if (!ColumnIndexDic.ContainsKey(each))
                {
                    ColumnIndexDic.Add(each, x);

                    //建立Column
                    sheet.Cells[0, x].Value = each;
                    x++;
                }
            }
        }

        private List<string> GetColumnName()
        {
            List<string> ColumnsName = new List<string>();
            ColumnsName.Add("班級");
            ColumnsName.Add("座號");
            ColumnsName.Add("學號");
            ColumnsName.Add("姓名");
            ColumnsName.Add("學年度");
            ColumnsName.Add("學期");
            ColumnsName.Add("科目名稱");
            ColumnsName.Add("科目級別");
            ColumnsName.Add("原始成績");
            ColumnsName.Add("原原始成績");
            ColumnsName.Add("調整比例");
            ColumnsName.Add("使用者");
            return ColumnsName;
        }

        private void Marges(List<SubjectObj> list1, List<SHSemesterScoreRecord> list2)
        {
            foreach (SubjectObj each in list1)
            {
                foreach (SHSemesterScoreRecord semesterScore in list2)
                {
                    foreach (SHSubjectScore subject in semesterScore.Subjects.Values)
                    {
                        if (CheckSubject(each, subject))
                        {
                            each.SubjectScore = subject;
                            each.student = semesterScore.Student;
                        }
                    }
                }
            }
        }

        private Dictionary<string, List<SHSemesterScoreRecord>> GetSemesterScore(List<string> StudentIDList)
        {
            Dictionary<string, List<SHSemesterScoreRecord>> dic = new Dictionary<string, List<SHSemesterScoreRecord>>();
            List<SHSemesterScoreRecord> SemesterScoreList = SHSemesterScore.SelectByStudentIDs(StudentIDList);
            foreach (SHSemesterScoreRecord ssr in SemesterScoreList)
            {
                if (ssr.SchoolYear.ToString() == _SchoolYear && ssr.Semester.ToString() == _Semester)
                {
                    if (!dic.ContainsKey(ssr.RefStudentID))
                        dic.Add(ssr.RefStudentID, new List<SHSemesterScoreRecord>());

                    dic[ssr.RefStudentID].Add(ssr);
                }
            }

            return dic;
        }

        /// <summary>
        /// 是否為相同科目
        /// </summary>
        private bool CheckSubject(SubjectObj each, SHSubjectScore subjectscore)
        {
            if (each.kcbs_sore.subject == subjectscore.Subject)
            {
                if (subjectscore.Level.HasValue)
                {
                    if (each.kcbs_sore.subject_level == subjectscore.Level.Value.ToString())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 取得科目成績Log UDT 的物件清單
        /// </summary>
        private Dictionary<string, List<SubjectObj>> GetkcbsDic(List<string> StudentIDList)
        {
            Dictionary<string, List<SubjectObj>> dic = new Dictionary<string, List<SubjectObj>>();
            string studentIDL = string.Join("','", StudentIDList);
            DataTable dt = tool._Q.Select(string.Format("select * from $kcbs.subjectscorelog where ref_student_id in ('{0}') and school_year='{1}' and semester='{2}'", studentIDL, _SchoolYear, _Semester));

            foreach (DataRow each in dt.Rows)
            {
                //資料轉換
                ScoreRow sRow = new ScoreRow(each);

                //每一個科目
                SubjectObj s = new SubjectObj(sRow);

                if (!dic.ContainsKey(sRow.ref_student_id))
                    dic.Add(sRow.ref_student_id, new List<SubjectObj>());

                dic[sRow.ref_student_id].Add(s);
            }

            return dic;
        }

        /// <summary>
        /// 取得學生
        /// </summary>
        private Dictionary<string, SHStudentRecord> GetStudent(List<string> StudentIDList)
        {
            Dictionary<string, SHStudentRecord> dic = new Dictionary<string, SHStudentRecord>();
            foreach (SHSchool.Data.SHStudentRecord student in SHSchool.Data.SHStudent.SelectByIDs(StudentIDList))
            {
                if (!dic.ContainsKey(student.ID))
                    dic.Add(student.ID, student);
            }

            return dic;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
