using ConsoleApp1;
using Newtonsoft.Json;
using System;
using System.Net;

class Program
{
    private static string APIUrl = "https://randomuser.me/api/?inc=name,dob,phone,cell&noinfo";

    public static string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    public static string GetDataWithoutAuthentication()
    {
        using (var client = new WebClient())
        {
            client.Headers.Add("Content-Type:application/json");
            //client.Headers.Add("Accept:application/json");

            var result = client.DownloadString(APIUrl);
            //Console.WriteLine(Environment.NewLine + result);
            return result;
        }
    }
    public static List<string> randomizeCreditCards()
    {
        Random rand = new Random();
        List<string> stringForCards = new List<string>();
        //Ограничим максимально возможное количество карт десятью
        for (int i = 0; i < rand.Next(1, 10); ++i)
        {
            int cardNumber1 = rand.Next(4572, 4999);
            int cardNumber2 = rand.Next(1000, 9999);
            int cardNumber3 = rand.Next(1000, 9999);
            int cardNumber4 = rand.Next(1000, 9999);
            string s = $"{cardNumber1} {cardNumber2} {cardNumber3} {cardNumber4}";
            stringForCards.Add(s);           
        }
        return stringForCards;
    }
    public static List<string> randomizePhoneNumbers()
    {
        Random rand = new Random();
        List<string> stringForPhones = new List<string>();
        //Ограничим максимально возможное количество номеров тремя
        for (int i = 0; i < rand.Next(1, 3); ++i)
        {
            int num = rand.Next(000, 999);
            int num2 = rand.Next(00, 99);
            int num3 = rand.Next(00, 99);
            string s = $"{"+7(904)"} {num} {num2} {num3}";
            stringForPhones.Add(s);
        }
        return stringForPhones;
    }
    public static List<Child> generateChilds(Person person)
    {
        Random rand = new Random();
        List<Child> childList = new List<Child>();
        for (int i = 0; i < rand.Next(1, 11); ++i)
        {
            Child child = new Child();
            child.Id = i;
            string childResultAPI = GetDataWithoutAuthentication();
            Root childResult = JsonConvert.DeserializeObject<Root>(childResultAPI);
            child.Gender = (childResult.results[0].name.title == "Mr") ? Gender.Male : Gender.Female;
            child.FirstName = childResult.results[0].name.first;
            child.LastName = person.LastName;
            child.BirthDate = person.BirthDate + 18 + i;
            childList.Add(child);
        }
        return childList;
    }
    public static List<Person> generatePersons()
    {
        List<Person> list = new List<Person>();
        for (int i = 0; i < 10000; ++i)
        {
            Person person = new Person();
            person.Id = i;
            Guid guid = Guid.NewGuid();
            person.TransportId = guid;
            string resultAPI = GetDataWithoutAuthentication();
            Root result = JsonConvert.DeserializeObject<Root>(resultAPI);

            person.FirstName = result.results[0].name.first;
            person.LastName = result.results[0].name.last;
            person.SequenceId = i;

            Random rand = new Random();


            person.CreditCardNumbers = randomizeCreditCards().ToArray();

            person.Age = result.results[0].dob.age;

            person.Phones = randomizePhoneNumbers().ToArray();

            string dob = result.results[0].dob.date.Date.ToShortDateString().Replace(".", "");
            person.BirthDate = Convert.ToInt64(dob);

            person.Salary = rand.Next(100000, 1000000);

            person.IsMarred = (2 == rand.Next(1, 2)) ? true : false;
            person.Gender = (result.results[0].name.title == "Mr") ? Gender.Male : Gender.Female;

            person.Children = generateChilds(person).ToArray();

            list.Add(person);
        }
        return list;
    }
    public static void serializePerson(List<Person> list)
    {
        ListPerson personList = new ListPerson();
        personList.results = new List<Person>();
        personList.results.AddRange(list);
        var json = System.Text.Json.JsonSerializer.Serialize(personList);
        File.WriteAllText(path + @"\Persons.json", json);
    }
    static void Main(string[] args)
    {
        List<Person> list = generatePersons();

        serializePerson(list);     

        var result = JsonConvert.DeserializeObject<ListPerson>(File.ReadAllText(path + @"\Persons.json"));

        int sumCard = 0;

        int dateNow = Convert.ToInt32(DateTime.Now.Year);
        int sumChildrenCount = 0;
        int sumChildrenAge = 0;
        foreach (Person person in result.results)
        {
            sumCard += person.CreditCardNumbers.Count();
            int averageAge = 0;
            foreach (var child in person.Children)
            {
                averageAge += dateNow - Convert.ToInt32(child.BirthDate % 10000);
            }
            averageAge = averageAge / person.Children.Count();
            sumChildrenAge += averageAge;
            sumChildrenCount += person.Children.Count();
        }
        sumChildrenAge = sumChildrenAge / sumChildrenCount;

        Console.WriteLine("persons count: " + result.results.Count);
        Console.WriteLine("persons credit card count:" + sumCard);
        Console.WriteLine("the average value of child age: " + sumChildrenAge);
    }
}