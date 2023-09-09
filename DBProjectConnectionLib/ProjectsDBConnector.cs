using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Reflection;

namespace DBProjectConnectionLib
{
    // ReSharper disable once InconsistentNaming
    public class ProjectsDBConnector:IDisposable
    {
        private static readonly string DBName = "ProjectsDB";

        public static string  DefaultConfigurationPath { get; set; }
         
        public  string ConnectionString;
        public SqlConnection Connection { get; private set; }

        //return  ConfigurationManager.OpenExeConfiguration(Assembly.GetAssembly(typeof(ProjectsDBConnector))
        //     .Location);
        public static string LocalConfigPath
        {
            get
            {
                var assemblyPath = Assembly.GetAssembly(typeof(ProjectsDBConnector)).Location;
                return Path.Combine(Path.GetDirectoryName(assemblyPath) ?? throw new InvalidOperationException(), "temporary_db.config");
            }
        }
            
        //public Configuration RoamingConfig =>
        //    ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);

        static ProjectsDBConnector()
        {
            DefaultConfigurationPath = LocalConfigPath;
        }

        public  ProjectsDBConnector()
        {
            Connect();
        }
        public ProjectsDBConnector(SqlConnectionStringBuilder builder,bool save=false)
        {
            Connect(builder, save);
        }
        public ProjectsDBConnector(string address, string login, string pass, bool save=false)
        {
            Connect(address, login, pass, save);
        }

        public void Connect(string address, string login, string pass, bool save)
        {
            SqlConnectionStringBuilder builder = BuildConnectionString(address, login, pass);
            Connect(builder, save);
        }

        public void Connect(SqlConnectionStringBuilder builder,bool save)
        {
            Connection = new SqlConnection(builder.ConnectionString);

            if (save)
            {
                SaveConnectionStringTo(DefaultConfigurationPath, builder);
            }

            ConnectionString = builder.ConnectionString;
        }

        public void Connect()
        {
            Connect(new SqlConnectionStringBuilder(LoadConnectionString()), false);
        }

        private static SqlConnectionStringBuilder BuildConnectionString(string address, string login, string pass)
        {
            var builder = new SqlConnectionStringBuilder()
            {
                DataSource = address,
                UserID = login,
                Password = pass,
                InitialCatalog = DBName
            };
           // builder.IntegratedSecurity = true;
            return builder;
        }

        private static void SaveConnectionStringTo(string configPath,SqlConnectionStringBuilder builder)
        {
            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = configPath };
            var config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            var connectionSetting = new ConnectionStringSettings(DBName, builder.ConnectionString);
            var connectionSection = config.ConnectionStrings;
            var sectionIndex = connectionSection.ConnectionStrings.IndexOf(connectionSetting);
            if (sectionIndex >= 0)
            {
                connectionSection.ConnectionStrings.RemoveAt(sectionIndex);
               
            }

            connectionSection.ConnectionStrings.Add(connectionSetting);
            connectionSection.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
            config.Save(ConfigurationSaveMode.Modified);
        }

        public static void SaveConnectionString(string address, string login, string pass)
        {
            SaveConnectionStringTo(DefaultConfigurationPath, BuildConnectionString(address, login, pass));
        }

        public static void RemoveConnectionString()
        {
            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = DefaultConfigurationPath };
            var config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            var connectionSetting = new ConnectionStringSettings(DBName, "");
            var connectionSection = config.ConnectionStrings;
            var sectionIndex = connectionSection.ConnectionStrings.IndexOf(connectionSetting);
            if (sectionIndex >= 0)
            {
                connectionSection.ConnectionStrings.RemoveAt(sectionIndex);
                connectionSection.SectionInformation.UnprotectSection();
            }

            config.Save(ConfigurationSaveMode.Modified);
        }

        public static string LoadConnectionString()
        {
            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = DefaultConfigurationPath };
            var config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            return config.ConnectionStrings.ConnectionStrings[DBName].ConnectionString;
        }

        public static bool TryLoadConnectionString(out string connectionString)
        {
            connectionString = "";
            try
            {
                connectionString = LoadConnectionString();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public void Open()
        {
            Connection.Open();
        }

        public void Close()
        {
            Connection.Close();
        }

        public ConnectionState GetConnectionState()
        {
            return Connection.State;
        }

        public void Dispose()
        {
            Connection?.Dispose();
        }
    }
}
