using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Model
{
    public class Users
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }
        public DateTime DateTimeCreated { get; set; }
    }
}
