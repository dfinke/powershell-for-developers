
namespace ShowUI
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Management.Automation;
    using System.Windows.Controls;

    
    public class ShowUISetting : DependencyObject
    {
        public static readonly DependencyProperty ControlNameProperty = DependencyProperty.RegisterAttached(
            "ControlName",
            typeof(string),
            typeof(ShowUISetting),
            new FrameworkPropertyMetadata());
        
        public static void SetControlName(UIElement element, string value)
        {
            element.SetValue(ControlNameProperty, value);
        }
        
        public static string GetControlName(UIElement element)
        {
            return (string)element.GetValue(ControlNameProperty);
        }

        public static readonly DependencyProperty StyleNameProperty = DependencyProperty.RegisterAttached(
            "StyleName",
            typeof(string),
            typeof(ShowUISetting),
            new FrameworkPropertyMetadata());

        public static void SetStylelName(UIElement element, string value)
        {
            element.SetValue(StyleNameProperty, value);
        }

        public static string GetStylelName(UIElement element)
        {
            return (string)element.GetValue(StyleNameProperty);
        }                       
    }

    public static class ShowUIExtensions
    {
        public static IEnumerable<DependencyObject> GetChildControl(this DependencyObject control,
                bool peekIntoNestedControl,
                Type[] byType,
                string[] byControlName,
                string[] byName,
                bool onlyDirectChildren)
        {
            bool hasEnumeratedChildren = false;
            Queue<DependencyObject> queue = new Queue<DependencyObject>();
            queue.Enqueue(control);
            while (queue.Count > 0)
            {
                DependencyObject parent = queue.Peek();
                string controlName = (string)parent.GetValue(ShowUI.ShowUISetting.ControlNameProperty);
                string name = String.Empty;
                if ((parent is FrameworkElement))
                {
                    name = (parent as FrameworkElement).Name;
                }

                if (byName != null && (!String.IsNullOrEmpty(name)))
                {
                    foreach (string n in byName)
                    {
                        if (String.Compare(n, name, true) == 0)
                        {
                            yield return parent;
                        }
                    }
                }
                else if (byControlName != null && (!String.IsNullOrEmpty(controlName)))
                {
                    foreach (string n in byControlName)
                    {
                        if (String.Compare(n, controlName, true) == 0)
                        {
                            yield return parent;
                        }
                    }
                }
                else if (byType != null)
                {
                    foreach (Type t in byType)
                    {
                        Type parentType = parent.GetType();
                        if (t.IsInterface && parentType.GetInterface(t.FullName) != null)
                        {
                            yield return parent;
                        }
                    }
                }
                else
                {
                    yield return parent;
                }

                int childCount = VisualTreeHelper.GetChildrenCount(parent);
                if (childCount > 0)
                {
                    if (!(hasEnumeratedChildren && onlyDirectChildren))
                    {
                        if ((!hasEnumeratedChildren) ||
                            ((String.IsNullOrEmpty(controlName) || peekIntoNestedControl)))
                        {
                            hasEnumeratedChildren = true;
                            for (int i = 0; i < childCount; i++)
                            {
                                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                                queue.Enqueue(child);
                            }
                        }
                    }
                }
                else
                {
                    if (parent is ContentControl)
                    {
                        object childObject = (parent as ContentControl).Content;
                        if (childObject != null && childObject is Visual)
                        {
                            queue.Enqueue(childObject as Visual);
                        }
                    }
                }

                queue.Dequeue();
            }

        }
    }
}

namespace ShowUI
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System.Management.Automation;
    using System.Windows;
    using System.Collections;
    using System.Management.Automation.Runspaces;
    using System.Timers;
    using System.Windows.Threading;
    using System.Threading;
    using System.Windows.Input;

    public class ShowUICommands
    {
        private static RoutedCommand backgroundPowerShellCommand = new RoutedCommand();

        public static RoutedCommand BackgroundPowerShellCommand
        {
            get
            {
                return backgroundPowerShellCommand;
            }
        }
    }
} 

namespace ShowUI
{
    using System;
    using System.Windows;
    using System.Windows.Data;
    using System.Management.Automation;   
    using System.Globalization;

    public class LanguagePrimitivesValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return LanguagePrimitives.ConvertTo(value, targetType);
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
        {
            return LanguagePrimitives.ConvertTo(value, targetType);
        }
    }
}

namespace ShowUI
{
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;
using System.Windows.Threading;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;


    public class WPFJob : Job, INotifyPropertyChanged
    {
        Runspace runspace;
        InitialSessionState initialSessionState;

        PowerShell powerShellCommand;
        Dispatcher JobDispatcher;
        public Window JobWindow;
        Thread jobThread;

        Hashtable namedControls;
        Runspace interopRunspace;

        Runspace GetWPFCurrentThreadRunspace(InitialSessionState sessionState)
        {
            InitialSessionState clone = sessionState.Clone();
            clone.ThreadOptions = PSThreadOptions.UseCurrentThread;
            SessionStateVariableEntry window = new SessionStateVariableEntry("Window", JobWindow, "");
            SessionStateVariableEntry namedControls = new SessionStateVariableEntry("NamedControls", this.namedControls, "");
            clone.Variables.Add(window);
            clone.Variables.Add(namedControls);
            return RunspaceFactory.CreateRunspace(clone);
        }


        delegate Collection<PSObject> RunScriptCallback(string script);
        delegate Collection<PSObject> RunScriptWithParameters(string script, Object parameters);

        public PSObject[] InvokeScriptInJob(string script, object parameters, bool async)
        {
            if (this.JobStateInfo.State == JobState.Running)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (JobWindow != null) { break; }
                    Thread.Sleep(50);
                }

                if (JobWindow == null)
                {
                    return null;
                }
                return (PSObject[])RunOnUIThread(
                    new DispatcherOperationCallback(
                    delegate
                    {
                        PowerShell psCmd = PowerShell.Create();
                        if (interopRunspace == null)
                        {
                            interopRunspace = GetWPFCurrentThreadRunspace(this.initialSessionState);
                            interopRunspace.Open();
                        }
                        psCmd.Runspace = interopRunspace;
                        psCmd.AddScript(script);
                        if (parameters is IDictionary)
                        {
                            psCmd.AddParameters(parameters as IDictionary);
                        }
                        else
                        {
                            if (parameters is IList)
                            {
                                psCmd.AddParameters(parameters as IList);
                            }
                        }
                        Collection<PSObject> results = psCmd.Invoke();
                        if (psCmd.InvocationStateInfo.Reason != null)
                        {
                            throw psCmd.InvocationStateInfo.Reason;
                        }
                        PSObject[] resultArray = new PSObject[results.Count + psCmd.Streams.Error.Count];
                        int count = 0;
                        if (psCmd.Streams.Error.Count > 0)
                        {
                            foreach (ErrorRecord err in psCmd.Streams.Error)
                            {
                                resultArray[count++] = new PSObject(err);
                            }
                        }
                        foreach (PSObject r in results)
                        {
                            resultArray[count++] = r;
                        }
                        return resultArray;
                    }),
                    async);
            }
            else
            {
                return null;
            }
        }

        object RunOnUIThread(DispatcherOperationCallback dispatcherMethod, bool async)
        {
            if (Application.Current != null)
            {
                if (Application.Current.Dispatcher.Thread == Thread.CurrentThread)
                {
                    // This avoids dispatching to the UI thread if we are already in the UI thread.
                    // Without this runing a command like 1/0 was throwing due to nested dispatches.
                    return dispatcherMethod.Invoke(null);
                }
            }

            Exception e = null;
            object returnValue = null;
            SynchronizationContext sync = new DispatcherSynchronizationContext(JobWindow.Dispatcher);
            if (sync == null) { return null; }
            if (async) {
                sync.Post(
                    new SendOrPostCallback(delegate(object obj)
                    {
                        try
                        {
                            returnValue = dispatcherMethod.Invoke(obj);
                        }
                        catch (Exception uiException)
                        {
                            e = uiException;
                        }
                    }),
                    null);

            } else {
                sync.Send(
                    new SendOrPostCallback(delegate(object obj)
                    {
                        try
                        {
                            returnValue = dispatcherMethod.Invoke(obj);
                        }
                        catch (Exception uiException)
                        {
                            e = uiException;
                        }
                    }),
                    null);

            }

            if (e != null)
            {
                throw new System.Reflection.TargetInvocationException(e.Message, e);
            }
            return returnValue;
        }


        public static InitialSessionState GetSessionStateForCommands(CommandInfo[] commands)
        {
            InitialSessionState iss = InitialSessionState.CreateDefault();
            Dictionary<string, SessionStateCommandEntry> commandCache = new Dictionary<string, SessionStateCommandEntry>();
            foreach (SessionStateCommandEntry ssce in iss.Commands)
            {
                commandCache[ssce.Name] = ssce;
            }
            iss.ApartmentState = ApartmentState.STA;
            iss.ThreadOptions = PSThreadOptions.ReuseThread;
            if (commands.Length == 0)
            {
                return iss;
            }
            foreach (CommandInfo cmd in commands)
            {
                if (cmd.Module != null)
                {                    
                        string manifestPath = cmd.Module.Path.Replace(".psm1",".psd1").Replace(".dll", ".psd1");
                        if (System.IO.File.Exists(manifestPath)) {  
                            iss.ImportPSModule(new string[] { manifestPath });
                        } else {
                            iss.ImportPSModule(new string[] { cmd.Module.Path });
                        }
                        
                        continue;
                }
                if (cmd is AliasInfo)
                {
                    CommandInfo loopCommand = cmd;
                    while (loopCommand is AliasInfo)
                    {
                        SessionStateAliasEntry alias = new SessionStateAliasEntry(loopCommand.Name, loopCommand.Definition);
                        iss.Commands.Add(alias);
                        loopCommand = (loopCommand as AliasInfo).ReferencedCommand;
                    }
                    if (loopCommand is FunctionInfo)
                    {
                        SessionStateFunctionEntry func = new SessionStateFunctionEntry(loopCommand.Name, loopCommand.Definition);
                        iss.Commands.Add(func);
                    }
                    if (loopCommand is CmdletInfo)
                    {
                        CmdletInfo cmdletData = loopCommand as CmdletInfo;
                        SessionStateCmdletEntry cmdlet = new SessionStateCmdletEntry(cmd.Name,
                                cmdletData.ImplementingType,
                                cmdletData.HelpFile);
                        iss.Commands.Add(cmdlet);
                    }
                }
                if (cmd is FunctionInfo)
                {
                    SessionStateFunctionEntry func = new SessionStateFunctionEntry(cmd.Name, cmd.Definition);
                    iss.Commands.Add(func);
                }
                if (cmd is CmdletInfo)
                {
                    CmdletInfo cmdletData = cmd as CmdletInfo;
                    SessionStateCmdletEntry cmdlet = new SessionStateCmdletEntry(cmd.Name,
                            cmdletData.ImplementingType,
                            cmdletData.HelpFile);
                    iss.Commands.Add(cmdlet);
                }
            }
            return iss;
        }

        public WPFJob(string name, string command, ScriptBlock scriptBlock)
            : base(command, name)
        {
            this.initialSessionState = InitialSessionState.CreateDefault();
            Start(scriptBlock, new Hashtable());
        }

        private WPFJob(ScriptBlock scriptBlock)
        {
            Start(scriptBlock, new Hashtable());
        }
        
        public WPFJob(string name, string command, ScriptBlock scriptBlock, InitialSessionState initalSessionState)
            : base(command, name)
        {
            this.initialSessionState = initalSessionState;
            Start(scriptBlock, new Hashtable());
        }
        
        public WPFJob(string name, string command, ScriptBlock scriptBlock, InitialSessionState initalSessionState, Hashtable parameters)
            : base(command, name)
        {
            this.initialSessionState = initalSessionState;
            Start(scriptBlock, parameters);
        }

        private WPFJob(string name, string command, ScriptBlock scriptBlock, InitialSessionState initalSessionState, Hashtable parameters, bool isChildJob)
            : base(command, name)
        {
            this.initialSessionState = initalSessionState;
            if (isChildJob)
            {
                Start(scriptBlock, parameters);
            }
            else
            {
                WPFJob childJob = new WPFJob(name, command, scriptBlock, initalSessionState, parameters, true);
                childJob.StateChanged += new EventHandler<JobStateEventArgs>(childJob_StateChanged);
                this.ChildJobs.Add(childJob);
            }
        }


        void childJob_StateChanged(object sender, JobStateEventArgs e)
        {
            this.SetJobState(e.JobStateInfo.State);            
        }

        /// <summary>
        /// Synchronizes Job State with Background Runspace
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void powerShellCommand_InvocationStateChanged(object sender, PSInvocationStateChangedEventArgs e)
        {
            try
            {
                if (e.InvocationStateInfo.State == PSInvocationState.Completed)
                {
                    runspace.Close();
                }
                if (e.InvocationStateInfo.State == PSInvocationState.Failed)
                {
                    ErrorRecord err = new ErrorRecord(e.InvocationStateInfo.Reason, "JobFailed", ErrorCategory.OperationStopped, this);
                    Error.Add(err);
                    runspace.Close();
                }
                JobState js = (JobState)Enum.Parse(typeof(JobState), e.InvocationStateInfo.State.ToString(), true);
                this.SetJobState(js);
            }
            catch
            {
            }
        }


        void Start(ScriptBlock scriptBlock, Hashtable parameters)
        {
            SessionStateAssemblyEntry windowsBase = new SessionStateAssemblyEntry(typeof(Dispatcher).Assembly.ToString());
            SessionStateAssemblyEntry presentationCore = new SessionStateAssemblyEntry(typeof(UIElement).Assembly.ToString());
            SessionStateAssemblyEntry presentationFramework = new SessionStateAssemblyEntry(typeof(Control).Assembly.ToString());
            initialSessionState.Assemblies.Add(windowsBase);
            initialSessionState.Assemblies.Add(presentationCore);
            initialSessionState.Assemblies.Add(presentationFramework);
            initialSessionState.Assemblies.Add(presentationFramework);
            runspace = RunspaceFactory.CreateRunspace(this.initialSessionState);
            runspace.ThreadOptions = PSThreadOptions.ReuseThread;
            runspace.ApartmentState = ApartmentState.STA;
            runspace.Open();
            powerShellCommand = PowerShell.Create();
            powerShellCommand.Runspace = runspace;
            jobThread = powerShellCommand.AddScript("[Threading.Thread]::CurrentThread").Invoke<Thread>()[0];

            powerShellCommand.Streams.Error = this.Error;
            this.Error.DataAdded += new EventHandler<DataAddedEventArgs>(Error_DataAdded);
            powerShellCommand.Streams.Warning = this.Warning;
            this.Warning.DataAdded += new EventHandler<DataAddedEventArgs>(Warning_DataAdded);
            powerShellCommand.Streams.Verbose = this.Verbose;
            this.Verbose.DataAdded += new EventHandler<DataAddedEventArgs>(Verbose_DataAdded);
            powerShellCommand.Streams.Debug = this.Debug;
            this.Debug.DataAdded += new EventHandler<DataAddedEventArgs>(Debug_DataAdded);
            powerShellCommand.Streams.Progress = this.Progress;
            this.Progress.DataAdded += new EventHandler<DataAddedEventArgs>(Progress_DataAdded);
            this.Output.DataAdded += new EventHandler<DataAddedEventArgs>(Output_DataAdded);
            powerShellCommand.Commands.Clear();
            powerShellCommand.Commands.AddScript(scriptBlock.ToString(), false);
            if (parameters.Count > 0)
            {
                powerShellCommand.AddParameters(parameters);
            }
            Collection<Visual> output = powerShellCommand.Invoke<Visual>();
            if (output.Count == 0)
            {
                return;
            }
            powerShellCommand.Commands.Clear();
            powerShellCommand.Commands.AddCommand("Show-Window").AddArgument(output[0]).AddParameter("OutputWindowFirst");
            Object var = powerShellCommand.Runspace.SessionStateProxy.GetVariable("NamedControls");
            if (var != null && ((var as Hashtable) != null))
            {
                namedControls = var as Hashtable;
            }
            JobDispatcher = Dispatcher.FromThread(jobThread);
            JobDispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(jobDispatcher_UnhandledException);
            powerShellCommand.InvocationStateChanged += new EventHandler<PSInvocationStateChangedEventArgs>(powerShellCommand_InvocationStateChanged);
            powerShellCommand.BeginInvoke<Object, PSObject>(null, this.Output);
            DateTime startTime = DateTime.Now;
            if (output[0] is FrameworkElement)
            {

                while (JobWindow == null)
                {
                    if ((DateTime.Now - startTime) > TimeSpan.FromSeconds(30))
                    {
                        this.SetJobState(JobState.Failed);
                        return;
                    }
                    System.Threading.Thread.Sleep(25);
                }
            }


        }

        void jobDispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ErrorRecord err = new ErrorRecord(e.Exception, "UnhandledException", ErrorCategory.OperationStopped, this);
            this.Error.Add(err);
            StopJob();
        }

        void Output_DataAdded(object sender, DataAddedEventArgs e)
        {
            PSDataCollection<PSObject> output = sender as PSDataCollection<PSObject>;
            if (output == null)
            {
                return;
            }
            if (output[e.Index].BaseObject is Window)
            {
                JobWindow = output[e.Index].BaseObject as Window;
            }
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Output"));
            }
        }

        void Progress_DataAdded(object sender, DataAddedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Progress"));
            }
        }

        void Debug_DataAdded(object sender, DataAddedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Debug"));
            }
        }

        void Verbose_DataAdded(object sender, DataAddedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Verbose"));
            }
        }

        void Warning_DataAdded(object sender, DataAddedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Warning"));
            }
        }

        void Error_DataAdded(object sender, DataAddedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Error"));
            }
        }

        /// <summary>
        /// If the comamnd is running, the job indicates it has more data
        /// </summary>
        public override bool HasMoreData
        {
            get
            {
                if (powerShellCommand.InvocationStateInfo.State == PSInvocationState.Running)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override string Location
        {
            get
            {
                if (this.JobStateInfo.State == JobState.Running && (JobWindow != null))
                {

                    return (string)RunOnUIThread(
                        new DispatcherOperationCallback(
                        delegate
                        {
                            return "Left: " + JobWindow.Left +
                                " Top: " + JobWindow.Top +
                                " Width: " + JobWindow.ActualWidth +
                                " Height: " + JobWindow.ActualHeight;
                        }),
                        false);
                }
                else
                {
                    return " ";
                }
            }
        }


        public override string StatusMessage
        {
            get { return string.Empty; }
        }

        public override void StopJob()
        {
            Dispatcher dispatch = Dispatcher.FromThread(jobThread);
            if (dispatch != null)
            {
                if (!dispatch.HasShutdownStarted)
                {
                    dispatch.InvokeShutdown();
                }
            }
            powerShellCommand.Stop();
            runspace.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                powerShellCommand.Dispose();
                runspace.Close();
                runspace.Dispose();
            }
            base.Dispose(disposing);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
 
namespace ShowUI
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System.Management.Automation;
    using System.Windows;
    using System.Collections;
    using System.Management.Automation.Runspaces;
    using System.Timers;
    using System.Windows.Threading;
    using System.Threading;

    public class PowerShellDataSource : INotifyPropertyChanged
    {
        Hashtable resources = new Hashtable();

        public Hashtable Resources
        {
            get { return this.resources; }
        }

        public Dispatcher Dispatcher
        {
            get;
            set;
        }

        object RunOnUIThread(DispatcherOperationCallback dispatcherMethod, bool async)
        {
            if (Application.Current != null)
            {
                if (Application.Current.Dispatcher.Thread == Thread.CurrentThread)
                {
                    // This avoids dispatching to the UI thread if we are already in the UI thread.
                    // Without this runing a command like 1/0 was throwing due to nested dispatches.
                    return dispatcherMethod.Invoke(null);
                }
            }

            Exception e = null;
            object returnValue = null;
            SynchronizationContext sync = new DispatcherSynchronizationContext(Dispatcher);
            if (sync == null) { return null; }
            if (async)
            {
                sync.Post(
                    new SendOrPostCallback(delegate(object obj)
                    {
                        try
                        {
                            returnValue = dispatcherMethod.Invoke(obj);
                        }
                        catch (Exception uiException)
                        {
                            e = uiException;
                        }
                    }),
                    null);

            }
            else
            {
                sync.Send(
                    new SendOrPostCallback(delegate(object obj)
                    {
                        try
                        {
                            returnValue = dispatcherMethod.Invoke(obj);
                        }
                        catch (Exception uiException)
                        {
                            e = uiException;
                        }
                    }),
                    null);

            }

            if (e != null)
            {
                throw new System.Reflection.TargetInvocationException(e.Message, e);
            }
            return returnValue;
        }


        public Object Parent
        {
            get { return this.parent; }
            set
            {
                this.parent = value;
                if (this.parent != null &&
                        this.parent.GetType().GetProperty("Dispatcher") != null)
                {
                    Dispatcher = this.parent.GetType().GetProperty("Dispatcher").GetValue(this.parent, null) as Dispatcher;
                }
            }
        }


        private Object parent;

        public PSObject[] Output
        {
            get
            {
                PSObject[] returnValue = new PSObject[outputCollection.Count];
                outputCollection.CopyTo(returnValue, 0);
                return returnValue;
            }
        }
 
 
        PSObject lastOutput;
        public PSObject LastOutput
        {
            get
            {
                return lastOutput;
            }
        }

        public ErrorRecord[] Error
        {
            get
            {
                ErrorRecord[] returnValue = new ErrorRecord[powerShellCommand.Streams.Error.Count];
                powerShellCommand.Streams.Error.CopyTo(returnValue, 0);
                return returnValue;
            }
        }
 
        ErrorRecord lastError;
        public ErrorRecord LastError
        {
            get
            {
                return this.lastError;
            }
        }

        public WarningRecord[] Warning
        {
            get
            {
                WarningRecord[] returnValue = new WarningRecord[powerShellCommand.Streams.Warning.Count];
                powerShellCommand.Streams.Warning.CopyTo(returnValue, 0);
                return returnValue;
            }
        }
 
        WarningRecord lastWarning;
 
        public WarningRecord LastWarning
        {
            get
            {
                return lastWarning;
            }
        }

        public VerboseRecord[] Verbose
        {
            get
            {
                VerboseRecord[] returnValue = new VerboseRecord[powerShellCommand.Streams.Verbose.Count];
                powerShellCommand.Streams.Verbose.CopyTo(returnValue, 0);
                return returnValue;
            }
        }
 
        VerboseRecord lastVerbose;
        public VerboseRecord LastVerbose
        {
            get
            {
                return lastVerbose;
            }
        }


        public DebugRecord[] Debug
        {
            get
            {
                DebugRecord[] returnValue = new DebugRecord[powerShellCommand.Streams.Debug.Count];
                powerShellCommand.Streams.Debug.CopyTo(returnValue, 0);
                return returnValue;
            }
        }
 
        DebugRecord lastDebug;
        public DebugRecord LastDebug
        {
            get
            {
                return lastDebug;
            }
        }


        public ProgressRecord[] Progress
        {
            get
            {
                ProgressRecord[] returnValue = new ProgressRecord[powerShellCommand.Streams.Progress.Count];
                powerShellCommand.Streams.Progress.CopyTo(returnValue, 0);
                return returnValue;
            }
        }

        public PSObject[] TimeStampedOutput
        {
            get
            {
                PSObject[] returnValue = new PSObject[timeStampedOutput.Count];
                timeStampedOutput.CopyTo(returnValue, 0);
                return returnValue;
            }
        }

        private PSObject lastTimeStampedOutput;

        public PSObject LastTimeStampedOutput
        {
            get
            {
                return this.lastTimeStampedOutput;
            }
        }


        ProgressRecord lastProgress;
 
        public ProgressRecord LastProgress
        {
            get
            {
                return lastProgress;
            }
        }
        
        public PowerShell Command
        {
            get {
                return powerShellCommand;
            }
        }

        public bool IsFinished
        {
            get
            {
                return (powerShellCommand.InvocationStateInfo.State == PSInvocationState.Completed ||
                        powerShellCommand.InvocationStateInfo.State == PSInvocationState.Failed ||
                        powerShellCommand.InvocationStateInfo.State == PSInvocationState.Stopped);
            }
        }

        public bool IsRunning
        {
            get
            {
                return (powerShellCommand.InvocationStateInfo.State == PSInvocationState.Running ||
                    powerShellCommand.InvocationStateInfo.State == PSInvocationState.Stopping);
            }
        }


        string script;

        PSDataCollection<PSObject> timeStampedOutput;

        public string Script
        {
            get
            {
                return script;
            }
            set
            {
                script = value;
                try
                {
                    powerShellCommand.Commands.Clear();
                    powerShellCommand.AddScript(script, false);
                    lastDebug = null;
                    lastError = null;
                    lastTimeStampedOutput = null;
                    outputCollection.Clear();
                    timeStampedOutput.Clear();
                    lastOutput = null;
                    lastProgress = null;
                    lastVerbose = null;
                    lastWarning = null;
                    powerShellCommand.BeginInvoke<Object, PSObject>(null, outputCollection);
                }
                catch
                {
 
                }
            }
        }

        void powerShellCommand_InvocationStateChanged(object sender, PSInvocationStateChangedEventArgs e)
        {
            if (e.InvocationStateInfo.State == PSInvocationState.Failed)
            {
                ErrorRecord err = new ErrorRecord(e.InvocationStateInfo.Reason, "PowerShellDataSource.TerminatingError", ErrorCategory.InvalidOperation, powerShellCommand);
                powerShellCommand.Streams.Error.Add(err);
            }
            if (Dispatcher != null)
            {
                RunOnUIThread(
                        new DispatcherOperationCallback(
                        delegate
                        {
                            NotifyInvocationStateChanged();
                            return null;
                        }),
                        true);
            }
            else
            {

                NotifyInvocationStateChanged();
            }


        }

        PowerShell powerShellCommand;
        PSDataCollection<PSObject> outputCollection;
        public PowerShellDataSource()
        {
            powerShellCommand =  PowerShell.Create();
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();
            powerShellCommand.Runspace = runspace;
            outputCollection = new PSDataCollection<PSObject>();
            timeStampedOutput = new PSDataCollection<PSObject>();
            outputCollection.DataAdded += new EventHandler<DataAddedEventArgs>(outputCollection_DataAdded);
            timeStampedOutput.DataAdded += new EventHandler<DataAddedEventArgs>(timeStampedOutput_DataAdded);
            powerShellCommand.Streams.Debug.DataAdded += new EventHandler<DataAddedEventArgs>(Debug_DataAdded);
            powerShellCommand.Streams.Error.DataAdded += new EventHandler<DataAddedEventArgs>(Error_DataAdded);
            powerShellCommand.Streams.Verbose.DataAdded += new EventHandler<DataAddedEventArgs>(Verbose_DataAdded);
            powerShellCommand.Streams.Progress.DataAdded += new EventHandler<DataAddedEventArgs>(Progress_DataAdded);
            powerShellCommand.Streams.Warning.DataAdded += new EventHandler<DataAddedEventArgs>(Warning_DataAdded);
        }

        #region Notification Methods
        void NotifyTimeStampedOutputChanged()
        {
            object sender;
            if (this.Parent != null)
            {
                sender = this.Parent;
            }
            else
            {
                sender = this;
            }

            if (PropertyChanged != null)
            {
                PropertyChanged(sender, new PropertyChangedEventArgs("TimeStampedOutput"));
            }

            if (TimeStampedOutputChanged != null)
            {
                TimeStampedOutputChanged(sender, new PropertyChangedEventArgs("TimeStampedOutput"));
            }
        }

        void NotifyInvocationStateChanged()
        {
            object sender;
            if (this.Parent != null)
            {
                sender = this.Parent;
            }
            else
            {
                sender = this;
            }

            if (PropertyChanged != null)
            {
                PropertyChanged(sender, new PropertyChangedEventArgs("IsFinished"));
                PropertyChanged(sender, new PropertyChangedEventArgs("IsRunning"));
            }

            if (IsFinishedChanged != null)
            {
                IsFinishedChanged(sender, new PropertyChangedEventArgs("IsFinished"));
            }

            if (IsRunningChanged != null)
            {
                IsRunningChanged(sender, new PropertyChangedEventArgs("IsRunning"));
            }
        }

        void NotifyErrorChanged()
        {
            object sender;
            if (this.Parent != null)
            {
                sender = this.Parent;
            }
            else
            {
                sender = this;
            }

            if (PropertyChanged != null)
            {
                PropertyChanged(sender, new PropertyChangedEventArgs("Error"));
                PropertyChanged(sender, new PropertyChangedEventArgs("LastError"));
            }

            if (ErrorChanged != null)
            {
                ErrorChanged(sender, new PropertyChangedEventArgs("Error"));
            }
        }
        void NotifyDebugChanged()
        {
            object sender;
            if (this.Parent != null)
            {
                sender = this.Parent;
            }
            else
            {
                sender = this;
            }

            if (PropertyChanged != null)
            {
                PropertyChanged(sender, new PropertyChangedEventArgs("Debug"));
                PropertyChanged(sender, new PropertyChangedEventArgs("LastDebug"));
            }

            if (DebugChanged != null)
            {
                DebugChanged(sender, new PropertyChangedEventArgs("Debug"));
            }
        }

        void NotifyOutputChanged()
        {
            object sender;
            if (this.Parent != null)
            {
                sender = this.Parent;
            }
            else
            {
                sender = this;
            }

            if (PropertyChanged != null)
            {
                PropertyChanged(sender, new PropertyChangedEventArgs("Output"));
                PropertyChanged(sender, new PropertyChangedEventArgs("LastOutput"));
            }

            if (OutputChanged != null)
            {
                OutputChanged(sender, new PropertyChangedEventArgs("Output"));
            }
        }

        void NotifyWarningChanged()
        {
            object sender;
            if (this.Parent != null)
            {
                sender = this.Parent;
            }
            else
            {
                sender = this;
            }

            if (PropertyChanged != null)
            {
                PropertyChanged(sender, new PropertyChangedEventArgs("Warning"));
                PropertyChanged(sender, new PropertyChangedEventArgs("LastWarning"));
            }

            if (WarningChanged != null)
            {
                WarningChanged(sender, new PropertyChangedEventArgs("Warning"));
            }
        }

        void NotifyVerboseChanged()
        {
            object sender;
            if (this.Parent != null)
            {
                sender = this.Parent;
            }
            else
            {
                sender = this;
            }

            if (PropertyChanged != null)
            {
                PropertyChanged(sender, new PropertyChangedEventArgs("Verbose"));
                PropertyChanged(sender, new PropertyChangedEventArgs("LastVerbose"));
            }

            if (VerboseChanged != null)
            {
                VerboseChanged(sender, new PropertyChangedEventArgs("Verbose"));
            }
        }

        void NotifyProgressChanged()
        {
            object sender;
            if (this.Parent != null)
            {
                sender = this.Parent;
            }
            else
            {
                sender = this;
            }

            if (PropertyChanged != null)
            {
                PropertyChanged(sender, new PropertyChangedEventArgs("Progress"));
                PropertyChanged(sender, new PropertyChangedEventArgs("LastProgress"));
            }

            if (ProgressChanged != null)
            {
                ProgressChanged(sender, new PropertyChangedEventArgs("Progress"));
            }
        }

        #endregion


        void timeStampedOutput_DataAdded(object sender, DataAddedEventArgs e)
        {
            PSDataCollection<PSObject> collection = sender as PSDataCollection<PSObject>;
            lastTimeStampedOutput = collection[e.Index];
            if (Dispatcher != null)
            {
                RunOnUIThread(
                        new DispatcherOperationCallback(
                        delegate
                        {
                            NotifyTimeStampedOutputChanged();
                            return null;
                        }),
                        true);
            }
            else
            {

                NotifyTimeStampedOutputChanged();
            }

        }
 
        void Debug_DataAdded(object sender, DataAddedEventArgs e)
        {
            PSDataCollection<DebugRecord> collection = sender as PSDataCollection<DebugRecord>;
            lastDebug = collection[e.Index];
            PSObject psObj = new PSObject(lastDebug);
            PSPropertyInfo propInfo = new PSNoteProperty("TimeStamp", DateTime.Now);
            psObj.Properties.Add(new PSNoteProperty("Stream", "Debug"));
            psObj.Properties.Add(propInfo);
            timeStampedOutput.Add(psObj);

            if (Dispatcher != null)
            {
                RunOnUIThread(
                        new DispatcherOperationCallback(
                        delegate
                        {
                            NotifyDebugChanged();
                            return null;
                        }),
                        true);
            }
            else
            {
                NotifyDebugChanged();
            }

        }
 
        void Error_DataAdded(object sender, DataAddedEventArgs e)
        {
            PSDataCollection<ErrorRecord> collection = sender as PSDataCollection<ErrorRecord>;
            this.lastError = collection[e.Index];
            PSObject psObj = new PSObject(lastError);
            PSPropertyInfo propInfo = new PSNoteProperty("TimeStamp", DateTime.Now);
            psObj.Properties.Add(new PSNoteProperty("Stream", "Error"));
            psObj.Properties.Add(propInfo);
            timeStampedOutput.Add(psObj);

            if (Dispatcher != null)
            {
                RunOnUIThread(
                        new DispatcherOperationCallback(
                        delegate
                        {
                            NotifyErrorChanged();
                            return null;
                        }),
                        true);
            }
            else
            {
                NotifyErrorChanged();
            }

        }
 
        void Warning_DataAdded(object sender, DataAddedEventArgs e)
        {
            PSDataCollection<WarningRecord> collection = sender as PSDataCollection<WarningRecord>;
            lastWarning = collection[e.Index];
            PSObject psObj = new PSObject(lastWarning);
            psObj.Properties.Add(new PSNoteProperty("TimeStamp", DateTime.Now));
            psObj.Properties.Add(new PSNoteProperty("Stream", "Warning"));
            timeStampedOutput.Add(psObj);
            if (Dispatcher != null)
            {
                RunOnUIThread(
                        new DispatcherOperationCallback(
                        delegate
                        {
                            NotifyWarningChanged();
                            return null;
                        }),
                        true);
            }
            else
            {
                NotifyWarningChanged();
            }


        }


        void Verbose_DataAdded(object sender, DataAddedEventArgs e)
        {
            PSDataCollection<VerboseRecord> collection = sender as PSDataCollection<VerboseRecord>;
            lastVerbose = collection[e.Index];
            PSObject psObj = new PSObject(lastVerbose);
            PSPropertyInfo propInfo = new PSNoteProperty("TimeStamp", DateTime.Now);
            psObj.Properties.Add(new PSNoteProperty("Stream", "Verbose"));
            psObj.Properties.Add(propInfo);
            timeStampedOutput.Add(psObj);
            if (Dispatcher != null)
            {
                RunOnUIThread(
                        new DispatcherOperationCallback(
                        delegate
                        {
                            NotifyVerboseChanged();
                            return null;
                        }),
                        true);
            }
            else
            {
                NotifyVerboseChanged();
            }
        }
 
        void Progress_DataAdded(object sender, DataAddedEventArgs e)
        {
            PSDataCollection<ProgressRecord> collection = sender as PSDataCollection<ProgressRecord>;
            lastProgress = collection[e.Index];
            PSObject psObj = new PSObject(lastProgress);
            PSPropertyInfo propInfo = new PSNoteProperty("TimeStamp", DateTime.Now);
            psObj.Properties.Add(new PSNoteProperty("Stream", "Progress"));
            psObj.Properties.Add(propInfo);
            timeStampedOutput.Add(psObj);
            if (Dispatcher != null)
            {
                RunOnUIThread(
                        new DispatcherOperationCallback(
                        delegate
                        {
                            NotifyProgressChanged();
                            return null;
                        }),
                        true);
            }
            else
            {
                NotifyProgressChanged();
            }
        }
 
        void outputCollection_DataAdded(object sender, DataAddedEventArgs e)
        {
            PSDataCollection<PSObject> collection = sender as PSDataCollection<PSObject>;
            lastOutput = collection[e.Index];
            PSObject psObj = new PSObject(lastOutput);
            PSPropertyInfo propInfo = new PSNoteProperty("TimeStamp", DateTime.Now);
            psObj.Properties.Add(new PSNoteProperty("Stream", "Output"));
            psObj.Properties.Add(propInfo);
            timeStampedOutput.Add(psObj);
            if (Dispatcher != null)
            {
                RunOnUIThread(
                        new DispatcherOperationCallback(
                        delegate
                        {
                            NotifyOutputChanged();
                            return null;
                        }),
                        true);
            }
            else
            {
                NotifyOutputChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangedEventHandler OutputChanged;
        public event PropertyChangedEventHandler ErrorChanged;
        public event PropertyChangedEventHandler WarningChanged;
        public event PropertyChangedEventHandler DebugChanged;
        public event PropertyChangedEventHandler VerboseChanged;
        public event PropertyChangedEventHandler ProgressChanged;
        public event PropertyChangedEventHandler IsFinishedChanged;
        public event PropertyChangedEventHandler IsRunningChanged;
        public event PropertyChangedEventHandler TimeStampedOutputChanged;
    }
}

namespace ShowUI
{

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;


   public class BindingTypeDescriptionProvider : TypeDescriptionProvider
   {
      private static readonly TypeDescriptionProvider DefaultTypeProvider = TypeDescriptor.GetProvider(typeof(Binding));

      public BindingTypeDescriptionProvider() : base(DefaultTypeProvider) { }

      public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
      {
         ICustomTypeDescriptor defaultDescriptor = base.GetTypeDescriptor(objectType, instance);
         return instance == null ? defaultDescriptor : new BindingCustomTypeDescriptor(defaultDescriptor);
      }
   }

   public class BindingCustomTypeDescriptor : CustomTypeDescriptor
   {
      public BindingCustomTypeDescriptor(ICustomTypeDescriptor parent) : base(parent) { }

      public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
      {
         PropertyDescriptor pd;
         var pdc = new PropertyDescriptorCollection(base.GetProperties(attributes).Cast<PropertyDescriptor>().ToArray());
         if ((pd = pdc.Find("Source", false)) != null)
         {
            pdc.Add(TypeDescriptor.CreateProperty(typeof(Binding), pd, new Attribute[] { new DefaultValueAttribute("null") }));
            pdc.Remove(pd);
         }
         return pdc;
      }
   }

   public class BindingConverter : ExpressionConverter
   {
      public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
      {
         return (destinationType == typeof(MarkupExtension)) ? true : false;
      }
      public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
      {
         if (destinationType == typeof(MarkupExtension))
         {
            var bindingExpression = value as BindingExpression;
            if (bindingExpression == null) throw new Exception();
            return bindingExpression.ParentBinding;
         }

         return base.ConvertTo(context, culture, value, destinationType);
      }
   }

   public static class XamlTricks
   {
      public static void FixSerialization()
      {
         // this is absolutely vital:
         TypeDescriptor.AddProvider(new BindingTypeDescriptionProvider(), typeof(Binding));
         TypeDescriptor.AddAttributes(typeof(BindingExpression), new Attribute[] { new TypeConverterAttribute(typeof(BindingConverter)) });
      }
   }
}


