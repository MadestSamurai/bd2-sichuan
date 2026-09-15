using BD2Sichuan;
internal static class IntervalTests
{
    public static int Run(string evidence)
    {
        int checks=0;void Check(bool ok,string label){checks++;if(!ok)throw new Exception(label);}
        Check(new SichuanRunCommand().IntervalMilliseconds==1000&&new SichuanActionLease().IntervalMilliseconds==1000&&new SichuanSettings().IntervalMilliseconds==1000,"default 1000 across UI command and lease");
        var now=DateTime.UtcNow;var s=new SichuanSnapshot{SessionId="s",ProcessId=1,Generation=1,ExecutionProtocol=3,Width=8,Height=1,Cells=new[]{1001,1001,1002,1002,1003,1003,1004,1004},State="ready",InputReady=true,RemainingSeconds=90};
        var c=new SichuanRunCommand{OwnerId="owner",SessionId="s",ProcessId=1,CreatedUtcTicks=now.Ticks,Automatic=true};
        var lease=new SichuanActionLease{OwnerId="owner",UntilUtcTicks=now.AddHours(1).Ticks};
        var engine=new SichuanRunEngine();int inputs=0;
        void Tick(double time)=>engine.Tick(s,c,lease,now,time,SichuanFastSolver.Choose,m=>{inputs++;s.Cells=SichuanExecutionGuard.ExpectedAfter(s.Cells,m.First,m.Second);return true;},()=>s);
        Tick(0);Tick(999);Check(inputs==1,"1000ms default has no premature second pair");Tick(1000);Check(inputs==2,"1000ms default dispatches on boundary");
        lease.IntervalMilliseconds=5000;Tick(2000);Check(inputs==2&&engine.Status.IntervalMilliseconds==5000,"slower live setting extends pending wait from last pair");
        lease.IntervalMilliseconds=50;Tick(2001);Check(inputs==3&&engine.Status.IntervalMilliseconds==50,"faster live setting advances next pair without restart");
        Tick(2050);Check(inputs==3,"updated minimum spacing enforced");Tick(2051);Check(inputs==4&&engine.Status.Reason=="board_cleared","last pair still confirmed at dynamic boundary");
        var settings=System.IO.Path.Combine(evidence,"interval-settings.json");Check(SichuanSettings.Read(settings).IntervalMilliseconds==1000,"missing settings default");
        foreach(var value in new[]{50,1000,1723,60000}){new SichuanSettings{IntervalMilliseconds=value}.Save(settings);Check(SichuanSettings.Read(settings).IntervalMilliseconds==value,"custom and endpoint setting persists");}
        foreach(var value in new[]{-1,0,49,60001,int.MaxValue})
        {
            bool rejected=false;try{new SichuanSettings{IntervalMilliseconds=value}.Save(settings);}catch(ArgumentOutOfRangeException){rejected=true;}Check(rejected,"invalid setting rejected");
            Check(!SichuanRunEngine.ValidInterval(value),"runtime rejects invalid interval");
        }
        var live=new SichuanLiveFiles(Path.Combine(evidence,"live-interval"));
        using(var link=new SichuanControlLink(live))
        {
            link.Start(new(){OwnerId="interval-test",SessionId="s",ProcessId=1,CreatedUtcTicks=DateTime.UtcNow.Ticks});
            link.SetInterval(1723);
            Check(SichuanJson.Read<SichuanActionLease>(Path.Combine(live.Root,"execution-lease.json"))!.IntervalMilliseconds==1723,"live interval is sent immediately, without UI poll");
        }
        var state=new SichuanConnectionState{ProcessId=21,ProcessStartTicks=300};
        Check(SichuanConnection.SameProcess(state,21,300)&&!SichuanConnection.SameProcess(state,21,301),"PID reuse does not reuse injected address");
        Check(!SichuanConnection.Fresh(new(){ProcessId=1,Runtime="BD2ArenaDefenseWatcher.Active.Runtime36",AtUtc=now.ToString("O")},1,now.AddSeconds(-1)),"workbench runtime cannot satisfy standalone connection");
        return checks;
    }
}
