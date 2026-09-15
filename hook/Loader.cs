using System;
using System.IO;
using System.Linq;
using System.Reflection;
namespace BD2Sichuan.Runtime
{
    public static class Loader
    {
        private static object engine;private static bool resolver;
        public static void Load()
        {
            lock(typeof(Loader))
            {
                if(!resolver){AppDomain.CurrentDomain.AssemblyResolve+=Resolve;resolver=true;}
                try
                {
                    var own=typeof(Loader).Assembly;
                    var conflict=AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a=>a!=own&&new[]{"BD2Sichuan.Runtime","BD2Fishing.Runtime","BD2ArenaDefenseWatcher.Active.Runtime","BD2ReplayCaptureHook."}.Any(p=>(a.GetName().Name??"").StartsWith(p,StringComparison.Ordinal)));
                    if(conflict!=null)throw new InvalidOperationException("当前游戏已加载其他BD2模块，请正常重启游戏后仅连接连连看工具："+conflict.GetName().Name);
                    if(engine==null)engine=Activator.CreateInstance(own.GetType("BD2Sichuan.Runtime.RuntimeEngine",true),true);
                    engine.GetType().GetMethod("Start",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(engine,null);
                }
                catch(Exception e)
                {
                    try{Unload();}catch{}
                    WriteStatus("error",e.GetBaseException().Message);
                }
            }
        }
        public static void Unload()
        {lock(typeof(Loader)){if(engine!=null)engine.GetType().GetMethod("Stop",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(engine,null);engine=null;WriteStatus("inactive","");}}
        internal static void WriteStatus(string state,string error)
        {try{LocalStorage.WriteJsonAtomically(Path.Combine(LocalStorage.DataRoot,"runtime.json"),new SichuanRuntimeStatus{State=state,Error=error,Runtime=SichuanIdentity.RuntimeName,AtUtc=DateTime.UtcNow.ToString("O"),ProcessId=System.Diagnostics.Process.GetCurrentProcess().Id});}catch(Exception e){LocalStorage.Log(e.Message);}}
        private static Assembly Resolve(object sender,ResolveEventArgs args)
        {if(new AssemblyName(args.Name).Name!="0Harmony")return null;using(var s=typeof(Loader).Assembly.GetManifestResourceStream("BD2Sichuan.Harmony.dll"))using(var b=new MemoryStream()){s.CopyTo(b);return Assembly.Load(b.ToArray());}}
    }
}
