using System.Text.Json;
using System.Runtime.Serialization.Json;
using BD2Sichuan;

var evidence=Path.Combine(AppContext.BaseDirectory,"test-data",DateTime.UtcNow.ToString("HHmmss-fff"));
Directory.CreateDirectory(evidence);
int checks=0,boards=0,pairs=0;var groups=new List<string>();
void Check(bool value,string label){checks++;if(!value)throw new Exception(label);}
void Reject(Action action,string label){try{action();}catch(ArgumentException){Check(true,label);return;}throw new Exception(label);}
// Independent oracle: enumerate all horizontal/vertical three-segment polylines.
// Unlike production BFS, there is no direction-state queue or shortest-path pruning.
bool Oracle(int w,int h,int[] b,int a,int z)
{
    if(a==z||!SichuanSolver.Pairable(b[a])||b[a]!=b[z])return false;
    bool Segment(int from,int to)
    {
        int x=from%w,y=from/w,tx=to%w,ty=to/w;
        if(x!=tx&&y!=ty)return false;
        int dx=Math.Sign(tx-x),dy=Math.Sign(ty-y);
        while(true){int i=y*w+x;if(i!=a&&i!=z&&b[i]!=0)return false;if(x==tx&&y==ty)return true;x+=dx;y+=dy;}
    }
    for(int x=0;x<w;x++){int p=(a/w)*w+x,q=(z/w)*w+x;if(Segment(a,p)&&Segment(p,q)&&Segment(q,z))return true;}
    for(int y=0;y<h;y++){int p=y*w+a%w,q=y*w+z%w;if(Segment(a,p)&&Segment(p,q)&&Segment(q,z))return true;}
    return false;
}
void PathValid(int w,int h,int[] cells,SichuanMove m)
{
    Check(m.Path.Length>=2&&m.Path[0]==m.First&&m.Path[^1]==m.Second,"path endpoints");
    int turns=0; (int,int)? previous=null;
    for(int i=1;i<m.Path.Length;i++)
    {
        int p=m.Path[i-1],q=m.Path[i];Check(q>=0&&q<w*h,"path remains in board");
        var d=(q%w-p%w,q/w-p/w);Check(Math.Abs(d.Item1)+Math.Abs(d.Item2)==1,"orthogonal adjacent path");
        if(previous!=null&&previous.Value!=d)turns++;previous=d;
        if(i<m.Path.Length-1)Check(cells[q]==0,"only empty intermediate cells");
    }
    Check(turns<=2,"two-turn maximum");
    Check(m.Path.Distinct().Count()==m.Path.Length,"path has no repeated cells");
}
void Compare(int w,int h,int[] b)
{
    boards++;var moves=SichuanSolver.Moves(w,h,b);var actual=moves.Select(m=>(m.First,m.Second)).ToHashSet();
    Check(actual.Count==moves.Count,"no duplicate pairs");foreach(var m in moves)PathValid(w,h,b,m);
    for(int a=0;a<b.Length;a++)for(int z=a+1;z<b.Length;z++)
    {pairs++;var fast=SichuanFastSolver.Path(w,h,b,a,z);Check((fast!=null)==actual.Contains((a,z)),"fast path differs from independent BFS");if(fast!=null)PathValid(w,h,b,new(a,z,fast));Check(actual.Contains((a,z))==Oracle(w,h,b,a,z),$"oracle mismatch {w}x{h}: {a},{z}: {string.Join(',',b)}");}
}
// Exhaust every arrangement of empty, matching tile and blocker in a 3x3 board.
for(int mask=0;mask<19683;mask++)
{
    int n=mask;var b=new int[9];for(int i=0;i<9;i++){b[i]=new[]{0,1001,10001}[n%3];n/=3;}Compare(3,3,b);
}
groups.Add("all 19,683 ternary 3x3 boards against independent oracle");
var random=new Random(20260914);var kinds=new[]{0,0,0,1001,1002,2001,11001,12001,10001,13001};
for(int n=0;n<500;n++){int w=random.Next(3,9),h=random.Next(3,9);Compare(w,h,Enumerable.Range(0,w*h).Select(_=>kinds[random.Next(kinds.Length)]).ToArray());}
groups.Add("500 seeded mixed tile boards against oracle");
Check(SichuanSolver.Moves(3,1,new[]{1001,10001,1001}).Count==0,"no imaginary outside border");
Check(SichuanSolver.Moves(2,2,new[]{1001,1002,1002,1001}).Count==0,"no diagonal crossing occupied tiles");
Check(SichuanSolver.Moves(2,1,new[]{12001,12001}).Count==0,"locks not clickable pairs");
foreach(var key in new[]{11001,11002,11003})
{
    var b=new[]{key,key,key+1000,12001,12002,12003,10001};var m=SichuanSolver.Moves(7,1,b).Single();var after=SichuanSolver.Apply(7,1,b,m);
    Check(after[0]==0&&after[1]==0&&after[2]==0,"key removes matching lock");
    for(int i=3;i<6;i++)Check(after[i]==(b[i]==key+1000?0:b[i]),"other color locks preserved");
    Check(after[6]==10001,"rock preserved");
}
Check(SichuanSolver.Cleared(new[]{0,10001})&&!SichuanSolver.Cleared(new[]{12001}),"rocks may remain but locks prevent cleared");
Reject(()=>SichuanSolver.Validate(1,1,new[]{1}),"unresolved random tile rejected");
Reject(()=>SichuanSolver.Validate(2,2,new[]{0}),"bad shape rejected");
Reject(()=>SichuanSolver.Apply(2,1,new[]{1001,1002},new(0,1,new[]{0,1})),"illegal move rejected");
groups.Add("source-defined boundary and special tile rules");
var fixture=new[]{0,0,0,0,0,0,0,0,0,1001,1002,11001,11001,1002,1001,0,0,2001,12001,10001,10001,12001,2001,0,0,2002,13001,2003,2003,13001,2002,0,0,1003,1004,1005,1005,1004,1003,0,0,0,0,0,0,0,0,0};
int quick=0;var route=SichuanSolver.Solve(8,6,fixture,3000,quick:m=>{quick++;Check(Oracle(8,6,fixture,m.First,m.Second),"quick pair legal");});
Check(route.Status=="route_found"&&route.Moves.Length==10&&quick==1,"full fixture route");
var rest=fixture;foreach(var m in route.Moves){Check(Oracle(8,6,rest,m.First,m.Second),"each solution move legal in evolving board");PathValid(8,6,rest,m);rest=SichuanSolver.Apply(8,6,rest,m);}
Check(SichuanSolver.Cleared(rest),"route actually clears board");
var first=SichuanSolver.Apply(8,6,fixture,route.Moves[0]);var resumed=SichuanSolver.Continue(8,6,fixture,first,route);
Check(resumed?.Reused==true&&resumed.Moves.Length==route.Moves.Length-1,"exact actual move reuses suffix");
var changed=(int[])first.Clone();var extra=Array.FindIndex(changed,x=>x==1001);changed[extra]=0;
Check(SichuanSolver.Continue(8,6,fixture,changed,route)==null,"extra bonus clear requires replan");
var no=SichuanSolver.Solve(2,2,new[]{1001,1002,1002,1001});Check(no.Status=="no_static_route"&&no.Moves.Length==0,"dead board status");
var limited=SichuanSolver.Solve(4,1,new[]{1001,1001,1002,1002},0);Check(limited.Status=="budget_exhausted"&&limited.Moves.Length==1,"budget fallback is only one legal pair");
using(var cancel=new CancellationTokenSource()){cancel.Cancel();try{SichuanSolver.Solve(8,6,fixture,token:cancel.Token);throw new Exception("cancel ignored");}catch(OperationCanceledException){checks++;}}
groups.Add("full route replay, exact continuation, changed board, cancellation and budget");
Check(SichuanIdentity.IsGameProcessName("BrownDust II"),"actual spaced game name accepted");
Check(SichuanIdentity.IsGameProcessName("browndust ii.exe"),"official executable name accepted case-insensitively");
Check(!SichuanIdentity.IsGameProcessName("BrownDustII"),"mistyped game name is not a separate compatibility alias");
Check(!SichuanIdentity.IsGameProcessName("BD2PrivateWorkbench"),"unrelated process name rejected");
var now=DateTime.UtcNow;var snap=new SichuanSnapshot{SessionId="test",Runtime="BD2Sichuan.Runtime2",CapturedAtUtc=now.ToString("O"),Width=8,Height=6,Cells=fixture,BoardHash=SichuanSolver.Hash(8,6,fixture),State="ready",ExecutionProtocol=3};
Check(SichuanLiveFiles.Invalid(snap,now,false)==null,"fresh snapshot accepted");
Check(SichuanLiveFiles.Invalid(snap,now.AddSeconds(4),false)!=null,"stale heartbeat rejected");
Check(SichuanLiveFiles.Invalid(snap,now.AddSeconds(-3),false)!=null,"future timestamp rejected");
snap.Runtime="BD2ArenaDefenseWatcher.Active.Runtime33";Check(SichuanLiveFiles.Invalid(snap,now,false)!=null,"old runtime rejected");snap.Runtime="BD2Sichuan.Runtime2";
snap.BoardHash="tampered";Check(SichuanLiveFiles.Invalid(snap,now,false)!=null,"bad hash rejected");snap.BoardHash=SichuanSolver.Hash(8,6,fixture);
snap.ProcessId=int.MaxValue;Check(SichuanLiveFiles.Invalid(snap,now)!=null,"exited process rejected");
snap.ProcessId=Environment.ProcessId;Check(SichuanLiveFiles.Invalid(snap,now)!=null,"live non-game process rejected");
if(args.Length==2&&args[0]=="--live-snapshot")
{
    var live=new SichuanLiveFiles(Path.GetFullPath(args[1])).Read()!;
    var error=SichuanLiveFiles.Invalid(live,DateTime.UtcNow);
    Check(error==null,"actual standalone snapshot and spaced game process accepted: "+error);
    File.WriteAllText(Path.Combine(evidence,"live-process-check.json"),JsonSerializer.Serialize(new{status="pass",live.ProcessId,live.Runtime,live.State,live.Width,live.Height,live.CapturedAtUtc,gameRequests=0,injection=false}));
}
var key1=SichuanLiveFiles.Key(snap);snap.Generation++;Check(SichuanLiveFiles.Key(snap)!=key1,"same board on new level not reused");
key1=SichuanLiveFiles.Key(snap);snap.SessionId="new process";Check(SichuanLiveFiles.Key(snap)!=key1,"same board on restarted runtime not reused");
var files=new SichuanLiveFiles(Path.Combine(evidence,"files"));files.Lease(true);var ticks=long.Parse(File.ReadAllText(Path.Combine(files.Root,"enabled-until.txt")));Check(ticks>DateTime.UtcNow.Ticks&&ticks<DateTime.UtcNow.AddSeconds(6).Ticks,"lease bounded to five seconds");
files.Lease(false);Check(File.ReadAllText(Path.Combine(files.Root,"enabled-until.txt"))=="0","lease explicitly disabled");
using(var f=File.Create(Path.Combine(files.Root,"latest.json")))new DataContractJsonSerializer(typeof(SichuanSnapshot)).WriteObject(f,snap);
var read=files.Read()!;Check(read.Cells.SequenceEqual(fixture)&&SichuanLiveFiles.Key(read)==SichuanLiveFiles.Key(snap),"Mono-compatible serializer roundtrip");
Check(SichuanLiveFiles.Invalid(read,now,false)==null,"Hook serializer accepted by GUI validator");
read.Cells=null!;Check(SichuanLiveFiles.Invalid(read,now,false)!=null,"null wire cells rejected");
groups.Add("freshness, runtime/process/generation isolation, atomic lease, wire serialization");
checks+=ExecutionTests.Run(evidence);checks+=IntervalTests.Run(evidence);groups.Add("guarded single/automatic execution, idempotency, readback, stop and wire roundtrip");
var report=new{status="pass",checks,boards,pairs,groups,gameRequests=0,injection=false};
File.WriteAllText(Path.Combine(evidence,"results.json"),JsonSerializer.Serialize(report,new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine(JsonSerializer.Serialize(report));Console.WriteLine(evidence);
