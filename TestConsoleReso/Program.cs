class Program
{
    static void Main()
    {
        DateTime myDate = DateTime.MinValue; // or any DateTime value

        string displayDate = myDate == DateTime.MinValue ? "N/A" : myDate.ToString("yyyy-MM-dd");
        Console.WriteLine(displayDate);
    }
}
