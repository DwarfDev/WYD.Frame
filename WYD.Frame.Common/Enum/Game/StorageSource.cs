namespace WYD.Frame.Common.Enum.Game;

public static class GameStorage
{
    public enum StorageSource
    {
        None = 0,
        Inventory = 1,
        Cargo = 2,
        Equips = 3
    }

    public static int GetSize(StorageSource source)
    {
        switch (source)
        {
            case StorageSource.Inventory:
                return 60;
            case StorageSource.Cargo:
                return 120;
            case StorageSource.Equips:
                return 18;
            default:
                return 0;
        }
    }
}
