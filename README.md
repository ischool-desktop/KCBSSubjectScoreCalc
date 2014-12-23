KCBSSubjectScoreCalc
====================

KCBSSubjectScoreCalc
學生->教務->成績作業->計算學期科目成績->康橋計算學期科目成績(中學部)

KCBSSubjectScoreReport
學生->資料統計->報表->康橋客製報表->成績調整資訊報表

====================

透過FISCA提供的動態串連寫法
來達到掛載此模組時,才提供成績調整功能
        dynamic obj = new ExpandoObject();
        obj.GetRange = new Func<string, List<string>>(DoSomething);
        InteractionService.RegisterAPI("CaredSummary", obj);

        dynamic summary = InteractionService.DiscoverAPI("CaredSummary");
        List<string> studs = summary.GetRange("yaoming") as List<string>;

        private static List<string> DoSomething(string args)
        {
            return null;
        }
