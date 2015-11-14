namespace NHibernate.vNext
{
    public class DataConfiguration 
    {
        public string ConnectionString { get; set; }
        public string Dialect { get; set; }
        public string Driver { get; set; }
        public bool UseDriverOracleODAC { get; set; }
        public string CoreAssembly { get; set; }
        public string MapAssembly { get; set; }

    }
}
