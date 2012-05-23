using System;
using System.Management.Automation.Runspaces;

namespace EmbeddedPSConsole
{
    public class PSConfig
    {
        private static string _profile;
        private static Runspace _rs;

        public static Runspace GetPSConfig { get { return rs; } }

        public static string Profile
        {
            get
            {
                return _profile;
            }
            set
            {
                _profile = value;
            }
        }

        private static Runspace rs
        {
            get
            {
                if (_rs == null)
                {
                    _rs = RunspaceFactory.CreateRunspace();
                    _rs.ThreadOptions = PSThreadOptions.UseCurrentThread;
                    _rs.Open();

                    return _rs;
                }
                return _rs;
            }
        }

        public static void AddVariable(string name, object value)
        {
            rs.SessionStateProxy.SetVariable(name, value);
        }

        internal static string RunCustomProfile(string fileName)
        {
            Profile = fileName;
            AddVariable("profile", System.IO.Path.Combine(Environment.CurrentDirectory, fileName));
            return PS.ExecutePS("");
        }
    }
}