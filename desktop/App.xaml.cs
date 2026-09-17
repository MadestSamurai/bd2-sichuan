using System.IO;
using BD2Sichuan.Localization;
using System.Reflection;
using System.Text.Json;
using System.Windows;
namespace BD2Sichuan.Desktop;
public partial class App:Application
{
    private Mutex? single;
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        if(e.Args.Length==2&&e.Args[0]=="--identity")
        {File.WriteAllText(e.Args[1],JsonSerializer.Serialize(new{runtime=SichuanIdentity.RuntimeName,toolFingerprint=BD2Sichuan.Compatibility.HookCompiler.ToolFingerprint,compatibility="local-interface-adaptation",defaultIntervalMs=1000}));Shutdown();return;}
        if(e.Args.Length==3&&e.Args[0]=="--check-client")
        {
            try { var result=await Task.Run(()=>BD2Sichuan.Compatibility.HookCompiler.Prepare(e.Args[1]));File.WriteAllText(e.Args[2],JsonSerializer.Serialize(result.Report));Shutdown(); }
            catch(Exception ex){File.WriteAllText(e.Args[2],JsonSerializer.Serialize(new{Status="unsupported",Error=ex.ToString(),Injection=false}));Shutdown(1);}return;
        }
        if(e.Args.Length==2&&e.Args[0]=="--smoke")
        {
            try{Directory.CreateDirectory(e.Args[1]);var window=new SichuanWindow(Path.Combine(Path.GetFullPath(e.Args[1]),"isolated",Guid.NewGuid().ToString("N")));MainWindow=window;await window.SmokeAsync(e.Args[1]);Shutdown(0);}
            catch(Exception ex){File.WriteAllText(Path.Combine(e.Args[1],"failure.txt"),ex.ToString());Shutdown(1);}return;
        }
        single=new Mutex(true,"Local\\BD2Sichuan.Desktop",out bool first);
        if(!first){var catalog=new LanguageCatalog(LanguagePreference.Read(SichuanIdentity.BoardRoot));MessageBox.Show(catalog.Text("连连看工具已经打开，请使用现有窗口。"),catalog.Text("BD2 连连看"));Shutdown();return;}
        var main=new SichuanWindow();MainWindow=main;main.Show();
    }
    protected override void OnExit(ExitEventArgs e){single?.Dispose();base.OnExit(e);}
}
