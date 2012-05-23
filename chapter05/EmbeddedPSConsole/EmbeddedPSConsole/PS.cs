using System.IO;
using System.Management.Automation;
using System.Text;

namespace EmbeddedPSConsole
{
    public static class PS
    {
        public static string ExecutePS(this string script)
        {
            var sb = new StringBuilder(string.Format("> {0}\r", script));

            powerShell.AddScript(script);
            powerShell.AddCommand("Out-String");
            powerShell.AddParameter("Width", 133);

            try
            {
                var results = powerShell.Invoke();
                if (powerShell.Streams.Error.Count > 0)
                {
                    foreach (var err in powerShell.Streams.Error)
                    {
                        AddErrorInfo(sb, err);
                    }
                    powerShell.Streams.Error.Clear();
                }
                else
                {
                    foreach (var item in results)
                    {
                        sb.Append(item);
                    }
                }
            }
            catch (System.Exception ex)
            {
                sb.Append(ex.Message);
            }

            powerShell.Commands.Clear();
            return sb.ToString();
        }

        static PowerShell _powerShell;

        static PowerShell powerShell
        {
            get
            {
                if (_powerShell == null)
                {
                    _powerShell = PowerShell.Create();
                    powerShell.Runspace = PSConfig.GetPSConfig;
                    if (!string.IsNullOrEmpty(PSConfig.Profile) && File.Exists(PSConfig.Profile))
                    {
                        var script = File.ReadAllText(PSConfig.Profile);
                        _powerShell.AddScript(script);
                        _powerShell.Invoke();
                        powerShell.Commands.Clear();
                    }
                }

                return _powerShell;
            }
        }

        private static void AddErrorInfo(StringBuilder sb, ErrorRecord err)
        {
            sb.Append(err.ToString());
            sb.AppendFormat("\r\n   +{0}", err.InvocationInfo.PositionMessage);
            sb.AppendFormat("\r\n   + CategoryInfo          :{0}", err.CategoryInfo);
            sb.AppendFormat("\r\n   + FullyQualifiedErrorId :{0}", err.FullyQualifiedErrorId.ToString());
            sb.AppendLine();
        }
    }
}