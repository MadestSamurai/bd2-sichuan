using System.Collections;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
var managed=Path.GetFullPath(args[0]);var hookPath=Path.GetFullPath(args[1]);Assembly? hook=null;
AppDomain.CurrentDomain.AssemblyResolve+=(_,e)=>
{
    var name=new AssemblyName(e.Name).Name;
    if(name=="0Harmony"&&hook!=null){using var s=hook.GetManifestResourceStream("BD2Sichuan.Harmony.dll")!;using var b=new MemoryStream();s.CopyTo(b);return Assembly.Load(b.ToArray());}
    var file=Path.Combine(managed,name+".dll");return File.Exists(file)?Assembly.LoadFrom(file):null;
};
hook=Assembly.LoadFrom(hookPath);var types=hook.GetTypes();int checks=0;
void Check(bool ok,string reason){checks++;if(!ok)throw new Exception(reason);}
var flags=BindingFlags.Static|BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic;
Check(hook.GetName().Name=="BD2Sichuan.Runtime2","independent runtime");
Check(!types.Any(t=>new[]{"WatcherEngine","PrivateCaptureLoader","Golden","Mirror","Monster","Soul","Guild","ReplayCapture"}.Any(x=>(t.FullName??"").Contains(x))),"minimal runtime only");
var game=Assembly.LoadFrom(Path.Combine(managed,"Assembly-CSharp.dll"));
var client=hook.GetType("BD2Sichuan.Runtime.SichuanClient")!;
Check((string)client.GetField("CompiledMvid",flags)!.GetRawConstantValue()! == game.ManifestModule.ModuleVersionId.ToString(),"connection-local MVID");
var bindings=hook.GetType("BD2Sichuan.Runtime.SichuanBindings")!;
Check((int)bindings.GetMethod("Validate",flags)!.Invoke(null,null)!>=30,"all role bindings");
MemberInfo Api(string role)=>(MemberInfo)bindings.GetMethod("Api",flags)!.Invoke(null,new object[]{role})!;
Type ValueType(MemberInfo member)=>member switch{FieldInfo f=>f.FieldType,PropertyInfo p=>p.PropertyType,_=>throw new Exception("Not readable")};
foreach(var role in new[]{"UI.NormalAnimations","UI.ComboAnimations","UI.LevelGroup","UI.Level","Model.Width","Model.Height"})
    Check(ValueType(Api(role))==typeof(int),"integer "+role);
foreach(var role in new[]{"UI.Paused","UI.Locked","Model.Playing","Model.Shuffling","View.Selected"})
    Check(ValueType(Api(role))==typeof(bool),"boolean "+role);
foreach(var role in new[]{"UI.Views","Model.Cells"})
    Check(typeof(IList).IsAssignableFrom(ValueType(Api(role))),"list "+role);
var active=(MethodInfo)Api("UI.Active");
Check(active.IsStatic&&active.ReturnType==typeof(bool)&&active.GetParameters().Single().ParameterType.IsByRef,"TryBoard out signature");
Check(active.GetParameters()[0].ParameterType.GetElementType()==game.GetType("SichuanBoardUI"),"TryBoard exact board output");
var click=(MethodInfo)Api("View.Click");
Check(!click.IsStatic&&click.ReturnType==typeof(void)&&click.GetParameters().Length==0,"native click signature");
var add=(MethodInfo)Api("Model.PairAdd");var remove=(MethodInfo)Api("Model.PairRemove");
Check(add.DeclaringType==remove.DeclaringType,"same pair model");
Check(add.DeclaringType!.GetEvents(flags).Count(e=>e.GetAddMethod(true)==add&&e.GetRemoveMethod(true)==remove)==1,"correct event add/remove association");
int first=-1,second=-1;Action<int,int> capture=(a,b)=>{first=a;second=b;};
var handler=(Delegate)client.GetMethod("CreatePairHandler",flags)!.Invoke(null,new object[]{capture})!;
Check(handler.GetType()==add.GetParameters().Single().ParameterType,"exact generated delegate type");
var arguments=handler.GetType().GenericTypeArguments.Select(t=>t.IsValueType?Activator.CreateInstance(t):null).ToArray();
arguments[0]=7;arguments[2]=9;handler.DynamicInvoke(arguments);
Check(first==7&&second==9,"pair callback forwards exact indices");
arguments[0]=2;arguments[2]=4;handler.DynamicInvoke(arguments);
Check(first==2&&second==4,"pair callback reusable");
var runtime=hook.GetType("BD2Sichuan.Runtime.RuntimeEngine")!;
foreach(var name in new[]{"Pump","Send"})Check(runtime.GetMethod(name,flags)!.Invoke(null,null) is MethodInfo,"ABI "+name);
var observer=hook.GetType("BD2Sichuan.Runtime.SichuanNetworkTrace")!;
var handlers=(IDictionary)observer.GetMethod("ResolveResponseHandlers",flags)!.Invoke(null,null)!;
Check(handlers.Count==2,"unique original Start/End callbacks");
var command=hook.GetType("BD2Sichuan.SichuanRunCommand")!;var instance=Activator.CreateInstance(command)!;
Check((int)command.GetProperty("IntervalMilliseconds")!.GetValue(instance)! ==1000,"default interval 1000 ms");
Check(ValueType(Api("View.Tile")).FullName=="UISprite","native tile image");
foreach(var reference in hook.GetReferencedAssemblies())
    Check(reference.Name=="0Harmony"||File.Exists(Path.Combine(managed,reference.Name+".dll")),"complete dependency "+reference.Name);
Console.WriteLine(JsonSerializer.Serialize(new{status="pass",checks,hookTypes=types.Length,responses=handlers.Count,
clientMvid=game.ManifestModule.ModuleVersionId,hookSha256=Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(hookPath))),gameRequests=0,injection=false}));
