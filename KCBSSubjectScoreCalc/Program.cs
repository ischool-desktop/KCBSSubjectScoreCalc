using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SmartSchool.Evaluation.Process.Wizards;
using SmartSchool.Customization.Data;
using System.Xml;
using FISCA.UDT;
using FISCA.Permission;

namespace KCBSSubjectScoreCalc
{
    public class Program
    {
        [FISCA.MainMethod]
        public static void main()
        {
            FISCA.InteractionService.RegisterAPI<ISubjectCalcPostProcess>(new Instance());

            //權限設定
            Catalog permission = RoleAclSource.Instance["學生"]["功能按鈕"];
            permission.Add(new RibbonFeature(Permission.康橋學期科目成績調整計算, "康橋學期科目成績調整計算"));
        }

        public class Instance : ISubjectCalcPostProcess
        {
            FISCA.UDT.AccessHelper _A;
            Dictionary<string, int> _subjDic;

            public Instance()
            {
                _A = new FISCA.UDT.AccessHelper();
                _subjDic = new Dictionary<string, int>();
            }

            public void ShowConfigForm()
            {
                if (!Permission.康橋學期科目成績調整計算權限)
                    return;

                new KCBSCalcSetting(_subjDic).ShowDialog();
            }

            public void PostProcess(int schoolYear, int semester, List<StudentRecord> list)
            {
                if (!Permission.康橋學期科目成績調整計算權限)
                    return;

                string user = FISCA.Authentication.DSAServices.UserAccount;

                List<string> ids = list.Select(x => x.StudentID).ToList();
                string str_id = string.Join("','", ids);

                if (ids.Count <= 0)
                    return;

                List<KCBSSubjectScoreLog> records = _A.Select<KCBSSubjectScoreLog>(string.Format("ref_student_id in ('{0}') and school_year={1} and semester={2}", str_id, schoolYear, semester));
                List<KCBSSubjectScoreLog> insert = new List<KCBSSubjectScoreLog>();

                foreach (StudentRecord stu in list)
                {
                    XmlElement xml = stu.Fields["SemesterSubjectCalcScore"] as XmlElement;
                    foreach (XmlElement elem in xml.SelectNodes("//Subject"))
                    {
                        string subj_name = elem.GetAttribute("科目").Trim();
                        string level = elem.GetAttribute("科目級別").Trim();
                        string score = elem.GetAttribute("原始成績").Trim();
                        decimal percentage = 0m;

                        string key = subj_name + "#" + level;

                        if (_subjDic.ContainsKey(key))
                        {
                            percentage = _subjDic[key] / 100m;

                            decimal new_score;

                            decimal.TryParse(score, out new_score);

                            new_score = new_score + (new_score * percentage);

                            new_score = Math.Round(new_score, 0, MidpointRounding.AwayFromZero);
                            double db_score = (double)new_score;
                            new_score = (decimal)db_score;

                            elem.SetAttribute("原始成績", new_score + "");

                            KCBSSubjectScoreLog log = new KCBSSubjectScoreLog();
                            log.RefStudentID = stu.StudentID;
                            log.SchoolYear = schoolYear;
                            log.Semester = semester;
                            log.SubjectName = subj_name;
                            decimal s;
                            decimal.TryParse(score, out s);
                            log.Score = s;
                            int l;
                            int.TryParse(level, out l);
                            log.SubjectLevel = l;
                            log.Percentage = _subjDic[key];
                            log.User = user;

                            insert.Add(log);
                        }
                    }
                }

                if (records.Count > 0)
                    _A.DeletedValues(records);

                if (insert.Count > 0)
                    _A.InsertValues(insert);
                 
            }
        }
    }
}
