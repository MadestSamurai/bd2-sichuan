using System.Diagnostics;
using System.Runtime.Serialization.Json;
using System.Text.Json;
using BD2Sichuan;
internal static class ExecutionTests
{
    public static int Run(string evidence)
    {
        int checks=0;void Check(bool ok,string text){checks++;if(!ok)throw new Exception(text);}
        var now=new DateTime(2026,9,14,8,0,0,DateTimeKind.Utc);
        SichuanSnapshot Board()=>new(){ExecutionProtocol=3,SessionId="session",ProcessId=321,Generation=3,Width=6,Height=1,
            Cells=new[]{1001,1001,1002,1002,1003,1003},InputReady=true,State="ready",RemainingSeconds=90,SelectedIndex=-1};
        SichuanRunCommand Command()=>new(){OwnerId=Guid.NewGuid().ToString("N"),SessionId="session",ProcessId=321,CreatedUtcTicks=now.Ticks,Automatic=true,IntervalMilliseconds=50};
        SichuanActionLease Lease(SichuanRunCommand c)=>new(){OwnerId=c.OwnerId,UntilUtcTicks=now.AddMinutes(5).Ticks,IntervalMilliseconds=c.IntervalMilliseconds};
        var s=Board();var c=Command();var lease=Lease(c);var engine=new SichuanRunEngine();int calls=0;double clock=1000;
        bool Apply(SichuanFastMove m){calls++;s.Cells=SichuanExecutionGuard.ExpectedAfter(s.Cells,m.First,m.Second);return true;}
        void Tick()=>engine.Tick(s,c,lease,now,clock,SichuanFastSolver.Choose,Apply,()=>s);
        Tick();Check(calls==1&&engine.Status.ConfirmedPairs==1&&engine.Receipt.Status=="completed","logical readback completes immediately");
        clock=1049;Tick();Check(calls==1,"49ms cannot dispatch next pair");
        clock=1050;s.AnimationCount=12;Tick();Check(calls==2,"50ms native fast cadence permits overlapping removal animations");
        clock=1100;s.State="paused";Tick();Check(engine.Active&&calls==2&&engine.Status.Reason=="paused","pause preserves intent without inputs");
        s.State="ready";s.InputReady=false;Tick();Check(engine.Active&&calls==2,"input lock waits");
        s.InputReady=true;s.SelectedIndex=4;Tick();Check(engine.Active&&calls==2&&engine.Status.Reason=="manual_selection_present","manual choice waits without revoking run");
        s.SelectedIndex=-1;s.State="shuffling";Tick();Check(engine.Active&&calls==2,"shuffle suspends inputs");
        s.State="ready";Tick();Check(calls==3&&!engine.Active&&engine.Status.Reason=="board_cleared","resume completes same round without extra settle observation");
        s.State="paused";clock=1200;Tick();Check(calls==3&&engine.Status.Reason=="board_cleared","settlement pause cannot overwrite successful completion");
        Tick();Check(calls==3,"consumed command never replays");
        foreach(var reason in new[]{"user_stop","window_closed","capture_disabled",""})
        {
            s=Board();c=Command();lease=Lease(c);engine=new();calls=0;clock=1000;Tick();lease.UntilUtcTicks=now.Ticks;lease.StopReason=reason;clock+=50;Tick();
            Check(calls==1&&!engine.Active&&engine.Status.Reason==(reason.Length>0?reason:"execution_heartbeat_expired"),"precise lease stop reason: "+reason);
        }
        foreach(var mutate in new Action<SichuanSnapshot>[] {v=>v.SessionId="other",v=>v.ProcessId++,v=>v.ExecutionProtocol=2})
        {s=Board();c=Command();lease=Lease(c);engine=new();calls=0;clock=1000;Tick();mutate(s);clock+=50;Tick();Check(calls==1&&!engine.Active&&engine.Status.Reason=="session_changed","different session cannot continue inputs");}
        s=Board();c=Command();lease=Lease(c);engine=new();calls=0;clock=1000;Tick();s.Generation++;clock+=50;Tick();Check(calls==1&&!engine.Active&&engine.Status.Reason=="round_changed","one explicit run does not cross rounds");
        s=Board();c=Command();lease=Lease(c);engine=new();calls=0;clock=1000;Tick();s.State="finished";s.RemainingSeconds=0;clock+=50;Tick();Check(engine.Status.Reason=="time_finished","timeout differs from clear");
        foreach(var confirmed in new[]{false,true})
        {
            s=Board();c=Command();lease=Lease(c);engine=new();int inputs=0;
            void Failed()=>engine.Tick(s,c,lease,now,1000,SichuanFastSolver.Choose,m=>{inputs++;return confirmed;},()=>s);
            Failed();Failed();Check(inputs==1&&!engine.Active&&engine.Receipt.Status=="unconfirmed","unconfirmed event/readback never retried");
        }
        s=Board();c=Command();c.Automatic=false;lease=Lease(c);engine=new();calls=0;clock=1000;Tick();Check(calls==1&&!engine.Active&&engine.Status.Reason=="single_pair_completed","single pair stops on confirmation");
        s=Board();c=Command();lease=Lease(c);engine=new();
        engine.Tick(s,c,lease,now,1000,SichuanFastSolver.Choose,m=>{s.Cells=new int[6];s.State="paused";return true;},()=>s);
        Check(engine.Receipt.Reason=="replan_after_game_effects"&&engine.Status.Reason=="board_cleared","extra combo clears and settlement pause complete immediately");
        s=Board();c=Command();lease=Lease(c);engine=new();calls=0;clock=1000;s.State="waiting_board";s.Cells=Array.Empty<int>();Tick();Check(engine.Active&&calls==0,"can arm before board");
        s=Board();Tick();Check(calls==1&&engine.Active,"armed request binds real board once");
        foreach(var mutate in new Action<SichuanRunCommand>[] {v=>v.Protocol=2,v=>v.SessionId="old",v=>v.ProcessId++,v=>v.CreatedUtcTicks=now.AddSeconds(-4).Ticks,v=>v.CreatedUtcTicks=now.AddSeconds(2).Ticks})
        {s=Board();c=Command();mutate(c);lease=Lease(c);engine=new();calls=0;Tick();Check(calls==0&&!engine.Active,"old/invalid run cannot dispatch");}
        foreach(var mutate in new Action<SichuanFastMove>[] {v=>v.First=-1,v=>v.Second=9,v=>v.Second=v.First,v=>v.Second=2,v=>v.Path=new[]{1,0},v=>v.Path=new[]{0,2,1},v=>v.Path=new[]{0,1,0,1}})
        {s=Board();var m=SichuanFastSolver.Choose(s);mutate(m);Check(SichuanExecutionGuard.Reject(s,m).Length>0,"invalid path rejected");}
        foreach(var mutate in new Action<SichuanSnapshot>[] {v=>v.ExecutionProtocol=2,v=>v.State="paused",v=>v.InputReady=false,v=>v.SelectedIndex=0,v=>v.RemainingSeconds=0,v=>v.Width=1})
        {s=Board();var m=SichuanFastSolver.Choose(s);mutate(s);Check(SichuanExecutionGuard.Reject(s,m).Length>0,"invalid board rejected");}
        var controller=new SichuanAutomationController();s=Board();var command=controller.Start(true,s,now);Check(controller.Active,"controller armed");
        s.Run=new(){OwnerId=command.OwnerId,State="waiting",Reason="paused",ConfirmedPairs=4};controller.Observe(s,now);Check(controller.Active&&controller.CompletedPairs==4,"observer preserves pause");
        s.Run.State="completed";s.Run.Reason="board_cleared";controller.Observe(s,now);Check(!controller.Active&&controller.Status.Contains("清空"),"observer completes successful clear");
        var files=new SichuanLiveFiles(Path.Combine(evidence,"control"));
        using(var link=new SichuanControlLink(files))
        {
            c=Command();c.CreatedUtcTicks=DateTime.UtcNow.Ticks;link.Start(c);
            var before=JsonSerializer.Deserialize<SichuanActionLease>(File.ReadAllText(Path.Combine(files.Root,"execution-lease.json")))!;
            Thread.Sleep(650); // No UI dispatcher exists: renewal must still happen.
            var after=JsonSerializer.Deserialize<SichuanActionLease>(File.ReadAllText(Path.Combine(files.Root,"execution-lease.json")))!;
            Check(after.UntilUtcTicks>before.UntilUtcTicks,"heartbeat independent of UI dispatcher");
            Check(after.UntilUtcTicks<DateTime.UtcNow.AddSeconds(3.1).Ticks,"crashed UI lease bounded to three seconds");
            link.Stop("user_stop");Thread.Sleep(350);
            var stopped=JsonSerializer.Deserialize<SichuanActionLease>(File.ReadAllText(Path.Combine(files.Root,"execution-lease.json")))!;
            Check(stopped.UntilUtcTicks==0&&stopped.StopReason=="user_stop","late heartbeat cannot re-arm explicit stop");
            using var wire=new MemoryStream(File.ReadAllBytes(Path.Combine(files.Root,"run-command.json")));
            var decoded=(SichuanRunCommand)new DataContractJsonSerializer(typeof(SichuanRunCommand)).ReadObject(wire)!;
            Check(decoded.Protocol==3&&decoded.OwnerId==c.OwnerId,"GUI to Mono run command wire roundtrip");
        }
        Check(File.ReadAllText(Path.Combine(files.Root,"enabled-until.txt"))=="0","dispose releases capture");
        // Actual recorded boards, compared against existing independent BFS implementation.
        var replayPath="artifacts/validation/sichuan-speed-20260914/runtime35-actions.json";
        var timing=new List<double>();int recorded=0;
        if(File.Exists(replayPath))
        {
            var records=JsonSerializer.Deserialize<SichuanActionReceipt[]>(File.ReadAllText(replayPath))!.Where(r=>r.Status=="dispatched").ToArray();
            foreach(var r in records)
            {
                s=Board();s.Width=18;s.Height=9;s.Cells=r.BeforeCells;
                Check(s.Cells.Length==162,"recorded live board shape");
                for(int warm=0;warm<3;warm++)SichuanFastSolver.Choose(s);
                var watch=Stopwatch.StartNew();var m=SichuanFastSolver.Choose(s);timing.Add(watch.Elapsed.TotalMilliseconds);
                Check(m!=null&&SichuanExecutionGuard.Reject(s,m)=="","fast choice valid on actual live board");
                Check(SichuanSolver.Moves(18,9,s.Cells).Any(x=>x.First==m!.First&&x.Second==m.Second),"independent BFS agrees with fast live choice");recorded++;
            }
        }
        timing.Sort();double Percentile(double p)=>timing.Count==0?0:timing[(int)((timing.Count-1)*p)];
        File.WriteAllText(Path.Combine(evidence,"fast-execution.json"),JsonSerializer.Serialize(new{checks,recordedBoards=recorded,chooseP50Ms=Percentile(.5),chooseP95Ms=Percentile(.95),configuredPairIntervalMs=50,liveStandaloneVerified=false},new JsonSerializerOptions{WriteIndented=true}));
        return checks;
    }
}
