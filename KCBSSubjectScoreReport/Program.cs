using FISCA;
using FISCA.Permission;
using K12.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCBSSubjectScoreReport
{
    public class Program
    {
        [MainMethod()]
        static public void Main()
        {

            NLDPanels.Student.RibbonBarItems["資料統計"]["報表"]["康橋客製報表"]["成績調整資訊報表"].Click += delegate
            {
                PrintForm p = new PrintForm();
                p.ShowDialog();
            };

            Catalog detail1 = RoleAclSource.Instance["學生"]["報表"];
            detail1.Add(new RibbonFeature(Permissions.成績調整資訊報表, "成績調整資訊報表"));

        }
    }
}
