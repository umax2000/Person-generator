using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Dob
    {
        public DateTime date { get; set; }
        public int age { get; set; }
    }

    public class Name
    {
        public string title { get; set; }
        public string first { get; set; }
        public string last { get; set; }
    }

    public class Result
    {
        public Name name { get; set; }
        public Dob dob { get; set; }
        public string phone { get; set; }
        public string cell { get; set; }
    }

    public class Root
    {
        public List<Result> results { get; set; }
    }
}
