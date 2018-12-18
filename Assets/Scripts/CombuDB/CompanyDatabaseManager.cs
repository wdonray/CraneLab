using System.Collections.Generic;

public class CompanyDatabaseManager
{
    private static Mouledoux.Components.Mediator.Subscriptions _subscriptions;
    private static Dictionary<string, DBInfo> _companyDbs;
    private static string _dbCode = "default";

    public static DBInfo GetDbInfo(string id)
    {
        return _companyDbs[id];
    }

    public static void Initialize()
    {
        #region Company DB Info
        // Tantrum LAB (default) ------------------------------------------------------------------
        DBInfo Tantrum = new DBInfo(
            "default",                                      // company ID
            "12a1f840-11e8-4467-8420-562afc849ffe",         // app ID 
            "ddaa00bc-e738-477f-9dd0-0124de3ea60a",         // private key
            "http://34.214.150.11/tantrum/");               // IP address


        // M&A ------------------------------------------------------------------------------------
        DBInfo MnA = new DBInfo(
            "2639342",
            "1",
            "2",
            "3");

        #endregion

        _companyDbs = new Dictionary<string, DBInfo>
        {
            {Tantrum.GetUniqueId(), Tantrum},
            {MnA.GetUniqueId(), MnA},
        };

    }

    public static void SetDBConnectionInfo(Combu.CombuManager dbManager)
    {
        if (_companyDbs == null) Initialize();

        DBInfo info                 = _companyDbs[_dbCode];
        dbManager.appId             = info.GetAppID();
        dbManager.appSecret         = info.GetPrivateKey();
        dbManager.urlRootProduction = info.GetIPAddress();        
    }

    public static void SetDBConnectionInfo()
    {
        SetDBConnectionInfo(Combu.CombuManager.instance);
    }



    public sealed class DBInfo
    {
        private string _uniqueId;
        private string _appId;
        private string _privateKey;
        private string _ipAddress;


        public string GetUniqueId()
        { return _uniqueId; }

        public string GetAppID()
        { return _appId; }

        public string GetPrivateKey()
        { return _privateKey; }

        public string GetIPAddress()
        { return _ipAddress; }



        public DBInfo(string id, string app, string key, string ip)
        {
            _uniqueId = id;
            _appId = app;
            _privateKey = key;
            _ipAddress = ip;
        }
    }
}