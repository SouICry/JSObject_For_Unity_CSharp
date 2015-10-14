A dynamic type implementation for JavaScript style objects in Unity C# 

#Background:
Unity uses mono 2.0, which does not have support for the dynamic type.
This class was originally created to store JSON data on the fly without needing to modify the data structure manually. I was designed to work with JSONParser (JSONObject parser specifically) as is, but can be easily expanded.
It works fine for smaller projects and during development, but I would recommend going back to fixed data structures before release since this adds unnecessary overhead.

 
#About
dynamic objects from this class mostly function the same way as JavaScript objects/variables, with properties accessible by `["property_name"]`, arrays accessible by `[index]` and mixed data type storage. These are backed by `Dictionary<string, dynamic>, `List<dynamic>` and data storage with implicitly converting type.

Ex. 

`dynamic d = 1; 

int i = 5 * d;   //5 

string s = d + i + " digits"; //"7 digits" 

float f = d % 4;  //1.0`
 
Supports string, float, int, bool, List(dynamic) and List{string, dynamic}. Doubles are automatically converted to floats. Nests infinitely. 

#Usage 
Values or references can be directly assigned from or to this dynamic object. Loosely typed, so most cast errors only show up at run time as an exception. 

    dynamic d = "squid"; 
	
    string s = d;                    //No cast necessary 
	
    d = 1;                           //Converted to int 
	
    s = (d + 4) % 5;                 //Supports standard math and string operations + - * / % and +(concat) 
	
    float f = d * 5;                 //and type conversions 
	
    Console.Write(d.ToString());     //Unfortunately ToString() is necessary due to a C# bug in Console. Other uses, like Debug.Log(d), Debug.Write(d), work perfectly. 
	
 
Some common methods and properties are implemented for List and Dictionary. The rest can be accessed by assigning it to a correct type reference. 

	dynamic d = new Dictionary{string, dynamic}();  
	
	d.Add("key", new List{dynamic});           
	
	d["key"].Count;                                          //0  Gets the Count for the list 
	
	d.ContainsKey("key");                                    //true 
	
	d["key"] = 15;                                           //Converts the list in the dictionary to int with value 15 
	
	Dictionary{string, dynamic}.KeyCollection keys = d.Keys; //Gets the usual list of keys 
	
	Dictionary{string, dynamic} dict = d;                    //Gets the actual dictionary 
	
	dict.GetEnumerator();                                    //Other methods available after conversion 