using System.Collections.Generic;

public class CompanyDatabaseManager
{
    private Dictionary<int, DBInfo> _companyDbs;

    public DBInfo GetDbInfo(int id)
    {
        return _companyDbs[id];
    }

    public void Initialize()
    {
        var MnA = new DBInfo(0, "1", "2", "3");

        _companyDbs = new Dictionary<int, DBInfo>
        {
            {MnA.GetId(), MnA}
        };
    }

    public struct DBInfo
    {
        private int _uniqueId;
        private string _appId;
        private string _privateKey;
        private string _ipAddress;

        public int GetId()
        {
            return _uniqueId;
        }

        public DBInfo(int id, string app, string key, string ip)
        {
            _uniqueId = id;
            _appId = app;
            _privateKey = key;
            _ipAddress = ip;
        }
    }
}