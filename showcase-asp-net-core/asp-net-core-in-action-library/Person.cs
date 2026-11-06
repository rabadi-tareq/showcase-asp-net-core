namespace asp_net_core_in_action_library
{
    public class Person(string firstName, string lastName)
    {
        public string FirstName { get; set; } = firstName;
        public string LastName { get; set; } = lastName;
    }
}
