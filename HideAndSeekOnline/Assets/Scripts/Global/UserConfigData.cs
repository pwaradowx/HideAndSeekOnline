namespace Project.Global
{
    [System.Serializable]
    public struct UserConfigData
    {
        public string UserName;

        public UserConfigData(string userName)
        {
            UserName = userName;
        }
    }
}
