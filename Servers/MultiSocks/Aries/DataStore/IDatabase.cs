namespace MultiSocks.Aries.DataStore
{
    public interface IDatabase
    {
        bool CreateNew(DbAccount info);
        DbAccount? GetByName(string? username);
        int AddPersona(int id, string persona);
        int DeletePersona(int id, string persona);
        int AddFriend(int id, string Friend);
        int DeleteFriend(int id, string Friend);
        int AddRival(int id, string Rival);
        int DeleteRival(int id, string Rival);
    }
}
