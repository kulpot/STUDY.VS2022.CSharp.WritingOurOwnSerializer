using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Linq;
using System.Reflection;

//ref link:https://www.youtube.com/watch?v=ET8uw2a6EnI&list=PLRwVmtr-pp05brRDYXh-OTAIi-9kYcw3r&index=18
// Serialize C# to XML
// add reference: System.Runtime.Serialization

[DataContract]
class Person
{
    [DataMember]
    public string FirstName { get; set; }
    [DataMember]
    public string LastName { get; set; }
    [DataMember]
    //[DataMember(Name = "Weight")] 
    public int Age { get; set; }
}

class MeSerializer
{
    Type targetType;
    public MeSerializer(Type targetType)
    {
        this.targetType = targetType;
        if(!targetType.IsDefined(typeof(DataContractAttribute),false))
            throw new Exception("No soup for you.");
    }
    public void WriteObject(Stream stream, object graph)
    {
        IEnumerable<PropertyInfo> serializableProperties =
            targetType.GetProperties().Where(p => p.IsDefined(typeof(DataMemberAttribute), false));
        var writer = new StreamWriter(stream);
        writer.WriteLine("<" + targetType.Name + ">");
        foreach(PropertyInfo propInfo in serializableProperties)
            writer.Write("<" + propInfo.Name + ">" + propInfo.GetValue(graph, null) +
                "</" + propInfo.Name + ">");
        writer.WriteLine("</" + targetType.Name + ">");
        writer.Flush(); // buffer then send to stream --important file IO
    }
}

class MainClass
{
    static void Main()
    {
        var me = new Person
        {
            FirstName = "Kulpot",
            LastName = "King",
            Age = 25
        };
        //var serializer = new DataContractSerializer(me.GetType());
        var serializer = new MeSerializer(me.GetType());
        var someRam = new MemoryStream(); // File IO
        serializer.WriteObject(someRam, me);
        someRam.Seek(0, SeekOrigin.Begin);
        Console.WriteLine(
            XElement.Parse( // Linq Xelement
                Encoding.ASCII.GetString(
                    someRam.GetBuffer()).Replace("\0", "")));
    }
}