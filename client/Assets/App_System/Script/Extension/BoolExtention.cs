public static class BoolExtention
{
    public static int ToInt(this bool self)
    {
        return self ? 0 : 1;
    }
    public static bool IsTrue(this bool self)
    {
        return self == true;
    }
    public static bool IsFalse(this bool self)
    {
        return self == false;
    }
}
