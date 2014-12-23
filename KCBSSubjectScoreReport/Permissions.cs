using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCBSSubjectScoreReport
{
    class Permissions
    {

        public static string 成績調整資訊報表 { get { return "KCBS.SubjectScoreLog.Report.PrintForm.cs"; } }
        public static bool 成績調整資訊報表權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[成績調整資訊報表].Executable;
            }
        }

    }
}
