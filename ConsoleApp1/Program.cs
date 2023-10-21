using Microsoft.VisualBasic;
using System.Globalization;

string format = "dd.MM.yyyy HH:mm";
DateTime dateStart;
DateTime dateEnd;
string date1 = new DateTime(2008, 10, 5,2,20,00).ToString(format);
Console.WriteLine(date1);

DateTime localDateTime;


if (DateTime.TryParseExact(
                    "01/01/2002 02:22",
                    "MM/dd/yyyy HH:mm",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out localDateTime))
{
    Console.WriteLine(localDateTime.TimeOfDay);

}

if (DateTime.TryParseExact("05.12.2021 11:00", format, null, DateTimeStyles.None, out dateStart))
{
    Console.WriteLine(dateStart.ToString(format));
    dateStart=dateStart.AddDays(1);
    //if(dateStart<=dateEnd.)
    //while(dateStart.!=dateEnd.Minute)
    //{

    //}
}