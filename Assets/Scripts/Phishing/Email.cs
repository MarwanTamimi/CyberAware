using System;

[Serializable]
public class Email
{
    public string subject;
    public string sender;
    public string body;
    public bool isPhishing;
}

[Serializable]
public class EmailList
{
    public Email[] emails;
}
