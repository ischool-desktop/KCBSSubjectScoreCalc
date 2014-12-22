using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCBSSubjectScoreCalc
{
    class Permission
    {
        public static string 康橋學期科目成績調整計算 { get { return "KCBSSubjectScoreCalc.CC7A52D3-75E1-4BD6-8782-6B3D6BB2829E"; } }

        public static bool 康橋學期科目成績調整計算權限
        {
            get { return FISCA.Permission.UserAcl.Current[康橋學期科目成績調整計算].Executable; }
        }
    }
}
