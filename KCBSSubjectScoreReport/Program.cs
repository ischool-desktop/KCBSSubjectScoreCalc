using FISCA;
using FISCA.Permission;
using FISCA.Presentation;
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
            MenuButton mb = NLDPanels.Student.RibbonBarItems["資料統計"]["報表"]["康橋客製報表"];
            mb["成績調整資訊報表"].Enable = false;
            mb["成績調整資訊報表"].Click += delegate
            {
                PrintForm p = new PrintForm();
                p.ShowDialog();
            };

            NLDPanels.Student.SelectedSourceChanged += delegate
            {
                bool selectCount = NLDPanels.Student.SelectedSource.Count > 0;
                mb["成績調整資訊報表"].Enable = selectCount && Permissions.成績調整資訊報表權限;

            };

            Catalog detail1 = RoleAclSource.Instance["學生"]["報表"];
            detail1.Add(new RibbonFeature(Permissions.成績調整資訊報表, "成績調整資訊報表"));

        }
    }
}
